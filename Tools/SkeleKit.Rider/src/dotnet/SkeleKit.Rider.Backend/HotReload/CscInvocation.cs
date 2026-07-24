namespace SkeleKit.Rider.Backend.HotReload;

sealed class CscInvocation
{
	public required string ProjectDir { get; init; }
	public required string AssemblyName { get; init; }
	public required List<string> Sources { get; init; }
	public required List<string> References { get; init; }
	public required List<string> Generators { get; init; }
	public required List<string> Defines { get; init; }
	public required string LangVersion { get; init; }
	public required bool AllowUnsafe { get; init; }

	public static CscInvocation Load(
		string argsPath,
		string? projectDir = null)
	{
		projectDir ??= Path.GetDirectoryName(Path.GetFullPath(argsPath))!;

		// the MSBuild target hands off @(CscCommandLineArgs) one per line
		List<string> raw = [.. File.ReadAllLines(argsPath)
			.Select(line => line.Trim())
			.Where(line => line.Length > 0)];

		List<string> sources = [];
		List<string> references = [];
		List<string> generators = [];
		List<string> defines = [];
		string langVersion = "latest";
		bool allowUnsafe = false;
		string assemblyName = "app";

		foreach (string arg in raw)
		{
			if (arg.StartsWith("/reference:"))
				references.Add(Rooted(projectDir, Unquote(Rest(arg, "/reference:"))));
			else if (arg.StartsWith("/analyzer:"))
				generators.Add(Rooted(projectDir, Unquote(Rest(arg, "/analyzer:"))));
			else if (arg.StartsWith("/embed:"))
				sources.Add(Rooted(projectDir, Unquote(Rest(arg, "/embed:"))));
			else if (arg.StartsWith("/define:"))
				defines.AddRange(Rest(arg, "/define:").Split([';'], StringSplitOptions.RemoveEmptyEntries));
			else if (arg.StartsWith("/langversion:"))
				langVersion = Rest(arg, "/langversion:");
			else if (arg is "/unsafe+" or "/unsafe")
				allowUnsafe = true;
			else if (arg.StartsWith("/out:"))
				assemblyName = Path.GetFileNameWithoutExtension(Unquote(Rest(arg, "/out:")));
			else if (!arg.StartsWith("/") && arg.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
				sources.Add(Rooted(projectDir, Unquote(arg)));
		}

		return new()
		{
			ProjectDir = projectDir,
			AssemblyName = assemblyName,
			Sources = sources,
			References = references,
			Generators = generators,
			Defines = defines,
			LangVersion = langVersion,
			AllowUnsafe = allowUnsafe
		};
	}

	static string Rest(
		string arg,
		string prefix) =>
		arg.Substring(prefix.Length);

	static string Unquote(
		string value) =>
		value.Trim('"');

	static string Rooted(
		string projectDir,
		string path) =>
		Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(projectDir, path));
}
