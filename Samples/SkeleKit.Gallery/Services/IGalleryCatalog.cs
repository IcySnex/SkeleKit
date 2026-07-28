using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Services;

internal interface IGalleryCatalog
{
	List<GallerySection> Controls { get; }
	List<GallerySection> Framework { get; }
	List<GallerySection> Platform { get; }

	List<GalleryTopic> Search(
		string query);
}
