using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace SkeleKit.Rider.Backend.HotReload;

static class Pe
{
	public static Guid ReadMvid(
		string path)
	{
		using FileStream stream = File.OpenRead(path);
		using PEReader pe = new(stream);
		MetadataReader reader = pe.GetMetadataReader();

		return reader.GetGuid(reader.GetModuleDefinition().Mvid);
	}
}
