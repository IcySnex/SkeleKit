using System.Collections.Specialized;
using System.Windows.Input;
using CoreFoundation;
using ObjCRuntime;

namespace SkeleKit;

public partial class CollectionView<TItem, TSection> : ISystemInsetScroll
{
	internal const string CellId = "SkeleCell";
	internal const string HeaderId = "SkeleHeader";
	internal const string FooterId = "SkeleFooter";
	internal const string LayoutHeaderKind = "SkeleLayoutHeader";
	internal const string LayoutFooterKind = "SkeleLayoutFooter";
	internal const string LayoutHeaderId = "SkeleLayoutHeader";
	internal const string LayoutFooterId = "SkeleLayoutFooter";

	// items carry stable NSNumber identifiers; the diffable store hashes identifiers natively on
	// every insert and comparison, where a managed key wrapper costs a trampoline per touch
	readonly Dictionary<object, NSNumber> keys = new(ReferenceEqualityComparer.Instance);
	readonly Dictionary<long, object> itemsById = [];
	nint nextIdentifier;

	readonly List<NSNumber> sectionKeys = [];

	// the identifiers applied per section, so a change re-keys one section instead of the world
	readonly List<NSNumber[]> sectionItems = [];
	readonly HashSet<int> dirtySections = [];
	bool structureDirty = true;
	int appendedFrom = -1;

	readonly Dictionary<ItemTemplateRegistration<TItem>, ICollectionItemView> sizingViews = [];
	readonly HashSet<ICollectionItemView> itemViews = [];
	readonly List<IndexedSourceChange> indexedChanges = [];

	CollectionSource? data;
	IndexedCollectionSource<TItem, TSection>? indexedData;
	FixedGridLayout<TItem, TSection>? fixedLayout;
	CollectionDelegate<TItem, TSection>? selection;
	EmptyCollectionHost? emptyHost;
	ItemTemplateRegistration<TItem>? defaultItemTemplate;

	ItemView<TSection>? headerSizingView;
	ItemView<TSection>? footerSizingView;
	FixedGridAnchor? pendingFixedGridAnchor;
	double fixedLayoutWidth = -1;

	bool snapshotQueued;
	bool indexedReloadQueued;
	bool initialScrollPending = true;
	bool usesSystemContentInsets;
	bool layoutInsetsSynced;
	Thickness layoutInsets;
	Thickness? insetGroupedHeaderInsets;
	Thickness? insetGroupedFooterInsets;
	nfloat keyboardCover;

	private protected override UIView CreateNative()
	{
		bool carousel = Layout.Kind is CollectionLayoutKind.Carousel;
		defaultItemTemplate = ItemTemplateSelector is null && ItemTemplate is Func<ItemView<TItem>> template
			? ItemTemplateRegistration<TItem>.CreateDefault(CellId, template)
			: null;

		UICollectionViewLayout nativeLayout = CreateNativeLayout();

		CollectionHost collection = new(this, nativeLayout)
		{
			BackgroundColor = UIColor.Clear,
			InsetsLayoutMarginsFromSafeArea = false,

			AlwaysBounceVertical = !carousel,
			AlwaysBounceHorizontal = carousel,

			ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never,
			AutomaticallyAdjustsScrollIndicatorInsets = false
		};

		if (ItemTemplateSelector is ItemTemplateSelector<TItem> selector)
		{
			foreach (ItemTemplateRegistration<TItem> itemTemplate in selector.Templates)
				collection.RegisterClassForCell(typeof(SkeleCell), itemTemplate.ReuseIdentifier);
		}
		else
		{
			collection.RegisterClassForCell(typeof(SkeleCell), CellId);
		}
		collection.RegisterClassForSupplementaryView(
			typeof(SkeleHeader),
			UICollectionElementKindSection.Header,
			HeaderId);
		collection.RegisterClassForSupplementaryView(
			typeof(SkeleHeader),
			UICollectionElementKindSection.Footer,
			FooterId);
		collection.RegisterClassForSupplementaryView(
			typeof(SkeleHeader),
			new NSString(LayoutHeaderKind),
			LayoutHeaderId);
		collection.RegisterClassForSupplementaryView(
			typeof(SkeleHeader),
			new NSString(LayoutFooterKind),
			LayoutFooterId);

		ConfigureDataSource(collection);

		selection = new(this);
		collection.Delegate = selection;

		if (Prefetch is not null)
			collection.PrefetchDataSource = selection;

		ApplyRefresh(collection);
		ApplyReorder(collection);

		return collection;
	}

	void ConfigureDataSource(
		UICollectionView collection)
	{
		indexedSourceMode = WantsIndexedSource;

		if (UsesIndexedSource)
		{
			indexedData = new(this, CellForIndex, SupplementaryFor);
			collection.DataSource = indexedData;
			return;
		}

		data = new(this, collection, CellFor)
		{
			SupplementaryViewProvider = SupplementaryFor
		};
	}

	partial void SourceKindChanged()
	{
		if (!IsRealized || indexedSourceMode == WantsIndexedSource)
			return;

		data = null;
		indexedData = null;
		indexedReloadQueued = false;
		indexedChanges.Clear();
		snapshotQueued = false;
		structureDirty = true;
		appendedFrom = -1;
		dirtySections.Clear();
		sectionItems.Clear();
		sectionKeys.Clear();
		keys.Clear();
		itemsById.Clear();

		ConfigureDataSource(Ui);
	}

	bool ISystemInsetScroll.UseSystemContentInsets()
	{
		if (Layout.Kind is CollectionLayoutKind.Carousel)
			return false;

		usesSystemContentInsets = true;
		Ui.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Automatic;
		Ui.AutomaticallyAdjustsScrollIndicatorInsets = true;
		SyncInsets();
		return true;
	}

	UIRefreshControl? refresh;
	ICommand? observedRefreshCommand;

	void ObserveRefreshCommand()
	{
		if (ReferenceEquals(observedRefreshCommand, RefreshCommand))
			return;

		StopObservingRefreshCommand();
		observedRefreshCommand = RefreshCommand;

		if (observedRefreshCommand is not null)
			observedRefreshCommand.CanExecuteChanged += OnRefreshCanExecuteChanged;
	}

	void StopObservingRefreshCommand()
	{
		if (observedRefreshCommand is not null)
			observedRefreshCommand.CanExecuteChanged -= OnRefreshCanExecuteChanged;

		observedRefreshCommand = null;
	}

	void OnRefreshCanExecuteChanged(
		object? sender,
		EventArgs e) =>
		MainThread.Post(ApplyRefreshCanExecute);

	void ApplyRefreshCanExecute()
	{
		if (refresh is not null)
			refresh.Enabled = RefreshCommand?.CanExecute(RefreshCommandParameter) is true;
	}

	void OnNativeRefreshTriggered(
		object? sender,
		EventArgs e) =>
		OnRefreshTriggered();

	void ApplyRefresh(
		UICollectionView collection)
	{
		if (RefreshCommand is null)
		{
			if (refresh is not null)
				refresh.Enabled = false;

			return;
		}

		if (refresh is null)
		{
			refresh = new();
			refresh.ValueChanged += OnNativeRefreshTriggered;
			collection.RefreshControl = refresh;
		}

		ApplyRefreshCanExecute();
	}

	partial void ApplyRefreshCommandCore()
	{
		if (!IsRealized)
			return;

		ObserveRefreshCommand();
		ApplyRefresh(Ui);
	}

	partial void ApplyRefreshingCore()
	{
		if (refresh is null)
			return;

		if (IsRefreshing.Value)
		{
			if (!refresh.Refreshing)
				refresh.BeginRefreshing();

			return;
		}

		// finishing under a held finger yanks the inset mid-drag: wait for the release
		if (Ui.Dragging)
		{
			endsAfterDrag = true;
			return;
		}

		EndNativeRefresh();
	}

	partial void ApplyKeyboardAvoidanceCore()
	{
		if (AvoidsKeyboard || !IsRealized)
			return;

		keyboardCover = 0;
		ApplyKeyboardLayout();
	}

	partial void ApplyKeyboardDismissCore()
	{
		if (!IsRealized)
			return;

		Ui.KeyboardDismissMode = KeyboardDismiss switch
		{
			KeyboardDismiss.OnDrag => UIScrollViewKeyboardDismissMode.OnDrag,
			KeyboardDismiss.Interactive => UIScrollViewKeyboardDismissMode.Interactive,
			_ => UIScrollViewKeyboardDismissMode.None
		};
	}

	bool endsAfterDrag;

	internal void OnDragEnded()
	{
		if (endsAfterDrag)
		{
			endsAfterDrag = false;
			EndNativeRefresh();
			return;
		}

		// a diff held back during a refreshing drag still has to land
		FlushChanges();
	}

	void EndNativeRefresh()
		=> FlushChanges(() =>
		{
			refresh?.EndRefreshing();
			SyncInsets();
		});

	UILongPressGestureRecognizer? reorderRecognizer;
	bool reordering;

	void ApplyReorder(
		UICollectionView collection)
	{
		// a row context menu owns the long-press; reorder then lives on the edit-mode handle
		if (ReorderCommand is null || ItemContextMenu.Count > 0 || ItemPreview is not null)
			return;

		UILongPressGestureRecognizer recognizer = null!;
		recognizer = new(() => TrackReorder(recognizer));

		reorderRecognizer = recognizer;
		collection.AddGestureRecognizer(recognizer);
	}

	void TrackReorder(
		UILongPressGestureRecognizer recognizer)
	{
		UICollectionView ui = Ui;

		switch (recognizer.State)
		{
			case UIGestureRecognizerState.Began:
				if (ui.IndexPathForItemAtPoint(recognizer.LocationInView(ui)) is NSIndexPath path)
					reordering = ui.BeginInteractiveMovementForItem(path);
				break;

			case UIGestureRecognizerState.Changed:
				if (reordering)
					ui.UpdateInteractiveMovement(recognizer.LocationInView(ui));
				break;

			case UIGestureRecognizerState.Ended:
				if (reordering)
				{
					reordering = false;
					ui.EndInteractiveMovement();
				}
				break;

			case UIGestureRecognizerState.Possible:
			case UIGestureRecognizerState.Cancelled:
			case UIGestureRecognizerState.Failed:
			default:
				if (reordering)
				{
					reordering = false;
					ui.CancelInteractiveMovement();
				}
				break;
		}
	}

	// the drop animation must settle against the moved data, not a stale snapshot
	partial void MovedInSource()
	{
		if (UsesIndexedSource)
			QueueIndexedReload();
		else
			QueueSnapshot();

		FlushChanges();
	}

	partial void ApplyEditingCore()
	{
		if (!IsRealized)
			return;

		// the mode must be known before edit mode begins, or the circles never show
		Ui.AllowsMultipleSelectionDuringEditing = MultiSelects;
		Ui.Editing = isEditing;

		ApplySelectionCore();
	}

	partial void ApplySelectionCore()
	{
		if (!IsRealized || SuppressSelectionSync)
			return;

		bool active = isEditing || !SelectsOnlyWhileEditing;
		bool multiActive = active && MultiSelects;

		if (Ui.AllowsMultipleSelection != multiActive)
			Ui.AllowsMultipleSelection = multiActive;

		// selection that does not apply in this mode leaves the tapped row's transient highlight alone
		if (!active || !SelectionConfigured)
			return;

		if (UsesIndexedSource)
		{
			foreach (NSIndexPath path in Ui.IndexPathsForVisibleItems)
				SyncIndexedSelection(path.Section, path.Row);

			return;
		}

		if (data is null)
			return;

		HashSet<NSIndexPath> wanted = [];

		if (singleSelects)
		{
			if (selectedItem is TItem item && keys.TryGetValue(item, out NSNumber? key) && data.GetIndexPath(key) is NSIndexPath path)
				wanted.Add(path);
		}
		else
		{
			foreach (TItem item in selectedItems!)
			{
				if (keys.TryGetValue(item, out NSNumber? key) && data.GetIndexPath(key) is NSIndexPath path)
					wanted.Add(path);
			}
		}

		foreach (NSIndexPath path in Ui.GetIndexPathsForSelectedItems() ?? [])
		{
			if (!wanted.Remove(path))
				Ui.DeselectItem(path, false);
		}

		foreach (NSIndexPath path in wanted)
			Ui.SelectItem(path, false, UICollectionViewScrollPosition.None);
	}

	partial void SyncIndexedSelection(
		int section,
		int index)
	{
		if (!UsesIndexedSource || !SelectionConfigured || (!isEditing && SelectsOnlyWhileEditing)
			|| ItemAt(section, index) is not TItem item)
			return;

		bool wanted = singleSelects
			? ReferenceEquals(selectedItem, item)
			: selectedItems?.Any(selected => ReferenceEquals(selected, item)) == true;
		NSIndexPath path = NSIndexPath.FromRowSection(index, section);
		bool selected = Ui.GetIndexPathsForSelectedItems()?.Contains(path) == true;

		if (wanted && !selected)
			Ui.SelectItem(path, false, UICollectionViewScrollPosition.None);
		else if (!wanted && selected)
			Ui.DeselectItem(path, false);
	}

	/// <summary>
	/// Scrolls the list until <paramref name="item"/> is visible, aligned to the given viewport edge.
	/// </summary>
	/// <param name="item">The item to bring into view.</param>
	/// <param name="position">The viewport edge the item aligns to.</param>
	/// <param name="animated">Whether the scroll is animated.</param>
	public void ScrollTo(
		TItem item,
		ScrollPosition position = ScrollPosition.Top,
		bool animated = true)
	{
		if (!IsRealized)
			return;

		if (UsesIndexedSource)
		{
			for (int section = 0; section < SectionCount; section++)
			{
				for (int index = 0; index < CountIn(section); index++)
				{
					if (!ReferenceEquals(ItemAt(section, index), item))
						continue;

					ScrollTo(section, index, position, animated);
					return;
				}
			}

			return;
		}

		if (data is null || !keys.TryGetValue(item, out NSNumber? key))
			return;

		if (data.GetIndexPath(key) is NSIndexPath path)
			Ui.ScrollToItem(path, NativeScrollPosition(position), animated);
	}

	/// <summary>
	/// Scrolls an indexed item into view.
	/// </summary>
	/// <param name="section">The zero-based section index.</param>
	/// <param name="item">The zero-based item index within the section.</param>
	/// <param name="position">Where the item lands in the viewport.</param>
	/// <param name="animated">Whether the scroll is animated.</param>
	public void ScrollTo(
		int section,
		int item,
		ScrollPosition position = ScrollPosition.Top,
		bool animated = true)
	{
		if (!IsRealized || section < 0 || section >= SectionCount || item < 0 || item >= CountIn(section))
			return;

		FlushChanges();
		Ui.LayoutIfNeeded();
		Ui.ScrollToItem(NSIndexPath.FromRowSection(item, section), NativeScrollPosition(position), animated);
	}

	/// <summary>
	/// Scrolls to the beginning, middle, or end of the collection's content.
	/// </summary>
	/// <param name="position">The content position to reveal.</param>
	/// <param name="animated">Whether the scroll is animated.</param>
	public void ScrollTo(
		ScrollPosition position,
		bool animated = true)
	{
		if (!IsRealized)
			return;

		FlushChanges();
		Ui.LayoutIfNeeded();
		SetContentPosition(position, animated);
	}

	UICollectionViewScrollPosition NativeScrollPosition(
		ScrollPosition position)
	{
		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;
		return position switch
		{
			ScrollPosition.Center => horizontal
				? UICollectionViewScrollPosition.CenteredHorizontally
				: UICollectionViewScrollPosition.CenteredVertically,
			ScrollPosition.Bottom => horizontal
				? UICollectionViewScrollPosition.Right
				: UICollectionViewScrollPosition.Bottom,
			_ => horizontal
				? UICollectionViewScrollPosition.Left
				: UICollectionViewScrollPosition.Top
		};
	}

	void SetContentPosition(
		ScrollPosition position,
		bool animated)
	{
		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;
		nfloat viewport = horizontal ? Ui.Bounds.Width : Ui.Bounds.Height;
		nfloat extent = horizontal ? Ui.ContentSize.Width : Ui.ContentSize.Height;
		if (viewport <= 0 || extent <= 0)
			return;

		nfloat leading = horizontal ? Ui.AdjustedContentInset.Left : Ui.AdjustedContentInset.Top;
		nfloat trailing = horizontal ? Ui.AdjustedContentInset.Right : Ui.AdjustedContentInset.Bottom;
		nfloat start = -leading;
		nfloat end = (nfloat)Math.Max((double)start, (double)(extent - viewport + trailing));
		nfloat target = position switch
		{
			ScrollPosition.Center => (start + end) / 2,
			ScrollPosition.Bottom => end,
			_ => start
		};
		CGPoint current = Ui.ContentOffset;
		Ui.SetContentOffset(horizontal ? new(target, current.Y) : new(current.X, target), animated);
	}

	internal void OnScrolled(
		double offset) =>
		Scrolled?.Invoke(offset);

	internal UISwipeActionsConfiguration? SwipeConfiguration(
		NSIndexPath indexPath,
		SwipeSide side)
	{
		if (ItemAt(indexPath.Section, indexPath.Row) is not TItem item)
			return null;

		List<UIContextualAction> actions = [];

		foreach (SwipeAction action in SwipeActions)
		{
			if (action.Side != side)
				continue;

			UIContextualAction native = UIContextualAction.FromContextualActionStyle(
				action.IsDestructive
					? UIContextualActionStyle.Destructive
					: UIContextualActionStyle.Normal,
				action.Text,
				(_, _, done) =>
				{
					if (action.Command is ICommand command && command.CanExecute(item))
						command.Execute(item);

					// the diff must land before done resets the swipe, or a removed row slides
					// back into view for a beat before the queued snapshot takes it out
					FlushChanges();
					done(true);
				});

			if (action.Icon is ImageSource icon)
				native.Image = icon.ResolveLocal();

			if (action.Background is Color background)
				native.BackgroundColor = background.ToUIColor();

			actions.Add(native);
		}

		return actions.Count == 0
			? null
			: UISwipeActionsConfiguration.FromActions([.. actions]);
	}

	// ReSharper disable once NotAccessedField.Local
	PreviewHost? activePreview;
	TItem? menuItem;

	internal UIContextMenuConfiguration? MenuConfiguration(
		NSIndexPath indexPath)
	{
		if (ItemContextMenu.Count == 0 && ItemPreview is null && PreviewShape is null)
			return null;

		if (ItemAt(indexPath.Section, indexPath.Row) is not TItem item)
			return null;

		menuItem = item;

		UIContextMenuContentPreviewProvider? preview = ItemPreview is Func<TItem, View> factory
			? () => activePreview = new(factory(item), Ui.Bounds.Width)
			: null;

		// the identifier carries the path to the platter-shaping callbacks
		return UIContextMenuConfiguration.Create(
			indexPath,
			preview,
			_ =>
			{
				UIAction[] entries = new UIAction[ItemContextMenu.Count];

				for (int index = 0; index < ItemContextMenu.Count; index++)
				{
					MenuAction entry = ItemContextMenu[index];
					object parameter = entry.CommandParameter ?? item;

					entries[index] = UIAction.Create(
						entry.Text,
						entry.Icon?.ResolveLocal(),
						null,
						_ =>
						{
							if (entry.Command is ICommand command && command.CanExecute(parameter))
								command.Execute(parameter);
						});
					entries[index].Subtitle = entry.Subtitle;

					if (entry.IsDestructive)
						entries[index].Attributes = UIMenuElementAttributes.Destructive;
				}

				return UIMenu.Create(entries);
			});
	}

	internal void CommitPreview()
	{
		if (PreviewCommand is not ICommand command || menuItem is not TItem item)
			return;

		if (command.CanExecute(item))
			command.Execute(item);
	}

	internal void EndPreview() =>
		activePreview = null;

	internal UITargetedPreview? ShapedPreview(
		UIContextMenuConfiguration configuration)
	{
		if (PreviewShape is not PreviewShape shape || !IsRealized || configuration.Identifier is not NSIndexPath indexPath)
			return null;

		if (Ui.CellForItem(indexPath) is not UICollectionViewCell cell)
			return null;

		nfloat padding = (nfloat)shape.Padding;

		UIPreviewParameters parameters = new()
		{
			VisiblePath = UIBezierPath.FromRoundedRect(
				cell.ContentView.Frame.Inset(-padding, -padding),
				(nfloat)shape.CornerRadius)
		};

		if (shape.Background is Color background)
			parameters.BackgroundColor = background.ToUIColor();

		return new(cell.ContentView, parameters);
	}

	NSObject? contentSizeObserver;

	partial void ObserveAccessibilityChanges()
	{
		if (contentSizeObserver is not null)
			return;

		contentSizeObserver = UIApplication.Notifications.ObserveContentSizeCategoryChanged((_, _) => InvalidateFixedGeometry(keepAnchor: true));
	}

	partial void UnobserveAccessibilityChanges()
	{
		if (contentSizeObserver is null)
			return;

		NSNotificationCenter.DefaultCenter.RemoveObserver(contentSizeObserver);
		contentSizeObserver = null;
	}

	private protected override void ApplyProperties()
	{
		HookSources();
		ReloadItems();
		ApplyEditingCore();
		ApplyKeyboardDismissCore();
		ObserveRefreshCommand();
		ApplyRefresh(Ui);
	}

	private protected override void OnUnrealized()
	{
		UnhookSources();
		ClearEmptyHost();
		StopObservingRefreshCommand();

		foreach (ICollectionItemView itemView in itemViews)
		{
			itemView.View.Unrealize();
			itemView.View.TintHost = null;
		}

		itemViews.Clear();
		sizingViews.Clear();
		sizedWithItem = false;

		// the next realization builds against a fresh data source
		structureDirty = true;
		appendedFrom = -1;
		dirtySections.Clear();
		sectionItems.Clear();
		data = null;
		indexedData = null;
		fixedLayout = null;
		pendingFixedGridAnchor = null;
		headerSizingView = null;
		footerSizingView = null;
		fixedLayoutWidth = -1;
		indexedReloadQueued = false;
		indexedChanges.Clear();
		indexedSourceMode = null;
		initialScrollPending = true;

		Header?.Unrealize();
		Footer?.Unrealize();

		if (refresh is not null)
			refresh.ValueChanged -= OnNativeRefreshTriggered;

		refresh = null;
		endsAfterDrag = false;

		if (reorderRecognizer is UILongPressGestureRecognizer recognizer && IsRealized)
		{
			Ui.RemoveGestureRecognizer(recognizer);
			recognizer.Dispose();
		}

		reorderRecognizer = null;

		activePreview = null;
	}

	UICollectionView Ui => (UICollectionView)Native;

	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		if (!IsRealized)
			return;

		foreach (UICollectionViewCell cell in Ui.VisibleCells)
		{
			if (cell is SkeleCell { Hosted: { } hosted })
				hosted.ReapplyVisuals();
		}

		Header?.ReapplyVisuals();
		Footer?.ReapplyVisuals();
		EmptyView?.ReapplyVisuals();
	}

	internal override void TintChanged()
	{
		if (!IsRealized)
			return;

		foreach (UICollectionViewCell cell in Ui.VisibleCells)
		{
			if (cell is not SkeleCell skele)
				continue;

			if (skele.Hosted is { LocalTint: null } hosted)
				hosted.TintChanged();

			skele.ApplySelectionTint();
		}

		if (Header is { LocalTint: null } header)
			header.TintChanged();

		if (Footer is { LocalTint: null } footer)
			footer.TintChanged();

		if (EmptyView is View empty && empty.LocalTint is null)
			empty.TintChanged();
	}

	partial void InvalidateVirtualizedChildren()
	{
		foreach (ICollectionItemView itemView in itemViews)
			itemView.View.InvalidateSubtree();

		if (!IsRealized)
			return;

		foreach (UICollectionViewCell nativeCell in Ui.VisibleCells)
		{
			if (nativeCell is not SkeleCell cell
				|| Ui.IndexPathForCell(cell) is not NSIndexPath indexPath)
				continue;

			CollectionLayout layout = LayoutForSection(indexPath.Section);
			cell.SetAutomaticMinimumHeight(layout.Kind is CollectionLayoutKind.List
				? SystemListMetrics.MinimumRowHeight(layout.Grouped)
				: 0);
		}

		Ui.CollectionViewLayout.InvalidateLayout();
		emptyHost?.SetNeedsLayout();
		Ui.SetNeedsLayout();
	}

	// a transient tap highlight is released on the way back; selection that applies outside editing persists
	internal override void PageWillAppear()
	{
		Header?.PageWillAppear();
		Footer?.PageWillAppear();

		// accessibility text size and the first-day setting can change while the page is off screen
		InvalidateFixedGeometry(keepAnchor: true);

		if (!IsRealized || isEditing || SelectsOutsideEditing)
			return;

		foreach (NSIndexPath path in Ui.GetIndexPathsForSelectedItems() ?? [])
			Ui.DeselectItem(path, true);
	}

	partial void ReloadItems()
	{
		fixedLayout?.MarkGeometryDirty();

		if (UsesIndexedSource)
		{
			indexedChanges.Clear();
			QueueIndexedReload();
			return;
		}

		// the section list itself changed: only a full rebuild knows the new shape
		structureDirty = true;
		QueueSnapshot();
	}

	partial void ReloadIndexTitles()
	{
		if (!IsRealized)
			return;

		if (UsesIndexedSource)
			QueueIndexedReload();
		else
			Ui.ReloadData();
	}

	partial void ApplyChange(
		int section)
	{
		fixedLayout?.MarkGeometryDirty();

		if (UsesIndexedSource)
		{
			QueueIndexedReload();
			return;
		}

		// an untracked sender means the sources changed shape under us
		if (section < 0)
		{
			ReloadItems();
			return;
		}

		dirtySections.Add(section);
		QueueSnapshot();
	}

	partial void ItemsChanged(
		int section,
		NotifyCollectionChangedEventArgs e)
	{
		if (UsesIndexedSource)
		{
			if (TryApplyIndexedContentChange(section, e))
				return;

			QueueIndexedReload(IndexedSourceChange.Items(section, e));
			return;
		}

		// a content-only replacement keeps its identifier: update the identity maps, rebind
		// any visible cell, and skip the snapshot entirely
		if (TryApplyContentChange(section, e))
			return;

		ApplyChange(section);
	}

	bool TryApplyIndexedContentChange(
		int section,
		NotifyCollectionChangedEventArgs e)
	{
		if (!IsRealized || indexedData is null
			|| section < 0 || section >= SectionCount
			|| e.Action is not NotifyCollectionChangedAction.Replace
			|| e.OldItems is not { Count: > 0 } oldItems
			|| e.NewItems is not { } newItems || oldItems.Count != newItems.Count
			|| e.OldStartingIndex < 0 || e.OldStartingIndex != e.NewStartingIndex)
			return false;

		int start = e.OldStartingIndex;
		if (start > CountIn(section) - oldItems.Count)
			return false;

		for (int offset = 0; offset < oldItems.Count; offset++)
		{
			if (oldItems[offset] is not TItem oldItem || newItems[offset] is not TItem newItem
				|| !ReferenceEquals(TemplateFor(oldItem), TemplateFor(newItem)))
				return false;

			if (multiSelects && !ReferenceEquals(oldItem, newItem)
				&& !SelectionRemapping.CanReplace(selectedItems, oldItem))
				return false;
		}

		for (int offset = 0; offset < oldItems.Count; offset++)
		{
			TItem oldItem = (TItem)oldItems[offset]!;
			TItem newItem = (TItem)newItems[offset]!;

			if (!ReferenceEquals(oldItem, newItem))
				RemapSelection(oldItem, newItem);

			RebindIndexedVisible(section, start + offset, newItem);
		}

		return true;
	}

	bool TryApplyContentChange(
		int section,
		NotifyCollectionChangedEventArgs e)
	{
		if (!IsRealized || data is null
			|| structureDirty || dirtySections.Count > 0 || appendedFrom >= 0
			|| section < 0 || section >= sectionItems.Count
			|| e.Action is not NotifyCollectionChangedAction.Replace
			|| e.OldItems is not { Count: > 0 } oldItems
			|| e.NewItems is not { } newItems || oldItems.Count != newItems.Count
			|| e.OldStartingIndex < 0 || e.OldStartingIndex != e.NewStartingIndex)
			return false;

		NSNumber[] identifiers = sectionItems[section];
		int start = e.OldStartingIndex;
		if (identifiers.Length != CountIn(section) || start > identifiers.Length - oldItems.Count)
			return false;

		// validate the whole range before touching either map: swaps, duplicate references,
		// and template changes still need the structural diff
		for (int offset = 0; offset < oldItems.Count; offset++)
		{
			if (oldItems[offset] is not TItem oldItem || newItems[offset] is not TItem newItem
				|| !keys.TryGetValue(oldItem, out NSNumber? key)
				|| !key.Equals(identifiers[start + offset])
				|| !ReferenceEquals(TemplateFor(oldItem), TemplateFor(newItem))
				|| (!ReferenceEquals(oldItem, newItem) && keys.ContainsKey(newItem)))
				return false;

			// A read-only selection cannot be remapped in place. Use the structural
			// path so it can drop the old selection without throwing mid-update.
			if (multiSelects && !ReferenceEquals(oldItem, newItem)
				&& !SelectionRemapping.CanReplace(selectedItems, oldItem))
				return false;
		}

		for (int offset = 0; offset < oldItems.Count; offset++)
		{
			TItem oldItem = (TItem)oldItems[offset]!;
			TItem newItem = (TItem)newItems[offset]!;

			if (ReferenceEquals(oldItem, newItem))
				continue;

			NSNumber key = identifiers[start + offset];
			keys.Remove(oldItem);
			keys[newItem] = key;
			itemsById[key.LongValue] = newItem;

			RemapSelection(oldItem, newItem);
			RebindVisible(key, newItem);
		}

		return true;
	}

	void RebindVisible(
		NSNumber key,
		TItem item)
	{
		if (data?.GetIndexPath(key) is not NSIndexPath path)
			return;

		if (Ui.CellForItem(path) is not SkeleCell { Hosted: ICollectionItemView hosted } cell)
			return;

		RebindVisible(cell, hosted, item);
	}

	void RebindIndexedVisible(
		int section,
		int index,
		TItem item)
	{
		NSIndexPath path = NSIndexPath.FromRowSection(index, section);
		if (Ui.CellForItem(path) is not SkeleCell { Hosted: ICollectionItemView hosted } cell)
			return;

		RebindVisible(cell, hosted, item);
	}

	static void RebindVisible(
		SkeleCell cell,
		ICollectionItemView hosted,
		TItem item)
	{

		if (UIAccessibility.IsReduceMotionEnabled)
		{
			hosted.SetItem(item);
			return;
		}

		// content changes animate the way structural snapshot changes already do; a
		// still-running animation from the previous item would fight the new values
		int token = ++cell.AnimationToken;
		hosted.View.CancelAnimations();

		View.Animate(
			Animation.Ease(0.25),
			() => hosted.SetItem(item),
			_ =>
			{
				if (cell.AnimationToken == token)
					cell.AnimationToken = 0;
			},
			layout: false);
	}

	void RemapSelection(
		TItem oldItem,
		TItem newItem)
	{
		if (singleSelects && ReferenceEquals(selectedItem, oldItem))
		{
			selectedItem = newItem;
			selectedItemBinding?.PushToSource(newItem);

			return;
		}

		if (!multiSelects || selectedItems is not IList<TItem> { IsReadOnly: false } list)
			return;

		for (int index = 0; index < list.Count; index++)
		{
			if (!ReferenceEquals(list[index], oldItem))
				continue;

			list[index] = newItem;
			return;
		}
	}

	partial void SectionsChanged(
		NotifyCollectionChangedEventArgs e)
	{
		fixedLayout?.MarkGeometryDirty();

		if (UsesIndexedSource)
		{
			QueueIndexedReload(IndexedSourceChange.Sections(e));
			return;
		}

		// sections appended at the tail extend the applied snapshot as-is
		if (!structureDirty
			&& sectionItems.Count > 0
			&& e.Action is NotifyCollectionChangedAction.Add
			&& e.OldItems is null
			&& e.NewItems is { Count: > 0 } added
			&& e.NewStartingIndex >= sectionItems.Count
			&& e.NewStartingIndex + added.Count == SectionCount)
		{
			appendedFrom = appendedFrom < 0 ? e.NewStartingIndex : Math.Min(appendedFrom, e.NewStartingIndex);
			QueueSnapshot();
			return;
		}

		ReloadItems();
	}

	void QueueIndexedReload(
		IndexedSourceChange? change = null)
	{
		if (!IsRealized || indexedData is null)
			return;

		fixedLayout?.MarkGeometryDirty();

		if (change is IndexedSourceChange sourceChange)
			indexedChanges.Add(sourceChange);

		if (indexedReloadQueued)
			return;

		indexedReloadQueued = true;
		DispatchQueue.MainQueue.DispatchAsync(() =>
		{
			if (!indexedReloadQueued)
				return;

			if (refresh is { Refreshing: true } && Ui.Dragging)
				return;

			indexedReloadQueued = false;
			if (IsRealized)
				ReloadIndexedData();
		});
	}

	void ReloadIndexedData()
	{
		if (!IsRealized || indexedData is null)
			return;

		UICollectionView view = Ui;
		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;
		double oldOffset = horizontal ? view.ContentOffset.X : view.ContentOffset.Y;
		double oldViewport = horizontal ? view.Bounds.Width : view.Bounds.Height;
		double oldExtent = horizontal ? view.ContentSize.Width : view.ContentSize.Height;
		double oldLeading = horizontal ? view.AdjustedContentInset.Left : view.AdjustedContentInset.Top;
		double oldTrailing = horizontal ? view.AdjustedContentInset.Right : view.AdjustedContentInset.Bottom;
		double oldEnd = Math.Max(-oldLeading, oldExtent - oldViewport + oldTrailing);
		bool pinnedToEnd = !initialScrollPending && Math.Abs(oldOffset - oldEnd) < 2;

		NSIndexPath? anchor = view.IndexPathsForVisibleItems
			.OrderBy(path => path.Section)
			.ThenBy(path => path.Row)
			.FirstOrDefault();
		double relative = 0;
		if (anchor is not null && view.GetLayoutAttributesForItem(anchor) is UICollectionViewLayoutAttributes oldAttributes)
			relative = (horizontal ? oldAttributes.Frame.X : oldAttributes.Frame.Y) - oldOffset;

		anchor = AdjustIndexedAnchor(anchor);
		indexedChanges.Clear();

		view.ReloadData();
		view.CollectionViewLayout.InvalidateLayout();
		view.LayoutIfNeeded();

		if (initialScrollPending)
		{
			ApplyInitialScroll();
			return;
		}

		if (pinnedToEnd)
		{
			SetContentPosition(ScrollPosition.Bottom, animated: false);
			return;
		}

		if (anchor is not null && anchor.Section < SectionCount && anchor.Row < CountIn((int)anchor.Section)
			&& view.GetLayoutAttributesForItem(anchor) is UICollectionViewLayoutAttributes newAttributes)
		{
			double target = (horizontal ? newAttributes.Frame.X : newAttributes.Frame.Y) - relative;
			CGPoint current = view.ContentOffset;
			view.SetContentOffset(horizontal ? new(target, current.Y) : new(current.X, target), false);
		}

		ApplySelectionCore();
	}

	NSIndexPath? AdjustIndexedAnchor(
		NSIndexPath? anchor)
	{
		if (anchor is null)
			return null;

		int section = (int)anchor.Section;
		int row = (int)anchor.Row;

		foreach (IndexedSourceChange change in indexedChanges)
		{
			if (change.IsSectionChange)
			{
				if (!TryAdjustIndex(ref section, change))
					return null;
			}
			else if (section == change.Section && !TryAdjustIndex(ref row, change))
				return null;
		}

		return NSIndexPath.FromRowSection(row, section);
	}

	static bool TryAdjustIndex(
		ref int index,
		IndexedSourceChange change)
	{
		switch (change.Action)
		{
			case NotifyCollectionChangedAction.Add:
				if (change.NewIndex >= 0 && index >= change.NewIndex)
					index += change.NewCount;
				return true;

			case NotifyCollectionChangedAction.Remove:
				if (change.OldIndex < 0)
					return false;

				if (index >= change.OldIndex + change.OldCount)
					index -= change.OldCount;
				else if (index >= change.OldIndex)
					index = change.OldIndex;
				return true;

			case NotifyCollectionChangedAction.Replace:
				if (change.OldIndex < 0 || change.NewIndex < 0)
					return false;

				if (index >= change.OldIndex + change.OldCount)
					index += change.NewCount - change.OldCount;
				else if (index >= change.OldIndex)
					index = change.NewIndex + Math.Min(index - change.OldIndex, Math.Max(0, change.NewCount - 1));
				return true;

			case NotifyCollectionChangedAction.Move:
				if (change.OldIndex < 0 || change.NewIndex < 0 || change.OldCount == 0)
					return false;

				if (index >= change.OldIndex && index < change.OldIndex + change.OldCount)
					index = change.NewIndex + index - change.OldIndex;
				else if (change.OldIndex < change.NewIndex
					&& index >= change.OldIndex + change.OldCount
					&& index < change.NewIndex + change.OldCount)
					index -= change.OldCount;
				else if (change.NewIndex < change.OldIndex
					&& index >= change.NewIndex && index < change.OldIndex)
					index += change.OldCount;
				return true;

			case NotifyCollectionChangedAction.Reset:
			default:
				return false;
		}
	}

	void QueueSnapshot()
	{
		if (!IsRealized || snapshotQueued)
			return;

		snapshotQueued = true;

		DispatchQueue.MainQueue.DispatchAsync(() =>
		{
			// a flush may have landed it already
			if (!snapshotQueued)
				return;

			// a batch update under a held refresh drag interrupts the touch and yanks the offset:
			// the diff stays queued and the drag's end flushes it, the way Mail lands new rows
			if (refresh is { Refreshing: true } && Ui.Dragging)
				return;

			snapshotQueued = false;

			if (IsRealized)
				ApplySnapshot();
		});
	}

	void FlushChanges(
		Action? completed = null)
	{
		if (!UsesIndexedSource)
		{
			FlushSnapshot(completed);
			return;
		}

		if (!indexedReloadQueued || !IsRealized)
		{
			completed?.Invoke();
			return;
		}

		indexedReloadQueued = false;
		ReloadIndexedData();
		completed?.Invoke();
	}

	// coalescing waits a turn, but UIKit's own animations (a collapsing swipe, a settling refresh
	// control) must not run against stale data: these apply the pending diff right now
	void FlushSnapshot(
		Action? completed = null)
	{
		if (!snapshotQueued || !IsRealized)
		{
			completed?.Invoke();
			return;
		}

		snapshotQueued = false;
		ApplySnapshot(completed);
	}

	internal void ApplyInitialScroll()
	{
		if (!initialScrollPending || !IsRealized)
			return;

		if (InitialScrollPosition is ScrollPosition.Top)
		{
			initialScrollPending = false;
			return;
		}

		if (SectionCount == 0 || IsEmpty)
			return;

		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;
		nfloat viewport = horizontal ? Ui.Bounds.Width : Ui.Bounds.Height;
		nfloat extent = horizontal ? Ui.ContentSize.Width : Ui.ContentSize.Height;
		if (viewport <= 0 || extent <= 0)
			return;

		initialScrollPending = false;
		SetContentPosition(InitialScrollPosition, animated: false);
	}

	readonly record struct FixedGridAnchor(
		int Section,
		double RelativeOffset);

	// a width, text size, or item count change moves the section tops; the anchor keeps the
	// first visible month in place across the rebuild
	internal void InvalidateFixedGeometry(
		bool keepAnchor)
	{
		if (fixedLayout is null || !IsRealized)
			return;

		FixedGridAnchor? anchor = keepAnchor ? CaptureFixedGridAnchor() : null;

		fixedLayout.MarkGeometryDirty();
		fixedLayout.InvalidateLayout();

		if (anchor is FixedGridAnchor value)
		{
			Ui.LayoutIfNeeded();
			RestoreFixedGridAnchor(value);
		}
	}

	internal void BeginFixedLayoutBoundsChange(
		double width)
	{
		if (fixedLayout is null || width == fixedLayoutWidth)
			return;

		fixedLayoutWidth = width;
		pendingFixedGridAnchor = CaptureFixedGridAnchor();
		fixedLayout.MarkGeometryDirty();
		fixedLayout.InvalidateLayout();
	}

	internal void EndFixedLayoutBoundsChange()
	{
		if (pendingFixedGridAnchor is not FixedGridAnchor anchor)
			return;

		pendingFixedGridAnchor = null;
		RestoreFixedGridAnchor(anchor);
	}

	FixedGridAnchor? CaptureFixedGridAnchor()
	{
		if (fixedLayout is null || !IsRealized)
			return null;

		NSIndexPath? path = Ui.IndexPathsForVisibleItems
			.OrderBy(path => path.Section)
			.ThenBy(path => path.Row)
			.FirstOrDefault();

		if (path is null || !fixedLayout.TrySectionTop((int)path.Section, out double top))
			return null;

		return new((int)path.Section, top - Ui.ContentOffset.Y);
	}

	void RestoreFixedGridAnchor(
		FixedGridAnchor anchor)
	{
		if (fixedLayout is null || !IsRealized || !fixedLayout.TrySectionTop(anchor.Section, out double top))
			return;

		double minimum = -Ui.AdjustedContentInset.Top;
		double maximum = Math.Max(minimum, fixedLayout.ContentHeight - Ui.Bounds.Height + Ui.AdjustedContentInset.Bottom);
		double y = Math.Clamp(top - anchor.RelativeOffset, minimum, maximum);
		CGPoint current = Ui.ContentOffset;

		if (Math.Abs((double)current.Y - y) < 0.5)
			return;

		Ui.SetContentOffset(new CGPoint(current.X, y), false);
	}

	// a full rebuild runs when the section list changes shape; item changes re-key only their own
	// section, which keeps a one-day edit out of the every-item marshalling path
	void ApplySnapshot(
		Action? completed = null)
	{
		fixedLayout?.MarkGeometryDirty();

		if (data is null)
		{
			completed?.Invoke();
			return;
		}

		int sections = SectionCount;

		while (sectionKeys.Count < sections)
			sectionKeys.Add(NSNumber.FromInt32(sectionKeys.Count));

		bool appending = appendedFrom >= 0
			&& appendedFrom == sectionItems.Count
			&& appendedFrom <= sections;

		// deleting identifiers from an applied snapshot costs several times more than appending
		// them, so a change covering a wide slice of sections is cheaper as a full rebuild than
		// as section-by-section re-keying
		bool mostlyDirty = dirtySections.Count * 4 > sections;

		bool full = structureDirty
			|| sectionItems.Count == 0
			|| (!appending && sectionItems.Count != sections)
			|| mostlyDirty;

		NSDiffableDataSourceSnapshot<NSNumber, NSNumber> snapshot = full
			? new()
			: data.Snapshot;

		if (full)
		{
			sectionItems.Clear();

			for (int section = 0; section < sections; section++)
				sectionItems.Add([]);

			if (sections > 0)
				snapshot.AppendSections(SectionKeyRange(sections, 0));
		}
		else if (appending)
		{
			while (sectionItems.Count < sections)
				sectionItems.Add([]);

			if (sections > appendedFrom)
				snapshot.AppendSections(SectionKeyRange(sections, appendedFrom));
		}

		// re-keying a section drops its old identifiers and re-appends them in current order
		for (int section = 0; section < sections; section++)
		{
			bool touched = full
				|| (appending && section >= appendedFrom)
				|| dirtySections.Contains(section);

			if (!touched)
				continue;

			NSNumber[] previous = sectionItems[section];

			if (previous.Length > 0)
				snapshot.DeleteItems(previous);

			sectionItems[section] = Rekey(snapshot, section, sectionKeys[section]);
		}

		structureDirty = false;
		appendedFrom = -1;
		dirtySections.Clear();

		Prune();

		bool animated = Ui.Window is not null;
		if (initialScrollPending && InitialScrollPosition is not ScrollPosition.Top)
		{
			data.ApplySnapshot(snapshot, animated, () =>
			{
				Ui.LayoutIfNeeded();
				ApplyInitialScroll();
				completed?.Invoke();
			});
		}
		else if (completed is null)
			data.ApplySnapshot(snapshot, animated);
		else
			data.ApplySnapshot(snapshot, animated, completed);

		// the first real item corrects the row height the empty template guessed
		if (Layout.Kind is CollectionLayoutKind.Grid && !sizedWithItem && ItemAt(0, 0) is not null)
			Ui.CollectionViewLayout.InvalidateLayout();

		// a diff can shuffle index paths under the checkmarks
		ApplySelectionCore();

		SyncHeaderChevrons();
	}

	NSNumber[] SectionKeyRange(
		int count,
		int start)
	{
		NSNumber[] keys = new NSNumber[count - start];
		for (int index = start; index < count; index++)
			keys[index - start] = sectionKeys[index];

		return keys;
	}

	NSNumber[] Rekey(
		NSDiffableDataSourceSnapshot<NSNumber, NSNumber> snapshot,
		int section,
		NSNumber sectionKey)
	{
		int count = Expanded(section) ? CountIn(section) : 0;
		if (count == 0)
			return [];

		NSNumber[] identifiers = new NSNumber[count];
		for (int index = 0; index < count; index++)
			identifiers[index] = KeyFor(ItemAt(section, index)!);

		snapshot.AppendItems(identifiers, sectionKey);
		return identifiers;
	}

	void SyncHeaderChevrons()
	{
		if (!IsGrouped)
			return;

		NSString kind = new(UICollectionElementKindSectionKey.Header.ToString());

		foreach (NSIndexPath path in Ui.GetIndexPathsForVisibleSupplementaryElements(kind))
		{
			if (Ui.GetSupplementaryView(kind, path) is SkeleHeader header)
				header.SetExpanded(Expanded(path.Section), animated: true);
		}
	}

	NSNumber KeyFor(
		object item)
	{
		if (!keys.TryGetValue(item, out NSNumber? key))
		{
			key = NSNumber.FromLong(++nextIdentifier);
			keys[item] = key;
			itemsById[key.LongValue] = item;
		}

		return key;
	}

	// replaced items leave a stale identifier behind; pruning every change would put the
	// O(all items) scan back on the hot path, so the map only shrinks once it outgrows the source
	const int PruneSlack = 256;

	void Prune()
	{
		int live = 0;
		for (int section = 0; section < SectionCount; section++)
			live += CountIn(section);

		if (keys.Count <= live + PruneSlack)
			return;

		HashSet<object> current = new(ReferenceEqualityComparer.Instance);

		for (int section = 0; section < SectionCount; section++)
		{
			for (int index = 0; index < CountIn(section); index++)
			{
				if (ItemAt(section, index) is TItem item)
					current.Add(item);
			}
		}

		foreach (object item in keys.Keys.ToArray())
		{
			if (!current.Contains(item))
			{
				itemsById.Remove(keys[item].LongValue);
				keys.Remove(item);
			}
		}
	}

	SkeleCell CellFor(
		UICollectionView collectionView,
		NSIndexPath indexPath,
		NSObject identifier)
	{
		if (identifier is not NSNumber itemKey
			|| !itemsById.TryGetValue(itemKey.LongValue, out object? bound)
			|| bound is not TItem item)
			throw new InvalidOperationException("The collection snapshot contains an invalid item identifier.");

		return BindCell(collectionView, indexPath, item);
	}

	internal SkeleCell CellForIndex(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		ObserveSection(indexPath.Section);
		if (ItemAt(indexPath.Section, indexPath.Row) is not TItem item)
			throw new InvalidOperationException("The indexed source returned no item for a visible index path.");

		return BindCell(collectionView, indexPath, item);
	}

	SkeleCell BindCell(
		UICollectionView collectionView,
		NSIndexPath indexPath,
		TItem item)
	{
		ItemTemplateRegistration<TItem> itemTemplate = TemplateFor(item);
		SkeleCell cell = (SkeleCell)collectionView.DequeueReusableCell(itemTemplate.ReuseIdentifier, indexPath);

		// the tree is built once per recycled cell, then only rebound
		if (cell.Hosted is null)
		{
			ICollectionItemView created = CreateItemView(itemTemplate);

			cell.Attach(
				created,
				SelectionConfigured,
				ReorderCommand is not null,
				ShowsSelectionCheckmark && SelectsOutsideEditing);
		}

		if (cell.Hosted is ICollectionItemView view)
		{
			// a running content-change animation from the previous item would fight the new values
			if (cell.AnimationToken != 0)
			{
				cell.AnimationToken = 0;
				view.View.CancelAnimations();
			}

			view.SetItem(item);
		}

		cell.SetRetainsHighlight(RetainsHighlight);

		CollectionLayout layout = LayoutForSection(indexPath.Section);
		cell.SetAutomaticMinimumHeight(layout.Kind is CollectionLayoutKind.List
			? SystemListMetrics.MinimumRowHeight(layout.Grouped)
			: 0);

		return cell;
	}

	internal SkeleHeader SupplementaryFor(
		UICollectionView collectionView,
		string kind,
		NSIndexPath indexPath)
	{
		if (kind == LayoutHeaderKind)
			return LayoutBoundaryFor(collectionView, kind, indexPath, Header, LayoutHeaderId);

		if (kind == LayoutFooterKind)
			return LayoutBoundaryFor(collectionView, kind, indexPath, Footer, LayoutFooterId);

		bool footer = kind == UICollectionElementKindSectionKey.Footer.ToString();
		ObserveSection(indexPath.Section);

		SkeleHeader header = (SkeleHeader)collectionView.DequeueReusableSupplementaryView(
			new NSString(kind),
			footer ? FooterId : HeaderId,
			indexPath);

		if (header.Hosted is null && (footer ? CreateFooterView() : CreateHeaderView()) is ItemView<TSection> view)
		{
			view.TintHost = this;
			header.Attach(view);
		}

		TSection? boundSection = SectionAt(indexPath.Section);
		header.Hidden = boundSection is null;

		if (header.Hosted is ItemView<TSection> hosted && boundSection is not null)
			hosted.SetItem(boundSection);

		CollectionLayout sectionLayout = LayoutForSection(indexPath.Section);
		header.SetContentInsets(sectionLayout is { Kind: CollectionLayoutKind.List, Grouped: false }
			? () => new(ContentInsets.Left, 0, ContentInsets.Right, 0)
			: null);

		if (!footer)
		{
			int section = indexPath.Section;
			header.SetExpandable(IsExpandable(section), Expanded(section), () => ToggleSection(section));
		}

		return header;
	}

	SkeleHeader LayoutBoundaryFor(
		UICollectionView collectionView,
		string kind,
		NSIndexPath indexPath,
		View? content,
		string reuseId)
	{
		SkeleHeader boundary = (SkeleHeader)collectionView.DequeueReusableSupplementaryView(
			new NSString(kind),
			reuseId,
			indexPath);

		if (boundary.Hosted is null && content is View view)
		{
			view.TintHost = this;
			boundary.Attach(view);
		}

		bool footer = kind == LayoutFooterKind;
		boundary.SetContentInsets(() => LayoutBoundaryInsets(footer));

		return boundary;
	}

	Thickness LayoutBoundaryInsets(
		bool footer)
	{
		Thickness insets = ContentInsets;
		Thickness? grouped = footer ? insetGroupedFooterInsets : insetGroupedHeaderInsets;

		if (grouped is Thickness horizontal)
			insets = new(horizontal.Left, insets.Top, horizontal.Right, insets.Bottom);

		return footer
			? new(insets.Left, 0, insets.Right, insets.Bottom)
			: new(insets.Left, insets.Top, insets.Right, 0);
	}

	void ICollectionHost.SyncEmptyState() =>
		SyncEmptyState();

	void ICollectionHost.ApplyInitialScroll() =>
		ApplyInitialScroll();

	void ICollectionHost.BeginFixedLayoutBoundsChange(
		double width) =>
		BeginFixedLayoutBoundsChange(width);

	void ICollectionHost.EndFixedLayoutBoundsChange() =>
		EndFixedLayoutBoundsChange();

	void ICollectionHost.SyncObservedSections() =>
		SyncObservedSections();

	void ICollectionHost.SyncInsets() =>
		SyncInsets();

	void ICollectionHost.SyncInsetGroupedBoundaryInsets() =>
		SyncInsetGroupedBoundaryInsets();

	void ICollectionHost.KeyboardChanged(
		Rect keyboard,
		bool hiding,
		double duration) =>
		OnKeyboardChanged(keyboard, hiding, duration);

	bool ICollectionHost.CanMove(
		int section,
		int index) =>
		CanMove(section, index);

	void ICollectionHost.Move(
		int fromSection,
		int fromIndex,
		int toSection,
		int toIndex) =>
		Move(fromSection, fromIndex, toSection, toIndex);

	string[]? ICollectionHost.IndexTitles()
	{
		if (SectionIndexTitle is not Func<TSection, string> letterOf || !IsGrouped)
			return null;

		if (indexTitles is IReadOnlyList<string> explicitTitles)
			return [.. explicitTitles];

		string[] titles = new string[SectionCount];
		for (int section = 0; section < SectionCount; section++)
			titles[section] = SectionAt(section) is TSection model ? letterOf(model) : "";

		return titles;
	}

	// jump to the first landable section at or after the tapped letter, so a gap letter lands ahead
	// like Contacts; every returned section has a live cell, which UIKit demands
	int ICollectionHost.IndexSection(
		string title)
	{
		if (SectionIndexTitle is not Func<TSection, string> letterOf)
			return 0;

		int last = -1;
		for (int section = 0; section < SectionCount; section++)
		{
			if (!Expanded(section) || CountIn(section) == 0 || SectionAt(section) is not TSection model)
				continue;

			last = section;

			if (string.CompareOrdinal(letterOf(model), title) >= 0)
				return section;
		}

		return last < 0 ? 0 : last;
	}

	void SyncInsets()
	{
		if (!IsRealized)
			return;

		SyncLayoutInsets();

		// while refreshing, UIKit holds the spinner open through the top inset; writing ours over
		// it collapses the spinner mid-spin. It restores our inset when EndRefreshing runs a sync
		if (refresh is { Refreshing: true })
			return;

		Thickness bled = BledInsets;
		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;

		UIEdgeInsets insets = horizontal
			? new(0, (nfloat)bled.Left, keyboardCover, (nfloat)bled.Right)
			: new((nfloat)bled.Top, 0, (nfloat)bled.Bottom + keyboardCover, 0);

		if (Ui.ContentInset == insets)
			return;

		Ui.ContentInset = insets;

		if (!usesSystemContentInsets)
		{
			Ui.VerticalScrollIndicatorInsets = insets;
			Ui.HorizontalScrollIndicatorInsets = insets;
		}
	}

	void SyncLayoutInsets()
	{
		Thickness insets = ContentInsets;
		if (layoutInsetsSynced && layoutInsets == insets)
			return;

		layoutInsets = insets;
		layoutInsetsSynced = true;
		Ui.CollectionViewLayout.InvalidateLayout();
	}

	void SyncEmptyState()
	{
		if (!IsRealized)
			return;

		if (EmptyView is not View empty)
		{
			ClearEmptyHost();
			return;
		}

		empty.TintHost = this;

		if (emptyHost is not EmptyCollectionHost host || !ReferenceEquals(host.Content, empty))
		{
			ClearEmptyHost();

			empty.SetParent(this);
			host = new(empty);
			emptyHost = host;
			Ui.BackgroundView = host;
		}

		host.ContentInsets = ContentInsets;
		host.KeyboardCover = keyboardCover;
		host.Hidden = !IsEmpty;
	}

	void ClearEmptyHost()
	{
		if (emptyHost is not EmptyCollectionHost host)
			return;

		Ui.BackgroundView = null;
		host.Content?.SetParent(null);
		host.Content?.Unrealize();
		host.Dispose();
		emptyHost = null;
	}

	void ApplyKeyboardLayout()
	{
		SyncInsets();

		if (emptyHost is EmptyCollectionHost host)
		{
			host.KeyboardCover = keyboardCover;
			host.LayoutIfNeeded();
		}
	}

	void OnKeyboardChanged(
		Rect keyboard,
		bool hiding,
		double duration)
	{
		if (!AvoidsKeyboard || !IsRealized)
			return;

		UICollectionView host = Ui;
		if (host.Window is null)
			return;

		nfloat covered = 0;

		if (!hiding)
		{
			CGRect frame = host.ConvertRectToView(host.Bounds, null);
			bool intersects = keyboard.Right > frame.GetMinX() && keyboard.Left < frame.GetMaxX();

			if (intersects)
			{
				covered = (nfloat)Math.Max(
					0,
					frame.GetMaxY() - keyboard.Top - host.SafeAreaInsets.Bottom);
			}
		}

		keyboardCover = covered;

		UIView.Animate(duration, ApplyKeyboardLayout);
	}

	ItemTemplateRegistration<TItem> TemplateFor(
		TItem item) =>
		ItemTemplateSelector?.Select(item)
		?? defaultItemTemplate
		?? throw new InvalidOperationException(
			$"CollectionView<{typeof(TItem).Name}> needs an ItemTemplate or ItemTemplateSelector.");

	ICollectionItemView CreateItemView(
		ItemTemplateRegistration<TItem> template)
	{
		ICollectionItemView created = template.Build();
		created.View.TintHost = this;
		itemViews.Add(created);
		return created;
	}

	ItemView<TSection>? CreateSectionView(
		Func<ItemView<TSection>>? template)
	{
		ItemView<TSection>? created = template?.Invoke();
		if (created is null)
			return null;

		created.TintHost = this;
		itemViews.Add(created);
		return created;
	}

	internal ItemView<TSection>? CreateHeaderView() =>
		CreateSectionView(SectionHeaderTemplate);

	internal ItemView<TSection>? CreateFooterView() =>
		CreateSectionView(SectionFooterTemplate);

	internal bool HasSectionHeader => SectionHeaderTemplate is not null;

	internal bool HasSectionFooter => SectionFooterTemplate is not null;

	internal FixedGridGeometry BuildFixedGridGeometry(
		double width)
	{
		Thickness insets = ContentInsets;
		double sectionWidth = Math.Max(1, width - insets.Horizontal - Layout.Spacing * 2);
		double columnWidth = Math.Max(1, (width - insets.Horizontal - Layout.Spacing * (Layout.Columns + 1)) / Layout.Columns);

		return new FixedGridGeometry(
			SectionCount,
			CountIn,
			Layout.Columns,
			Layout.Spacing,
			RowHeight(columnWidth, Layout.ItemAspectRatio),
			MeasureSectionView(ref headerSizingView, SectionHeaderTemplate, sectionWidth),
			MeasureSectionView(ref footerSizingView, SectionFooterTemplate, sectionWidth),
			insets,
			width);
	}

	double MeasureSectionView(
		ref ItemView<TSection>? sizingView,
		Func<ItemView<TSection>>? template,
		double width)
	{
		if (template is null)
			return 0;

		sizingView ??= CreateSectionView(template);

		if (sizingView is null)
			return 0;

		if (SectionAt(0) is TSection section)
			sizingView.SetItem(section);

		sizingView.Measure(new(width, double.PositiveInfinity));
		return sizingView.DesiredSize.Height;
	}

	internal void Select(
		int section,
		int index)
	{
		if (ItemAt(section, index) is not TItem item)
			return;

		SelectFromTap(item);

		if (ItemActivation is ICommand command && command.CanExecute(item))
			command.Execute(item);
	}

	internal bool CanSelect(
		int section,
		int index)
	{
		if (ItemAt(section, index) is not TItem item)
			return false;

		return ItemActivation is not ICommand command || command.CanExecute(item);
	}

	UICollectionViewLayout CreateNativeLayout()
	{
		if (!Layout.IsFixedGeometry)
			return CreateLayout(SectionHeaderTemplate is not null, SectionFooterTemplate is not null);

		if (SectionLayout is not null)
			throw new InvalidOperationException("CollectionLayout.FixedGrid does not support a per-section SectionLayout.");

		if (Header is not null || Footer is not null)
			throw new InvalidOperationException("CollectionLayout.FixedGrid does not support the collection Header or Footer.");

		fixedLayout = new(this);
		return fixedLayout;
	}

	UICollectionViewCompositionalLayout CreateLayout(
		bool headers,
		bool footers)
	{
		bool mixed = SectionLayout is not null;

		// A provider keeps the collection's outer insets explicit and additive. UIKit's inset-grouped
		// list spacing remains intact inside them instead of silently replacing the requested margin.
		UICollectionViewCompositionalLayout result = new((index, environment) =>
		{
			CollectionLayout selected = LayoutForSection((int)index);

			return ApplyCollectionInsets(
				Section(selected, headers, footers, environment, mixed),
				(int)index,
				selected);
		});

		return AddLayoutBoundaries(result);
	}

	CollectionLayout LayoutForSection(
		int index) =>
		SectionLayout is Func<TSection, CollectionLayout> perSection
		&& SectionAt(index) is TSection section
			? perSection(section)
			: Layout;

	UICollectionViewCompositionalLayout AddLayoutBoundaries(
		UICollectionViewCompositionalLayout layout)
	{
		List<NSCollectionLayoutBoundarySupplementaryItem> boundaries = [];

		if (Header is not null)
		{
			NSCollectionLayoutBoundarySupplementaryItem header = Boundary(LayoutHeaderKind, footer: false);
			header.PinToVisibleBounds = PinsHeader;
			header.ZIndex = PinsHeader ? 2 : 0;
			boundaries.Add(header);
		}

		if (Footer is not null)
			boundaries.Add(Boundary(LayoutFooterKind, footer: true));

		UICollectionViewCompositionalLayoutConfiguration configuration = layout.Configuration;
		configuration.ContentInsetsReference = UIContentInsetsReference.None;

		if (boundaries.Count > 0)
			configuration.BoundarySupplementaryItems = [.. boundaries];

		layout.Configuration = configuration;

		return layout;
	}

	NSCollectionLayoutSection Section(
		CollectionLayout layout,
		bool headers,
		bool footers,
		INSCollectionLayoutEnvironment environment,
		bool mixed) =>
		layout.Kind switch
		{
			CollectionLayoutKind.Grid => GridSection(
				layout,
				headers,
				footers,
				(nfloat)Math.Max(1, environment.Container.EffectiveContentSize.Width - ContentInsets.Horizontal)),
			CollectionLayoutKind.Carousel => CarouselSection(
				layout,
				headers,
				footers,
				mixed ? (nfloat)Math.Max(1, RowHeight(layout.ItemWidth)) : null),
			_ => NSCollectionLayoutSection.GetSection(ListConfiguration(layout, headers, footers), environment)
		};

	NSCollectionLayoutSection ApplyCollectionInsets(
		NSCollectionLayoutSection section,
		int index,
		CollectionLayout layout)
	{
		Thickness outer = ContentInsets;

		// Inset-grouped lists already use UIKit's horizontal system margins. Count those as
		// satisfying SystemInsetEdges instead of doubling them; explicit Padding stays additive.
		if (layout is { Kind: CollectionLayoutKind.List, Grouped: true })
			outer = new(Padding.Left, outer.Top, Padding.Right, outer.Bottom);

		NSDirectionalEdgeInsets current = section.ContentInsets;
		bool rightToLeft = Ui.EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft;
		nfloat leading = (nfloat)(rightToLeft ? outer.Right : outer.Left);
		nfloat trailing = (nfloat)(rightToLeft ? outer.Left : outer.Right);

		section.ContentInsetsReference = UIContentInsetsReference.None;
		NSDirectionalEdgeInsets applied = new(
			current.Top + (Header is null && index == 0 ? (nfloat)outer.Top : 0),
			current.Leading + leading,
			current.Bottom + (Footer is null && index == SectionCount - 1 ? (nfloat)outer.Bottom : 0),
			current.Trailing + trailing);
		section.ContentInsets = applied;

		return section;
	}

	internal void SyncInsetGroupedBoundaryInsets()
	{
		Thickness? header = ResolvedInsetGroupedInsets(0, insetGroupedHeaderInsets);
		Thickness? footer = ResolvedInsetGroupedInsets(SectionCount - 1, insetGroupedFooterInsets);

		if (insetGroupedHeaderInsets == header && insetGroupedFooterInsets == footer)
			return;

		insetGroupedHeaderInsets = header;
		insetGroupedFooterInsets = footer;
		Ui.CollectionViewLayout.InvalidateLayout();
	}

	Thickness? ResolvedInsetGroupedInsets(
		int section,
		Thickness? previous)
	{
		if (section < 0 || LayoutForSection(section) is not { Kind: CollectionLayoutKind.List, Grouped: true })
			return null;

		if (CountIn(section) == 0
			|| Ui.GetLayoutAttributesForItem(NSIndexPath.FromRowSection(0, section)) is not UICollectionViewLayoutAttributes attributes)
			return previous;

		CGRect frame = attributes.Frame;
		return new(
			Math.Max(0, frame.GetMinX()),
			0,
			Math.Max(0, Ui.Bounds.Width - frame.GetMaxX()),
			0);
	}

	UICollectionLayoutListConfiguration ListConfiguration(
		CollectionLayout layout,
		bool headers,
		bool footers)
	{
		UICollectionLayoutListAppearance appearance = layout.Grouped
			? UICollectionLayoutListAppearance.InsetGrouped
			: UICollectionLayoutListAppearance.Plain;

		// a list configuration paints its own opaque background, which would cover the empty view
		UICollectionLayoutListConfiguration configuration = new(appearance)
		{
			BackgroundColor = UIColor.Clear,
			ShowsSeparators = ShowsSeparators,
			HeaderMode = headers
				? UICollectionLayoutListHeaderMode.Supplementary
				: UICollectionLayoutListHeaderMode.None,
			FooterMode = footers
				? UICollectionLayoutListFooterMode.Supplementary
				: UICollectionLayoutListFooterMode.None
		};

		// a separator configuration overrides ShowsSeparators, so it has to carry the visibility too
		if (SeparatorInsets is Thickness separator)
		{
			NSDirectionalEdgeInsets insets = new(0, (nfloat)separator.Left, 0, (nfloat)separator.Right);

			UIListSeparatorVisibility visibility = ShowsSeparators
				? UIListSeparatorVisibility.Automatic
				: UIListSeparatorVisibility.Hidden;

			configuration.SeparatorConfiguration = new(appearance)
			{
				TopSeparatorInsets = insets,
				BottomSeparatorInsets = insets,
				TopSeparatorVisibility = visibility,
				BottomSeparatorVisibility = visibility
			};
		}

		// native swipe actions: UIKit owns the gesture, the animation and the full-swipe
		if (SwipeActions.Count > 0)
		{
			configuration.TrailingSwipeActionsConfigurationProvider =
				path => SwipeConfiguration(path, SwipeSide.Trailing)!;

			configuration.LeadingSwipeActionsConfigurationProvider =
				path => SwipeConfiguration(path, SwipeSide.Leading)!;
		}

		return configuration;
	}

	static void AddBoundaries(
		NSCollectionLayoutSection section,
		bool headers,
		bool footers)
	{
		List<NSCollectionLayoutBoundarySupplementaryItem> boundaries = [];

		if (headers)
			boundaries.Add(Boundary(footer: false));

		if (footers)
			boundaries.Add(Boundary(footer: true));

		if (boundaries.Count > 0)
			section.BoundarySupplementaryItems = [.. boundaries];
	}

	static NSCollectionLayoutBoundarySupplementaryItem Boundary(
		bool footer) =>
		Boundary(
			(footer ? UICollectionElementKindSectionKey.Footer : UICollectionElementKindSectionKey.Header).ToString(),
			footer);

	static NSCollectionLayoutBoundarySupplementaryItem Boundary(
		string kind,
		bool footer) =>
		NSCollectionLayoutBoundarySupplementaryItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateEstimated(44)),
			kind,
			footer ? NSRectAlignment.Bottom : NSRectAlignment.Top);

	bool sizedWithItem;

	double RowHeight(
		double width,
		double? aspectRatio = null)
	{
		TItem? item = ItemAt(0, 0);
		ItemTemplateRegistration<TItem>? itemTemplate = item is not null
			? TemplateFor(item)
			: defaultItemTemplate;

		// A selector cannot choose a template before the first item exists. The first
		// real snapshot invalidates the layout and replaces this temporary estimate.
		if (itemTemplate is null)
			return aspectRatio is double ratio ? width / ratio : 44;

		if (!sizingViews.TryGetValue(itemTemplate, out ICollectionItemView? sizingView))
		{
			sizingView = CreateItemView(itemTemplate);
			sizingViews.Add(itemTemplate, sizingView);
		}

		if (item is not null)
		{
			sizingView.SetItem(item);
			sizedWithItem = true;
		}

		if (aspectRatio is double resolvedRatio)
			return AspectRowHeight(sizingView.View, width, resolvedRatio);

		sizingView.View.Measure(new(width, double.PositiveInfinity));
		return sizingView.View.DesiredSize.Height;
	}

	static double AspectRowHeight(
		View view,
		double width,
		double aspectRatio)
	{
		Thickness margin = view.Margin;
		double availableWidth = Math.Max(0, width - margin.Horizontal);
		double requested = double.IsNaN(view.Height)
			? availableWidth / aspectRatio
			: view.Height;
		double minimum = double.IsNaN(view.MinHeight) ? 0 : view.MinHeight;
		double height = Math.Max(Math.Min(requested, view.MaxHeight), minimum);

		return height + margin.Vertical;
	}

	NSCollectionLayoutSection GridSection(
		CollectionLayout layout,
		bool headers,
		bool footers,
		nfloat width)
	{
		nfloat spacing = (nfloat)layout.Spacing;
		double column = Math.Max(1, (width - spacing * (layout.Columns + 1)) / layout.Columns);
		nfloat height = (nfloat)Math.Max(1, RowHeight(column, layout.ItemAspectRatio));

		NSCollectionLayoutItem item = NSCollectionLayoutItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f / layout.Columns),
				NSCollectionLayoutDimension.CreateAbsolute(height)));

		NSCollectionLayoutGroup group = NSCollectionLayoutGroup.CreateHorizontal(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateAbsolute(height)),
			item,
			layout.Columns);

		group.InterItemSpacing = NSCollectionLayoutSpacing.CreateFixed(spacing);

		NSCollectionLayoutSection section = NSCollectionLayoutSection.Create(group);
		section.InterGroupSpacing = spacing;
		section.ContentInsets = new(spacing, spacing, spacing, spacing);

		AddBoundaries(section, headers, footers);

		return section;
	}

	// height is absolute when the carousel is one section of a mixed layout, or fills the collection when
	// it is the whole thing
	static NSCollectionLayoutSection CarouselSection(
		CollectionLayout layout,
		bool headers,
		bool footers,
		nfloat? height = null)
	{
		nfloat spacing = (nfloat)layout.Spacing;

		NSCollectionLayoutDimension groupHeight = height is nfloat absolute
			? NSCollectionLayoutDimension.CreateAbsolute(absolute)
			: NSCollectionLayoutDimension.CreateFractionalHeight(1f);

		NSCollectionLayoutItem item = NSCollectionLayoutItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateFractionalHeight(1f)));

		NSCollectionLayoutGroup group = NSCollectionLayoutGroup.CreateHorizontal(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateAbsolute((nfloat)layout.ItemWidth),
				groupHeight),
			item,
			1);

		NSCollectionLayoutSection section = NSCollectionLayoutSection.Create(group);
		section.InterGroupSpacing = spacing;
		section.OrthogonalScrollingBehavior = layout.Snap switch
		{
			CarouselSnap.LeadingBoundary => UICollectionLayoutSectionOrthogonalScrollingBehavior.ContinuousGroupLeadingBoundary,
			CarouselSnap.LeadingBoundaryPeek => UICollectionLayoutSectionOrthogonalScrollingBehavior.ContinuousGroupLeadingBoundary,
			CarouselSnap.Item => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPaging,
			CarouselSnap.ItemPeek => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPaging,
			CarouselSnap.ItemCentered => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPagingCentered,
			CarouselSnap.Page => UICollectionLayoutSectionOrthogonalScrollingBehavior.Paging,
			_ => UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous
		};
		nfloat leadingInset = layout.Snap is CarouselSnap.LeadingBoundaryPeek or CarouselSnap.ItemPeek
			? spacing * 2
			: spacing;

		section.ContentInsets = new(0, leadingInset, 0, spacing);

		AddBoundaries(section, headers, footers);

		return section;
	}
}

internal sealed class CollectionDelegate<TItem, TSection>(
	CollectionView<TItem, TSection> element) : UICollectionViewDelegate, IUICollectionViewDataSourcePrefetching
	where TItem : class
	where TSection : class, ISection<TItem>
{
	bool CanInteract(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		if (collectionView.CellForItem(indexPath) is SkeleCell { Hosted: { } hosted }
			&& !hosted.IsInteractionEnabled)
			return false;

		return element.EditingNow || element.CanSelect(indexPath.Section, indexPath.Row);
	}

	public override bool ShouldHighlightItem(
		UICollectionView collectionView,
		NSIndexPath indexPath) =>
		CanInteract(collectionView, indexPath);

	public override bool ShouldSelectItem(
		UICollectionView collectionView,
		NSIndexPath indexPath) =>
		CanInteract(collectionView, indexPath);

	// a transient highlight is released right away; selection that applies outside editing takes over
	public override void ItemSelected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		if (element.EditingNow)
		{
			element.EditSelect(indexPath.Section, indexPath.Row, true);
			return;
		}

		if (!element.RetainsHighlight && !element.SelectsOutsideEditing)
			collectionView.DeselectItem(indexPath, true);

		element.Select(indexPath.Section, indexPath.Row);
	}

	public override void ItemDeselected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		if (element.EditingNow)
		{
			element.EditSelect(indexPath.Section, indexPath.Row, false);
			return;
		}

		element.DeselectFromTap(indexPath.Section, indexPath.Row);
	}

	public override void WillDisplayCell(
		UICollectionView collectionView,
		UICollectionViewCell cell,
		NSIndexPath indexPath) =>
		element.OnWillDisplay(indexPath.Section, indexPath.Row);

	public override void Scrolled(
		UIScrollView scrollView)
	{
		double offset = scrollView.ContentOffset.Y + scrollView.AdjustedContentInset.Top;
		element.OnScrolled(offset);

		if (scrollView is CollectionHost host)
			host.NotifyScrollOffsetChanged(offset);
	}

	public override void DraggingEnded(
		UIScrollView scrollView,
		bool willDecelerate) =>
		element.OnDragEnded();

	public override UIContextMenuConfiguration? GetContextMenuConfiguration(
		UICollectionView collectionView,
		NSIndexPath indexPath,
		CGPoint point) =>
		element.MenuConfiguration(indexPath);

	public override UITargetedPreview? GetPreviewForHighlightingContextMenu(
		UICollectionView collectionView,
		UIContextMenuConfiguration configuration) =>
		element.ShapedPreview(configuration);

	public override UITargetedPreview? GetPreviewForDismissingContextMenu(
		UICollectionView collectionView,
		UIContextMenuConfiguration configuration) =>
		element.ShapedPreview(configuration);

	// the commit waits out the dismissal: anything presented mid-teardown is torn down with it
	public override void WillPerformPreviewAction(
		UICollectionView collectionView,
		UIContextMenuConfiguration configuration,
		IUIContextMenuInteractionCommitAnimating animator) =>
		animator.AddCompletion(element.CommitPreview);

	public override void WillEndContextMenuInteraction(
		UICollectionView collectionView,
		UIContextMenuConfiguration configuration,
		IUIContextMenuInteractionAnimating? animator) =>
		element.EndPreview();


	readonly Dictionary<NSIndexPath, CancellationTokenSource> prefetches = [];

	public void PrefetchItems(
		UICollectionView collectionView,
		NSIndexPath[] indexPaths)
	{
		foreach (NSIndexPath path in indexPaths)
		{
			if (element.PrefetchUrl(path.Section, path.Row) is not string url || prefetches.ContainsKey(path))
				continue;

			CancellationTokenSource cancellation = new();
			prefetches[path] = cancellation;

			_ = WarmAsync(url, path, cancellation);
		}
	}

	public void CancelPrefetching(
		UICollectionView collectionView,
		NSIndexPath[] indexPaths)
	{
		foreach (NSIndexPath path in indexPaths)
		{
			if (prefetches.Remove(path, out CancellationTokenSource? cancellation))
			{
				cancellation.Cancel();
				cancellation.Dispose();
			}
		}
	}

	// warming the loader's cache is the whole job; a failed prefetch is invisible by design
	async Task WarmAsync(
		string url,
		NSIndexPath path,
		CancellationTokenSource cancellation)
	{
		try
		{
			await Image.Loader.LoadAsync(url, cancellation.Token);
		}
		catch
		{
			// ignored :3
		}
		finally
		{
			prefetches.Remove(path);
			cancellation.Dispose();
		}
	}
}

internal sealed class CollectionHost : UICollectionView, INavigationAccessoryScrollSource
{
	readonly ICollectionHost? element;

	public event Action<double>? ScrollOffsetChanged;

	public CollectionHost(
		ICollectionHost element,
		UICollectionViewLayout layout) : base(CGRect.Empty, layout)
	{
		this.element = element;

		NSNotificationCenter.DefaultCenter.AddObserver(this, new("keyboardFrameChanged:"), UIKeyboard.WillChangeFrameNotification, null);
		NSNotificationCenter.DefaultCenter.AddObserver(this, new("keyboardHidden:"), UIKeyboard.WillHideNotification, null);
	}

	// ReSharper disable once UnusedMember.Local
	public CollectionHost(
		NativeHandle handle) : base(handle)
	{ }


	internal void NotifyScrollOffsetChanged(
		double offset) =>
		ScrollOffsetChanged?.Invoke(offset);


	// ReSharper disable once UnusedMember.Local
	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification)
	{
		CGRect frame = UIKeyboard.FrameEndFromNotification(notification);

		element?.KeyboardChanged(
			new(frame.X, frame.Y, frame.Width, frame.Height),
			hiding: false,
			UIKeyboard.AnimationDurationFromNotification(notification));
	}

	// ReSharper disable once UnusedMember.Local
	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		element?.KeyboardChanged(
			Rect.Zero,
			hiding: true,
			UIKeyboard.AnimationDurationFromNotification(notification));


	public override void LayoutSubviews()
	{
		element?.SyncInsets();
		element?.BeginFixedLayoutBoundsChange(Bounds.Width);

		base.LayoutSubviews();

		element?.EndFixedLayoutBoundsChange();
		element?.ApplyInitialScroll();
		element?.SyncObservedSections();
		element?.SyncInsetGroupedBoundaryInsets();
		element?.SyncEmptyState();
	}

	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
	}
}

internal sealed class EmptyCollectionHost : UIView
{
	nfloat keyboardCover;
	Thickness contentInsets;


	public EmptyCollectionHost(
		View content)
	{
		Content = content;
		AddSubview(content.Realize());
	}

	// ReSharper disable once UnusedMember.Local
	public EmptyCollectionHost(
		NativeHandle handle) : base(handle)
	{ }


	internal View? Content { get; }

	internal Thickness ContentInsets
	{
		get => contentInsets;
		set
		{
			if (contentInsets == value)
				return;

			contentInsets = value;
			SetNeedsLayout();
		}
	}

	internal nfloat KeyboardCover
	{
		get => keyboardCover;
		set
		{
			if (keyboardCover == value)
				return;

			keyboardCover = value;
			SetNeedsLayout();
		}
	}


	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		if (Content is not View)
			return;

		Size available = new(
			Math.Max(0, Bounds.Width - contentInsets.Horizontal),
			Math.Max(0, Bounds.Height - contentInsets.Vertical - keyboardCover));

		Content.Measure(available);
		Content.Arrange(new(contentInsets.Left, contentInsets.Top, available.Width, available.Height));
	}
}

internal sealed class PreviewHost : UIViewController
{
	readonly View? content;
	readonly nfloat width;

	public PreviewHost(
		View content,
		nfloat width)
	{
		this.content = content;
		this.width = width;
	}

	// ReSharper disable once UnusedMember.Local
	public PreviewHost(
		NativeHandle handle) : base(handle)
	{ }


	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		if (content is null)
			return;

		View!.BackgroundColor = UIColor.SystemBackground;
		View.AddSubview(content.Realize());

		// an explicit Width on the preview root sizes the peek; default is the list's width
		nfloat effective = double.IsFinite(content.Width) ? (nfloat)content.Width : width;

		content.Measure(new(effective, double.PositiveInfinity));
		PreferredContentSize = new(effective, (nfloat)content.DesiredSize.Height);
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		content?.Arrange(new(0, 0, View!.Bounds.Width, View.Bounds.Height));
	}
}

internal sealed class CollectionSource : UICollectionViewDiffableDataSource<NSNumber, NSNumber>
{
	readonly ICollectionHost? element;

	public CollectionSource(
		ICollectionHost element,
		UICollectionView collectionView,
		UICollectionViewDiffableDataSourceCellProvider cellProvider) : base(collectionView, cellProvider)
	{
		this.element = element;
	}

	// ReSharper disable once UnusedMember.Local
	public CollectionSource(
		NativeHandle handle) : base(handle)
	{ }


	public override bool CanMoveItem(
		UICollectionView collectionView,
		NSIndexPath indexPath) =>
		element?.CanMove(indexPath.Section, indexPath.Row) == true;

	// the binding's ReorderingHandlers is an empty stub, so the element applies the move itself
	public override void MoveItem(
		UICollectionView collectionView,
		NSIndexPath sourceIndexPath,
		NSIndexPath destinationIndexPath) =>
		element?.Move(sourceIndexPath.Section, sourceIndexPath.Row, destinationIndexPath.Section, destinationIndexPath.Row);

	// the index bar validates every title against a live cell during reloadData, which runs before the
	// first async snapshot lands: advertise titles only once the collection actually has rows
	public override string[]? GetIndexTitles(
		UICollectionView collectionView)
	{
		nint sections = collectionView.NumberOfSections();

		for (nint section = 0; section < sections; section++)
		{
			if (collectionView.NumberOfItemsInSection(section) > 0)
				return element?.IndexTitles();
		}

		return null;
	}

	public override NSIndexPath GetIndexPath(
		UICollectionView collectionView,
		string title,
		nint atIndex) =>
		NSIndexPath.FromRowSection(0, element?.IndexSection(title) ?? 0);
}

internal sealed class IndexedCollectionSource<TItem, TSection>(
	CollectionView<TItem, TSection> element,
	Func<UICollectionView, NSIndexPath, SkeleCell> cell,
	Func<UICollectionView, string, NSIndexPath, SkeleHeader> supplementary) : UICollectionViewDataSource
	where TItem : class
	where TSection : class, ISection<TItem>
{
	readonly ICollectionHost host = element;

	public override nint NumberOfSections(UICollectionView collectionView) =>
		element.SectionCount;

	public override nint GetItemsCount(
		UICollectionView collectionView,
		nint section) =>
		element.Expanded((int)section) ? element.CountIn((int)section) : 0;

	public override UICollectionViewCell GetCell(
		UICollectionView collectionView,
		NSIndexPath indexPath) =>
		cell(collectionView, indexPath);

	public override UICollectionReusableView GetViewForSupplementaryElement(
		UICollectionView collectionView,
		NSString elementKind,
		NSIndexPath indexPath) =>
		supplementary(collectionView, elementKind.ToString(), indexPath);

	public override bool CanMoveItem(
		UICollectionView collectionView,
		NSIndexPath indexPath) =>
		element.CanMove(indexPath.Section, indexPath.Row);

	public override void MoveItem(
		UICollectionView collectionView,
		NSIndexPath sourceIndexPath,
		NSIndexPath destinationIndexPath) =>
		element.Move(
			sourceIndexPath.Section,
			sourceIndexPath.Row,
			destinationIndexPath.Section,
			destinationIndexPath.Row);

	public override string[]? GetIndexTitles(
		UICollectionView collectionView) =>
		host.IndexTitles();

	public override NSIndexPath GetIndexPath(
		UICollectionView collectionView,
		string title,
		nint atIndex) =>
		NSIndexPath.FromRowSection(0, host.IndexSection(title));
}

internal sealed class SkeleCell(
	NativeHandle handle) : UICollectionViewListCell(handle)
{
	public View? Hosted { get; private set; }

	// non-zero while a content-change animation is running, so reuse can cancel it
	internal int AnimationToken { get; set; }

	ICollectionItemView? source;
	bool selects;
	bool reorders;
	bool selectionCheckmark;
	bool editing;
	bool selected;

	UICellAccessoryCheckmark? selectionMark;

	Brush? highlight;
	bool retainsHighlight = true;
	double automaticMinimumHeight;

	public void Attach(
		ICollectionItemView item,
		bool selects,
		bool reorders,
		bool selectionCheckmark)
	{
		source = item;
		Hosted = item.View;
		highlight = item.HighlightBackground;
		this.selects = selects;
		this.reorders = reorders;
		this.selectionCheckmark = selectionCheckmark;
		selected = Selected;

		// one write; repaints during a peek desync the portal
		BackgroundConfiguration = UIBackgroundConfiguration.ClearConfiguration;

		ContentView.AddSubview(Hosted.Realize());

		item.ObserveHighlightBackground(SetHighlightBackground);
		item.ObserveAccessories(ApplyAccessories);

		ApplyAccessories();
	}

	// system affordances, with the edit-mode circle and drag handle around them
	void ApplyAccessories()
	{
		if (source is null)
			return;

		List<UICellAccessory> accessories = [];
		selectionMark = null;

		if (selects)
			accessories.Add(new UICellAccessoryMultiselect());

		// the collection's own selection indicator, innermost of the trailing group
		if (selectionCheckmark)
		{
			selectionMark = new UICellAccessoryCheckmark
			{
				DisplayedState = UICellAccessoryDisplayedState.WhenNotEditing,
				IsHidden = !selected,
				TintColor = Hosted?.EffectiveTint?.ToUIColor()
			};

			accessories.Add(selectionMark);
		}

		foreach (ItemAccessory accessory in source.Accessories)
		{
			// hidden accessories still reserve their slot in the list layout, so they are left out
			if (!accessory.ResolvedIsVisible || !ShowsNow(accessory))
				continue;

			accessories.Add(NativeAccessory(accessory));
		}

		if (reorders)
			accessories.Add(new UICellAccessoryReorder());

		Accessories = [.. accessories];
		SetNeedsLayout();
	}

	internal void ApplySelectionTint()
	{
		if (selectionMark is not null)
			selectionMark.TintColor = Hosted?.EffectiveTint?.ToUIColor();
	}

	bool ShowsNow(
		ItemAccessory accessory) =>
		accessory.Display switch
		{
			AccessoryDisplay.WhenEditing => editing,
			AccessoryDisplay.WhenNotEditing => !editing,
			_ => true
		};

	UICellAccessory NativeAccessory(
		ItemAccessory accessory)
	{
		UICellAccessory native = accessory switch
		{
			CheckmarkAccessory => new UICellAccessoryCheckmark(),
			DisclosureAccessory => new UICellAccessoryDisclosureIndicator(),
			DetailAccessory detail => new UICellAccessoryDetail
			{
				ActionHandler = () => RunDetailCommand(detail)
			},
			LabelAccessory label => new UICellAccessoryLabel(label.ResolvedText ?? "")
			{
				Font = Fonts.Preferred(label.TextStyle, label.FontWeight, FontDesign.Default)
			},
			_ => throw new InvalidOperationException($"Unknown item accessory '{accessory.GetType().Name}'.")
		};

		if (accessory.ResolvedTint is Color tint)
			native.TintColor = tint.ToUIColor();

		return native;
	}

	void RunDetailCommand(
		DetailAccessory accessory)
	{
		object? item = source?.CurrentItem;

		if (accessory.ResolvedCommand is ICommand command && command.CanExecute(item))
			command.Execute(item);
	}

	public void SetAutomaticMinimumHeight(
		double value) =>
		automaticMinimumHeight = value;

	public void SetHighlightBackground(
		Brush? value)
	{
		highlight = value;

		if (lit)
			Hosted?.SetBackgroundOverride(value);

		SetNeedsUpdateConfiguration();
	}

	public void SetRetainsHighlight(
		bool value)
	{
		if (retainsHighlight == value)
			return;

		retainsHighlight = value;
		SetNeedsUpdateConfiguration();
	}

	bool lit;

	public override void UpdateConfiguration(
		UICellConfigurationState state)
	{
		// edit mode and selection both reshape the accessory set
		if (editing != state.Editing || (selectionCheckmark && selected != state.Selected))
		{
			editing = state.Editing;
			selected = state.Selected;
			ApplyAccessories();
		}

		if (highlight is null)
			return;

		bool wantsLit = state.Highlighted || state.Selected && (retainsHighlight || state.Editing);
		if (wantsLit == lit && Hosted is not null)
			return;

		// the pressed look lands at once; releasing fades it outside the selection update, where
		// UIKit disables animations
		bool fades = lit && !wantsLit;
		lit = wantsLit;

		if (Hosted is not View hosted)
			return;

		if (!fades)
		{
			hosted.SetBackgroundOverride(wantsLit ? highlight : null);
			return;
		}

		DispatchQueue.MainQueue.DispatchAsync(() =>
		{
			if (!lit)
				UIView.Animate(0.25, () => hosted.SetBackgroundOverride(null));
		});
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		if (Hosted is null)
			return;

		// UIKit insets the content view to clear the accessories; the hosted view paints the whole
		// row while its content steps back into the cleared gutter, like a system list row
		CGRect content = ContentView.Frame;
		nfloat leading = content.X;
		nfloat trailing = (nfloat)Math.Max(0, (double)Bounds.Width - (double)content.GetMaxX());

		source?.SetContentInsets(leading, trailing);
		Hosted.Arrange(new(-leading, 0, Bounds.Width, ContentView.Bounds.Height));
	}

	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		if (Hosted is null)
			return layoutAttributes;

		Hosted.Measure(new(layoutAttributes.Frame.Width, double.PositiveInfinity));

		double height = Hosted.DesiredSize.Height;
		if (double.IsNaN(Hosted.Height) && double.IsNaN(Hosted.MinHeight))
		{
			height = Math.Max(height, automaticMinimumHeight);

			if (double.IsFinite(Hosted.MaxHeight))
				height = Math.Min(height, Hosted.MaxHeight + Hosted.Margin.Vertical);
		}

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)height;
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}
}

internal sealed class SkeleHeader(
	NativeHandle handle) : UICollectionReusableView(handle)
{
	const int ChevronEdge = 16;
	const int ChevronGap = 8;

	static readonly UIImageSymbolConfiguration ChevronConfiguration = UIImageSymbolConfiguration.Create(13, UIImageSymbolWeight.Semibold);


	UIImageView? chevron;
	UITapGestureRecognizer? tap;
	Action? toggle;
	bool expanded;
	Func<Thickness>? contentInsets;


	public View? Hosted { get; private set; }


	public void Attach(
		View view)
	{
		Hosted = view;

		AddSubview(view.Realize());
	}

	public void SetContentInsets(
		Func<Thickness>? insets)
	{
		contentInsets = insets;
		SetNeedsLayout();
	}

	public void SetExpandable(
		bool expandable,
		bool isExpanded,
		Action onToggle)
	{
		toggle = onToggle;

		if (!expandable)
		{
			chevron?.Hidden = true;
			tap?.Enabled = false;

			return;
		}

		if (chevron is null)
		{
			chevron = new(UIImage.GetSystemImage("chevron.right", ChevronConfiguration))
			{
				TintColor = UIColor.TertiaryLabel
			};
			chevron.SizeToFit();
			AddSubview(chevron);
		}

		if (tap is null)
		{
			tap = new(OnHeaderTapped);
			AddGestureRecognizer(tap);
		}

		chevron.Hidden = false;
		tap.Enabled = true;

		SetExpanded(isExpanded, animated: false);
		SetNeedsLayout();
	}

	public void SetExpanded(
		bool isExpanded,
		bool animated)
	{
		expanded = isExpanded;

		if (chevron is null)
			return;

		CGAffineTransform transform = isExpanded
			? CGAffineTransform.MakeRotation((nfloat)(Math.PI / 2))
			: CGAffineTransform.MakeIdentity();

		if (animated)
			Animate(0.25, () => chevron.Transform = transform);
		else
			chevron.Transform = transform;
	}

	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		if (Hosted is null)
			return layoutAttributes;

		Thickness insets = contentInsets?.Invoke() ?? Thickness.Zero;
		double width = Math.Max(0, layoutAttributes.Frame.Width - insets.Horizontal);
		Hosted.Measure(new(width, double.PositiveInfinity));

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)(Hosted.DesiredSize.Height + insets.Vertical);
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		Thickness insets = contentInsets?.Invoke() ?? Thickness.Zero;
		nfloat rightInset = 0;

		if (chevron is { Hidden: false })
		{
			CGSize size = chevron.Bounds.Size;
			chevron.Center = new(
				Bounds.Width - (nfloat)insets.Right - ChevronEdge - size.Width / 2,
				Bounds.Height / 2);
			rightInset = size.Width + ChevronEdge + ChevronGap;
		}

		Hosted?.Arrange(new(
			insets.Left,
			insets.Top,
			Math.Max(0, Bounds.Width - insets.Horizontal - rightInset),
			Math.Max(0, Bounds.Height - insets.Vertical)));
	}

	void OnHeaderTapped()
	{
		SetExpanded(!expanded, animated: true);
		toggle?.Invoke();
	}
}
