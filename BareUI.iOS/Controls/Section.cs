namespace BareUI;

/// <summary>
/// A titled group of items in a <c>CollectionView</c>.
/// </summary>
public class Section<TItem>(
	string title,
	IReadOnlyList<TItem> items)
	where TItem : class
{
	/// <summary>
	/// The header's text, or whatever the header template binds to.
	/// </summary>
	public string Title { get; } = title;

	/// <summary>
	/// The items in this group.
	/// </summary>
	public IReadOnlyList<TItem> Items { get; } = items;
}
