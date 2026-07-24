using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

// Turns a saved source file into an Edit-and-Continue delta for one assembly.
//
// One engine per assembly: the app and each project it references keep their own compilation, baseline
// and module id, which is what lets an edit in a referenced library reload as readily as one in the app.
sealed class ReloadEngine
{
	public enum Outcome
	{
		Unchanged,
		Applied,
		Skipped,
		Failed
	}

	readonly AppProject project;
	readonly Baseline baseline;
	readonly Dictionary<string, SyntaxTree> trees;

	Compilation snapshot;
	int domain;
	int module;

	ReloadEngine(
		AppProject project,
		Compilation compilation,
		Baseline baseline)
	{
		this.project = project;
		this.baseline = baseline;
		snapshot = compilation;

		trees = compilation.SyntaxTrees
			.Where(tree => !string.IsNullOrEmpty(tree.FilePath) && File.Exists(tree.FilePath))
			.GroupBy(tree => Path.GetFullPath(tree.FilePath), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
	}

	public Guid Mvid => baseline.Mvid;

	// Rebuilds the assembly's compilation and proves it matches the build the app is running. Returns
	// null when it cannot be reconstructed faithfully, because applying a delta from a compilation that
	// disagrees with the deployed assembly corrupts the running app rather than failing cleanly.
	public static ReloadEngine? Create(
		AppProject project,
		Action<string> log)
	{
		List<string>? commandLine = MsBuild.CscCommandLineArgs(project, log);
		if (commandLine is null)
			return null;

		CscInvocation csc = CscInvocation.Parse(commandLine, project.ProjectDir);

		MetadataShape? deployed = MetadataShape.OfAssembly(project.DeployedDll);
		if (deployed is null)
		{
			log($"{project.AssemblyName}: cannot read {Path.GetFileName(project.DeployedDll)}");
			return null;
		}

		// running every generator the real build ran is the faithful reconstruction; without generators
		// is the fallback for a generator we cannot host, and it still matches for most projects
		foreach (bool runGenerators in new[] { true, false })
		{
			Compilation compilation = Project.Build(csc, runGenerators, log).Compilation;

			MetadataShape? rebuilt = MetadataShape.OfCompilation(compilation);
			if (rebuilt is null)
			{
				log($"{project.AssemblyName}: the rebuilt compilation does not compile{(runGenerators ? ", retrying without source generators" : "")}");
				continue;
			}

			if (!rebuilt.Matches(deployed))
			{
				log($"{project.AssemblyName}: rebuilt {rebuilt}, deployed has {deployed}{(runGenerators ? ", retrying without source generators" : "")}");
				foreach (string missing in deployed.Missing(rebuilt).Take(5))
					log($"    missing from the rebuild: {missing}");

				continue;
			}

			return new(project, compilation, new(project.DeployedDll, compilation));
		}

		log($"{project.AssemblyName}: cannot reproduce the deployed assembly, so hot reload is off for it");

		return null;
	}

	// The app has to be running the assembly we baselined against, or every delta lands on the wrong
	// metadata rows.
	public bool Matches(
		SdbConnection connection,
		out string reason)
	{
		if (!Resolve(connection))
		{
			reason = $"{project.AssemblyName} is not loaded in the app";
			return false;
		}

		Guid running = connection.ModuleMvid(module);
		if (running == Guid.Empty || running == baseline.Mvid)
		{
			reason = "";
			return true;
		}

		reason = $"the app is running a different build of {project.AssemblyName} ({running:D}) than the one on disk ({baseline.Mvid:D}); rebuild and redeploy";
		module = 0;

		return false;
	}

	public Outcome Apply(
		string path,
		SdbConnection connection,
		Action<string> log,
		Action<string> notice)
	{
		if (!trees.TryGetValue(path, out SyntaxTree? oldTree))
			return Outcome.Unchanged;

		string text = ReadStable(path);
		if (text == oldTree.ToString())
			return Outcome.Unchanged;

		string name = Path.GetFileName(path);

		SyntaxTree newTree = CSharpSyntaxTree.ParseText(
			SourceText.From(text, Encoding.UTF8),
			(CSharpParseOptions)oldTree.Options,
			path);

		Compilation updated = snapshot.ReplaceSyntaxTree(oldTree, newTree);
		List<SemanticEdit> edits = Differ.Edits(snapshot, updated, oldTree, newTree, out List<string> rude);

		// a structural change (added or removed member, changed signature) needs metadata rows the
		// baseline does not have; applying it crashes the app, so keep the baseline and say so
		if (rude.Count > 0)
		{
			foreach (string note in rude)
				log($"  ! {note}");

			log($"  {name}: structural change, restart to apply");
			notice($"Skipped {name}: added or removed a member — restart to apply.");

			return Outcome.Skipped;
		}

		if (edits.Count == 0)
		{
			Accept(path, newTree, updated);
			return Outcome.Unchanged;
		}

		using MemoryStream metadata = new();
		using MemoryStream il = new();
		using MemoryStream pdb = new();

		// nothing is ever added: a structural edit was rejected above
		EmitDifferenceResult result = updated.EmitDifference(baseline.Emit, edits, _ => false, metadata, il, pdb, CancellationToken.None);
		if (!result.Success)
		{
			log($"  {name}: could not build the delta");
			foreach (Diagnostic diagnostic in result.Diagnostics.Where(entry => entry.Severity == DiagnosticSeverity.Error).Take(3))
				log($"    {diagnostic}");

			notice($"Hot reload failed for {name}: could not build the delta.");

			return Outcome.Failed;
		}

		// Mono's Edit-and-Continue cannot resolve a type the baseline never referenced (the first use of
		// Debug.WriteLine, say), and the app dies when the edited method runs
		if (AddsNewReferences(metadata.ToArray()))
		{
			log($"  {name}: adds a new type reference, restart to apply");
			notice($"Skipped {name}: uses a type the running build never referenced — restart to apply.");
			Accept(path, newTree, updated);

			return Outcome.Skipped;
		}

		if (!Resolve(connection))
		{
			log($"  {name}: {project.AssemblyName} is not loaded in the app yet");
			return Outcome.Failed;
		}

		int error = connection.ApplyChanges(domain, module, metadata.ToArray(), il.ToArray(), pdb.ToArray());
		if (error != 0)
		{
			log($"  {name}: APPLY_CHANGES error {error}");
			notice($"Hot reload failed for {name}: apply error {error}.");
			module = 0;

			return Outcome.Failed;
		}

		if (result.Baseline is EmitBaseline next)
			baseline.Emit = next;

		Accept(path, newTree, updated);

		log($"  {name}: {edits.Count} edit(s), reloaded");
		notice($"Hot reloaded {name}.");

		return Outcome.Applied;
	}

	void Accept(
		string path,
		SyntaxTree tree,
		Compilation compilation)
	{
		trees[path] = tree;
		snapshot = compilation;
	}

	bool Resolve(
		SdbConnection connection)
	{
		if (module != 0)
			return true;

		domain = connection.RootDomain();
		module = connection.FindModule(domain, project.AssemblyName);

		return module != 0;
	}

	// True if the delta introduces a TypeRef or AssemblyRef the baseline did not have.
	static bool AddsNewReferences(
		byte[] metadataDelta)
	{
		try
		{
			using MetadataReaderProvider provider = MetadataReaderProvider.FromMetadataStream(new MemoryStream(metadataDelta));
			MetadataReader reader = provider.GetMetadataReader();

			return reader.GetTableRowCount(TableIndex.TypeRef) > 0
				|| reader.GetTableRowCount(TableIndex.AssemblyRef) > 0;
		}
		catch
		{
			return false;
		}
	}

	// an editor may have the file open for writing the instant the watcher fires
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
