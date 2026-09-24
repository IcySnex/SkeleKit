using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace SkeleKit;

/// <summary>
/// A data-driven list, grid, or carousel.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class CollectionView<TItem> : CollectionView<TItem, ISection<TItem>>
	where TItem : class
{
	/// <summary>
	/// Scrolls an indexed item into view.
	/// </summary>
	/// <param name="item">The zero-based item index.</param>
	/// <param name="position">Where the item lands in the viewport.</param>
	/// <param name="animated">Whether the scroll is animated.</param>
	public void ScrollTo(
		int item,
		ScrollPosition position = ScrollPosition.Top,
		bool animated = true) =>
		ScrollTo(0, item, position, animated);
}

/// <summary>
/// A data-driven list, grid, or carousel whose groups carry their own section model.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TSection">The section model the header and footer templates bind to.</typeparam>
public partial class CollectionView<TItem, TSection> : Container, ICollectionHost
	where TItem : class
	where TSection : class, ISection<TItem>
{
	// hooks live only while realized
	bool hooked;

	readonly List<INotifyCollectionChanged> sectionItemHooks = [];

	readonly List<INotifyPropertyChanged> sectionStateHooks = [];

	// maps each hooked items collection and section model to its section index
	readonly Dictionary<object, int> sectionIndex = new(ReferenceEqualityComparer.Instance);

	// sections with a live item or state subscription, pruned when they leave the viewport
	readonly HashSet<int> observedSections = [];

	int loadMoreFiredAt = -1;

	internal bool SuppressSelectionSync;


	private protected override bool ClipsByDefault => true;

	private protected override bool SupportsLayeredBackground => false;

	// UIKit owns the collection's cells and supplementary views. They are not Container children.
	private protected override bool SynchronizesNativeChildren => false;

	internal override bool Scrolls => true;

	internal ICommand? ItemActivation => itemCommand;

	internal bool IsGrouped => sections is not null;

	bool WantsIndexedSource => sections is not null
		? sections is IVirtualizedList<TSection>
		: itemsSource is IVirtualizedList<TItem>;

	// a source whose section type cannot expand never needs the section model for this question
	static readonly bool SectionsCanExpand = typeof(IExpandableSection<TItem>).IsAssignableFrom(typeof(TSection));

	internal bool UsesIndexedSource => indexedSourceMode ?? WantsIndexedSource;

	bool? indexedSourceMode;

	internal int SectionCount => sections?.Count ?? 1;

	internal bool EditingNow => isEditing;

	internal bool MultiSelects => multiSelects;

	internal bool SelectionConfigured => singleSelects || multiSelects;

	internal bool SelectsOutsideEditing => SelectionConfigured && !SelectsOnlyWhileEditing;

	internal bool IsEmpty
	{
		get
		{
			for (int section = 0; section < SectionCount; section++)
			{
				if (CountIn(section) > 0)
					return false;
			}

			return true;
		}
	}

	/// <summary>
	/// The items to show.
	/// </summary>
	/// <remarks>
	/// Changes animate into place when the list is an <c>ObservableCollection</c>.
	/// An <see cref="IVirtualizedList{TItem}"/> is read directly by index without creating a snapshot.
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
	/// <remarks>
	/// Use <see cref="IVirtualizedSectionList{TItem, TSection}"/> when section counts and items
	/// should be produced without first creating every section model.
	/// </remarks>
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
	/// Chooses a strongly typed cell template from each item's runtime type.
	/// </summary>
	/// <remarks>
	/// Takes precedence over <see cref="ItemTemplate"/>. Register every concrete runtime item type before the collection is realized.
	/// </remarks>
	public ItemTemplateSelector<TItem>? ItemTemplateSelector { get; set; }

	/// <summary>
	/// One view above every section. It scrolls with the collection unless <see cref="PinsHeader"/> is enabled.
	/// </summary>
	public View? Header
	{
		get => header;
		set => SetBoundaryContent(ref header, value);
	}
	View? header;

	/// <summary>
	/// Keeps <see cref="Header"/> visible at the collection's top edge while its content scrolls beneath it.
	/// </summary>
	public bool PinsHeader { get; set; }

	/// <summary>
	/// One view below every section. It scrolls with the collection.
	/// </summary>
	public View? Footer
	{
		get => footer;
		set => SetBoundaryContent(ref footer, value);
	}
	View? footer;

	/// <summary>
	/// Builds a header for each section. Bound to the section model.
	/// </summary>
	public Func<ItemView<TSection>>? SectionHeaderTemplate { get; set; }

	/// <summary>
	/// Builds a footer for each section. Bound to the section model.
	/// </summary>
	public Func<ItemView<TSection>>? SectionFooterTemplate { get; set; }

	/// <summary>
	/// How the items are arranged.
	/// </summary>
	public CollectionLayout Layout { get; set; } = CollectionLayout.List();

	/// <summary>
	/// Gives each section its own layout, or null to arrange every section with <see cref="Layout"/>.
	/// </summary>
	/// <remarks>
	/// Mixes arrangements in one collection, like a carousel row above a list. Every section shares the configured item template or selector.
	/// </remarks>
	public Func<TSection, CollectionLayout>? SectionLayout { get; set; }

	/// <summary>
	/// Command invoked with the tapped item.
	/// </summary>
	public ICommand? ItemCommand
	{
		get => itemCommand;
		set => Set(ref itemCommand, value, affectsMeasure: false);
	}
	ICommand? itemCommand;

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
	/// Whether a highlighted row keeps its background until the page next appears.
	/// </summary>
	/// <remarks>
	/// The row appearance is defined by <see cref="ItemView{TItem}.HighlightBackground"/>. With a selection
	/// binding a false value still shows the selection checkmark; only the background is released.
	/// </remarks>
	public bool RetainsHighlight { get; set; } = true;

	/// <summary>
	/// Whether taps change the selection only while editing. Defaults to false.
	/// </summary>
	/// <remarks>
	/// When true, normal taps only activate: <see cref="ItemCommand"/> runs and the transient highlight
	/// behaves as if no selection were bound. Edit mode selects with the shape of <see cref="SelectedItem"/>
	/// or <see cref="SelectedItems"/>.
	/// </remarks>
	public bool SelectsOnlyWhileEditing { get; set; }

	/// <summary>
	/// Whether selected rows show a trailing checkmark drawn by the collection.
	/// </summary>
	public bool ShowsSelectionCheckmark { get; set; }

	/// <summary>
	/// Maps a section to its letter in the fast-scroll index, or null for no index.
	/// </summary>
	/// <remarks>
	/// Grouped list layouts only. Tapping a letter jumps to that section.
	/// </remarks>
	public Func<TSection, string>? SectionIndexTitle { get; set; }

	/// <summary>
	/// Explicit labels for the fast-scroll index, or null to show one per section.
	/// </summary>
	/// <remarks>
	/// A tapped letter with no section jumps to the nearest one at or after it.<br/>
	/// Has no effect without <see cref="SectionIndexTitle"/>, which still supplies each section's letter.
	/// </remarks>
	public BindableList<string> IndexTitles
	{
		get => new(indexTitles);
		set => indexTitlesBinding = Register(indexTitlesBinding, value.Expression, value.Value, SetIndexTitles);
	}
	IReadOnlyList<string>? indexTitles;
	Binding<IReadOnlyList<string>?>? indexTitlesBinding;

	/// <summary>
	/// Invoked when the user scrolls within <see cref="LoadMoreThreshold"/> items of the end.
	/// </summary>
	/// <remarks>
	/// Fires once per item count.
	/// </remarks>
	public ICommand? LoadMoreCommand { get; set; }

	/// <summary>
	/// The parameter passed to <see cref="LoadMoreCommand"/>.
	/// </summary>
	public object? LoadMoreCommandParameter { get; set; }

	/// <summary>
	/// How many items from the end <see cref="LoadMoreCommand"/> fires at.
	/// </summary>
	public int LoadMoreThreshold { get; set; } = 4;

	/// <summary>
	/// Where the collection starts after its first nonempty layout.
	/// </summary>
	public ScrollPosition InitialScrollPosition { get; set; } = ScrollPosition.Top;

	/// <summary>
	/// Shown instead of the items while the source is empty.
	/// </summary>
	public View? EmptyView { get; set; }


	void SetBoundaryContent(
		ref View? field,
		View? value)
	{
		if (ReferenceEquals(field, value))
			return;

		field?.SetParent(null);
		field = value;
		field?.SetParent(this);
	}

	private protected override void PropagateBindingContext()
	{
		Header?.OnBindingContextChanged();
		Footer?.OnBindingContextChanged();
	}

	private protected override void InvalidateChildren()
	{
		Header?.InvalidateSubtree();
		Footer?.InvalidateSubtree();
		EmptyView?.InvalidateSubtree();
		InvalidateVirtualizedChildren();
	}

	partial void InvalidateVirtualizedChildren();

	/// <summary>
	/// Command invoked when the user pulls to refresh.
	/// </summary>
	/// <remarks>
	/// Setting it installs the refresh control.
	/// The <see cref="ICommand.CanExecute(object?)"/> controls whether the user can pull to refresh.
	/// </remarks>
	public ICommand? RefreshCommand
	{
		get => refreshCommand;
		set
		{
			if (ReferenceEquals(refreshCommand, value))
				return;

			refreshCommand = value;
			ApplyRefreshCommand();
		}
	}
	ICommand? refreshCommand;

	/// <summary>
	/// The parameter passed to <see cref="RefreshCommand"/>.
	/// </summary>
	public object? RefreshCommandParameter
	{
		get;
		set => Set(ref field, value, ApplyRefreshCommand, affectsMeasure: false);
	}

	/// <summary>
	/// Whether the refresh spinner is showing.
	/// </summary>
	/// <remarks>
	/// With a two-way binding, pulling sets it to true and the ViewModel sets it to false when done.
	/// </remarks>
	public Bindable<bool> IsRefreshing
	{
		get => isRefreshing;
		set => isRefreshingBinding = Register(isRefreshingBinding, value, value => Set(ref isRefreshing, value, ApplyRefreshing, affectsMeasure: false));
	}
	bool isRefreshing;
	Binding<bool>? isRefreshingBinding;

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

	/// <summary>
	/// Invoked after a drag-to-reorder with an <see cref="ItemMove{TItem}"/>.
	/// </summary>
	/// <remarks>
	/// Setting it enables a long-press drag, unless a context menu owns that gesture; the edit-mode handle always drags.<br/>
	/// The move is already applied to the source when it fires.
	/// </remarks>
	public ICommand? ReorderCommand { get; set; }

	/// <summary>
	/// Whether the collection is in edit mode, showing selection circles and reorder handles.
	/// </summary>
	public Bindable<bool> IsEditing
	{
		get => isEditing;
		set => isEditingBinding = Register(isEditingBinding, value, value => Set(ref isEditing, value, ApplyEditing, affectsMeasure: false));
	}
	bool isEditing;
	Binding<bool>? isEditingBinding;

	/// <summary>
	/// The selected item in single-selection mode, or null for no selection.
	/// </summary>
	/// <remarks>
	/// Binding it enables single selection: taps replace the selection, and a two-way binding pushes the
	/// tapped item to the source. Use <see cref="SelectsOnlyWhileEditing"/> to keep normal taps as
	/// activation. Bind either this or <see cref="SelectedItems"/>, never both.
	/// </remarks>
	public Bindable<TItem?> SelectedItem
	{
		get => new(selectedItem);
		set => selectedItemBinding = Register(selectedItemBinding, value, SetSelectedItem);
	}
	TItem? selectedItem;
	Binding<TItem?>? selectedItemBinding;

	/// <summary>
	/// The selected items in multiple-selection mode, or null for no selection.
	/// </summary>
	/// <remarks>
	/// Binding it enables multiple selection: taps toggle rows, and mutating the bound collection moves the
	/// selection. Give it an <c>ObservableCollection</c> so taps can write back. Use
	/// <see cref="SelectsOnlyWhileEditing"/> to keep normal taps as activation. Bind either this or
	/// <see cref="SelectedItem"/>, never both.
	/// </remarks>
	public BindableList<TItem> SelectedItems
	{
		get => new(selectedItems);
		set => selectedItemsBinding = Register(selectedItemsBinding, value.Expression, value.Value, SetSelectedItems);
	}
	IReadOnlyList<TItem>? selectedItems;
	Binding<IReadOnlyList<TItem>?>? selectedItemsBinding;

	bool singleSelects;
	bool multiSelects;

	/// <summary>
	/// Invoked as the collection scrolls, with the vertical offset in points.
	/// </summary>
	public Action<double>? Scrolled { get; set; }

	/// <summary>
	/// Whether the collection is inset so the keyboard never covers its content.
	/// </summary>
	public bool AvoidsKeyboard
	{
		get;
		set => Set(ref field, value, ApplyKeyboardAvoidance, affectsMeasure: false);
	} = true;

	/// <summary>
	/// How dragging the collection dismisses the keyboard.
	/// </summary>
	public KeyboardDismiss KeyboardDismiss
	{
		get;
		set => Set(ref field, value, ApplyKeyboardDismiss, affectsMeasure: false);
	} = KeyboardDismiss.OnDrag;


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

		SourceKindChanged();
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

		SourceKindChanged();
		HookSectionItems();
		ReloadItems();
	}

	void SetSelectedItem(
		TItem? value)
	{
		if (multiSelects)
			throw new InvalidOperationException("Bind either SelectedItem or SelectedItems, not both.");

		singleSelects = true;

		if (ReferenceEquals(selectedItem, value))
			return;

		selectedItem = value;
		ApplySelection();
	}

	void SetSelectedItems(
		IReadOnlyList<TItem>? value)
	{
		if (singleSelects)
			throw new InvalidOperationException("Bind either SelectedItem or SelectedItems, not both.");

		multiSelects = true;

		if (ReferenceEquals(selectedItems, value))
			return;

		if (hooked && selectedItems is INotifyCollectionChanged old)
			old.CollectionChanged -= OnSelectedItemsChanged;

		selectedItems = value;

		if (hooked && selectedItems is INotifyCollectionChanged live)
			live.CollectionChanged += OnSelectedItemsChanged;

		ApplySelection();
	}

	void SetIndexTitles(
		IReadOnlyList<string>? value)
	{
		if (ReferenceEquals(indexTitles, value))
			return;

		if (hooked && indexTitles is INotifyCollectionChanged old)
			old.CollectionChanged -= OnIndexTitlesChanged;

		indexTitles = value;

		if (hooked && indexTitles is INotifyCollectionChanged live)
			live.CollectionChanged += OnIndexTitlesChanged;

		ReloadIndexTitles();
	}

	void OnIndexTitlesChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args) =>
		ReloadIndexTitles();

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

		if (indexTitles is INotifyCollectionChanged titles)
			titles.CollectionChanged += OnIndexTitlesChanged;

		HookSectionItems();
		ObserveAccessibilityChanges();
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

		if (indexTitles is INotifyCollectionChanged titles)
			titles.CollectionChanged -= OnIndexTitlesChanged;

		UnhookSectionItems();
		UnobserveAccessibilityChanges();

		hooked = false;
	}

	// each section's items are their own source, not just the list of sections
	void HookSectionItems()
	{
		UnhookSectionItems();

		if (!hooked)
			return;

		// An indexed source may contain a very large, lazy section list. Observe sections
		// only when UIKit asks to display them instead of enumerating the source here.
		if (UsesIndexedSource && sections is not null)
			return;

		if (sections is not IReadOnlyList<TSection> groups)
		{
			if (itemsSource is not null)
				sectionIndex[itemsSource] = 0;

			return;
		}

		for (int section = 0; section < groups.Count; section++)
		{
			TSection group = groups[section];

			if (group.Items is INotifyCollectionChanged live)
			{
				live.CollectionChanged += OnSectionItemsChanged;
				sectionItemHooks.Add(live);
				sectionIndex[live] = section;
			}

			if (group is INotifyPropertyChanged notifier)
			{
				notifier.PropertyChanged += OnSectionPropertyChanged;
				sectionStateHooks.Add(notifier);
				sectionIndex[notifier] = section;
			}
		}
	}

	internal void ObserveSection(
		int section)
	{
		if (!hooked || !UsesIndexedSource || sections is not IReadOnlyList<TSection> groups
			|| section < 0 || section >= groups.Count)
			return;

		observedSections.Add(section);

		TSection group = groups[section];

		if (group.Items is INotifyCollectionChanged live && !sectionIndex.ContainsKey(live))
		{
			live.CollectionChanged += OnSectionItemsChanged;
			sectionItemHooks.Add(live);
			sectionIndex[live] = section;
		}

		if (group is INotifyPropertyChanged notifier && !sectionIndex.ContainsKey(notifier))
		{
			notifier.PropertyChanged += OnSectionPropertyChanged;
			sectionStateHooks.Add(notifier);
			sectionIndex[notifier] = section;
		}
	}

	// A virtualized source materializes sections on demand and may replace an evicted instance.
	// Drop the subscriptions of every section that left the viewport, so a source can keep its
	// own cache bounded without the collection pinning old section models.
	internal void SyncObservedSections()
	{
		if (!hooked || !UsesIndexedSource || sections is null || observedSections.Count == 0)
			return;

		HashSet<int> visible = [];

		foreach (NSIndexPath path in Ui.IndexPathsForVisibleItems)
			visible.Add((int)path.Section);

		if (SectionHeaderTemplate is not null || SectionFooterTemplate is not null)
		{
			foreach (NSString kind in new NSString[]
			{
				UICollectionElementKindSectionKey.Header,
				UICollectionElementKindSectionKey.Footer
			})
			{
				foreach (NSIndexPath path in Ui.GetIndexPathsForVisibleSupplementaryElements(kind))
					visible.Add((int)path.Section);
			}
		}

		foreach (int section in observedSections.ToArray())
		{
			if (!visible.Contains(section))
				UnobserveSection(section);
		}
	}

	void UnobserveSection(
		int section)
	{
		observedSections.Remove(section);

		foreach (object hook in sectionIndex
			.Where(pair => pair.Value == section)
			.Select(pair => pair.Key)
			.ToArray())
		{
			if (hook is INotifyCollectionChanged live)
			{
				live.CollectionChanged -= OnSectionItemsChanged;
				sectionItemHooks.Remove(live);
			}
			else if (hook is INotifyPropertyChanged notifier)
			{
				notifier.PropertyChanged -= OnSectionPropertyChanged;
				sectionStateHooks.Remove(notifier);
			}

			sectionIndex.Remove(hook);
		}
	}

	void UnhookSectionItems()
	{
		foreach (INotifyCollectionChanged hook in sectionItemHooks)
			hook.CollectionChanged -= OnSectionItemsChanged;

		sectionItemHooks.Clear();

		foreach (INotifyPropertyChanged hook in sectionStateHooks)
			hook.PropertyChanged -= OnSectionPropertyChanged;

		sectionStateHooks.Clear();
		sectionIndex.Clear();
		observedSections.Clear();
	}

	// -1 means the change cannot be pinned to a section, which callers treat as a rebuild
	int SectionIndexFor(
		object? sender) =>
		sender is not null && sectionIndex.TryGetValue(sender, out int section) ? section : -1;

	void OnItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ItemsChanged(0, e);

	void OnSectionsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e)
	{
		HookSectionItems();
		SectionsChanged(e);
	}

	void OnSectionItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ItemsChanged(SectionIndexFor(sender), e);

	void OnSectionPropertyChanged(
		object? sender,
		PropertyChangedEventArgs e)
	{
		if (e.PropertyName is nameof(IExpandableSection<>.IsExpanded))
			ApplyChange(SectionIndexFor(sender));
	}

	void OnSelectedItemsChanged(
		object? sender,
		NotifyCollectionChangedEventArgs e) =>
		ApplySelection();

	internal void OnRefreshTriggered()
	{
		object? parameter = RefreshCommandParameter;

		if (RefreshCommand is not ICommand command || !command.CanExecute(parameter))
		{
			Set(ref isRefreshing, false, affectsMeasure: false);
			isRefreshingBinding?.PushToSource(false);
			ApplyRefreshing();
			return;
		}

		Set(ref isRefreshing, true, affectsMeasure: false);
		isRefreshingBinding?.PushToSource(true);
		command.Execute(parameter);
	}

	void ApplyRefreshing() =>
		ApplyRefreshingCore();

	void ApplyRefreshCommand() =>
		ApplyRefreshCommandCore();

	void ApplyEditing() =>
		ApplyEditingCore();

	void ApplySelection() =>
		ApplySelectionCore();

	void ApplyKeyboardAvoidance() =>
		ApplyKeyboardAvoidanceCore();

	void ApplyKeyboardDismiss() =>
		ApplyKeyboardDismissCore();

	// needs a writable list; an array throws on RemoveAt
	IList<TItem>? WritableIn(
		int section)
	{
		IList<TItem>? list = sections is IReadOnlyList<TSection> groups
			? section >= 0 && section < groups.Count ? groups[section].Items as IList<TItem> : null
			: itemsSource as IList<TItem>;

		return list is { IsReadOnly: false } ? list : null;
	}

	partial void ApplyRefreshingCore();

	partial void ApplyRefreshCommandCore();

	partial void ApplyEditingCore();

	partial void ApplySelectionCore();

	partial void ApplyKeyboardAvoidanceCore();

	partial void ApplyKeyboardDismissCore();

	partial void ReloadItems();

	partial void ReloadIndexTitles();

	partial void ApplyChange(
		int section);

	partial void ItemsChanged(
		int section,
		NotifyCollectionChangedEventArgs e);

	partial void SectionsChanged(
		NotifyCollectionChangedEventArgs e);

	partial void MovedInSource();

	partial void SourceKindChanged();

	partial void ObserveAccessibilityChanges();

	partial void UnobserveAccessibilityChanges();


	/// <inheritdoc/>
	protected override Size MeasureOverride(
		Size availableSize) =>
		new(double.IsFinite(availableSize.Width) ? availableSize.Width : 0, double.IsFinite(availableSize.Height) ? availableSize.Height : 0);


	internal int CountIn(
		int section) =>
		sections is IVirtualizedSectionList<TItem, TSection> virtualized
			? section >= 0 && section < virtualized.Count ? virtualized.GetItemCount(section) : 0
			: sections is IReadOnlyList<TSection> groups
			? section >= 0 && section < groups.Count ? groups[section].Items.Count : 0
			: itemsSource?.Count ?? 0;

	internal TItem? ItemAt(
		int section,
		int index)
	{
		if (sections is IVirtualizedSectionList<TItem, TSection> virtualized)
		{
			return section >= 0 && section < virtualized.Count
				&& index >= 0 && index < virtualized.GetItemCount(section)
				? virtualized.GetItem(section, index)
				: null;
		}

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

	internal bool IsExpandable(
		int section) =>
		SectionAt(section) is IExpandableSection<TItem>;

	internal bool Expanded(
		int section) =>
		!SectionsCanExpand
		|| SectionAt(section) is not IExpandableSection<TItem> expandable
		|| expandable.IsExpanded;

	internal void ToggleSection(
		int section)
	{
		if (SectionAt(section) is not IExpandableSection<TItem> expandable)
			return;

		expandable.IsExpanded = !expandable.IsExpanded;
		ApplyChange(section);
	}

	internal string? PrefetchUrl(
		int section,
		int index) =>
		ItemAt(section, index) is TItem item
			? Prefetch?.Invoke(item)
			: null;

	// a normal tap activates; selection follows only when taps select outside editing
	internal void SelectFromTap(
		TItem item)
	{
		if (!SelectsOutsideEditing)
			return;

		if (multiSelects)
		{
			if (selectedItems is not IList<TItem> list || list.Contains(item))
				return;

			list.Add(item);
			ApplySelection();
			return;
		}

		if (ReferenceEquals(selectedItem, item))
			return;

		selectedItem = item;
		selectedItemBinding?.PushToSource(item);
	}

	// multiple selection removes on deselect; a single-mode deselect is the old row being replaced
	internal void DeselectFromTap(
		int section,
		int index)
	{
		if (!SelectsOutsideEditing || !multiSelects || selectedItems is not IList<TItem> list || ItemAt(section, index) is not TItem item)
			return;

		if (list.Remove(item))
			ApplySelection();
	}

	internal void EditSelect(
		int section,
		int index,
		bool selected)
	{
		if (ItemAt(section, index) is not TItem item)
			return;

		SuppressSelectionSync = true;

		try
		{
			if (multiSelects && selectedItems is IList<TItem> list)
			{
				if (selected)
				{
					if (!list.Contains(item))
						list.Add(item);
				}
				else
					list.Remove(item);
			}
			else if (singleSelects && selected)
			{
				selectedItem = item;
				selectedItemBinding?.PushToSource(item);
			}
		}
		finally
		{
			SuppressSelectionSync = false;
		}
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

	internal void OnWillDisplay(
		int section,
		int row)
	{
		if (UsesIndexedSource)
		{
			ObserveSection(section);
			SyncIndexedSelection(section, row);
		}

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

		// once per item count
		if (total - position - 1 > LoadMoreThreshold || loadMoreFiredAt == total)
			return;

		loadMoreFiredAt = total;

		object? parameter = LoadMoreCommandParameter;

		if (command.CanExecute(parameter))
			command.Execute(parameter);
	}

	partial void SyncIndexedSelection(
		int section,
		int index);
}

internal interface ICollectionHost
{
	void ApplyInitialScroll();

	void BeginFixedLayoutBoundsChange(
		double width);

	void EndFixedLayoutBoundsChange();

	void SyncObservedSections();

	void KeyboardChanged(
		Rect keyboard,
		bool hiding,
		double duration);

	void SyncEmptyState();

	void SyncInsets();

	void SyncInsetGroupedBoundaryInsets();

	bool CanMove(
		int section,
		int index);

	void Move(
		int fromSection,
		int fromIndex,
		int toSection,
		int toIndex);

	string[]? IndexTitles();

	int IndexSection(
		string title);
}
