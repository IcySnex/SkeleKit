using System.Text.RegularExpressions;

namespace SkeleKit.Rider.Backend.HotReload;

// A .NET iOS app in the solution that hot reload can target, plus where its last build landed.
//
// Discovery reads the solution and project files as text rather than going through Rider's project
// model. The model would need the solution to be fully loaded, a read lock, and API that shifts
// between Rider releases; the files give the same two answers (which projects exist, which ones
// target iOS) with none of that, and they are readable the moment the component starts.
sealed class AppProject
{
	static readonly Regex TargetFrameworkPattern = new(
		@"<TargetFrameworks?>([^<]*)</TargetFrameworks?>",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	static readonly Regex AssemblyNamePattern = new(
		@"<AssemblyName>([^<]*)</AssemblyName>",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	static readonly Regex OutputTypePattern = new(
		@"<OutputType>\s*(Exe|WinExe)\s*</OutputType>",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	static readonly Regex SlnProjectPattern = new(
		"^Project\\(\"\\{[^}]*\\}\"\\)\\s*=\\s*\"[^\"]*\",\\s*\"([^\"]+)\"",
		RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

	static readonly Regex SlnxProjectPattern = new(
		"<Project\\s+Path=\"([^\"]+)\"",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	static readonly Regex ProjectReferencePattern = new(
		"<ProjectReference\\b[^>]*\\bInclude=\"([^\"]+)\"[^>]*>",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	static readonly Regex ReferenceOutputAssemblyFalsePattern = new(
		"\\bReferenceOutputAssembly\\s*=\\s*\"false\"",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public required string ProjectFile { get; init; }
	public required string ProjectDir { get; init; }
	public required string AssemblyName { get; init; }
	public required string DeployedDll { get; init; }
	public required string Configuration { get; init; }
	public required string TargetFramework { get; init; }
	public required string RuntimeIdentifier { get; init; }
	public required bool IsExecutable { get; init; }

	public override string ToString() =>
		$"{AssemblyName} ({Configuration}/{TargetFramework}/{RuntimeIdentifier})";

	// Every .NET iOS project in the solution that has a build we could hot reload. A project with no
	// build output yet is left out; it comes back on the next pass, after the user builds.
	public static List<AppProject> Discover(
		string solutionFile)
	{
		List<AppProject> found = [];

		foreach (string projectFile in ProjectFiles(solutionFile))
		{
			AppProject? project = TryLoad(projectFile, requireIos: true, preferTargetFramework: null);
			if (project is AppProject candidate)
				found.Add(candidate);
		}

		return found;
	}

	// Whether the solution has a .NET iOS project at all, regardless of whether it has been built. This
	// is the gate for taking over the debug ports, so a freshly cloned solution is ready on its first
	// Debug rather than only after a build and a restart.
	public static bool AnyIosProject(
		string solutionFile)
	{
		foreach (string projectFile in ProjectFiles(solutionFile))
		{
			try
			{
				if (TargetsIos(File.ReadAllText(projectFile)))
					return true;
			}
			catch { }
		}

		return false;
	}

	// The app plus every project it references, transitively. An edit in a referenced library reloads
	// the same way an edit in the app does, since each assembly carries its own baseline and module.
	public static List<AppProject> WithReferences(
		AppProject app)
	{
		List<AppProject> projects = [app];
		HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase) { app.ProjectFile };
		Queue<string> queue = new();
		queue.Enqueue(app.ProjectFile);

		while (queue.Count > 0)
		{
			foreach (string reference in ProjectReferences(queue.Dequeue()))
			{
				if (!seen.Add(reference))
					continue;

				queue.Enqueue(reference);

				AppProject? project = TryLoad(reference, requireIos: false, preferTargetFramework: app.TargetFramework);
				if (project is AppProject candidate)
					projects.Add(UseAppDeployment(candidate, app));
			}
		}

		return projects;
	}

	// Referenced projects are linked into the deployed app and can have a different MVID from their
	// loose bin output. Baseline against the exact copy the runtime loaded; otherwise every library edit
	// is rejected as a build mismatch even though app-project edits work.
	static AppProject UseAppDeployment(
		AppProject project,
		AppProject app)
	{
		string outputDirectory = Path.GetDirectoryName(app.DeployedDll)!;
		string appDirectory = outputDirectory.EndsWith(".app", StringComparison.OrdinalIgnoreCase)
			? outputDirectory
			: Path.Combine(outputDirectory, app.AssemblyName + ".app");
		string deployed = Path.Combine(appDirectory, project.AssemblyName + ".dll");
		if (!File.Exists(deployed))
			return project;

		return new()
		{
			ProjectFile = project.ProjectFile,
			ProjectDir = project.ProjectDir,
			AssemblyName = project.AssemblyName,
			DeployedDll = deployed,
			Configuration = project.Configuration,
			TargetFramework = project.TargetFramework,
			RuntimeIdentifier = project.RuntimeIdentifier,
			IsExecutable = false
		};
	}

	static List<string> ProjectReferences(
		string projectFile)
	{
		List<string> references = [];
		string projectDir = Path.GetDirectoryName(projectFile)!;

		string text;
		try
		{
			text = File.ReadAllText(projectFile);
		}
		catch
		{
			return references;
		}

		foreach (Match match in ProjectReferencePattern.Matches(text))
		{
			// Analyzer/source-generator projects are build inputs, not runtime modules. Watching and
			// warming them only delays readiness, then module resolution inevitably fails.
			if (ReferenceOutputAssemblyFalsePattern.IsMatch(match.Value))
				continue;

			string path = match.Groups[1].Value.Replace('\\', Path.DirectorySeparatorChar);
			if (path.IndexOf('$') >= 0)
				continue;

			try
			{
				string full = Path.GetFullPath(Path.Combine(projectDir, path));
				if (File.Exists(full))
					references.Add(full);
			}
			catch { }
		}

		return references;
	}

	static AppProject? TryLoad(
		string projectFile,
		bool requireIos,
		string? preferTargetFramework)
	{
		string text;
		try
		{
			text = File.ReadAllText(projectFile);
		}
		catch
		{
			return null;
		}

		if (requireIos && !TargetsIos(text))
			return null;

		string projectDir = Path.GetDirectoryName(projectFile)!;
		Match assemblyName = AssemblyNamePattern.Match(text);
		string name = assemblyName.Success && assemblyName.Groups[1].Value.Trim().Length > 0
			? assemblyName.Groups[1].Value.Trim()
			: Path.GetFileNameWithoutExtension(projectFile);

		string? dll = NewestBuildOutput(projectDir, name, preferTargetFramework);
		if (dll is null)
			return null;

		(string configuration, string targetFramework, string runtimeIdentifier) = SplitOutputPath(projectDir, dll);

		return new()
		{
			ProjectFile = projectFile,
			ProjectDir = projectDir,
			AssemblyName = name,
			DeployedDll = dll,
			Configuration = configuration,
			TargetFramework = targetFramework,
			RuntimeIdentifier = runtimeIdentifier,
			IsExecutable = OutputTypePattern.IsMatch(text)
				|| dll.IndexOf($"{Path.DirectorySeparatorChar}{name}.app{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) >= 0
		};
	}

	static bool TargetsIos(
		string projectText)
	{
		foreach (Match match in TargetFrameworkPattern.Matches(projectText))
			foreach (string framework in match.Groups[1].Value.Split(';'))
				if (framework.Trim().IndexOf("-ios", StringComparison.OrdinalIgnoreCase) >= 0)
					return true;

		return false;
	}

	// The dll Rider just deployed. Picking the newest match instead of composing the path from
	// Configuration/TargetFramework/RuntimeIdentifier means we never have to guess which of those the
	// run configuration used. A multi-targeting library has one output per framework, so the app's own
	// framework decides which is the deployed one.
	static string? NewestBuildOutput(
		string projectDir,
		string assemblyName,
		string? preferTargetFramework)
	{
		string bin = Path.Combine(projectDir, "bin");
		if (!Directory.Exists(bin))
			return null;

		List<string> candidates;
		try
		{
			candidates = [.. Directory.EnumerateFiles(bin, assemblyName + ".dll", SearchOption.AllDirectories)];
		}
		catch
		{
			return null;
		}

		if (preferTargetFramework is string framework && framework.Length > 0)
		{
			string segment = Path.DirectorySeparatorChar + framework + Path.DirectorySeparatorChar;
			List<string> matching = [.. candidates.Where(path => path.IndexOf(segment, StringComparison.OrdinalIgnoreCase) >= 0)];

			if (matching.Count > 0)
				candidates = matching;
		}

		string? newest = null;
		DateTime newestWrite = DateTime.MinValue;

		foreach (string candidate in candidates)
		{
			DateTime written = File.GetLastWriteTimeUtc(candidate);
			if (written <= newestWrite)
				continue;

			newest = candidate;
			newestWrite = written;
		}

		return newest;
	}

	// bin/<Configuration>/<TargetFramework>/<RuntimeIdentifier>/<App>.dll, with the last two optional.
	static (string Configuration, string TargetFramework, string RuntimeIdentifier) SplitOutputPath(
		string projectDir,
		string dll)
	{
		string relative = dll.Substring(Path.Combine(projectDir, "bin").Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		string[] segments = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

		string configuration = segments.Length > 1 ? segments[0] : "Debug";
		string targetFramework = segments.Length > 2 ? segments[1] : "";
		string runtimeIdentifier = segments.Length > 3 ? segments[2] : "";

		return (configuration, targetFramework, runtimeIdentifier);
	}

	static List<string> ProjectFiles(
		string solutionFile)
	{
		List<string> files = [];
		string solutionDir = Path.GetDirectoryName(Path.GetFullPath(solutionFile))!;

		string text;
		try
		{
			text = File.ReadAllText(solutionFile);
		}
		catch
		{
			return GlobProjectFiles(solutionDir);
		}

		Regex pattern = solutionFile.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase)
			? SlnxProjectPattern
			: SlnProjectPattern;

		foreach (Match match in pattern.Matches(text))
		{
			string path = match.Groups[1].Value.Replace('\\', Path.DirectorySeparatorChar);
			if (!path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
				continue;

			string full = Path.GetFullPath(Path.Combine(solutionDir, path));
			if (File.Exists(full))
				files.Add(full);
		}

		// a directory-based solution, or a format we did not recognize
		return files.Count > 0 ? files : GlobProjectFiles(solutionDir);
	}

	static List<string> GlobProjectFiles(
		string solutionDir)
	{
		try
		{
			return [.. Directory.EnumerateFiles(solutionDir, "*.csproj", SearchOption.AllDirectories)
				.Where(path => path.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal) < 0
					&& path.IndexOf($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal) < 0)];
		}
		catch
		{
			return [];
		}
	}
}
