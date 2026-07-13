using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace BareUI;

internal interface ICollectionHost
{
	void SyncEmptyState();

	void SyncInsets();
}

/// <summary>
/// A data-driven list, grid, or carousel scroll layout container wrapping native platform components.
/// </summary>
/// <typeparam name="TItem">The underlying object instance type managed by item container collections.</typeparam>
public partial class CollectionView<TItem> : View, ICollectionHost where TItem : class
{
	/// <summary>
	/// The items to show. Every collection change animates into place.
	/// </summary>
	public Bindable<ObservableCollection<TItem>?> ItemsSource
	{
		get => itemsSource;
		set => itemsSourceBinding = Register(itemsSourceBinding, value, SetItemsSource);
	}
	ObservableCollection<TItem>? itemsSource;
	Binding<ObservableCollection<TItem>?>? itemsSourceBinding;

	/// <summary>
	/// Titled groups, each with its own header. Takes precedence over <see cref="ItemsSource"/>.
	/// </summary>
	public Bindable<ObservableCollection<Section<TItem>>?> GroupedItemsSource
	{
		get => sections;
		set => sectionsBinding = Register(sectionsBinding, value, SetSections);
	}
	ObservableCollection<Section<TItem>>? sections;
	Binding<ObservableCollection<Section<TItem>>?>? sectionsBinding;

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
	public ICommand? SelectionCommand
	{
		get => selectionCommand;
		set => Set(ref selectionCommand, value, affectsMeasure: false);
	}
	ICommand? selectionCommand;

	internal ICommand? Selection =>
		selectionCommand;

	/// <summary>
	/// Shown instead of the items while the source is empty.
	/// </summary>
	public View? EmptyView { get; set; }

	/// <summary>
	/// Command invoked when the user pulls to refresh. Setting it enables the refresh control.
	/// </summary>
	public ICommand? RefreshCommand { get; set; }

	/// <summary>
	/// Whether the refresh spinner is showing. Two-way: the pull sets it true, the ViewModel sets it false when done.
	/// </summary>
	public Bindable<bool> IsRefreshing
	{
		get => isRefreshing;
		set => isRefreshingBinding = Register(isRefreshingBinding, value, value => Set(ref isRefreshing, value, ApplyRefreshing, affectsMeasure: false));
	}
	bool isRefreshing;
	Binding<bool>? isRefreshingBinding;

	void OnRefreshTriggered()
	{
		Set(ref isRefreshing, true, affectsMeasure: false);
		isRefreshingBinding?.PushToSource(true);

		if (RefreshCommand is { } command && command.CanExecute(null))
			command.Execute(null);
	}

	void ApplyRefreshing() =>
		ApplyRefreshingCore();

	partial void ApplyRefreshingCore();

	/// <summary>
	/// Actions revealed by swiping a row. List layouts only.
	/// </summary>
	public IList<SwipeAction> SwipeActions { get; } = [];

	/// <summary>
	/// Entries in a row's long-press context menu.
	/// </summary>
	public IList<MenuAction> ContextMenu { get; } = [];

	/// <summary>
	/// Invoked as the collection scrolls, with the vertical offset in points.
	/// </summary>
	public Action<double>? Scrolled { get; set; }


	private protected override bool ClipsByDefault => true;

	internal override bool Scrolls => true;

	// it scrolls itself, so it takes the space it is offered rather than sizing to its content
	protected override Size MeasureOverride(
		Size availableSize) =>
		new(double.IsFinite(availableSize.Width) ? availableSize.Width : 0, double.IsFinite(availableSize.Height) ? availableSize.Height : 0);

	void SetItemsSource(
		ObservableCollection<TItem>? value)
	{
		if (ReferenceEquals(itemsSource, value))
			return;

		if (itemsSource is not null)
			itemsSource.CollectionChanged -= OnItemsChanged;

		itemsSource = value;

		if (itemsSource is not null)
			itemsSource.CollectionChanged += OnItemsChanged;

		ReloadItems();
	}

	void SetSections(
		ObservableCollection<Section<TItem>>? value)
	{
		if (ReferenceEquals(sections, value))
			return;

		if (sections is not null)
			sections.CollectionChanged -= OnSectionsChanged;

		sections = value;

		if (sections is not null)
			sections.CollectionChanged += OnSectionsChanged;

		ReloadItems();
	}

	// a flat source change maps 1:1 onto the native batch update
	void OnItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ApplyChange();

	void OnSectionsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ReloadItems();

	partial void ReloadItems();

	partial void ApplyChange();


	internal bool IsGrouped => sections is not null;

	internal int SectionCount => sections?.Count ?? 1;

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
