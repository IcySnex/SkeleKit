using JetBrains.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.Rider.Backend.HotReload;

internal static class Differ
{
	sealed class BodyStripper : CSharpSyntaxRewriter
	{
		public static readonly BodyStripper Instance = new();


		public override SyntaxNode? VisitMethodDeclaration(
			MethodDeclarationSyntax node) =>
			base.VisitMethodDeclaration(node
				.WithBody(null)
				.WithExpressionBody(null)
				.WithSemicolonToken(default));

		public override SyntaxNode? VisitConstructorDeclaration(
			ConstructorDeclarationSyntax node) =>
			base.VisitConstructorDeclaration(node
				.WithBody(null)
				.WithExpressionBody(null)
				.WithInitializer(null)
				.WithSemicolonToken(default));

		public override SyntaxNode? VisitPropertyDeclaration(
			PropertyDeclarationSyntax node)
		{
			AccessorListSyntax? accessors = node.AccessorList;
			accessors = accessors?.WithAccessors(new(
				accessors.Accessors.Select(accessor => accessor
					.WithBody(null)
					.WithExpressionBody(null)
					.WithSemicolonToken(default))));

			return base.VisitPropertyDeclaration(node
				.WithAccessorList(accessors)
				.WithExpressionBody(null)
				.WithSemicolonToken(default));
		}
	}


	static bool TokensEquivalent(
		SyntaxNode oldShape,
		SyntaxNode newShape)
	{
		using IEnumerator<SyntaxToken> oldTokens = oldShape.DescendantTokens().GetEnumerator();
		using IEnumerator<SyntaxToken> newTokens = newShape.DescendantTokens().GetEnumerator();

		while (true)
		{
			bool hasOld = oldTokens.MoveNext();
			bool hasNew = newTokens.MoveNext();
			if (hasOld != hasNew)
				return false;

			if (!hasOld)
				return true;

			if (oldTokens.Current.RawKind != newTokens.Current.RawKind
				|| oldTokens.Current.Text != newTokens.Current.Text)
				return false;
		}
	}

	static Dictionary<string, SyntaxNode> Bodies(
		SyntaxTree tree,
		SemanticModel model)
	{
		Dictionary<string, SyntaxNode> map = [];

		foreach (SyntaxNode node in tree.GetRoot().DescendantNodes())
		{
			if (node is not (MethodDeclarationSyntax or ConstructorDeclarationSyntax or PropertyDeclarationSyntax))
				continue;

			ISymbol? symbol = model.GetDeclaredSymbol(node);
			if (symbol is not null)
				map[symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = node;
		}

		return map;
	}


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

		SyntaxNode oldShape = BodyStripper.Instance.Visit(oldTree.GetRoot());
		SyntaxNode newShape = BodyStripper.Instance.Visit(newTree.GetRoot());
		if (!TokensEquivalent(oldShape, newShape))
			rude.Add("structural or unsupported edit (needs restart)");

		Dictionary<string, SyntaxNode> oldBodies = Bodies(oldTree, oldModel);
		Dictionary<string, SyntaxNode> newBodies = Bodies(newTree, newModel);

		foreach ((string? key, SyntaxNode? newNode) in newBodies)
		{
			if (!oldBodies.TryGetValue(key, out SyntaxNode? oldNode))
			{
				rude.Add($"added member (needs restart): {key}");
				continue;
			}

			if (oldNode.IsEquivalentTo(newNode))
				continue;

			if (oldModel.GetDeclaredSymbol(oldNode) is not ISymbol oldSymbol
				|| newModel.GetDeclaredSymbol(newNode) is not ISymbol newSymbol)
				continue;

			edits.Add(new(SemanticEditKind.Update, oldSymbol, newSymbol));
		}

		foreach (string key in oldBodies.Keys)
		{
			if (!newBodies.ContainsKey(key))
				rude.Add($"removed member (needs restart): {key}");
		}

		return edits;
	}
}
