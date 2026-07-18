using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace BareUI;

internal interface ICollectionHost
{
	void SyncEmptyState();

	void SyncInsets();

	bool CanMove(
		int section,
		int index);

	void Move(
		int fromSection,
		int fromIndex,
		int toSection,
		int toIndex);
}

/// <summary>
/// A data-driven list, grid, or carousel.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class CollectionView<TItem> : CollectionView<TItem, ISection<TItem>>
	where TItem : class;

/// <summary>
/// A data-driven list, grid, or carousel whose groups carry their own section model.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TSection">The section model the header and footer templates bind to.</typeparam>
public partial class CollectionView<TItem, TSection> : View, ICollectionHost
	where TItem : class
	where TSection : class, ISection<TItem>
{
	/// <summary>
	/// The items to show.
	/// </summary>
	/// <remarks>
	/// Changes animate into place when the list is an <c>ObservableCollection</c>.
	/// </remarks>
	public BindableList<TItem> ItemsSource
	{
		get => new(itemsSource);
		set => itemsSourceBinding = Register(itemsSourceBinding, value.Expression, value.Value, SetItemsSource);
	}
	IReadOnlyList<TItem>? itemsSource;
	Binding<IReadOnlyList<TItem>?>? itemsSourceBinding;

	/// <summary>
	/// Groups, each with its own header. Takes precedence over <see cref="ItemsSource"/>.
	/// </summary>
	public BindableList<TSection> GroupedItemsSource
	{
		get => new(sections);
		set => sectionsBinding = Register(sectionsBinding, value.Expression, value.Value, SetSections);
	}
	IReadOnlyList<TSection>? sections;
	Binding<IReadOnlyList<TSection>?>? sectionsBinding;

	/// <summary>
	/// Builds the element tree for a cell. Called once per recycled cell, never per item.
	/// </summary>
	public Func<ItemView<TItem>>? ItemTemplate { get; set; }

	/// <summary>
	/// Builds a section header. Bound to the section model.
	/// </summary>
	public Func<ItemView<TSection>>? HeaderTemplate { get; set; }

	/// <summary>
	/// Builds a section footer. Bound to the section model.
	/// </summary>
	public Func<ItemView<TSection>>? FooterTemplate { get; set; }

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
	/// Whether rows draw their separator lines.
	/// </summary>
	/// <remarks>
	/// List layouts only.
	/// </remarks>
	public bool ShowsSeparators { get; set; } = true;

	/// <summary>
	/// Leading/trailing insets for the separator lines, or null for the system default.
	/// </summary>
	/// <remarks>
	/// List layouts only.
	/// </remarks>
	public Thickness? SeparatorInsets { get; set; }

	/// <summary>
	/// Whether a tapped row shows a highlight until the page is next appeared.
	/// </summary>
	public bool HighlightsSelection { get; set; } = true;

	/// <summary>
	/// The tapped row's highlight color, or null for the system gray.
	/// </summary>
	public Color? HighlightColor { get; set; }

	/// <summary>
	/// Invoked when the user scrolls within <see cref="LoadMoreThreshold"/> items of the end.
	/// </summary>
	/// <remarks>
	/// Fires once per item count.
	/// </remarks>
	public ICommand? LoadMoreCommand { get; set; }

	/// <summary>
	/// How many items from the end <see cref="LoadMoreCommand"/> fires at.
	/// </summary>
	public int LoadMoreThreshold { get; set; } = 4;

	/// <summary>
	/// Shown instead of the items while the source is empty.
	/// </summary>
	public View? EmptyView { get; set; }

	/// <summary>
	/// Command invoked when the user pulls to refresh.
	/// </summary>
	/// <remarks>
	/// Setting it enables the refresh control.
	/// </remarks>
	public ICommand? RefreshCommand { get; set; }

	/// <summary>
	/// Whether the refresh spinner is showing.
	/// </summary>
	/// <remarks>
	/// Two-way: the pull sets it true, the ViewModel sets it false when done.
	/// </remarks>
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

		if (RefreshCommand is ICommand command && command.CanExecute(null))
			command.Execute(null);
	}

	void ApplyRefreshing() =>
		ApplyRefreshingCore();

	partial void ApplyRefreshingCore();

	/// <summary>
	/// Actions revealed by swiping a row.
	/// </summary>
	/// <remarks>
	/// List layouts only.
	/// </remarks>
	public IList<SwipeAction> SwipeActions { get; } = [];

	/// <summary>
	/// Entries in a row's long-press context menu.
	/// </summary>
	/// <remarks>
	/// Each command is invoked with the row's item.
	/// </remarks>
	public IList<MenuAction> ItemContextMenu { get; } = [];

	/// <summary>
	/// Builds the floating preview shown over a row's context menu, given the row's item.
	/// </summary>
	/// <remarks>
	/// Without it the row itself is the preview.
	/// </remarks>
	public Func<TItem, View>? ItemPreview { get; set; }

	/// <summary>
	/// Shapes the row itself as the lifted platter: padding around the content and a corner radius.
	/// </summary>
	/// <remarks>
	/// Null keeps the system shape.
	/// </remarks>
	public PreviewShape? PreviewShape { get; set; }

	/// <summary>
	/// Invoked with the row's item when its context-menu preview is tapped.
	/// </summary>
	public ICommand? PreviewCommand { get; set; }

	/// <summary>
	/// Maps an item to the image url to warm before its row scrolls on.
	/// </summary>
	/// <remarks>
	/// Setting it enables prefetching through the app's image loader.
	/// </remarks>
	public Func<TItem, string?>? Prefetch { get; set; }

	internal string? PrefetchUrl(
		int section,
		int index) =>
		ItemAt(section, index) is TItem item
			? Prefetch?.Invoke(item)
			: null;

	/// <summary>
	/// Invoked after a drag-to-reorder with an <see cref="ItemMove{TItem}"/>.
	/// </summary>
	/// <remarks>
	/// Setting it enables a long-press drag, unless a context menu owns that gesture; the edit-mode handle always drags.<br/>
	/// The move is already applied to the source when it fires.
	/// </remarks>
	public ICommand? ReorderCommand { get; set; }

	/// <summary>
	/// Whether the collection is in edit mode, showing selection circles and reorder handles. Two-way.
	/// </summary>
	public Bindable<bool> IsEditing
	{
		get => isEditing;
		set => isEditingBinding = Register(isEditingBinding, value, value => Set(ref isEditing, value, ApplyEditing, affectsMeasure: false));
	}
	bool isEditing;
	Binding<bool>? isEditingBinding;

	/// <summary>
	/// The items checked while editing.
	/// </summary>
	/// <remarks>
	/// Give it an <c>ObservableCollection</c>: taps keep it in sync, and mutating it moves the checkmarks.
	/// </remarks>
	public BindableList<TItem> SelectedItems
	{
		get => new(selectedItems);
		set => selectedItemsBinding = Register(selectedItemsBinding, value.Expression, value.Value, SetSelectedItems);
	}
	IReadOnlyList<TItem>? selectedItems;
	Binding<IReadOnlyList<TItem>?>? selectedItemsBinding;

	void ApplyEditing() =>
		ApplyEditingCore();

	partial void ApplyEditingCore();

	void SetSelectedItems(
		IReadOnlyList<TItem>? value)
	{
		if (ReferenceEquals(selectedItems, value))
			return;

		if (hooked && selectedItems is INotifyCollectionChanged old)
			old.CollectionChanged -= OnSelectedItemsChanged;

		selectedItems = value;

		if (hooked && selectedItems is INotifyCollectionChanged live)
			live.CollectionChanged += OnSelectedItemsChanged;

		ApplySelection();
	}

	void OnSelectedItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ApplySelection();

	void ApplySelection() =>
		ApplySelectionCore();

	partial void ApplySelectionCore();

	internal bool EditingNow => isEditing;

	internal bool MultiSelects => selectedItems is not null;

	// a tap in edit mode toggles membership instead of firing SelectionCommand
	internal bool suppressSelectionSync;

	internal void EditSelect(
		int section,
		int index,
		bool selected)
	{
		if (ItemAt(section, index) is not TItem item || selectedItems is not IList<TItem> list)
			return;

		suppressSelectionSync = true;

		try
		{
			if (selected)
			{
				if (!list.Contains(item))
					list.Add(item);
			}
			else
				list.Remove(item);
		}
		finally
		{
			suppressSelectionSync = false;
		}
	}

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
		IReadOnlyList<TItem>? value)
	{
		if (ReferenceEquals(itemsSource, value))
			return;

		if (hooked && itemsSource is INotifyCollectionChanged old)
			old.CollectionChanged -= OnItemsChanged;

		itemsSource = value;

		if (hooked && itemsSource is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;

		ReloadItems();
	}

	void SetSections(
		IReadOnlyList<TSection>? value)
	{
		if (ReferenceEquals(sections, value))
			return;

		if (hooked && sections is INotifyCollectionChanged old)
			old.CollectionChanged -= OnSectionsChanged;

		sections = value;

		if (hooked && sections is INotifyCollectionChanged live)
			live.CollectionChanged += OnSectionsChanged;

		HookSectionItems();
		ReloadItems();
	}

	// a source outlives its view: hooks live only while realized
	bool hooked;

	readonly List<INotifyCollectionChanged> sectionItemHooks = [];

	void HookSources()
	{
		if (hooked)
			return;

		hooked = true;

		if (itemsSource is INotifyCollectionChanged items)
			items.CollectionChanged += OnItemsChanged;

		if (sections is INotifyCollectionChanged groups)
			groups.CollectionChanged += OnSectionsChanged;

		if (selectedItems is INotifyCollectionChanged selection)
			selection.CollectionChanged += OnSelectedItemsChanged;

		HookSectionItems();
	}

	void UnhookSources()
	{
		if (!hooked)
			return;

		if (itemsSource is INotifyCollectionChanged items)
			items.CollectionChanged -= OnItemsChanged;

		if (sections is INotifyCollectionChanged groups)
			groups.CollectionChanged -= OnSectionsChanged;

		if (selectedItems is INotifyCollectionChanged selection)
			selection.CollectionChanged -= OnSelectedItemsChanged;

		UnhookSectionItems();

		hooked = false;
	}

	// each section's own items are a source in their own right, not just the list of sections
	void HookSectionItems()
	{
		UnhookSectionItems();

		if (!hooked)
			return;

		foreach (TSection section in sections ?? [])
			if (section.Items is INotifyCollectionChanged live)
			{
				live.CollectionChanged += OnSectionItemsChanged;
				sectionItemHooks.Add(live);
			}
	}

	void UnhookSectionItems()
	{
		foreach (INotifyCollectionChanged hook in sectionItemHooks)
			hook.CollectionChanged -= OnSectionItemsChanged;

		sectionItemHooks.Clear();
	}

	// a flat source change maps 1:1 onto the native batch update
	void OnItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ApplyChange();

	void OnSectionsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e)
	{
		HookSectionItems();
		ReloadItems();
	}

	void OnSectionItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ApplyChange();

	partial void ReloadItems();

	partial void ApplyChange();


	internal bool IsGrouped => sections is not null;

	internal int SectionCount => sections?.Count ?? 1;

	internal int CountIn(
		int section) =>
		sections is IReadOnlyList<TSection> groups
			? section >= 0 && section < groups.Count ? groups[section].Items.Count : 0
			: itemsSource?.Count ?? 0;

	internal TItem? ItemAt(
		int section,
		int index)
	{
		IReadOnlyList<TItem>? items = sections is IReadOnlyList<TSection> groups
			? section >= 0 && section < groups.Count ? groups[section].Items : null
			: itemsSource;

		return items is not null && index >= 0 && index < items.Count
			? items[index]
			: null;
	}

	internal TSection? SectionAt(
		int index) =>
		sections is IReadOnlyList<TSection> groups && index >= 0 && index < groups.Count
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


	// reorder needs a list it can write back into; an array answers IList but throws on RemoveAt
	IList<TItem>? WritableIn(
		int section)
	{
		IList<TItem>? list = sections is IReadOnlyList<TSection> groups
			? section >= 0 && section < groups.Count ? groups[section].Items as IList<TItem> : null
			: itemsSource as IList<TItem>;

		return list is { IsReadOnly: false } ? list : null;
	}

	internal bool CanMove(
		int section,
		int index) =>
		ReorderCommand is not null && WritableIn(section) is not null && ItemAt(section, index) is not null;

	internal void Move(
		int fromSection,
		int fromIndex,
		int toSection,
		int toIndex)
	{
		if (WritableIn(fromSection) is not IList<TItem> from || WritableIn(toSection) is not IList<TItem> to)
			return;

		if (fromIndex < 0 || fromIndex >= from.Count)
			return;

		TItem item = from[fromIndex];

		if (ReferenceEquals(from, to))
		{
			if (toIndex >= from.Count)
				toIndex = from.Count - 1;

			if (fromIndex == toIndex)
				return;

			if (from is ObservableCollection<TItem> observable)
				observable.Move(fromIndex, toIndex);
			else
			{
				from.RemoveAt(fromIndex);
				from.Insert(toIndex, item);
			}
		}
		else
		{
			from.RemoveAt(fromIndex);

			if (toIndex > to.Count)
				toIndex = to.Count;

			to.Insert(toIndex, item);
		}

		MovedInSource();

		if (ReorderCommand is ICommand command)
		{
			ItemMove<TItem> move = new(item, fromSection, fromIndex, toSection, toIndex);

			if (command.CanExecute(move))
				command.Execute(move);
		}
	}

	// the native side lands the matching snapshot before UIKit's drop animation settles
	partial void MovedInSource();


	int loadMoreFiredAt = -1;

	internal void OnWillDisplay(
		int section,
		int row)
	{
		if (LoadMoreCommand is not ICommand command)
			return;

		int total = 0;
		int position = row;

		for (int index = 0; index < SectionCount; index++)
		{
			if (index < section)
				position += CountIn(index);

			total += CountIn(index);
		}

		// once per item count: one crossing asks for one page
		if (total - position - 1 > LoadMoreThreshold || loadMoreFiredAt == total)
			return;

		loadMoreFiredAt = total;

		if (command.CanExecute(null))
			command.Execute(null);
	}
}
