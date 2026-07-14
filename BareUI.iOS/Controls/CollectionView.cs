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
public class CollectionView<TItem> : CollectionView<TItem, ISection<TItem>>
	where TItem : class;

/// <summary>
/// A data-driven list, grid, or carousel scroll layout container whose groups carry their own section model.
/// </summary>
/// <typeparam name="TItem">The underlying object instance type managed by item container collections.</typeparam>
/// <typeparam name="TSection">The section model the header and footer templates bind to.</typeparam>
public partial class CollectionView<TItem, TSection> : View, ICollectionHost
	where TItem : class
	where TSection : class, ISection<TItem>
{
	/// <summary>
	/// The items to show. Changes animate into place when the list is an <c>ObservableCollection</c>.
	/// </summary>
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
	/// Whether rows draw their separator lines. List layouts only.
	/// </summary>
	public bool ShowsSeparators { get; set; } = true;

	/// <summary>
	/// Leading/trailing insets for the separator lines, or null for the system default. List layouts only.
	/// </summary>
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
	/// Invoked when the user scrolls within <see cref="LoadMoreThreshold"/> items of the end. Fires once per item count.
	/// </summary>
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
	/// Entries in a row's long-press context menu. Each command is invoked with the row's item.
	/// </summary>
	public IList<MenuAction> ItemContextMenu { get; } = [];

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

		if (itemsSource is INotifyCollectionChanged old)
			old.CollectionChanged -= OnItemsChanged;

		itemsSource = value;

		if (itemsSource is INotifyCollectionChanged live)
			live.CollectionChanged += OnItemsChanged;

		ReloadItems();
	}

	void SetSections(
		IReadOnlyList<TSection>? value)
	{
		if (ReferenceEquals(sections, value))
			return;

		if (sections is INotifyCollectionChanged old)
			old.CollectionChanged -= OnSectionsChanged;

		sections = value;

		if (sections is INotifyCollectionChanged live)
			live.CollectionChanged += OnSectionsChanged;

		HookSectionItems();
		ReloadItems();
	}

	// each section's own items are a source in their own right, not just the list of sections
	void HookSectionItems()
	{
		foreach (INotifyCollectionChanged hook in sectionItemHooks)
			hook.CollectionChanged -= OnSectionItemsChanged;

		sectionItemHooks.Clear();

		foreach (TSection section in sections ?? [])
			if (section.Items is INotifyCollectionChanged live)
			{
				live.CollectionChanged += OnSectionItemsChanged;
				sectionItemHooks.Add(live);
			}
	}
	readonly List<INotifyCollectionChanged> sectionItemHooks = [];

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

	internal TSection? SectionAt(
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


	int loadMoreFiredAt = -1;

	internal void OnWillDisplay(
		int section,
		int row)
	{
		if (LoadMoreCommand is not { } command)
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
