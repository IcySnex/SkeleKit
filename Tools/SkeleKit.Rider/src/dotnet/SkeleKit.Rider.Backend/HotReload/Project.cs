using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Rider.Backend.HotReload;

sealed class Project
{
	public required CSharpCompilation Compilation { get; set; }
	public required CSharpParseOptions ParseOptions { get; init; }

	public static Project Build(
		CscInvocation csc)
	{
		CSharpParseOptions parseOptions = new CSharpParseOptions(LanguageVersion.Preview)
			.WithPreprocessorSymbols(csc.Defines);

		List<SyntaxTree> trees = [.. csc.Sources
			.Where(File.Exists)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Select(path => CSharpSyntaxTree.ParseText(SourceText.From(File.ReadAllText(path), Encoding.UTF8), parseOptions, path))];

		List<MetadataReference> references = [.. csc.References
			.Where(File.Exists)
			.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];

		CSharpCompilationOptions options = new(
			OutputKind.ConsoleApplication,
			allowUnsafe: csc.AllowUnsafe,
			nullableContextOptions: NullableContextOptions.Enable,
			optimizationLevel: OptimizationLevel.Debug,
			deterministic: true,
			platform: Platform.AnyCpu);

		CSharpCompilation compilation = CSharpCompilation.Create(csc.AssemblyName, trees, references, options);

		ISourceGenerator[] generators = LoadGenerators(csc.Generators);
		if (generators.Length > 0)
		{
			CSharpGeneratorDriver
				.Create(generators, parseOptions: parseOptions)
				.RunGeneratorsAndUpdateCompilation(compilation, out Compilation updated, out _);

			compilation = (CSharpCompilation)updated;
		}

		return new()
		{
			Compilation = compilation,
			ParseOptions = parseOptions
		};
	}

	static ISourceGenerator[] LoadGenerators(
		List<string> analyzerPaths)
	{
		List<ISourceGenerator> generators = [];

		// only the generators that produce app symbols; the framework interop generators re-emit
		// nfloat / assembly attributes the SDK already wrote, which collide
		string[] wanted = ["SkeleKit.Generators", "CommunityToolkit.Mvvm"];

		foreach (string path in analyzerPaths.Where(File.Exists).Where(path => wanted.Any(path.Contains)))
		{
			Assembly assembly;
			try
			{
				assembly = Assembly.LoadFrom(path);
			}
			catch
			{
				continue;
			}

			foreach (Type type in SafeTypes(assembly))
			{
				if (type.IsAbstract || type.GetCustomAttribute<GeneratorAttribute>() is null)
					continue;

				object? instance = Activator.CreateInstance(type);

				if (instance is IIncrementalGenerator incremental)
					generators.Add(incremental.AsSourceGenerator());
				else if (instance is ISourceGenerator source)
					generators.Add(source);
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
			return exception.Types.Where(type => type is not null)!;
		}
	}
}
