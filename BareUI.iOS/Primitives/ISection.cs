namespace BareUI;

/// <summary>
/// A group of items in a <c>CollectionView</c>.
/// </summary>
/// <remarks>
/// Implement it on your own section model, which the header and footer templates bind to.
/// </remarks>
/// <typeparam name="TItem">The element type of the group.</typeparam>
public interface ISection<out TItem>
{
	/// <summary>
	/// The items in this group.
	/// </summary>
	/// <remarks>
	/// Mutations animate when the list is an <c>ObservableCollection</c>.
	/// </remarks>
	IReadOnlyList<TItem> Items { get; }
}
