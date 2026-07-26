using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SkeleKit.Generators;

/// <summary>
/// Generates page registration and the application Build() entry point.
/// </summary>
[Generator]
public sealed class PageGenerator : IIncrementalGenerator
{
	const string HotReloadSymbol = "SKELEKIT_HOT_RELOAD";


	sealed record Page(
		string View,
		string? ViewModel,
		bool Singleton);

	sealed record Candidate(
		Page? Page,
		Diagnostic? Diagnostic);


	static readonly DiagnosticDescriptor InvalidPage = new(
		"SKEL001",
		"Invalid page declaration",
		"'{0}' is marked [Page] but does not inherit SkeleKit.ContentView",
		"SkeleKit",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	static readonly DiagnosticDescriptor AbstractPage = new(
		"SKEL002",
		"Page cannot be abstract",
		"'{0}' is marked [Page] but is abstract",
		"SkeleKit",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	static readonly DiagnosticDescriptor MissingConstructor = new(
		"SKEL003",
		"Page constructor is missing",
		"'{0}' must declare an accessible {1}",
		"SkeleKit",
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);


	public void Initialize(
		IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<Candidate> candidates = context.SyntaxProvider.ForAttributeWithMetadataName(
			"SkeleKit.PageAttribute",
			static (node, _) => node is ClassDeclarationSyntax,
			static (attribute, _) => Extract(attribute));

		IncrementalValueProvider<ImmutableArray<string>> hotReloadTypes = context.CompilationProvider.Select(
			static (compilation, _) => HotReloadTypes(compilation));

		context.RegisterSourceOutput(
			candidates.Collect().Combine(hotReloadTypes),
			static (production, input) =>
			{
				foreach (Diagnostic diagnostic in input.Left
					.Select(static candidate => candidate.Diagnostic)
					.OfType<Diagnostic>())
					production.ReportDiagnostic(diagnostic);

				production.AddSource(
					"GeneratedPages.g.cs",
					Emit(
						input.Left
							.Select(static candidate => candidate.Page)
							.OfType<Page>()
							.ToImmutableArray(),
						input.Right));
			});
	}


	static ImmutableArray<string> HotReloadTypes(
		Compilation compilation)
	{
		bool enabled = compilation.SyntaxTrees
			.Select(static tree => tree.Options)
			.OfType<CSharpParseOptions>()
			.Any(static options => options.PreprocessorSymbolNames.Contains(HotReloadSymbol));

		if (!enabled
			|| compilation.GetTypeByMetadataName("SkeleKit.ContentView") is not INamedTypeSymbol marker)
			return ImmutableArray<string>.Empty;

		ImmutableArray<string>.Builder types = ImmutableArray.CreateBuilder<string>();
		Collect(marker.ContainingAssembly.GlobalNamespace, types);

		return types
			.Distinct(StringComparer.Ordinal)
			.OrderBy(static type => type, StringComparer.Ordinal)
			.ToImmutableArray();
	}

	static void Collect(
		INamespaceSymbol @namespace,
		ImmutableArray<string>.Builder types)
	{
		foreach (INamespaceSymbol child in @namespace.GetNamespaceMembers())
			Collect(child, types);

		foreach (INamedTypeSymbol type in @namespace.GetTypeMembers())
			Collect(type, containingTypeIsPublic: true, types);
	}

	static void Collect(
		INamedTypeSymbol type,
		bool containingTypeIsPublic,
		ImmutableArray<string>.Builder types)
	{
		bool isPublic = containingTypeIsPublic
			&& type.DeclaredAccessibility == Accessibility.Public
			&& type.CanBeReferencedByName;

		if (isPublic)
		{
			INamedTypeSymbol reference = type.IsGenericType
				? type.ConstructUnboundGenericType()
				: type;

			types.Add(reference.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
		}

		foreach (INamedTypeSymbol nested in type.GetTypeMembers())
			Collect(nested, isPublic, types);
	}

	static bool IsAccessible(
		IMethodSymbol constructor) =>
		constructor.DeclaredAccessibility is
			Accessibility.Public or
			Accessibility.Internal or
			Accessibility.ProtectedOrInternal;

	static Location? LocationOf(
		INamedTypeSymbol view) =>
		view.Locations.FirstOrDefault();

	static Candidate Extract(
		GeneratorAttributeSyntaxContext context)
	{
		INamedTypeSymbol view = (INamedTypeSymbol)context.TargetSymbol;
		string viewName = view.ToDisplayString();

		if (view.IsAbstract)
			return new(null, Diagnostic.Create(AbstractPage, LocationOf(view), viewName));

		INamedTypeSymbol? viewModel = null;
		bool contentView = false;

		for (INamedTypeSymbol? baseType = view.BaseType; baseType is not null; baseType = baseType.BaseType)
		{
			if (baseType.Name != "ContentView"
				|| baseType.ContainingNamespace.ToDisplayString() != "SkeleKit")
				continue;

			contentView = true;

			if (baseType.IsGenericType && baseType.TypeArguments.Length == 1)
				viewModel = baseType.TypeArguments[0] as INamedTypeSymbol;

			break;
		}

		if (!contentView)
			return new(null, Diagnostic.Create(InvalidPage, LocationOf(view), viewName));

		bool singleton = context.Attributes[0].NamedArguments.Any(
			static argument => argument.Key == "Singleton" && argument.Value.Value is true);

		if (viewModel is null)
		{
			bool hasConstructor = view.InstanceConstructors.Any(
				static constructor =>
					IsAccessible(constructor)
					&& constructor.Parameters.Length == 0);

			if (!hasConstructor)
			{
				return new(
					null,
					Diagnostic.Create(
						MissingConstructor,
						LocationOf(view),
						viewName,
						"parameterless constructor or a manual UsePages(...) registration"));
			}
		}
		else
		{
			bool hasConstructor = view.InstanceConstructors.Any(
				constructor =>
					IsAccessible(constructor)
					&& constructor.Parameters.Length >= 1
					&& SymbolEqualityComparer.Default.Equals(constructor.Parameters[0].Type, viewModel)
					&& constructor.Parameters.Skip(1).All(static parameter => parameter.IsOptional));

			if (!hasConstructor)
			{
				return new(
					null,
					Diagnostic.Create(
						MissingConstructor,
						LocationOf(view),
						viewName,
						$"constructor whose first parameter is {viewModel.ToDisplayString()}"));
			}
		}

		return new(
			new(
				viewName,
				viewModel?.ToDisplayString(),
				singleton),
			null);
	}

	static SourceText Emit(
		ImmutableArray<Page> pages,
		ImmutableArray<string> hotReloadTypes)
	{
		StringBuilder source = new();

		source.AppendLine("// <auto-generated/>");
		source.AppendLine("#nullable enable");
		source.AppendLine();
		source.AppendLine("namespace SkeleKit;");
		source.AppendLine();
		source.AppendLine("/// <summary>");
		source.AppendLine("/// Registration for [Page] views, generated at compile time.");
		source.AppendLine("/// </summary>");
		source.AppendLine("public static class GeneratedPages");
		source.AppendLine("{");
		source.AppendLine("\t/// <summary>");
		source.AppendLine("\t/// Registers every [Page] view as a default. Manual registrations take precedence.");
		source.AppendLine("\t/// </summary>");
		source.AppendLine("\tpublic static SkeleApplicationBuilder UsePages(");
		source.AppendLine("\t\tthis SkeleApplicationBuilder builder) =>");
		source.AppendLine("\t\tbuilder.UsePages(pages =>");
		source.AppendLine("\t\t{");

		foreach (Page page in pages)
		{
			string lifetime = page.Singleton ? "AddSingleton" : "AddTransient";

			source.AppendLine(page.ViewModel is null
				? $"\t\t\tpages.{lifetime}<global::{page.View}>(() => new global::{page.View}());"
				: $"\t\t\tpages.{lifetime}((global::{page.ViewModel} viewModel) => new global::{page.View}(viewModel));");
		}

		source.AppendLine("\t\t}, false);");
		source.AppendLine();
		source.AppendLine("\t/// <summary>");
		source.AppendLine("\t/// Applies generated page registrations and builds the application.");
		source.AppendLine("\t/// </summary>");
		if (hotReloadTypes.Length > 0)
			source.AppendLine("\t[global::System.Diagnostics.CodeAnalysis.DynamicDependency(nameof(SeedHotReloadTypeReferences))]");
		source.AppendLine("\tpublic static SkeleApplication Build(");
		source.AppendLine("\t\tthis SkeleApplicationBuilder builder) =>");
		source.AppendLine("\t\tbuilder.UsePages().BuildCore();");

		if (hotReloadTypes.Length > 0)
		{
			source.AppendLine();
			source.AppendLine("\t// Retained by DynamicDependency but never called. Returning the Type array prevents");
			source.AppendLine("\t// the linker from replacing its TypeRefs with no-ops while adding no startup work.");
			source.AppendLine("\tprivate static global::System.Type[] SeedHotReloadTypeReferences() =>");
			source.AppendLine("\t[");

			foreach (string type in hotReloadTypes)
				source.AppendLine($"\t\ttypeof({type}),");

			source.AppendLine("\t];");
		}

		source.AppendLine("}");

		return SourceText.From(source.ToString(), Encoding.UTF8);
	}
}
