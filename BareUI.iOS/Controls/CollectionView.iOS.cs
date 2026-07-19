using System.Windows.Input;
using CoreFoundation;
using ObjCRuntime;

namespace BareUI;

public partial class CollectionView<TItem, TSection>
{
	internal const string CellId = "BareCell";
	internal const string HeaderId = "BareHeader";
	internal const string FooterId = "BareFooter";

	readonly Dictionary<object, ItemKey> keys = new(ReferenceEqualityComparer.Instance);
	readonly List<NSNumber> sectionKeys = [];

	CollectionSource? data;
	CollectionDelegate<TItem, TSection>? selection;

	bool snapshotQueued;

	private protected override UIView CreateNative()
	{
		bool carousel = Layout.Kind is CollectionLayoutKind.Carousel;

		CollectionHost collection = new(this, CreateLayout(Layout, HeaderTemplate is not null, FooterTemplate is not null))
		{
			BackgroundColor = UIColor.Clear,

			// only bounce along the axis that actually scrolls: a carousel must not drag vertically,
			// and a list must not drag sideways
			AlwaysBounceVertical = !carousel,
			AlwaysBounceHorizontal = carousel,

			// we own the insets; UIKit's guessing is what made a vertical list drift sideways
			ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never
		};

		collection.RegisterClassForCell(typeof(BareCell), CellId);
		collection.RegisterClassForSupplementaryView(
			typeof(BareHeader),
			UICollectionElementKindSection.Header,
			HeaderId);
		collection.RegisterClassForSupplementaryView(
			typeof(BareHeader),
			UICollectionElementKindSection.Footer,
			FooterId);

		data = new(this, collection, CellFor)
		{
			SupplementaryViewProvider = SupplementaryFor
		};

		selection = new(this);
		collection.Delegate = selection;

		if (Prefetch is not null)
			collection.PrefetchDataSource = selection;

		ApplyRefresh(collection);
		ApplyReorder(collection);

		return collection;
	}

	UIRefreshControl? refresh;

	void ApplyRefresh(
		UICollectionView collection)
	{
		if (RefreshCommand is null)
			return;

		refresh = new();
		refresh.ValueChanged += (_, _) => OnRefreshTriggered();

		collection.RefreshControl = refresh;
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
		FlushSnapshot();
	}

	// land the refresh's own changes first, then collapse the spinner — running both in
	// one turn animates the diff against a moving content inset
	void EndNativeRefresh() =>
		FlushSnapshot(() =>
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
		QueueSnapshot();
		FlushSnapshot();
	}

	partial void ApplyEditingCore()
	{
		if (!IsRealized)
			return;

		// the mode must be known before edit mode begins, or the circles never show
		Ui.AllowsMultipleSelectionDuringEditing = MultiSelects;
		Ui.Editing = isEditing;

		if (isEditing)
			ApplySelectionCore();
		else
			ClearSelection();
	}

	partial void ApplySelectionCore()
	{
		if (!IsRealized || data is null || suppressSelectionSync || !isEditing)
			return;

		HashSet<NSIndexPath> wanted = [];

		foreach (TItem item in selectedItems ?? [])
			if (keys.TryGetValue(item, out ItemKey? key) && data.GetIndexPath(key) is NSIndexPath path)
				wanted.Add(path);

		foreach (NSIndexPath path in Ui.GetIndexPathsForSelectedItems() ?? [])
			if (!wanted.Remove(path))
				Ui.DeselectItem(path, false);

		foreach (NSIndexPath path in wanted)
			Ui.SelectItem(path, false, UICollectionViewScrollPosition.None);
	}

	void ClearSelection()
	{
		if (!IsRealized)
			return;

		foreach (NSIndexPath path in Ui.GetIndexPathsForSelectedItems() ?? [])
			Ui.DeselectItem(path, false);

		if (selectedItems is IList<TItem> { Count: > 0 } list)
		{
			suppressSelectionSync = true;

			try
			{
				list.Clear();
			}
			finally
			{
				suppressSelectionSync = false;
			}
		}
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
		if (!IsRealized || data is null || !keys.TryGetValue(item, out ItemKey? key))
			return;

		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;

		UICollectionViewScrollPosition native = position switch
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

		if (data.GetIndexPath(key) is NSIndexPath path)
			Ui.ScrollToItem(path, native, animated);
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
					FlushSnapshot();
					done(true);
				});

			if (action.Icon is string icon)
				native.Image = UIImage.GetSystemImage(icon);

			if (action.Background is Color background)
				native.BackgroundColor = background.ToUIColor();

			actions.Add(native);
		}

		return actions.Count == 0
			? null
			: UISwipeActionsConfiguration.FromActions([.. actions]);
	}

	// roots the preview controller while UIKit presents it, or its managed peer dies mid-peek
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

					entries[index] = UIAction.Create(
						entry.Text,
						entry.Icon is string icon ? UIImage.GetSystemImage(icon) : null,
						null,
						_ =>
						{
							if (entry.Command is ICommand command && command.CanExecute(item))
								command.Execute(item);
						});

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

	// deliberately Velura-verbatim: no caching, no disposal, VisiblePath only
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

	private protected override void ApplyProperties()
	{
		HookSources();
		ReloadItems();
		ApplyEditingCore();
	}

	private protected override void OnUnrealized() =>
		UnhookSources();

	UICollectionView Ui =>
		(UICollectionView)Native;

	internal override void ReapplyVisuals()
	{
		base.ReapplyVisuals();

		if (!IsRealized)
			return;

		foreach (UICollectionViewCell cell in Ui.VisibleCells)
			if (cell is BareCell { Hosted: { } hosted })
				hosted.ReapplyVisuals();

		EmptyView?.ReapplyVisuals();
	}

	internal override void TintChanged()
	{
		if (!IsRealized)
			return;

		foreach (UICollectionViewCell cell in Ui.VisibleCells)
			if (cell is BareCell { Hosted: { LocalTint: null } hosted })
				hosted.TintChanged();

		if (EmptyView is { LocalTint: null } empty)
			empty.TintChanged();
	}

	// the row tapped on the way out un-highlights on the way back; edit-mode checkmarks stay
	internal override void PageAppeared()
	{
		if (!IsRealized || isEditing)
			return;

		foreach (NSIndexPath path in Ui.GetIndexPathsForSelectedItems() ?? [])
			Ui.DeselectItem(path, true);
	}

	partial void ReloadItems() =>
		QueueSnapshot();

	partial void ApplyChange() =>
		QueueSnapshot();

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

	void ApplySnapshot(
		Action? completed = null)
	{
		if (data is null)
		{
			completed?.Invoke();
			return;
		}

		NSDiffableDataSourceSnapshot<NSNumber, ItemKey> snapshot = new();

		int sections = SectionCount;
		while (sectionKeys.Count < sections)
			sectionKeys.Add(NSNumber.FromInt32(sectionKeys.Count));

		for (int section = 0; section < sections; section++)
		{
			NSNumber sectionKey = sectionKeys[section];
			snapshot.AppendSections([sectionKey]);

			int count = CountIn(section);
			if (count == 0)
				continue;

			ItemKey[] items = new ItemKey[count];
			for (int index = 0; index < count; index++)
				items[index] = KeyFor(ItemAt(section, index)!);

			snapshot.AppendItems(items, sectionKey);
		}

		Prune();

		// animating an off-screen collection is wasted work
		bool animated = Ui.Window is not null;

		if (completed is null)
			data.ApplySnapshot(snapshot, animated);
		else
			data.ApplySnapshot(snapshot, animated, completed);

		// the first real item corrects the row height the empty template guessed
		if (Layout.Kind is CollectionLayoutKind.Grid && !sizedWithItem && ItemAt(0, 0) is not null)
			Ui.CollectionViewLayout.InvalidateLayout();

		// a diff can shuffle index paths under the checkmarks
		ApplySelectionCore();
	}

	ItemKey KeyFor(
		object item)
	{
		if (!keys.TryGetValue(item, out ItemKey? key))
			keys[item] = key = new(item);

		return key;
	}

	void Prune()
	{
		int live = 0;
		for (int section = 0; section < SectionCount; section++)
			live += CountIn(section);

		if (keys.Count <= live)
			return;

		HashSet<object> current = new(ReferenceEqualityComparer.Instance);

		for (int section = 0; section < SectionCount; section++)
			for (int index = 0; index < CountIn(section); index++)
				if (ItemAt(section, index) is TItem item)
					current.Add(item);

		foreach (object item in keys.Keys.ToArray())
			if (!current.Contains(item))
				keys.Remove(item);
	}

	BareCell CellFor(
		UICollectionView collectionView,
		NSIndexPath indexPath,
		NSObject identifier)
	{
		BareCell cell = (BareCell)collectionView.DequeueReusableCell(CellId, indexPath);

		// the tree is built once per recycled cell, then only rebound
		if (cell.Hosted is null)
		{
			ItemView<TItem> created = CreateItemView();
			created.TintHost = this;

			cell.Attach(
				created,
				HighlightsSelection ? HighlightColor?.ToUIColor() ?? UIColor.SystemGray4 : null,
				MultiSelects,
				ReorderCommand is not null);
		}

		if (cell.Hosted is ItemView<TItem> view && identifier is ItemKey { Item: TItem item })
			view.Item = item;

		return cell;
	}

	BareHeader SupplementaryFor(
		UICollectionView collectionView,
		string kind,
		NSIndexPath indexPath)
	{
		bool footer = kind == UICollectionElementKindSectionKey.Footer.ToString();

		BareHeader header = (BareHeader)collectionView.DequeueReusableSupplementaryView(
			new NSString(kind),
			footer ? FooterId : HeaderId,
			indexPath);

		if (header.Hosted is null && (footer ? CreateFooterView() : CreateHeaderView()) is ItemView<TSection> view)
		{
			view.TintHost = this;
			header.Attach(view);
		}

		if (header.Hosted is ItemView<TSection> hosted)
			hosted.Item = SectionAt(indexPath.Section);

		return header;
	}

	void ICollectionHost.SyncEmptyState() =>
		SyncEmptyState();

	void ICollectionHost.SyncInsets() =>
		SyncInsets();

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

	void SyncInsets()
	{
		if (!IsRealized)
			return;

		// while refreshing, UIKit holds the spinner open through the top inset; writing ours over
		// it collapses the spinner mid-spin. It restores our inset when EndRefreshing runs a sync
		if (refresh is { Refreshing: true })
			return;

		Thickness bled = BledInsets;
		bool horizontal = Layout.Kind is CollectionLayoutKind.Carousel;

		UIEdgeInsets insets = horizontal
			? new(0, (nfloat)bled.Left, 0, (nfloat)bled.Right)
			: new((nfloat)bled.Top, 0, (nfloat)bled.Bottom, 0);

		if (Ui.ContentInset == insets)
			return;

		Ui.ContentInset = insets;
		Ui.VerticalScrollIndicatorInsets = insets;
		Ui.HorizontalScrollIndicatorInsets = insets;
	}

	void SyncEmptyState()
	{
		if (EmptyView is not View empty || !IsRealized)
			return;

		empty.TintHost = this;
		UIView native = empty.Realize();

		if (!ReferenceEquals(Ui.BackgroundView, native))
			Ui.BackgroundView = native;

		native.Hidden = !IsEmpty;
	}

	internal ItemView<TItem> CreateItemView() =>
		ItemTemplate?.Invoke()
		?? throw new InvalidOperationException(
			$"CollectionView<{typeof(TItem).Name}> needs an ItemTemplate.");

	internal ItemView<TSection>? CreateHeaderView() =>
		HeaderTemplate?.Invoke();

	internal ItemView<TSection>? CreateFooterView() =>
		FooterTemplate?.Invoke();

	internal void Select(
		int section,
		int index)
	{
		if (ItemAt(section, index) is not TItem item || Selection is not ICommand command)
			return;

		if (command.CanExecute(item))
			command.Execute(item);
	}

	UICollectionViewLayout CreateLayout(
		CollectionLayout layout,
		bool headers,
		bool footers)
	{
		switch (layout.Kind)
		{
			case CollectionLayoutKind.Grid:
				// absolute row heights from our measure; estimated sizing breaks the peek portal
				return new UICollectionViewCompositionalLayout((_, environment) =>
					GridSection(layout, headers, footers, environment.Container.EffectiveContentSize.Width));

			case CollectionLayoutKind.Carousel:
				return new UICollectionViewCompositionalLayout(CarouselSection(layout));

			default:
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

				return UICollectionViewCompositionalLayout.GetLayout(configuration);
		}
	}

	static NSCollectionLayoutBoundarySupplementaryItem Boundary(
		bool footer) =>
		NSCollectionLayoutBoundarySupplementaryItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateEstimated(44)),
			(footer ? UICollectionElementKindSectionKey.Footer : UICollectionElementKindSectionKey.Header).ToString(),
			footer ? NSRectAlignment.Bottom : NSRectAlignment.Top);

	ItemView<TItem>? sizingCell;
	bool sizedWithItem;

	double RowHeight(
		double width)
	{
		sizingCell ??= CreateItemView();

		if (ItemAt(0, 0) is TItem item)
		{
			sizingCell.Item = item;
			sizedWithItem = true;
		}

		sizingCell.Measure(new(width, double.PositiveInfinity));
		return sizingCell.DesiredSize.Height;
	}

	NSCollectionLayoutSection GridSection(
		CollectionLayout layout,
		bool headers,
		bool footers,
		nfloat width)
	{
		nfloat spacing = (nfloat)layout.Spacing;
		double column = Math.Max(1, (width - spacing * (layout.Columns + 1)) / layout.Columns);
		nfloat height = (nfloat)Math.Max(1, RowHeight(column));

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

		List<NSCollectionLayoutBoundarySupplementaryItem> boundaries = [];

		if (headers)
			boundaries.Add(Boundary(footer: false));

		if (footers)
			boundaries.Add(Boundary(footer: true));

		if (boundaries.Count > 0)
			section.BoundarySupplementaryItems = [.. boundaries];

		return section;
	}

	static NSCollectionLayoutSection CarouselSection(
		CollectionLayout layout)
	{
		nfloat spacing = (nfloat)layout.Spacing;

		NSCollectionLayoutItem item = NSCollectionLayoutItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateFractionalHeight(1f)));

		NSCollectionLayoutGroup group = NSCollectionLayoutGroup.CreateHorizontal(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateAbsolute((nfloat)layout.ItemWidth),
				NSCollectionLayoutDimension.CreateFractionalHeight(1f)),
			item,
			1);

		NSCollectionLayoutSection section = NSCollectionLayoutSection.Create(group);
		section.InterGroupSpacing = spacing;
		section.OrthogonalScrollingBehavior = layout.Snap switch
		{
			CarouselSnap.LeadingBoundary => UICollectionLayoutSectionOrthogonalScrollingBehavior.ContinuousGroupLeadingBoundary,
			CarouselSnap.LeadingBoundaryPeek => UICollectionLayoutSectionOrthogonalScrollingBehavior.ContinuousGroupLeadingBoundary,
			CarouselSnap.Item => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPaging,
			CarouselSnap.ItemCentered => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPagingCentered,
			CarouselSnap.Page => UICollectionLayoutSectionOrthogonalScrollingBehavior.Paging,
			_ => UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous
		};
		nfloat leadingInset = layout.Snap is CarouselSnap.LeadingBoundaryPeek
			? spacing * 2
			: spacing;

		section.ContentInsets = new(0, leadingInset, 0, spacing);

		return section;
	}
}

internal sealed class CollectionDelegate<TItem, TSection>(
	CollectionView<TItem, TSection> element) : UICollectionViewDelegate, IUICollectionViewDataSourcePrefetching
	where TItem : class
	where TSection : class, ISection<TItem>
{
	// PageAppeared releases it, so it stays lit under a pushed page
	public override void ItemSelected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		if (element.EditingNow)
		{
			element.EditSelect(indexPath.Section, indexPath.Row, true);
			return;
		}

		if (!element.HighlightsSelection)
			collectionView.DeselectItem(indexPath, true);

		element.Select(indexPath.Section, indexPath.Row);
	}

	public override void ItemDeselected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		if (element.EditingNow)
			element.EditSelect(indexPath.Section, indexPath.Row, false);
	}

	public override void WillDisplayCell(
		UICollectionView collectionView,
		UICollectionViewCell cell,
		NSIndexPath indexPath) =>
		element.OnWillDisplay(indexPath.Section, indexPath.Row);

	public override void Scrolled(
		UIScrollView scrollView) =>
		element.OnScrolled(scrollView.ContentOffset.Y + scrollView.AdjustedContentInset.Top);

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
		IUIContextMenuInteractionAnimating animator) =>
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
			if (prefetches.Remove(path, out CancellationTokenSource? cancellation))
			{
				cancellation.Cancel();
				cancellation.Dispose();
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
		{ }
		finally
		{
			prefetches.Remove(path);
			cancellation.Dispose();
		}
	}
}

internal sealed class CollectionHost : UICollectionView
{
	readonly ICollectionHost? element;

	public CollectionHost(
		ICollectionHost element,
		UICollectionViewLayout layout) : base(CGRect.Empty, layout)
	{
		this.element = element;
	}

	// ReSharper disable once UnusedMember.Local
	public CollectionHost(
		NativeHandle handle) : base(handle)
	{ }

	public override void LayoutSubviews()
	{
		element?.SyncInsets();

		base.LayoutSubviews();

		element?.SyncEmptyState();
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

		View.BackgroundColor = UIColor.SystemBackground;
		View.AddSubview(content.Realize());

		// an explicit Width on the preview root sizes the peek; default is the list's width
		nfloat effective = double.IsFinite(content.Width) ? (nfloat)content.Width : width;

		content.Measure(new(effective, double.PositiveInfinity));
		PreferredContentSize = new CGSize(effective, (nfloat)content.DesiredSize.Height);
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		content?.Arrange(new(0, 0, View.Bounds.Width, View.Bounds.Height));
	}
}

internal sealed class CollectionSource : UICollectionViewDiffableDataSource<NSNumber, ItemKey>
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
}

internal sealed class BareCell(
	NativeHandle handle) : UICollectionViewListCell(handle)
{
	public View? Hosted { get; private set; }

	UIColor? highlight;

	public void Attach(
		View view,
		UIColor? highlight,
		bool multiselects,
		bool reorders)
	{
		Hosted = view;
		this.highlight = highlight;

		// one write; repaints during a peek desync the portal
		BackgroundConfiguration = UIBackgroundConfiguration.ClearConfiguration;

		ContentView.AddSubview(view.Realize());

		// edit-mode accessories: the circle and the drag handle
		List<UICellAccessory> accessories = [];

		if (multiselects)
			accessories.Add(new UICellAccessoryMultiselect());

		if (reorders)
			accessories.Add(new UICellAccessoryReorder());

		if (accessories.Count > 0)
			Accessories = [.. accessories];
	}

	bool lit;

	// a list cell ignores SelectedBackgroundView
	public override void UpdateConfiguration(
		UICellConfigurationState state)
	{
		if (highlight is null)
			return;

		bool wantsLit = state.Selected || state.Highlighted;
		if (wantsLit == lit && Hosted is not null)
			return;

		lit = wantsLit;

		UIBackgroundConfiguration background = UIBackgroundConfiguration.ClearConfiguration;

		if (wantsLit)
			background.BackgroundColor = highlight;

		BackgroundConfiguration = background;
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		Hosted?.Arrange(new(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height));
	}

	// self-sizing: the compositional layout asks, our engine answers
	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		if (Hosted is null)
			return layoutAttributes;

		Hosted.Measure(new(layoutAttributes.Frame.Width, double.PositiveInfinity));

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)Hosted.DesiredSize.Height;
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}
}

internal sealed class BareHeader(
	NativeHandle handle) : UICollectionReusableView(handle)
{
	public View? Hosted { get; private set; }


	public void Attach(
		View view)
	{
		Hosted = view;

		AddSubview(view.Realize());
	}

	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		if (Hosted is null)
			return layoutAttributes;

		Hosted.Measure(new(layoutAttributes.Frame.Width, double.PositiveInfinity));

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)Hosted.DesiredSize.Height;
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		Hosted?.Arrange(new(0, 0, Bounds.Width, Bounds.Height));
	}
}
