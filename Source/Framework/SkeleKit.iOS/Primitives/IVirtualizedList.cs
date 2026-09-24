namespace SkeleKit;

/// <summary>
/// A logical list whose items are read by index as they are needed.
/// </summary>
/// <remarks>
/// <see cref="CollectionView{TItem}"/> uses a direct native data source for this list instead of
/// materializing every item into a diffable snapshot. Implement <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>
/// as well when the list changes after it is shown.
/// </remarks>
/// <typeparam name="TItem">The item type.</typeparam>
public interface IVirtualizedList<out TItem> : IReadOnlyList<TItem>;

/// <summary>
/// A logical section list that can return item counts and items without first creating every section model.
/// </summary>
/// <typeparam name="TItem">The item type within each section.</typeparam>
/// <typeparam name="TSection">The section model type.</typeparam>
public interface IVirtualizedSectionList<out TItem, out TSection> : IVirtualizedList<TSection>
	where TSection : ISection<TItem>
{
	/// <summary>
	/// Returns the number of items in a section.
	/// </summary>
	int GetItemCount(
		int section);

	/// <summary>
	/// Returns an item by its section and item index.
	/// </summary>
	TItem GetItem(
		int section,
		int item);
}
