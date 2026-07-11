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
		get => Bindable.From<IReadOnlyList<TItem>?>(itemsSource);
		set => itemsSourceBinding = Register(itemsSourceBinding, value, SetItemsSource);
	}
	IReadOnlyList<TItem>? itemsSource;
	Binding<IReadOnlyList<TItem>?>? itemsSourceBinding;

	/// <summary>
	/// Titled groups, each with its own header. Takes precedence over <see cref="ItemsSource"/>.
	/// </summary>
	public Bindable<IReadOnlyList<Section<TItem>>?> GroupedItemsSource
	{
		get => Bindable.From<IReadOnlyList<Section<TItem>>?>(sections);
		set => sectionsBinding = Register(sectionsBinding, value, SetSections);
	}
	IReadOnlyList<Section<TItem>>? sections;
	Binding<IReadOnlyList<Section<TItem>>?>? sectionsBinding;

	/// <summary>
	/// Builds the element tree for a cell. Called once per recycled cell, never per item.
	/// </summary>
	public Func<ItemView<TItem>>? ItemTemplate { get; set; }

	/// <summary>
	/// Builds a section header. Bound to the <see cref="Section{TItem}"/>.
	/// </summary>
	public Func<ItemView<Section<TItem>>>? HeaderTemplate { get; set; }

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


	// it scrolls itself, so it takes the space it is offered rather than sizing to its content
	protected override Size MeasureOverride(
		Size availableSize) =>
		new(
			double.IsFinite(availableSize.Width) ? availableSize.Width : 0,
			double.IsFinite(availableSize.Height) ? availableSize.Height : 0);

	void SetItemsSource(
		IReadOnlyList<TItem>? value)
	{
		if (ReferenceEquals(itemsSource, value))
			return;

		if (itemsSource is INotifyCollectionChanged old)
			old.CollectionChanged -= OnItemsChanged;

		itemsSource = value;

		if (itemsSource is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;

		ReloadItems();
	}

	void SetSections(
		IReadOnlyList<Section<TItem>>? value)
	{
		if (ReferenceEquals(sections, value))
			return;

		if (sections is INotifyCollectionChanged old)
			old.CollectionChanged -= OnSectionsChanged;

		sections = value;

		if (sections is INotifyCollectionChanged live)
			live.CollectionChanged += OnSectionsChanged;

		ReloadItems();
	}

	// a flat source change maps 1:1 onto the native batch update
	void OnItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ApplyChange(e);

	void OnSectionsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ReloadItems();

	partial void ReloadItems();

	partial void ApplyChange(
		NotifyCollectionChangedEventArgs change);


	internal bool IsGrouped =>
		sections is not null;

	internal int SectionCount =>
		sections?.Count ?? 1;

	internal int CountIn(
		int section) =>
		sections is { } groups
			? section >= 0 && section < groups.Count ? groups[section].Items.Count : 0
			: itemsSource?.Count ?? 0;

	internal TItem? ItemAt(
		int section,
		int index)
	{
		IReadOnlyList<TItem>? items = sections is { } groups
			? section >= 0 && section < groups.Count ? groups[section].Items : null
			: itemsSource;

		return items is not null && index >= 0 && index < items.Count
			? items[index]
			: null;
	}

	internal Section<TItem>? SectionAt(
		int index) =>
		sections is { } groups && index >= 0 && index < groups.Count
			? groups[index]
			: null;

	internal bool IsEmpty
	{
		get
		{
			for (int section = 0; section < SectionCount; section++)
				if (CountIn(section) > 0)
					return false;

			return true;
		}
	}
}
