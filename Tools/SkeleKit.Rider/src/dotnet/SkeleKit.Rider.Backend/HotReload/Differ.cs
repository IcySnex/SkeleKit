using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.Rider.Backend.HotReload;

static class Differ
{
	public static List<SemanticEdit> Edits(
		Compilation oldCompilation,
		Compilation newCompilation,
		SyntaxTree oldTree,
		SyntaxTree newTree,
		out List<string> rude)
	{
		rude = [];
		List<SemanticEdit> edits = [];

		SemanticModel oldModel = oldCompilation.GetSemanticModel(oldTree);
		SemanticModel newModel = newCompilation.GetSemanticModel(newTree);

		Dictionary<string, SyntaxNode> oldBodies = Bodies(oldTree);
		Dictionary<string, SyntaxNode> newBodies = Bodies(newTree);

		foreach (KeyValuePair<string, SyntaxNode> pair in newBodies)
		{
			string key = pair.Key;
			SyntaxNode newNode = pair.Value;

			if (!oldBodies.TryGetValue(key, out SyntaxNode? oldNode))
			{
				rude.Add($"added member (needs restart): {key}");
				continue;
			}

			if (Body(oldNode) == Body(newNode))
				continue;

			if (oldModel.GetDeclaredSymbol(oldNode) is not ISymbol oldSymbol
				|| newModel.GetDeclaredSymbol(newNode) is not ISymbol newSymbol)
				continue;

			edits.Add(new(SemanticEditKind.Update, oldSymbol, newSymbol));
		}

		foreach (string key in oldBodies.Keys)
			if (!newBodies.ContainsKey(key))
				rude.Add($"removed member (needs restart): {key}");

		return edits;
	}

	static Dictionary<string, SyntaxNode> Bodies(
		SyntaxTree tree)
	{
		Dictionary<string, SyntaxNode> map = [];

		foreach (SyntaxNode node in tree.GetRoot().DescendantNodes())
		{
			string? key = node switch
			{
				MethodDeclarationSyntax method => Key(method, method.Identifier.Text, method.ParameterList),
				ConstructorDeclarationSyntax constructor => Key(constructor, ".ctor", constructor.ParameterList),
				PropertyDeclarationSyntax property => Key(property, property.Identifier.Text, null),
				_ => null
			};

			if (key is not null)
				map[key] = node;
		}

		return map;
	}

	static string Key(
		SyntaxNode node,
		string name,
		ParameterListSyntax? parameters)
	{
		string type = node.Ancestors()
			.OfType<TypeDeclarationSyntax>()
			.Select(declaration => declaration.Identifier.Text)
			.Aggregate("", (outer, inner) => inner + "+" + outer);

		string signature = parameters is null
			? name
			: name + "(" + string.Join(",", parameters.Parameters.Select(parameter => parameter.Type?.ToString())) + ")";

		return type + signature;
	}

	static string Body(
		SyntaxNode node) =>
		node switch
		{
			MethodDeclarationSyntax method => method.Body?.ToString() ?? method.ExpressionBody?.ToString() ?? "",
			ConstructorDeclarationSyntax constructor => constructor.Body?.ToString() ?? "",
			PropertyDeclarationSyntax property => property.ToString(),
			_ => ""
		};
}
