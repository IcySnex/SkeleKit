using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace SkeleKit.Rider.Backend.HotReload;

sealed class Baseline
{
	static readonly Guid EncLocalSlotMap = new("755F52A8-91C5-45BE-B4B8-209571E552BD");
	static readonly Guid EncLambdaAndClosureMap = new("A643004C-0240-496F-A783-30D64F4979DE");

	readonly PEReader pe;
	readonly MetadataReader metadata;
	readonly MetadataReader? pdb;

	public EmitBaseline Emit { get; set; }
	public Guid Mvid { get; }

	public Baseline(
		string dllPath,
		Compilation compilation)
	{
		pe = new(File.OpenRead(dllPath));
		metadata = pe.GetMetadataReader();
		Mvid = metadata.GetGuid(metadata.GetModuleDefinition().Mvid);

		string pdbPath = Path.ChangeExtension(dllPath, ".pdb");
		if (File.Exists(pdbPath))
			pdb = MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(pdbPath)).GetMetadataReader();

		ModuleMetadata module = ModuleMetadata.CreateFromStream(File.OpenRead(dllPath));

		Emit = EmitBaseline.CreateInitialBaseline(
			compilation,
			module,
			DebugInformation,
			LocalSignature,
			hasPortableDebugInformation: pdb is not null);
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
