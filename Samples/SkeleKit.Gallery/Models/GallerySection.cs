namespace SkeleKit.Gallery.Models;

internal sealed record GallerySection(
	string Title,
	string Symbol,
	IReadOnlyList<GalleryTopic> Items) : ISection<GalleryTopic>;
