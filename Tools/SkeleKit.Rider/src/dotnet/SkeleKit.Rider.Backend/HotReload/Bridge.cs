using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using JetBrains.Lifetimes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

// The in-backend sdb proxy: becomes the app's Mono soft-debugger so it can apply EnC deltas over the
// debugger connection (the only path allowed while a debugger is attached). Rider attaches to the IDE
// port; the bridge relays sdb traffic so breakpoints work and injects apply-changes on the side. On a
// source save it re-emits the delta with the host's Roslyn (EmitDifference) and applies it.
//
// Adapted from Tools/SkeleKit.HotReload/Sdb/DebugBridge: Start() binds the listeners and returns the
// ports, then runs accept/relay/watch on background threads tied to the solution lifetime.
sealed class Bridge
{
	const int AppPort = 9987;
	const int IdePort = 9986;
	const int ReloadPort = 9988;

	readonly string cscArgs;
	readonly string deployedDll;
	readonly string projectDir;
	readonly Action<string> log;

	Socket? ideListener;
	Socket? reloadClient;
	int domain;
	int module;

	public Bridge(
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

	// Binds the app/IDE/reload listeners and returns (idePort, appPort). The app connects to appPort;
	// Rider attaches to idePort. The Roslyn baseline, app accept, relay, and file watch run on a
	// background thread that unwinds when the lifetime terminates.
	public (int IdePort, int AppPort) Start(
		Lifetime lifetime)
	{
		Socket appListener = Bind(AppPort);
		ideListener = Bind(IdePort);
		Socket reloadListener = Bind(ReloadPort);

		lifetime.OnTermination(() =>
		{
			Close(appListener);
			Close(ideListener);
			Close(reloadListener);
			Close(reloadClient);
		});

		AcceptReloadChannel(reloadListener);

		Thread thread = new(() =>
		{
			try
			{
				Run(lifetime, appListener);
			}
			catch (Exception exception)
			{
				log($"bridge stopped: {exception.Message}");
			}
		})
		{
			IsBackground = true,
			Name = "skele-bridge"
		};
		thread.Start();

		return (IdePort, AppPort);
	}

	// Temporary de-risking entry: run only the Roslyn engine (compile the app on the host Roslyn,
	// baseline off the deployed dll, smoke-emit a delta) with no sockets. Remove once the frontend
	// executor drives the real flow.
	public void SelfTest()
	{
		log($"self-test: building compilation from {Path.GetFileName(cscArgs)}...");
		CscInvocation csc = CscInvocation.Load(cscArgs, projectDir);
		Project project = Project.Build(csc);
		Compilation compilation = project.Compilation;

		Diagnostic[] errors = [.. compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)];
		log($"self-test: {compilation.SyntaxTrees.Count()} trees, {errors.Length} errors");
		if (errors.Length > 0)
		{
			log($"  first error: {errors[0]}");
			return;
		}

		Baseline baseline = new(deployedDll, compilation);
		log($"self-test: baseline MVID {baseline.Mvid}");

		SmokeEmit(compilation, baseline);
	}

	void Run(
		Lifetime lifetime,
		Socket appListener)
	{
		log($"building compilation from {Path.GetFileName(cscArgs)}...");
		CscInvocation csc = CscInvocation.Load(cscArgs, projectDir);
		Project project = Project.Build(csc);
		Compilation compilation = project.Compilation;

		Diagnostic[] errors = [.. compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)];
		if (errors.Length > 0)
		{
			log($"compilation has {errors.Length} errors, cannot baseline; first: {errors[0]}");
			return;
		}

		Baseline baseline = new(deployedDll, compilation);
		log($"baseline ready (MVID {baseline.Mvid})");

		SmokeEmit(compilation, baseline);

		log($"waiting for the app on 127.0.0.1:{AppPort}");
		SdbConnection sdb = AcceptApp(appListener);

		log($"app connected (suspended); attach Rider \"Mono Remote\" to 127.0.0.1:{IdePort}");
		Socket ide = ideListener!.Accept();
		sdb.Relay(ide);
		log("IDE attached — breakpoints + hot reload. Edit a .cs file to reload.");

		Watch(lifetime, compilation, sdb, baseline, csc.AssemblyName);
	}

	// Proves the whole delta engine runs on the host's Roslyn: parse the app, baseline off the deployed
	// dll, force a body edit, and EmitDifference. Logs the delta sizes. Temporary de-risking scaffold.
	void SmokeEmit(
		Compilation compilation,
		Baseline baseline)
	{
		SyntaxTree? oldTree = compilation.SyntaxTrees.FirstOrDefault(tree =>
			!string.IsNullOrEmpty(tree.FilePath)
			&& File.Exists(tree.FilePath)
			&& tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => method.Body is not null));

		if (oldTree is null)
		{
			log("smoke-emit: no method with a block body found");
			return;
		}

		MethodDeclarationSyntax target = oldTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First(method => method.Body is not null);
		int insert = target.Body!.OpenBraceToken.Span.End;
		string oldText = oldTree.ToString();
		string newText = oldText.Substring(0, insert) + "/*skele-smoke*/" + oldText.Substring(insert);

		SyntaxTree newTree = CSharpSyntaxTree.ParseText(SourceText.From(newText, Encoding.UTF8), (CSharpParseOptions)oldTree.Options, oldTree.FilePath);
		Compilation newCompilation = compilation.ReplaceSyntaxTree(oldTree, newTree);
		List<SemanticEdit> edits = Differ.Edits(compilation, newCompilation, oldTree, newTree, out List<string> rude);

		log($"smoke-emit: target={Path.GetFileName(oldTree.FilePath)} edits={edits.Count} rude={rude.Count}");
		if (edits.Count == 0)
			return;

		using MemoryStream metadata = new();
		using MemoryStream il = new();
		using MemoryStream pdb = new();

		List<MethodDefinitionHandle> updated = [];
		EmitDifferenceResult result = newCompilation.EmitDifference(baseline.Emit, edits, metadata, il, pdb, updated, CancellationToken.None);

		log($"smoke-emit: success={result.Success} meta={metadata.Length} il={il.Length} pdb={pdb.Length} updated={updated.Count}");
		foreach (Diagnostic diagnostic in result.Diagnostics.Take(3))
			log($"  smoke-emit diag: {diagnostic}");
	}

	void EnsureModule(
		SdbConnection sdb,
		string assemblyName)
	{
		if (module != 0)
			return;

		domain = sdb.RootDomain();
		module = sdb.FindModule(domain, assemblyName);
	}

	void Watch(
		Lifetime lifetime,
		Compilation compilation,
		SdbConnection sdb,
		Baseline baseline,
		string assemblyName)
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

				foreach (string note in rude)
					log($"  ! {note}");

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

				EnsureModule(sdb, assemblyName);
				if (module == 0)
				{
					log("  app module not found yet, try again");
					return;
				}

				int error = sdb.ApplyChanges(domain, module, metadata.ToArray(), il.ToArray(), pdb.ToArray());
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

	SdbConnection AcceptApp(
		Socket appListener)
	{
		// the app opens the debugger connection first, then one more for stdout/stderr
		SdbConnection debug = SdbConnection.Adopt(appListener.Accept());

		Thread thread = new(() =>
		{
			try
			{
				SdbConnection.PipeOutput(appListener.Accept());
			}
			catch { }
		})
		{
			IsBackground = true
		};
		thread.Start();

		return debug;
	}

	void AcceptReloadChannel(
		Socket reloadListener)
	{
		Thread thread = new(() =>
		{
			try
			{
				while (true)
					reloadClient = reloadListener.Accept();
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

	static Socket Bind(
		int port)
	{
		Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		listener.Bind(new IPEndPoint(IPAddress.Loopback, port));
		listener.Listen(4);

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
