using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

// Rebuilds the app's Roslyn compilation from the command line csc was given, so the delta engine has
// something to diff against and a baseline that lines up with the deployed assembly.
sealed class Project
{
	// the compiler assemblies an analyzer may ask for, which it must share with us to be usable at all
	static readonly string[] Unified =
	[
		"Microsoft.CodeAnalysis",
		"System.Collections.Immutable",
		"System.Reflection.Metadata"
	];

	// analyzer assemblies stay loaded for the life of the backend, so reuse them across sessions
	static readonly ConcurrentDictionary<string, ISourceGenerator[]> GeneratorCache = new(StringComparer.OrdinalIgnoreCase);

	static int resolverInstalled;

	public required CSharpCompilation Compilation { get; init; }
	public required CSharpParseOptions ParseOptions { get; init; }

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
			.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];

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

	// The SDK asks for "latest", meaning whatever the compiler that ran the build supports. Ours is a
	// different build of Roslyn, and mapping that to LanguageVersion.Latest drops features the app is
	// already using, so an open-ended request becomes Preview. An explicit version is honored as given.
	static LanguageVersion Language(
		string langVersion) =>
		langVersion.ToLowerInvariant() switch
		{
			"" or "latest" or "latestmajor" or "default" or "preview" => LanguageVersion.Preview,
			string explicitVersion => LanguageVersionFacts.TryParse(explicitVersion, out LanguageVersion parsed) ? parsed : LanguageVersion.Preview
		};

	// A source generator is built against the compiler that shipped with its SDK and asks for that exact
	// version. We are hosted by a different build of Roslyn, so without this every SDK generator fails to
	// load and the compilation comes out missing whatever they were supposed to write.
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

			if (instance is IIncrementalGenerator incremental)
				generators.Add(incremental.AsSourceGenerator());
			else if (instance is ISourceGenerator source)
				generators.Add(source);
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
			return exception.Types.Where(type => type is not null)!;
		}
	}
}
