namespace SkeleKit.Gallery;

internal sealed record GalleryCategory(
	string Title,
	string Description,
	string Symbol,
	Color Accent,
	IReadOnlyList<string> Components)
{
	public string ComponentCount =>
		$"{Components.Count} topics";


	public bool Matches(
		string query)
	{
		if (string.IsNullOrWhiteSpace(query))
			return true;

		return Title.Contains(query, StringComparison.OrdinalIgnoreCase)
			|| Description.Contains(query, StringComparison.OrdinalIgnoreCase)
			|| Components.Any(component => component.Contains(query, StringComparison.OrdinalIgnoreCase));
	}
}
