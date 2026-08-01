using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Services.Abstract;

internal interface IGalleryCatalog
{
	List<GallerySection> Controls { get; }

	List<GallerySection> Framework { get; }

	List<GallerySection> Platform { get; }


	List<GalleryTopic> Search(
		string query,
		GalleryArea? area);
}
