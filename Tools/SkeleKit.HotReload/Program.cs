using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.HotReload;

static class Program
{
	const int Port = 9988;

	static int Main(
		string[] args)
	{
		if (args is ["bridge", string bridgeArgs, string bridgeDll, string bridgeProjectDir, ..])
			return new Sdb.DebugBridge(CscInvocation.Load(bridgeArgs, bridgeProjectDir), bridgeDll, bridgeProjectDir).Run();

		if (args.Length < 2)
		{
			Console.WriteLine("usage: skele-hotreload <cscargs> <deployed.dll> [projectDir]");
			Console.WriteLine("       skele-hotreload bridge <cscargs> <deployed.dll> <projectDir>");
			return 1;
		}

		string projectDir = args.Length > 2 ? args[2] : Path.GetDirectoryName(Path.GetFullPath(args[0]))!;
		CscInvocation csc = CscInvocation.Load(args[0], projectDir);
		string deployedDll = args[1];

		Server? server = Server.Bind(Port);
		if (server is null)
		{
			Console.WriteLine($"a host is already running on 127.0.0.1:{Port}, exiting");
			return 0;
		}

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

		server.Accept();
		Console.WriteLine($"listening on 127.0.0.1:{Port} — waiting for the app, edit a .cs file to hot reload");

		Dictionary<string, SyntaxTree> trees = compilation.SyntaxTrees
			.Where(tree => !string.IsNullOrEmpty(tree.FilePath) && File.Exists(tree.FilePath))
			.GroupBy(tree => Path.GetFullPath(tree.FilePath), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

		object gate = new();
		using FileSystemWatcher watcher = new(csc.ProjectDir)
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
					Microsoft.CodeAnalysis.Text.SourceText.From(text, System.Text.Encoding.UTF8),
					(CSharpParseOptions)oldTree.Options,
					path);
				Compilation newCompilation = compilation.ReplaceSyntaxTree(oldTree, newTree);

				List<SemanticEdit> edits = Differ.Edits(compilation, newCompilation, oldTree, newTree, out List<string> rude);
				foreach (string note in rude)
					Console.WriteLine($"  ! {note}");

				if (edits.Count == 0)
				{
					if (rude.Count == 0)
						Console.WriteLine($"  {Path.GetFileName(path)}: no reloadable change");

					compilation = newCompilation;
					trees[path] = newTree;
					return;
				}

				Apply(ref baseline, ref compilation, ref trees, server, newCompilation, newTree, oldTree, path, edits);
			}
		}

		watcher.Changed += OnChanged;
		watcher.Created += OnChanged;
		watcher.Renamed += (sender, change) => OnChanged(sender, change);
		watcher.EnableRaisingEvents = true;

		// auto-start leaves this process detached, so it must never linger: exit once the app has been
		// gone for a while (the next build restarts a fresh one)
		TimeSpan idle = TimeSpan.FromMinutes(15);
		while (true)
		{
			Thread.Sleep(TimeSpan.FromSeconds(30));

			if (!server.EverConnected)
				continue;

			if (!server.HasClient && DateTime.UtcNow - server.LastActivity > idle)
			{
				Console.WriteLine("idle, exiting");
				return 0;
			}
		}
	}

	static void Apply(
		ref Baseline baseline,
		ref Compilation compilation,
		ref Dictionary<string, SyntaxTree> trees,
		Server server,
		Compilation newCompilation,
		SyntaxTree newTree,
		SyntaxTree oldTree,
		string path,
		List<SemanticEdit> edits)
	{
		using MemoryStream metadata = new();
		using MemoryStream il = new();
		using MemoryStream pdb = new();

		List<System.Reflection.Metadata.MethodDefinitionHandle> updated = [];
		EmitDifferenceResult result = newCompilation.EmitDifference(baseline.Emit, edits, metadata, il, pdb, updated, CancellationToken.None);

		if (!result.Success)
		{
			Console.WriteLine($"  emit failed for {Path.GetFileName(path)}:");
			foreach (Diagnostic diagnostic in result.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).Take(5))
				Console.WriteLine($"    {diagnostic}");
			return;
		}

		bool sent = server.Send(baseline.Mvid, metadata.ToArray(), il.ToArray(), pdb.ToArray());
		Console.WriteLine($"  {Path.GetFileName(path)}: {edits.Count} edit(s), delta {metadata.Length + il.Length}B → {(sent ? "sent" : "no app connected")}");

		baseline.Emit = result.Baseline;
		compilation = newCompilation;
		trees[path] = newTree;
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
