namespace SkeleKit.Gallery.Models;

internal sealed record GalleryTopic(
	string Title,
	string Summary,
	string Symbol,
	Color Accent,
	GalleryArea Area,
	Type? Destination = null)
{
	public string SearchSummary => $"{Area} · {Summary}";
}
