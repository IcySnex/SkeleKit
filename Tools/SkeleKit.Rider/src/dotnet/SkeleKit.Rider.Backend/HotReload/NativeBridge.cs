using System.Net;
using System.Net.Sockets;
using JetBrains.Lifetimes;

namespace SkeleKit.Rider.Backend.HotReload;

internal sealed class NativeBridge(
	string solutionFile,
	Action<string> log)
{
	// the in-app agent dials this one, so unlike the debug ports it cannot move
	const int ReloadPort = 9988;
	const int DebounceMilliseconds = 150;

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
			catch
			{
				// i guess bruh
			}
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

	static int FreePort()
	{
		using Socket probe = Bind(0);

		return ((IPEndPoint)probe.LocalEndPoint).Port;
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
		catch
		{
			// grr;
		}
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

	static bool IsConnected(
		Socket socket)
	{
		try
		{
			return !socket.Poll(0, SelectMode.SelectRead) || socket.Available != 0;
		}
		catch
		{
			return false;
		}
	}


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
	DebuggerWorkerSync? debuggerWorker;
	bool buildReady;
	bool watchersReady;
	bool watcherFailed;
	bool readyNoticeSent;


	public int AppPort { get; private set; }
	public int RiderPort { get; private set; }


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

		EndSession();

		lock (sessionGate)
		{
			sdb = connection;
			sessionDef = session = lifetime.CreateNested();
			sessionVersion++;
			engines = new(StringComparer.OrdinalIgnoreCase);
			buildReady = false;
			watchersReady = false;
			watcherFailed = false;
			readyNoticeSent = false;
		}

		long version;
		lock (sessionGate)
			version = sessionVersion;

		Start(() => Prepare(session.Lifetime, version), "skele-engine-start");
	}

	void EndSession(
		SdbConnection? closing = null)
	{
		LifetimeDefinition? session;

		lock (sessionGate)
		{
			if (closing is not null && !ReferenceEquals(sdb, closing))
				return;

			session = Interlocked.Exchange(ref sessionDef, null);
			if (session is null)
				return;

			sessionVersion++;
			sdb = null;
			engines = new(StringComparer.OrdinalIgnoreCase);
			watched = [];
			buildReady = false;
			watchersReady = false;
			watcherFailed = false;
			readyNoticeSent = false;
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

			ReloadEngine? appEngine = null;
			foreach (AppProject project in projects)
			{
				if (!session.IsAlive)
					return;

				ReloadEngine? engine = EngineFor(project, version);
				if (string.Equals(project.ProjectFile, app.ProjectFile, StringComparison.OrdinalIgnoreCase))
					appEngine = engine;
			}

			SdbConnection? connection;
			lock (sessionGate)
				connection = version == sessionVersion ? sdb : null;
			if (connection is null)
				return;

			bool matches = false;
			string? unavailable = null;
			if (appEngine is null)
				unavailable = $"could not compile {app.AssemblyName}; see the Rider log";
			else
			{
				try
				{
					if (!appEngine.Matches(connection, out string reason))
						unavailable = reason;
					else
						matches = true;
				}
				catch (Exception exception)
				{
					unavailable = exception.Message;
					log($"could not verify the running build of {app.AssemblyName}: {exception.Message}");
				}
			}

			Watch(session, projects, version);

			lock (sessionGate)
			{
				if (!session.IsAlive || version != sessionVersion)
					return;

				buildReady = matches;
				watchersReady = true;
			}

			Start(() => Drain(session, version), "skele-reload-worker");

			if (unavailable is not null)
				Notice($"Hot reload unavailable: {unavailable}.", connection);
			else
				TryNoticeReady(version);
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
		List<AppProject> projects,
		long version)
	{
		foreach (AppProject project in projects)
		{
			FileSystemWatcher watcher = new(project.ProjectDir)
			{
				IncludeSubdirectories = true,
				Filter = "*.cs",
				InternalBufferSize = 64 * 1024,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
			};

			watcher.Changed += OnChanged;
			watcher.Created += OnChanged;
			watcher.Renamed += OnChanged;
			watcher.Error += (_, error) => OnWatcherFailed(project, error.GetException(), version);
			watcher.EnableRaisingEvents = true;

			session.OnTermination(() =>
			{
				watcher.EnableRaisingEvents = false;
				watcher.Dispose();
			});
		}
	}

	void OnWatcherFailed(
		AppProject project,
		Exception exception,
		long version)
	{
		SdbConnection? connection;
		bool notify;

		lock (sessionGate)
		{
			if (version != sessionVersion)
				return;

			connection = sdb;
			notify = !watcherFailed;
			watcherFailed = true;
		}

		log($"file watcher on {project.AssemblyName} failed: {exception.Message}");

		if (notify)
			Notice("Hot reload watcher failed; restart the debug session.", connection);
	}

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
					{
						if (!session.IsAlive)
							return;
					}
				}
			}

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

			if (engine.Apply(
					path,
					connection,
					debuggerWorker!,
					log,
					message => Notice(message, connection)) == ReloadEngine.Outcome.Applied)
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

		foreach (AppProject project in projects)
		{
			if (path.StartsWith(project.ProjectDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
				&& (owner is null || project.ProjectDir.Length > owner.ProjectDir.Length))
				owner = project;
		}

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
				SendAll(client, [0]);
		}
		catch
		{
			lock (sessionGate)
			{
				if (ReferenceEquals(reloadClient, client))
					reloadClient = null;
			}

			Close(client);
		}
	}

	void OnReloadClient(
		Socket socket)
	{
		Socket? previous;
		long version;
		lock (sessionGate)
		{
			previous = reloadClient;
			reloadClient = socket;
			version = sessionVersion;
		}

		Close(previous);
		TryNoticeReady(version);
	}

	void TryNoticeReady(
		long version)
	{
		SdbConnection? connection = null;
		Socket? stale = null;

		lock (sessionGate)
		{
			if (version != sessionVersion
				|| !buildReady
				|| !watchersReady
				|| watcherFailed
				|| readyNoticeSent)
				return;

			if (reloadClient is null)
				return;

			if (!IsConnected(reloadClient))
			{
				stale = reloadClient;
				reloadClient = null;
			}
			else if (sdb is not null)
			{
				connection = sdb;
				readyNoticeSent = true;
			}
		}

		Close(stale);

		if (connection is not null)
			Notice("Hot reload ready.", connection);
	}

	Socket? ConnectRider()
	{
		for (int attempt = 0; attempt < 100; attempt++)
		{
			Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.Connect(new IPEndPoint(IPAddress.Loopback, RiderPort));

				return socket;
			}
			catch
			{
				Close(socket);
				Thread.Sleep(50);
			}
		}

		return null;
	}


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

		Socket? reloadListener = TryBind(ReloadPort);
		if (reloadListener is null)
			log($"port {ReloadPort} is taken, so the app cannot be asked to rebuild its UI after a reload");

		RiderPort = FreePort();
		debuggerWorker = new(RiderPort);

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
}
