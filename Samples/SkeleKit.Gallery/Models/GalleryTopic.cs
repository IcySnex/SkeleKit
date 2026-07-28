namespace SkeleKit.Gallery.Models;

internal sealed record GalleryTopic(
	string Title,
	string Summary,
	string Symbol,
	Color Accent,
	GalleryArea Area);
