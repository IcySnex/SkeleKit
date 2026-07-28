namespace SkeleKit.Gallery.Models;

internal sealed record GallerySection(
	string Title,
	IReadOnlyList<GalleryTopic> Items) : ISection<GalleryTopic>;
