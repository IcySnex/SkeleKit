using System.Collections.Specialized;
using System.Windows.Input;

namespace BareUI;

/// <summary>
/// A virtualized list, grid or carousel over <c>UICollectionView</c>. Cells are recycled: the element tree is built once per cell and rebound on reuse.
/// </summary>
public partial class CollectionView<TItem> : View
	where TItem : class
{
	/// <summary>
	/// The items to show. Live updates when the list also implements <c>INotifyCollectionChanged</c>.
	/// </summary>
	public Bindable<IReadOnlyList<TItem>?> ItemsSource
	{
		get => Bindable.From(itemsSource);
		set => itemsSourceBinding = Register(itemsSourceBinding, value, SetItemsSource);
	}
	IReadOnlyList<TItem>? itemsSource;
	Binding<IReadOnlyList<TItem>?>? itemsSourceBinding;

	/// <summary>
	/// Builds the element tree for a cell. Called once per recycled cell, never per item.
	/// </summary>
	public Func<ItemView<TItem>>? ItemTemplate { get; set; }

	/// <summary>
	/// How the items are arranged.
	/// </summary>
	public CollectionLayout Layout { get; set; } = CollectionLayout.List();

	/// <summary>
	/// Invoked with the tapped item.
	/// </summary>
	public ICommand? SelectionCommand { get; set; }

	/// <summary>
	/// Shown instead of the items while the source is empty.
	/// </summary>
	public View? EmptyView { get; set; }


	void SetItemsSource(
		IReadOnlyList<TItem>? value)
	{
		if (ReferenceEquals(itemsSource, value))
			return;

		if (itemsSource is INotifyCollectionChanged old)
			old.CollectionChanged -= OnCollectionChanged;

		itemsSource = value;

		if (itemsSource is INotifyCollectionChanged live)
			live.CollectionChanged += OnCollectionChanged;

		ReloadItems();
	}

	void OnCollectionChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ReloadItems();

	partial void ReloadItems();

	// the item at an index, for the native data source
	internal TItem? ItemAt(
		int index) =>
		itemsSource is { } items && index >= 0 && index < items.Count
			? items[index]
			: null;

	internal int ItemCount =>
		itemsSource?.Count ?? 0;
}
