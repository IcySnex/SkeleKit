using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Mono.Cecil;

namespace SkeleKit.Rider.Backend.HotReload;

internal sealed class Project
{
	static readonly string[] Unified =
	[
		"Microsoft.CodeAnalysis",
		"System.Collections.Immutable",
		"System.Reflection.Metadata"
	];
	static readonly ConcurrentDictionary<string, ISourceGenerator[]> GeneratorCache = new(StringComparer.OrdinalIgnoreCase);

	static int resolverInstalled;


	static MetadataReference Reference(
		string path,
		Dictionary<string, Version> versionOverrides)
	{
		if (!versionOverrides.TryGetValue(path, out Version? version))
			return MetadataReference.CreateFromFile(path);

		using AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(path, new()
		{
			InMemory = true,
			ReadingMode = ReadingMode.Immediate
		});
		assembly.Name.Version = version;

		using MemoryStream image = new();
		assembly.Write(image);

		return MetadataReference.CreateFromImage(
			[..image.ToArray()],
			filePath: path);
	}

	static LanguageVersion Language(
		string langVersion) =>
		langVersion.ToLowerInvariant() switch
		{
			"" or "latest" or "latestmajor" or "default" or "preview" => LanguageVersion.Preview,
			string explicitVersion => LanguageVersionFacts.TryParse(explicitVersion, out LanguageVersion parsed) ? parsed : LanguageVersion.Preview
		};

	static void UnifyCompilerAssemblies()
	{
		if (Interlocked.Exchange(ref resolverInstalled, 1) == 1)
			return;

		AppDomain.CurrentDomain.AssemblyResolve += (_, request) =>
		{
			string simpleName = new AssemblyName(request.Name).Name;
			if (!Unified.Any(prefix => simpleName == prefix || simpleName.StartsWith(prefix + ".", StringComparison.Ordinal)))
				return null;

			return AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(loaded => string.Equals(loaded.GetName().Name, simpleName, StringComparison.Ordinal));
		};
	}

	static ISourceGenerator[] LoadGenerators(
		List<string> analyzerPaths,
		Action<string> log)
	{
		UnifyCompilerAssemblies();

		List<ISourceGenerator> generators = [];

		foreach (string path in analyzerPaths.Where(File.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
		{
			ISourceGenerator[] loaded = GeneratorCache.GetOrAdd(path, key => Load(key, log));
			generators.AddRange(loaded);
		}

		log($"  {generators.Count} source generator(s)");

		return [.. generators];
	}

	static ISourceGenerator[] Load(
		string path,
		Action<string> log)
	{
		Assembly assembly;
		try
		{
			assembly = Assembly.LoadFrom(path);
		}
		catch (Exception exception)
		{
			log($"  cannot load {Path.GetFileName(path)}: {exception.Message}");
			return [];
		}

		List<ISourceGenerator> generators = [];

		foreach (Type type in SafeTypes(assembly))
		{
			if (type.IsAbstract || type.GetCustomAttribute<GeneratorAttribute>() is null)
				continue;

			object? instance;
			try
			{
				instance = Activator.CreateInstance(type);
			}
			catch (Exception exception)
			{
				log($"  cannot create {type.FullName}: {exception.Message}");
				continue;
			}

			switch (instance)
			{
				case IIncrementalGenerator incremental:
					generators.Add(incremental.AsSourceGenerator());
					break;
				case ISourceGenerator source:
					generators.Add(source);
					break;
			}
		}

		return [.. generators];
	}

	static IEnumerable<Type> SafeTypes(
		Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException exception)
		{
			return exception.Types.Where(type => type is not null);
		}
	}


	public static Project Build(
		CscInvocation csc,
		bool runGenerators,
		Action<string> log)
	{
		CSharpParseOptions parseOptions = new CSharpParseOptions(Language(csc.LangVersion))
			.WithPreprocessorSymbols(csc.Defines)
			.WithFeatures(csc.Features);

		List<SyntaxTree> trees = [.. csc.Sources
			.Where(File.Exists)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Select(path => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(path), Encoding.UTF8), parseOptions, path))];

		List<MetadataReference> references = [.. csc.References
			.Where(File.Exists)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Select(path => Reference(path, csc.ReferenceVersionOverrides))];

		CSharpCompilationOptions options = new(
			csc.OutputKind,
			mainTypeName: csc.MainTypeName,
			allowUnsafe: csc.AllowUnsafe,
			checkOverflow: csc.CheckOverflow,
			nullableContextOptions: csc.Nullable,
			optimizationLevel: csc.Optimize ? OptimizationLevel.Release : OptimizationLevel.Debug,
			deterministic: true,
			platform: Platform.AnyCpu);

		CSharpCompilation compilation = CSharpCompilation.Create(csc.AssemblyName, trees, references, options);

		if (runGenerators)
		{
			ISourceGenerator[] generators = LoadGenerators(csc.Analyzers, log);
			if (generators.Length > 0)
			{
				CSharpGeneratorDriver
					.Create(
						generators,
						AnalyzerConfig.AdditionalTexts(csc.AdditionalFiles),
						parseOptions,
						AnalyzerConfig.Load(csc.AnalyzerConfigs))
					.RunGeneratorsAndUpdateCompilation(compilation, out Compilation updated, out ImmutableArray<Diagnostic> diagnostics);

				foreach (Diagnostic diagnostic in diagnostics.Where(entry => entry.Severity == DiagnosticSeverity.Error).Take(3))
					log($"  generator: {diagnostic}");

				compilation = (CSharpCompilation)updated;
			}
		}

		return new()
		{
			Compilation = compilation,
			ParseOptions = parseOptions
		};
	}

	public required CSharpCompilation Compilation { get; init; }
	public required CSharpParseOptions ParseOptions { get; init; }
}
