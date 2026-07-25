using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;

namespace SkeleKit.Rider.Backend.HotReload;

internal sealed class CscInvocation
{
	static Dictionary<string, Version> AssemblyReferences(
		string path)
	{
		Dictionary<string, Version> references = new(StringComparer.OrdinalIgnoreCase);

		try
		{
			using FileStream stream = File.OpenRead(path);
			using PEReader pe = new(stream);
			MetadataReader metadata = pe.GetMetadataReader();

			foreach (AssemblyReferenceHandle handle in metadata.AssemblyReferences)
			{
				AssemblyReference reference = metadata.GetAssemblyReference(handle);
				references[metadata.GetString(reference.Name)] = reference.Version;
			}
		}
		catch
		{
			// ignoreeee :3
		}

		return references;
	}

	static AssemblyName? AssemblyIdentity(
		string path)
	{
		if (!File.Exists(path))
			return null;

		try
		{
			return System.Reflection.AssemblyName.GetAssemblyName(path);
		}
		catch
		{
			return null;
		}
	}

	static void AddFeature(
		Dictionary<string, string> features,
		string feature)
	{
		int separator = feature.IndexOf('=');

		features[separator < 0 ? feature : feature.Substring(0, separator)] =
			separator < 0 ? "true" : feature.Substring(separator + 1);
	}

	static OutputKind Kind(
		string target) =>
		target.ToLowerInvariant() switch
		{
			"library" => OutputKind.DynamicallyLinkedLibrary,
			"module" => OutputKind.NetModule,
			"winexe" => OutputKind.WindowsApplication,
			"winmdobj" => OutputKind.WindowsRuntimeMetadata,
			_ => OutputKind.ConsoleApplication
		};

	static NullableContextOptions Nullability(
		string mode) =>
		mode.ToLowerInvariant() switch
		{
			"enable" => NullableContextOptions.Enable,
			"warnings" => NullableContextOptions.Warnings,
			"annotations" => NullableContextOptions.Annotations,
			_ => NullableContextOptions.Disable
		};

	static string? Value(
		string arg,
		string prefix) =>
		arg.StartsWith(prefix, StringComparison.Ordinal) ? Unquote(arg.Substring(prefix.Length)) : null;

	static string Unquote(
		string value) =>
		value.Trim('"');

	static string Rooted(
		string projectDir,
		string path) =>
		Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(projectDir, path));


	public static CscInvocation Parse(
		IEnumerable<string> commandLine,
		string projectDir)
	{
		List<string> sources = [];
		List<string> references = [];
		List<string> analyzers = [];
		List<string> analyzerConfigs = [];
		List<string> additionalFiles = [];
		List<string> defines = [];
		Dictionary<string, string> features = new(StringComparer.Ordinal);
		string langVersion = "latest";
		bool allowUnsafe = false;
		bool checkOverflow = false;
		bool optimize = false;
		string assemblyName = "app";
		string? mainTypeName = null;
		OutputKind outputKind = OutputKind.ConsoleApplication;
		NullableContextOptions nullable = NullableContextOptions.Disable;

		foreach (string line in commandLine)
		{
			string arg = line.Trim();
			if (arg.Length == 0)
				continue;

			if (Value(arg, "/reference:") is string reference)
				references.Add(Rooted(projectDir, reference));
			else if (Value(arg, "/analyzer:") is string analyzer)
				analyzers.Add(Rooted(projectDir, analyzer));
			else if (Value(arg, "/analyzerconfig:") is string analyzerConfig)
				analyzerConfigs.Add(Rooted(projectDir, analyzerConfig));
			else if (Value(arg, "/additionalfile:") is string additionalFile)
				additionalFiles.Add(Rooted(projectDir, additionalFile));
			else if (Value(arg, "/embed:") is string embedded)
				sources.Add(Rooted(projectDir, embedded));
			else if (Value(arg, "/define:") is string define)
				defines.AddRange(define.Split([';'], StringSplitOptions.RemoveEmptyEntries));
			else if (Value(arg, "/features:") is string feature)
				AddFeature(features, feature);
			else if (Value(arg, "/langversion:") is string language)
				langVersion = language;
			else if (Value(arg, "/out:") is string output)
				assemblyName = Path.GetFileNameWithoutExtension(output);
			else if (Value(arg, "/main:") is string main)
				mainTypeName = main;
			else if (Value(arg, "/target:") is string target)
				outputKind = Kind(target);
			else if (Value(arg, "/nullable:") is string nullableMode)
				nullable = Nullability(nullableMode);
			else
			{
				switch (arg)
				{
					case "/nullable" or "/nullable+":
						nullable = NullableContextOptions.Enable;
						break;
					case "/unsafe" or "/unsafe+":
						allowUnsafe = true;
						break;
					case "/checked" or "/checked+":
						checkOverflow = true;
						break;
					case "/optimize" or "/optimize+" or "/o" or "/o+":
						optimize = true;
						break;
					default:
						{
							if (!arg.StartsWith("/") && arg.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
								sources.Add(Rooted(projectDir, Unquote(arg)));
							break;
						}
				}
			}
		}

		return new()
		{
			ProjectDir = projectDir,
			AssemblyName = assemblyName,
			Sources = sources,
			References = references,
			Analyzers = analyzers,
			AnalyzerConfigs = analyzerConfigs,
			AdditionalFiles = additionalFiles,
			Defines = defines,
			Features = features,
			LangVersion = langVersion,
			AllowUnsafe = allowUnsafe,
			CheckOverflow = checkOverflow,
			Optimize = optimize,
			OutputKind = outputKind,
			Nullable = nullable,
			MainTypeName = mainTypeName
		};
	}


	public required string ProjectDir { get; init; }
	public required string AssemblyName { get; init; }
	public required List<string> Sources { get; init; }
	public required List<string> References { get; init; }
	public Dictionary<string, Version> ReferenceVersionOverrides { get; } = new(StringComparer.OrdinalIgnoreCase);
	public required List<string> Analyzers { get; init; }
	public required List<string> AnalyzerConfigs { get; init; }
	public required List<string> AdditionalFiles { get; init; }
	public required List<string> Defines { get; init; }
	public required Dictionary<string, string> Features { get; init; }
	public required string LangVersion { get; init; }
	public required bool AllowUnsafe { get; init; }
	public required bool CheckOverflow { get; init; }
	public required bool Optimize { get; init; }
	public required OutputKind OutputKind { get; init; }
	public required NullableContextOptions Nullable { get; init; }
	public required string? MainTypeName { get; init; }


	public void AlignReferencesWithDeployment(
		string deployedDll,
		Action<string> log)
	{
		Dictionary<string, Version> expected = AssemblyReferences(deployedDll);
		string deployedDirectory = Path.GetDirectoryName(deployedDll)!;

		foreach (string t in References)
		{
			AssemblyName? current = AssemblyIdentity(t);
			if (current?.Name is not string name
				|| current.Version is not Version currentVersion
				|| !expected.TryGetValue(name, out Version? expectedVersion)
				|| currentVersion == expectedVersion)
				continue;

			string candidate = Path.Combine(deployedDirectory, Path.GetFileName(t));
			AssemblyName? deployed = AssemblyIdentity(candidate);
			if (deployed?.Name != name || deployed.Version != expectedVersion)
				continue;

			ReferenceVersionOverrides[t] = expectedVersion;
			log($"  retargeting {name} reference {currentVersion} to deployed {expectedVersion}");
		}
	}
}
