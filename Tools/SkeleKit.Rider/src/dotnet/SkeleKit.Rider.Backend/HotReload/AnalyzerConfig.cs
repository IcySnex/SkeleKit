using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

internal sealed class AnalyzerConfig : AnalyzerConfigOptionsProvider
{
	sealed class Options(
		Dictionary<string, string> values) : AnalyzerConfigOptions
	{
		public override IEnumerable<string> Keys => values.Keys;

		public override bool TryGetValue(
			string key,
			out string value) =>
			values.TryGetValue(key, out value!);
	}

	sealed class Text(
		string path) : AdditionalText
	{
		public override string Path { get; } = path;

		public override SourceText? GetText(
			CancellationToken cancellationToken = default)
		{
			try
			{
				return SourceText.From(File.ReadAllText(Path), Encoding.UTF8);
			}
			catch
			{
				return null;
			}
		}
	}


	static void Read(
		string path,
		Dictionary<string, string> global,
		Dictionary<string, Dictionary<string, string>> perFile)
	{
		string[] lines;
		try
		{
			lines = File.ReadAllLines(path);
		}
		catch
		{
			return;
		}

		Dictionary<string, string> preamble = new(AnalyzerConfigOptions.KeyComparer);
		List<(string Section, Dictionary<string, string> Values)> sections = [];
		Dictionary<string, string> current = preamble;

		foreach (string raw in lines)
		{
			string line = raw.Trim();
			if (line.Length == 0 || line[0] is '#' or ';')
				continue;

			if (line[0] == '[' && line[line.Length - 1] == ']')
			{
				current = new(AnalyzerConfigOptions.KeyComparer);
				sections.Add((line.Substring(1, line.Length - 2), current));
				continue;
			}

			int separator = line.IndexOf('=');
			if (separator < 0)
				continue;

			current[line.Substring(0, separator).Trim()] = line.Substring(separator + 1).Trim();
		}

		if (!preamble.TryGetValue("is_global", out string? isGlobal) || !string.Equals(isGlobal, "true", StringComparison.OrdinalIgnoreCase))
			return;

		foreach (KeyValuePair<string, string> entry in preamble)
		{
			if (string.Equals(entry.Key, "is_global", StringComparison.OrdinalIgnoreCase) || string.Equals(entry.Key, "global_level", StringComparison.OrdinalIgnoreCase))
				continue;

			global[entry.Key] = entry.Value;
		}

		// in a global config a section header is a full file path, not a glob
		foreach ((string section, Dictionary<string, string> values) in sections)
		{
			string key = Normalize(section);
			if (!perFile.TryGetValue(key, out Dictionary<string, string>? existing))
				perFile[key] = existing = new(AnalyzerConfigOptions.KeyComparer);

			foreach (KeyValuePair<string, string> entry in values)
				existing[entry.Key] = entry.Value;
		}
	}

	static string Normalize(
		string path)
	{
		try
		{
			return Path.GetFullPath(path.Replace('\\', Path.DirectorySeparatorChar));
		}
		catch
		{
			return path;
		}
	}


	public static AdditionalText[] AdditionalTexts(
		IEnumerable<string> paths) =>
		[.. paths.Where(File.Exists).Select(AdditionalText (path) => new Text(path))];

	public static AnalyzerConfig Load(
		IEnumerable<string> configPaths)
	{
		Dictionary<string, string> global = new(AnalyzerConfigOptions.KeyComparer);
		Dictionary<string, Dictionary<string, string>> perFile = new(StringComparer.OrdinalIgnoreCase);

		foreach (string path in configPaths.Where(File.Exists))
			Read(path, global, perFile);

		return new(
			new(global),
			perFile.ToDictionary(entry => entry.Key, entry => new Options(entry.Value), StringComparer.OrdinalIgnoreCase));
	}


	static readonly Options None = new([]);

	readonly Options global;
	readonly Dictionary<string, Options> perFile;

	AnalyzerConfig(
		Options global,
		Dictionary<string, Options> perFile)
	{
		this.global = global;
		this.perFile = perFile;
	}


	Options Lookup(
		string path)
	{
		if (path.Length == 0)
			return None;

		return perFile.TryGetValue(Normalize(path), out Options? options) ? options : None;
	}


	public override AnalyzerConfigOptions GlobalOptions => global;

	public override AnalyzerConfigOptions GetOptions(
		SyntaxTree tree) =>
		Lookup(tree.FilePath);

	public override AnalyzerConfigOptions GetOptions(
		AdditionalText textFile) =>
		Lookup(textFile.Path);
}
