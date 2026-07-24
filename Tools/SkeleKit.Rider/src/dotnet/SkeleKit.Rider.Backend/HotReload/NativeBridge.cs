using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using JetBrains.Lifetimes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

// Sits inside Rider's native iOS debug session (the ports are rerouted here by the frontend advice):
// the app connects to AppPort, Rider listens on RiderPort. For each app connection we open one to
// Rider and relay transparently, so breakpoints/stepping/console are untouched. The first (sdb)
// connection is MITM'd so that on a source save we inject apply-changes over it (host Roslyn builds
// the delta), then signal the app on ReloadPort to rebuild its live UI.
sealed class NativeBridge
{
	const int AppPort = 10098;
	const int RiderPort = 10099;
	const int ReloadPort = 9988;

	readonly string cscArgs;
	readonly string deployedDll;
	readonly string projectDir;
	readonly Action<string> log;

	SdbConnection? sdb;
	Socket? reloadClient;
	Lifetime lifetime;
	LifetimeDefinition? sessionDef;
	int connections;
	int engineStarted;
	int domain;
	int module;
	string assemblyName = "";

	public NativeBridge(
		string cscArgs,
		string deployedDll,
		string projectDir,
		Action<string> log)
	{
		this.cscArgs = cscArgs;
		this.deployedDll = deployedDll;
		this.projectDir = projectDir;
		this.log = log;
	}

	public void Start(
		Lifetime lifetime)
	{
		this.lifetime = lifetime;

		Socket appListener = Bind(AppPort);
		Socket reloadListener = Bind(ReloadPort);

		lifetime.OnTermination(() =>
		{
			Close(appListener);
			Close(reloadListener);
			Close(reloadClient);
			connections = 0;
			engineStarted = 0;
			sdb = null;
			module = 0;
		});

		Accept(appListener, OnApp);
		Accept(reloadListener, socket => reloadClient = socket);

		log($"native bridge up: app :{AppPort} -> Rider :{RiderPort}, reload :{ReloadPort}");
	}

	// the sdb connection dropped (app died/detached); reset per-session state so the next Debug
	// re-MITMs cleanly and rebuilds a baseline against the freshly-deployed dll
	void EndSession()
	{
		LifetimeDefinition? def = Interlocked.Exchange(ref sessionDef, null);
		def?.Terminate();

		connections = 0;
		engineStarted = 0;
		module = 0;
		sdb = null;
		log("debug session ended");
	}

	void OnApp(
		Socket appSocket)
	{
		Socket riderSocket;
		try
		{
			riderSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			riderSocket.Connect(new IPEndPoint(IPAddress.Loopback, RiderPort));
		}
		catch (Exception exception)
		{
			log($"could not reach Rider on {RiderPort}: {exception.Message}");
			Close(appSocket);
			return;
		}

		// the app opens the sdb debugger connection first; MITM it for injection, dumb-relay the rest.
		// build the engine baseline now (Rider has just deployed the current dll)
		if (Interlocked.Increment(ref connections) == 1)
		{
			sessionDef = lifetime.CreateNested();
			sdb = SdbConnection.Mitm(appSocket, riderSocket, EndSession);
			StartEngine(sessionDef.Lifetime);
		}
		else
		{
			DumbRelay(appSocket, riderSocket);
		}
	}

	void StartEngine(
		Lifetime sessionLifetime)
	{
		if (Interlocked.Exchange(ref engineStarted, 1) == 1)
			return;

		Thread engine = new(() =>
		{
			try
			{
				RunEngine(sessionLifetime);
			}
			catch (Exception exception)
			{
				log($"engine stopped: {exception.Message}");
			}
		})
		{
			IsBackground = true,
			Name = "skele-native-engine"
		};
		engine.Start();
	}

	void RunEngine(
		Lifetime lifetime)
	{
		if (!File.Exists(cscArgs) || !File.Exists(deployedDll))
		{
			log($"engine idle: build the app with EnableHotReload first ({Path.GetFileName(cscArgs)} missing)");
			return;
		}

		log($"building compilation from {Path.GetFileName(cscArgs)}...");
		CscInvocation csc = CscInvocation.Load(cscArgs, projectDir);
		assemblyName = csc.AssemblyName;
		Project project = Project.Build(csc);
		Compilation compilation = project.Compilation;

		Diagnostic[] errors = [.. compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)];
		if (errors.Length > 0)
		{
			log($"compilation has {errors.Length} errors, cannot baseline; first: {errors[0]}");
			return;
		}

		Baseline baseline = new(deployedDll, compilation);
		log($"engine ready (MVID {baseline.Mvid}); edit a .cs file to hot reload");

		Watch(lifetime, compilation, baseline);
	}

	void Watch(
		Lifetime lifetime,
		Compilation compilation,
		Baseline baseline)
	{
		Dictionary<string, SyntaxTree> trees = compilation.SyntaxTrees
			.Where(tree => !string.IsNullOrEmpty(tree.FilePath) && File.Exists(tree.FilePath))
			.GroupBy(tree => Path.GetFullPath(tree.FilePath), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

		object gate = new();
		Compilation snapshot = compilation;

		FileSystemWatcher watcher = new(projectDir)
		{
			IncludeSubdirectories = true,
			Filter = "*.cs",
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
		};

		void OnChanged(
			object _,
			FileSystemEventArgs change)
		{
			string path = Path.GetFullPath(change.FullPath);

			lock (gate)
			{
				if (!trees.TryGetValue(path, out SyntaxTree? oldTree))
					return;

				string text = ReadStable(path);
				if (text == oldTree.ToString())
					return;

				SyntaxTree newTree = CSharpSyntaxTree.ParseText(
					SourceText.From(text, Encoding.UTF8),
					(CSharpParseOptions)oldTree.Options,
					path);

				Compilation newCompilation = snapshot.ReplaceSyntaxTree(oldTree, newTree);
				List<SemanticEdit> edits = Differ.Edits(snapshot, newCompilation, oldTree, newTree, out List<string> rude);

				// a structural change (added/removed member, changed signature) can push a delta that
				// crashes the app; skip it and keep the baseline, the user restarts to pick it up
				if (rude.Count > 0)
				{
					foreach (string note in rude)
						log($"  ! {note}");
					log($"  {Path.GetFileName(path)}: structural change, restart to apply (not hot-reloaded)");
					return;
				}

				if (edits.Count == 0)
				{
					snapshot = newCompilation;
					trees[path] = newTree;
					return;
				}

				using MemoryStream metadata = new();
				using MemoryStream il = new();
				using MemoryStream pdb = new();

				List<MethodDefinitionHandle> updated = [];
				EmitDifferenceResult result = newCompilation.EmitDifference(baseline.Emit, edits, metadata, il, pdb, updated, CancellationToken.None);
				if (!result.Success)
				{
					log($"  emit failed for {Path.GetFileName(path)}");
					return;
				}

				if (sdb is not SdbConnection connection)
				{
					log("  no debug session yet");
					return;
				}

				if (module == 0)
				{
					domain = connection.RootDomain();
					module = connection.FindModule(domain, assemblyName);
				}
				if (module == 0)
				{
					log("  app module not found yet, try again");
					return;
				}

				int error = connection.ApplyChanges(domain, module, metadata.ToArray(), il.ToArray(), pdb.ToArray());
				if (error != 0)
				{
					log($"  APPLY_CHANGES error {error} for {Path.GetFileName(path)}");
					return;
				}

				SignalReload();
				log($"  {Path.GetFileName(path)}: {edits.Count} edit(s), reloaded");

				baseline.Emit = result.Baseline;
				snapshot = newCompilation;
				trees[path] = newTree;
			}
		}

		watcher.Changed += OnChanged;
		watcher.Created += OnChanged;
		watcher.Renamed += (sender, change) => OnChanged(sender, change);
		watcher.EnableRaisingEvents = true;

		lifetime.OnTermination(() => watcher.Dispose());
	}

	void SignalReload()
	{
		try
		{
			reloadClient?.Send(new byte[28]);
		}
		catch { }
	}

	static void DumbRelay(
		Socket a,
		Socket b)
	{
		Pump(a, b);
		Pump(b, a);
	}

	static void Pump(
		Socket from,
		Socket to)
	{
		Thread thread = new(() =>
		{
			try
			{
				byte[] buffer = new byte[8192];
				while (true)
				{
					int read = from.Receive(buffer);
					if (read == 0)
						break;

					to.Send(buffer.AsSpan(0, read).ToArray());
				}
			}
			catch { }
			finally
			{
				// propagate the disconnect so the peer (and Rider's session) tears down
				Close(from);
				Close(to);
			}
		})
		{
			IsBackground = true,
			Name = "skele-dumb-pump"
		};
		thread.Start();
	}

	static void Accept(
		Socket listener,
		Action<Socket> onAccept)
	{
		Thread thread = new(() =>
		{
			try
			{
				while (true)
					onAccept(listener.Accept());
			}
			catch { }
		})
		{
			IsBackground = true,
			Name = "skele-accept"
		};
		thread.Start();
	}

	static Socket Bind(
		int port)
	{
		Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		listener.Bind(new IPEndPoint(IPAddress.Loopback, port));
		listener.Listen(8);

		return listener;
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

	static string ReadStable(
		string path)
	{
		for (int attempt = 0; ; attempt++)
		{
			try
			{
				using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using StreamReader reader = new(stream);

				return reader.ReadToEnd();
			}
			catch (IOException) when (attempt < 10)
			{
				Thread.Sleep(30);
			}
		}
	}
}
