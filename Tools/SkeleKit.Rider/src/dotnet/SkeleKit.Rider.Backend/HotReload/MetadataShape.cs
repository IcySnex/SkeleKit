using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.Rider.Backend.HotReload;

// The declarations an assembly contains, used to check that the compilation we rebuilt is the same
// program the app is running.
//
// The reason to check at all: if a source generator silently did not run, whole types go missing from
// our copy, every later diff is computed against the wrong code, and the app takes a delta that does
// not describe it. That fails in confusing ways at runtime rather than at the point of the mistake.
//
// Only declarations written in source are compared. Compiler-synthesized plumbing (closures, collection
// expression helpers, extension metadata) is named and ordered differently by different builds of
// Roslyn, and ours is never the exact one that produced the deployed assembly. Row order is ignored for
// the same reason, and can be: Roslyn matches an edited member to its baseline row by name and
// signature, not by position.
sealed class MetadataShape
{
	readonly HashSet<string> declarations;

	MetadataShape(
		HashSet<string> declarations)
	{
		this.declarations = declarations;
	}

	public int Count => declarations.Count;

	public override string ToString() =>
		$"{declarations.Count} declarations";

	public bool Matches(
		MetadataShape deployed)
	{
		// The iOS linker injects static constructors into a rooted assembly (native registration
		// plumbing) even when it preserves every source declaration. Those extra methods do not change
		// the identity of source members Roslyn updates. Everything produced by the compilation must
		// still exist in the deployed module, and every deployed-only declaration must be one of those
		// linker constructors.
		if (!declarations.IsSubsetOf(deployed.declarations))
			return false;

		return deployed.declarations
			.Except(declarations)
			.All(declaration => declaration.EndsWith("..cctor", StringComparison.Ordinal));
	}

	public IEnumerable<string> Missing(
		MetadataShape other) =>
		declarations.Except(other.declarations);

	public static MetadataShape? OfAssembly(
		string path)
	{
		try
		{
			using FileStream stream = File.OpenRead(path);
			using PEReader pe = new(stream);

			return Of(pe.GetMetadataReader());
		}
		catch
		{
			return null;
		}
	}

	public static MetadataShape? OfCompilation(
		Compilation compilation)
	{
		try
		{
			using MemoryStream assembly = new();

			EmitResult result = compilation.Emit(assembly);
			if (!result.Success)
				return null;

			assembly.Position = 0;
			using PEReader pe = new(assembly);

			return Of(pe.GetMetadataReader());
		}
		catch
		{
			return null;
		}
	}

	static MetadataShape Of(
		MetadataReader reader)
	{
		HashSet<string> declarations = new(StringComparer.Ordinal);

		foreach (TypeDefinitionHandle handle in reader.TypeDefinitions)
		{
			TypeDefinition type = reader.GetTypeDefinition(handle);
			string name = $"{reader.GetString(type.Namespace)}.{reader.GetString(type.Name)}";
			if (Synthesized(name))
				continue;

			declarations.Add(name);

			foreach (MethodDefinitionHandle method in type.GetMethods())
			{
				string signature = $"{name}.{reader.GetString(reader.GetMethodDefinition(method).Name)}";
				if (!Synthesized(signature))
					declarations.Add(signature);
			}
		}

		return new(declarations);
	}

	static bool Synthesized(
		string name) =>
		name.IndexOf('<') >= 0;
}
