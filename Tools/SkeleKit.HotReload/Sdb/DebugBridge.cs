using System.Net;
using System.Net.Sockets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.HotReload.Sdb;

// The unified hot-reload path: becomes the app's Mono soft-debugger, so it can apply EnC deltas over
// the debugger connection (the only path allowed while a debugger is attached). If an IDE attaches
// (Rider "Mono Remote" → the ide port), the bridge relays the sdb stream so breakpoints work and
// injects its apply-changes on the side; if none attaches, it drives the app itself (hot reload only).
sealed class DebugBridge
{
	const int AppPort = 9987;
	const int IdePort = 9986;
	const int ReloadPort = 9988;

	readonly CscInvocation csc;
	readonly string deployedDll;
	readonly string projectDir;
	readonly bool selfDrive;

	Socket? ideListener;
	Socket? reloadClient;
	int domain;
	int module;

	public DebugBridge(
		CscInvocation csc,
		string deployedDll,
		string projectDir,
		bool selfDrive)
	{
		this.csc = csc;
		this.deployedDll = deployedDll;
		this.projectDir = projectDir;
		this.selfDrive = selfDrive;
	}

	public int Run()
	{
		Console.WriteLine($"building compilation ({csc.Sources.Count} sources, {csc.References.Count} refs)...");
		Project project = Project.Build(csc);
		Compilation compilation = project.Compilation;

		Diagnostic[] errors = [.. compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)];
		if (errors.Length > 0)
		{
			Console.WriteLine($"compilation has {errors.Length} errors, cannot baseline:");
			foreach (Diagnostic error in errors.Take(5))
				Console.WriteLine($"  {error}");

			return 2;
		}

		Baseline baseline = new(deployedDll, compilation);
		Console.WriteLine($"baseline ready (MVID {baseline.Mvid})");

		AcceptReloadChannel();
		StartIdeListener();

		Console.WriteLine($"waiting for the app's debugger on 127.0.0.1:{AppPort}...");
		SdbConnection sdb = AcceptApp();

		if (selfDrive)
		{
			sdb.SelfDrive();
			(_, int major, int minor) = sdb.Version();
			sdb.SetProtocolVersion(major, minor);
			sdb.Resume();
			Thread.Sleep(2000);
			EnsureModule(sdb);
			Console.WriteLine("no IDE — hot reload only. Edit a .cs file to reload.");
		}
		else
		{
			Console.WriteLine($"app ready (suspended) — attach Rider \"Mono Remote\" to 127.0.0.1:{IdePort} whenever (no rush)");
			Socket ide = WaitForIde();
			sdb.Relay(ide);
			Console.WriteLine("IDE attached — breakpoints + hot reload. Edit a .cs file to reload.");
		}

		Watch(compilation, sdb, baseline);
		return 0;
	}

	void EnsureModule(
		SdbConnection sdb)
	{
		if (module != 0)
			return;

		domain = sdb.RootDomain();
		module = sdb.FindModule(domain, csc.AssemblyName);
	}

	void Watch(
		Compilation compilation,
		SdbConnection sdb,
		Baseline baseline)
	{
		Dictionary<string, SyntaxTree> trees = compilation.SyntaxTrees
			.Where(tree => !string.IsNullOrEmpty(tree.FilePath) && File.Exists(tree.FilePath))
			.GroupBy(tree => Path.GetFullPath(tree.FilePath), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

		object gate = new();
		Compilation snapshot = compilation;

		using FileSystemWatcher watcher = new(projectDir)
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
					SourceText.From(text, System.Text.Encoding.UTF8),
					(CSharpParseOptions)oldTree.Options,
					path);

				Compilation newCompilation = snapshot.ReplaceSyntaxTree(oldTree, newTree);
				List<SemanticEdit> edits = Differ.Edits(snapshot, newCompilation, oldTree, newTree, out List<string> rude);

				foreach (string note in rude)
					Console.WriteLine($"  ! {note}");

				if (edits.Count == 0)
				{
					snapshot = newCompilation;
					trees[path] = newTree;
					return;
				}

				using MemoryStream metadata = new();
				using MemoryStream il = new();
				using MemoryStream pdb = new();

				List<System.Reflection.Metadata.MethodDefinitionHandle> updated = [];
				EmitDifferenceResult result = newCompilation.EmitDifference(baseline.Emit, edits, metadata, il, pdb, updated, CancellationToken.None);
				if (!result.Success)
				{
					Console.WriteLine($"  emit failed for {Path.GetFileName(path)}");
					return;
				}

				// the app has assemblies loaded by now (the IDE resumed it), so the lookup succeeds
				EnsureModule(sdb);
				if (module == 0)
				{
					Console.WriteLine("  app module not found yet, try again");
					return;
				}

				int error = sdb.ApplyChanges(domain, module, metadata.ToArray(), il.ToArray(), pdb.ToArray());
				if (error != 0)
				{
					Console.WriteLine($"  APPLY_CHANGES error {error} for {Path.GetFileName(path)}");
					return;
				}

				SignalReload();
				Console.WriteLine($"  {Path.GetFileName(path)}: {edits.Count} edit(s), reloaded");

				baseline.Emit = result.Baseline;
				snapshot = newCompilation;
				trees[path] = newTree;
			}
		}

		watcher.Changed += OnChanged;
		watcher.Created += OnChanged;
		watcher.Renamed += (sender, change) => OnChanged(sender, change);
		watcher.EnableRaisingEvents = true;

		Thread.Sleep(Timeout.Infinite);
	}

	SdbConnection AcceptApp()
	{
		Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		listener.Bind(new IPEndPoint(IPAddress.Loopback, AppPort));
		listener.Listen(4);

		// the app opens the debugger connection first, then one more for stdout/stderr; take exactly
		// those two and stop listening (leaving it open lets the app storm connections until it runs
		// out of file descriptors)
		SdbConnection debug = SdbConnection.Adopt(listener.Accept());

		Thread thread = new(() =>
		{
			try
			{
				SdbConnection.PipeOutput(listener.Accept());
			}
			catch { }
			finally
			{
				listener.Dispose();
			}
		})
		{
			IsBackground = true
		};
		thread.Start();

		return debug;
	}

	void StartIdeListener()
	{
		ideListener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		ideListener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		ideListener.Bind(new IPEndPoint(IPAddress.Loopback, IdePort));
		ideListener.Listen(1);
	}

	Socket WaitForIde() => ideListener!.Accept();

	void AcceptReloadChannel()
	{
		Thread thread = new(() =>
		{
			try
			{
				Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				listener.Bind(new IPEndPoint(IPAddress.Loopback, ReloadPort));
				listener.Listen(1);

				while (true)
					reloadClient = listener.Accept();
			}
			catch { }
		})
		{
			IsBackground = true
		};

		thread.Start();
	}

	void SignalReload()
	{
		try
		{
			reloadClient?.Send(new byte[28]);
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
