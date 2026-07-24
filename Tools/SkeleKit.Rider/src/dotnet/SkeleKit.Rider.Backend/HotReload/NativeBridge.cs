using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
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
	SdbConnection? output;
	Socket? reloadClient;
	Lifetime lifetime;
	LifetimeDefinition? sessionDef;

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
			EndSession();
		});

		Accept(appListener, OnApp);
		if (reloadListener is not null)
			Accept(reloadListener, socket => reloadClient = socket);

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

		// the newest output connection carries our notices; writes to a closed one are harmless
		SdbConnection.Mitm(appSocket, riderSocket, OnSdbIdentified, EndSession, connection => output = connection);
	}

	// Write a line to Rider's debug console. It goes over a raw stdout connection so it can never
	// corrupt the debugger stream.
	void Notice(
		string message) =>
		output?.SendToIde(Encoding.UTF8.GetBytes($"[SkeleKit] {message}\n"));

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
			engines = new(StringComparer.OrdinalIgnoreCase);
		}

		// Rider has just deployed, so this is the moment the build on disk and the build in the app
		// agree; everything is baselined against it
		Start(() => Prepare(session.Lifetime), "skele-engine-start");
	}

	// the debugger connection dropped, so the app died or detached; reset per-session state and let the
	// next Debug rebuild against whatever it deploys
	void EndSession()
	{
		LifetimeDefinition? session;

		lock (sessionGate)
		{
			session = Interlocked.Exchange(ref sessionDef, null);
			if (session is null)
				return;

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
		Lifetime session)
	{
		try
		{
			List<AppProject> apps = AppProject.Discover(solutionFile);
			if (apps.Count == 0)
				return;

			// the project Rider built last is the one it just deployed
			AppProject app = apps.OrderByDescending(candidate => File.GetLastWriteTimeUtc(candidate.DeployedDll)).First();
			List<AppProject> projects = AppProject.WithReferences(app);

			lock (sessionGate)
			{
				if (!session.IsAlive)
					return;

				watched = projects;
			}

			log($"debugging {app.AssemblyName}; watching {string.Join(", ", projects.Select(project => project.AssemblyName))}");

			Watch(session, projects);
			Start(() => Drain(session), "skele-reload-worker");

			// warm the app's own compilation now so the first edit does not pay for it
			EngineFor(app);
		}
		catch (Exception exception)
		{
			log($"could not prepare hot reload: {exception.Message}");
		}
	}

	ReloadEngine? EngineFor(
		AppProject project)
	{
		lock (sessionGate)
			if (engines.TryGetValue(project.ProjectFile, out ReloadEngine? cached))
				return cached;

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
			engines[project.ProjectFile] = engine;

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
		Lifetime session)
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

				ApplyOne(path);
			}
		}
	}

	void ApplyOne(
		string path)
	{
		try
		{
			AppProject? project = Owner(path);
			if (project is null)
				return;

			SdbConnection? connection;
			lock (sessionGate)
				connection = sdb;

			if (connection is null)
				return;

			ReloadEngine? engine = EngineFor(project);
			if (engine is null)
				return;

			if (!engine.Matches(connection, out string reason))
			{
				log($"  {Path.GetFileName(path)}: {reason}");
				Notice(reason);

				return;
			}

			if (engine.Apply(path, connection, log, Notice) == ReloadEngine.Outcome.Applied)
				SignalReload();
		}
		catch (Exception exception)
		{
			// a bad delta or a stuck debugger connection must never take the backend down with it
			log($"hot reload error on {Path.GetFileName(path)}: {exception.Message}");
			Notice($"Hot reload failed for {Path.GetFileName(path)}: {exception.Message}");
		}
	}

	AppProject? Owner(
		string path)
	{
		List<AppProject> projects;
		lock (sessionGate)
			projects = watched;

		AppProject? owner = null;

		// the innermost project wins, so a nested project is not claimed by the one above it
		foreach (AppProject project in projects)
			if (path.StartsWith(project.ProjectDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
				&& (owner is null || project.ProjectDir.Length > owner.ProjectDir.Length))
				owner = project;

		return owner;
	}

	void SignalReload()
	{
		try
		{
			reloadClient?.Send(new byte[28]);
		}
		catch { }
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
}
