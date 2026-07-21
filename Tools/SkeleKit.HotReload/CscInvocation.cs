using System.Text.Json;

namespace SkeleKit.HotReload;

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
		string jsonPath)
	{
		string projectDir = Path.GetDirectoryName(Path.GetFullPath(jsonPath))!;

		using JsonDocument document = JsonDocument.Parse(File.ReadAllText(jsonPath));
		string[] raw = [.. document.RootElement
			.GetProperty("Items")
			.GetProperty("CscCommandLineArgs")
			.EnumerateArray()
			.Select(item => item.GetProperty("Identity").GetString()!)];

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
				references.Add(Rooted(projectDir, Unquote(arg["/reference:".Length..])));
			else if (arg.StartsWith("/analyzer:"))
				generators.Add(Rooted(projectDir, Unquote(arg["/analyzer:".Length..])));
			else if (arg.StartsWith("/embed:"))
				sources.Add(Rooted(projectDir, Unquote(arg["/embed:".Length..])));
			else if (arg.StartsWith("/define:"))
				defines.AddRange(arg["/define:".Length..].Split(';', StringSplitOptions.RemoveEmptyEntries));
			else if (arg.StartsWith("/langversion:"))
				langVersion = arg["/langversion:".Length..];
			else if (arg is "/unsafe+" or "/unsafe")
				allowUnsafe = true;
			else if (arg.StartsWith("/out:"))
				assemblyName = Path.GetFileNameWithoutExtension(Unquote(arg["/out:".Length..]));
			else if (!arg.StartsWith('/') && arg.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
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

	static string Unquote(
		string value) =>
		value.Trim('"');

	static string Rooted(
		string projectDir,
		string path) =>
		Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(projectDir, path));
}
