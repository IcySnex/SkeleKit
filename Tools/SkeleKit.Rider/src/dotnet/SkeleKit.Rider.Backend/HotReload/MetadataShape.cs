using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.Rider.Backend.HotReload;

internal sealed class MetadataShape
{
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
		if (!declarations.IsSubsetOf(deployed.declarations))
			return false;

		return deployed.declarations
			.Except(declarations)
			.All(declaration => declaration.EndsWith("..cctor", StringComparison.Ordinal));
	}

	public IEnumerable<string> Missing(
		MetadataShape other) =>
		declarations.Except(other.declarations);
}
