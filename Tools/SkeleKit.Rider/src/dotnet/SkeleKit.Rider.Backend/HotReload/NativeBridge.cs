using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using JetBrains.Lifetimes;

namespace SkeleKit.Rider.Backend.HotReload;

// Sits inside Rider's own native iOS debug session and adds hot reload to it.
//
// The frontend points the session's two debug ports at us, so the app connects to AppPort while Rider
// listens on RiderPort. Every connection is relayed straight through, which is what keeps breakpoints,
// stepping and the console working; the debugger connection is additionally frame-parsed so a saved
// file can be turned into an Edit-and-Continue delta and injected. Applying a delta updates method
// bodies but does not redraw anything, so afterwards the app is nudged on ReloadPort to rebuild its
// live UI. An app that does not answer there still gets its code updated.
sealed class NativeBridge
{
	// the in-app agent dials this one, so unlike the debug ports it cannot move
	const int ReloadPort = 9988;

	const int DebounceMilliseconds = 150;

	readonly string solutionFile;
	readonly Action<string> log;

	readonly object queueGate = new();
	readonly HashSet<string> queued = new(StringComparer.OrdinalIgnoreCase);

	readonly object sessionGate = new();

	SdbConnection? sdb;
	Socket? reloadClient;
	Lifetime lifetime;
	LifetimeDefinition? sessionDef;
	long sessionVersion;

	List<AppProject> watched = [];
	Dictionary<string, ReloadEngine?> engines = new(StringComparer.OrdinalIgnoreCase);

	public int AppPort { get; private set; }
	public int RiderPort { get; private set; }

	public NativeBridge(
		string solutionFile,
		Action<string> log)
	{
		this.solutionFile = solutionFile;
		this.log = log;
	}

	// Binds the bridge and reports whether there is anything here to hot reload. False leaves the ports
	// unpublished, and an iOS debug session then runs exactly as it would without the plugin.
	public bool Start(
		Lifetime lifetime)
	{
		this.lifetime = lifetime;

		if (!AppProject.AnyIosProject(solutionFile))
		{
			log("no .NET iOS project in this solution; leaving iOS debug sessions alone");
			return false;
		}

		Socket appListener = Bind(0);
		AppPort = ((IPEndPoint)appListener.LocalEndPoint).Port;
		RiderPort = FreePort();

		Socket? reloadListener = TryBind(ReloadPort);
		if (reloadListener is null)
			log($"port {ReloadPort} is taken, so the app cannot be asked to rebuild its UI after a reload");

		lifetime.OnTermination(() =>
		{
			Close(appListener);
			Close(reloadListener);
			Socket? client;
			lock (sessionGate)
			{
				client = reloadClient;
				reloadClient = null;
			}
			Close(client);
			EndSession();
		});

		Accept(appListener, OnApp);
		if (reloadListener is not null)
			Accept(reloadListener, OnReloadClient);

		log($"bridge up: app :{AppPort} -> Rider :{RiderPort}, reload :{ReloadPort}");

		return true;
	}

	void OnApp(
		Socket appSocket)
	{
		Socket? riderSocket = ConnectRider();
		if (riderSocket is null)
		{
			log($"could not reach Rider on {RiderPort}; the debugger worker never started listening");
			Close(appSocket);

			return;
		}

		SdbConnection.Mitm(appSocket, riderSocket, OnSdbIdentified, EndSession);
	}

	// Use Mono's USER_LOG debugger event—the same path as Debug.WriteLine—so Rider owns presentation
	// and the message appears in the existing Debug output.
	void Notice(
		string message,
		SdbConnection? expected = null)
	{
		SdbConnection? connection;
		lock (sessionGate)
		{
			connection = sdb;
			if (expected is not null && !ReferenceEquals(connection, expected))
				return;
		}

		try { connection?.UserLog(message); }
		catch (Exception exception) { log($"could not publish debugger notice: {exception.Message}"); }
	}

	void OnSdbIdentified(
		SdbConnection connection)
	{
		LifetimeDefinition session;

		// a session whose socket never closed cleanly would otherwise keep its watchers and worker alive
		EndSession();

		lock (sessionGate)
		{
			sdb = connection;
			sessionDef = session = lifetime.CreateNested();
			sessionVersion++;
			engines = new(StringComparer.OrdinalIgnoreCase);
		}

		// Rider has just deployed, so this is the moment the build on disk and the build in the app
		// agree; everything is baselined against it
		long version;
		lock (sessionGate)
			version = sessionVersion;

		Start(() => Prepare(session.Lifetime, version), "skele-engine-start");
	}

	// the debugger connection dropped, so the app died or detached; reset per-session state and let the
	// next Debug rebuild against whatever it deploys
	void EndSession(
		SdbConnection? closing = null)
	{
		LifetimeDefinition? session;

		lock (sessionGate)
		{
			// A previous session can finish closing after its replacement has already connected. It must
			// not tear down the replacement's watchers and engine state.
			if (closing is not null && !ReferenceEquals(sdb, closing))
				return;

			session = Interlocked.Exchange(ref sessionDef, null);
			if (session is null)
				return;

			sessionVersion++;
			sdb = null;
			engines = new(StringComparer.OrdinalIgnoreCase);
			watched = [];
		}

		session.Terminate();
		lock (queueGate)
			queued.Clear();

		log("debug session ended");
	}

	void Prepare(
		Lifetime session,
		long version)
	{
		try
		{
			List<AppProject> apps = AppProject.Discover(solutionFile);
			if (apps.Count == 0)
				return;

			// the project Rider built last is the one it just deployed
			List<AppProject> executables = [.. apps.Where(candidate => candidate.IsExecutable)];
			AppProject app = (executables.Count > 0 ? executables : apps)
				.OrderByDescending(candidate => File.GetLastWriteTimeUtc(candidate.DeployedDll))
				.First();
			List<AppProject> projects = AppProject.WithReferences(app);

			lock (sessionGate)
			{
				if (!session.IsAlive || version != sessionVersion)
					return;

				watched = projects;
			}

			log($"debugging {app.AssemblyName}; watching {string.Join(", ", projects.Select(project => project.AssemblyName))}");

			// Every engine must snapshot source before its first save. Lazy creation on the first library
			// edit builds from the already-edited file and silently treats that edit as the baseline.
			// Warm all runtime projects once, before enabling their watchers.
			foreach (AppProject project in projects)
			{
				if (!session.IsAlive)
					return;

				EngineFor(project, version);
			}

			Watch(session, projects);
			Start(() => Drain(session, version), "skele-reload-worker");

			SdbConnection? connection;
			lock (sessionGate)
				connection = version == sessionVersion ? sdb : null;
			if (connection is not null)
				Notice("Hot reload ready.", connection);
		}
		catch (Exception exception)
		{
			log($"could not prepare hot reload: {exception.Message}");
		}
	}

	ReloadEngine? EngineFor(
		AppProject project,
		long version)
	{
		lock (sessionGate)
		{
			if (version != sessionVersion)
				return null;

			if (engines.TryGetValue(project.ProjectFile, out ReloadEngine? cached))
				return cached;
		}

		ReloadEngine? engine = null;
		try
		{
			engine = ReloadEngine.Create(project, log);
		}
		catch (Exception exception)
		{
			log($"{project.AssemblyName}: {exception.Message}");
		}

		lock (sessionGate)
		{
			// Engine construction runs generators and may take seconds. Never let an engine completed
			// by an old session leak into the replacement session's cache.
			if (version != sessionVersion)
				return null;

			engines[project.ProjectFile] = engine;
		}

		if (engine is not null)
			log($"{project.AssemblyName} ready (MVID {engine.Mvid:D})");

		return engine;
	}

	void Watch(
		Lifetime session,
		List<AppProject> projects)
	{
		foreach (AppProject project in projects)
		{
			FileSystemWatcher watcher = new(project.ProjectDir)
			{
				IncludeSubdirectories = true,
				Filter = "*.cs",
				// a save storm (a formatter, a branch switch) otherwise overflows and drops events
				InternalBufferSize = 64 * 1024,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
			};

			watcher.Changed += OnChanged;
			watcher.Created += OnChanged;
			watcher.Renamed += OnChanged;
			watcher.Error += (_, error) => log($"file watcher on {project.AssemblyName} failed: {error.GetException().Message}");
			watcher.EnableRaisingEvents = true;

			session.OnTermination(() =>
			{
				watcher.EnableRaisingEvents = false;
				watcher.Dispose();
			});
		}
	}

	// The watcher's own thread must never block, or its buffer overflows and edits go missing; queue the
	// path and let the worker do the compiling.
	void OnChanged(
		object sender,
		FileSystemEventArgs change)
	{
		lock (queueGate)
		{
			queued.Add(Path.GetFullPath(change.FullPath));
			Monitor.Pulse(queueGate);
		}
	}

	void Drain(
		Lifetime session,
		long version)
	{
		while (session.IsAlive)
		{
			string[] batch;

			lock (queueGate)
			{
				while (queued.Count == 0)
				{
					if (!Monitor.Wait(queueGate, 500))
						if (!session.IsAlive)
							return;
				}
			}

			// an editor writes a file two or three times per save, and a formatter touches several at
			// once; collect the burst before compiling anything
			Thread.Sleep(DebounceMilliseconds);

			lock (queueGate)
			{
				batch = [.. queued];
				queued.Clear();
			}

			foreach (string path in batch)
			{
				if (!session.IsAlive)
					return;

				ApplyOne(path, version);
			}
		}
	}

	void ApplyOne(
		string path,
		long version)
	{
		SdbConnection? connection = null;

		try
		{
			AppProject? project = Owner(path, version);
			if (project is null)
				return;

			lock (sessionGate)
				connection = version == sessionVersion ? sdb : null;

			if (connection is null)
				return;

			ReloadEngine? engine = EngineFor(project, version);
			if (engine is null)
				return;

			if (!engine.Matches(connection, out string reason))
			{
				log($"  {Path.GetFileName(path)}: {reason}");
				Notice(reason, connection);

				return;
			}

			if (engine.Apply(path, connection, log, message => Notice(message, connection)) == ReloadEngine.Outcome.Applied)
				SignalReload(version);
		}
		catch (Exception exception)
		{
			// a bad delta or a stuck debugger connection must never take the backend down with it
			log($"hot reload error on {Path.GetFileName(path)}: {exception.Message}");
			Notice($"Hot reload failed for {Path.GetFileName(path)}: {exception.Message}", connection);
		}
	}

	AppProject? Owner(
		string path,
		long version)
	{
		List<AppProject> projects;
		lock (sessionGate)
		{
			if (version != sessionVersion)
				return null;

			projects = watched;
		}

		AppProject? owner = null;

		// the innermost project wins, so a nested project is not claimed by the one above it
		foreach (AppProject project in projects)
			if (path.StartsWith(project.ProjectDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
				&& (owner is null || project.ProjectDir.Length > owner.ProjectDir.Length))
				owner = project;

		return owner;
	}

	void SignalReload(
		long version)
	{
		Socket? client;
		lock (sessionGate)
			client = version == sessionVersion ? reloadClient : null;

		try
		{
			if (client is not null)
				SendAll(client, new byte[28]);
		}
		catch
		{
			lock (sessionGate)
				if (ReferenceEquals(reloadClient, client))
					reloadClient = null;

			Close(client);
		}
	}

	void OnReloadClient(
		Socket socket)
	{
		Socket? previous;
		lock (sessionGate)
		{
			previous = reloadClient;
			reloadClient = socket;
		}

		Close(previous);
	}

	// Rider's debugger worker may start listening a moment after the app connects, especially on a
	// second session, so retry rather than dropping the app's debug connection.
	Socket? ConnectRider()
	{
		for (int attempt = 0; attempt < 100; attempt++)
		{
			try
			{
				Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(new IPEndPoint(IPAddress.Loopback, RiderPort));

				return socket;
			}
			catch
			{
				Thread.Sleep(50);
			}
		}

		return null;
	}

	static void Accept(
		Socket listener,
		Action<Socket> onAccept)
	{
		Start(() =>
		{
			try
			{
				while (true)
					onAccept(listener.Accept());
			}
			catch { }
		}, "skele-accept");
	}

	static void Start(
		ThreadStart body,
		string name)
	{
		Thread thread = new(body)
		{
			IsBackground = true,
			Name = name
		};
		thread.Start();
	}

	// A port for Rider's debugger worker to listen on. We cannot hold it ourselves, so pick one nothing
	// is using and hand it over.
	static int FreePort()
	{
		HashSet<int> taken = [.. IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Select(endpoint => endpoint.Port)];

		for (int attempt = 0; attempt < 64; attempt++)
		{
			Socket probe = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				probe.Bind(new IPEndPoint(IPAddress.Loopback, 0));
				int port = ((IPEndPoint)probe.LocalEndPoint).Port;

				if (!taken.Contains(port))
					return port;
			}
			finally
			{
				Close(probe);
			}
		}

		return 10099;
	}

	static Socket Bind(
		int port)
	{
		Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.Bind(new IPEndPoint(IPAddress.Loopback, port));
		listener.Listen(8);

		return listener;
	}

	static Socket? TryBind(
		int port)
	{
		try
		{
			return Bind(port);
		}
		catch
		{
			return null;
		}
	}

	static void Close(
		Socket? socket)
	{
		try
		{
			socket?.Dispose();
		}
		catch { }
	}

	static void SendAll(
		Socket socket,
		byte[] data)
	{
		int sent = 0;
		while (sent < data.Length)
		{
			int chunk = socket.Send(data, sent, data.Length - sent, SocketFlags.None);
			if (chunk == 0)
				throw new EndOfStreamException();

			sent += chunk;
		}
	}
}
