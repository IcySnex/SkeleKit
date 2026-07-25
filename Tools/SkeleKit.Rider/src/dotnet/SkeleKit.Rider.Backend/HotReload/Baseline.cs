using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.Rider.Backend.HotReload;

// The deployed assembly, as the starting point every delta is generated against.
//
// The dll and pdb are read into memory rather than mapped, so a rebuild can replace them while a
// session is running without us holding the old files open.
sealed class Baseline
{
	static readonly Guid EncLocalSlotMap = new("755F52A8-91C5-45BE-B4B8-209571E552BD");
	static readonly Guid EncLambdaAndClosureMap = new("A643004C-0240-496F-A783-30D64F4979DE");

	readonly PEReader pe;
	readonly MetadataReader metadata;
	readonly MetadataReader? pdb;
	readonly HashSet<string> referencedTypes = new(StringComparer.Ordinal);

	public EmitBaseline Emit { get; set; }
	public Guid Mvid { get; }

	// Mono can resolve a runtime type in an edited body only if the deployed module already had a
	// TypeRef for it. Scope is deliberately ignored: compiler facades and type forwarding can make
	// Roslyn name System.Runtime while the deployed row names the implementation assembly.
	public bool ContainsType(
		string metadataName) =>
		referencedTypes.Contains(metadataName);

	public Baseline(
		string dllPath,
		Compilation compilation)
	{
		byte[] assembly = File.ReadAllBytes(dllPath);

		pe = new(new MemoryStream(assembly));
		metadata = pe.GetMetadataReader();
		Mvid = metadata.GetGuid(metadata.GetModuleDefinition().Mvid);
		foreach (TypeReferenceHandle handle in metadata.TypeReferences)
			referencedTypes.Add(TypeReferenceName(handle));

		string pdbPath = Path.ChangeExtension(dllPath, ".pdb");
		if (File.Exists(pdbPath))
			pdb = MetadataReaderProvider.FromPortablePdbStream(new MemoryStream(File.ReadAllBytes(pdbPath))).GetMetadataReader();

		Emit = EmitBaseline.CreateInitialBaseline(
			compilation,
			ModuleMetadata.CreateFromStream(new MemoryStream(assembly)),
			DebugInformation,
			LocalSignature,
			hasPortableDebugInformation: pdb is not null);
	}

	string TypeReferenceName(
		TypeReferenceHandle handle)
	{
		TypeReference type = metadata.GetTypeReference(handle);
		string name = metadata.GetString(type.Name);

		if (type.ResolutionScope.Kind == HandleKind.TypeReference)
			return TypeReferenceName((TypeReferenceHandle)type.ResolutionScope) + "+" + name;

		string @namespace = metadata.GetString(type.Namespace);
		return @namespace.Length == 0 ? name : @namespace + "." + name;
	}

	EditAndContinueMethodDebugInformation DebugInformation(
		MethodDefinitionHandle handle) =>
		EditAndContinueMethodDebugInformation.Create(
			ReadCustomDebugInfo(handle, EncLocalSlotMap),
			ReadCustomDebugInfo(handle, EncLambdaAndClosureMap));

	ImmutableArray<byte> ReadCustomDebugInfo(
		MethodDefinitionHandle handle,
		Guid kind)
	{
		if (pdb is null)
			return [];

		foreach (CustomDebugInformationHandle entry in pdb.GetCustomDebugInformation(handle))
		{
			CustomDebugInformation info = pdb.GetCustomDebugInformation(entry);
			if (pdb.GetGuid(info.Kind) == kind)
				return [.. pdb.GetBlobBytes(info.Value)];
		}

		return [];
	}

	StandaloneSignatureHandle LocalSignature(
		MethodDefinitionHandle handle)
	{
		MethodDefinition method = metadata.GetMethodDefinition(handle);
		if (method.RelativeVirtualAddress == 0)
			return default;

		return pe.GetMethodBody(method.RelativeVirtualAddress).LocalSignature;
	}
}
