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

	UICollectionViewDiffableDataSource<NSNumber, ItemKey>? data;
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

		data = new(collection, CellFor)
		{
			SupplementaryViewProvider = SupplementaryFor
		};

		selection = new(this);
		collection.Delegate = selection;

		ApplyRefresh(collection);

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

	/// <summary>
	/// Scrolls the item into view.
	/// </summary>
	/// <param name="item">The item to scroll to.</param>
	/// <param name="position">Where the item lands in the viewport.</param>
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

		if (data.GetIndexPath(key) is { } path)
			Ui.ScrollToItem(path, native, animated);
	}

	internal void OnScrolled(
		double offset) =>
		Scrolled?.Invoke(offset);

	internal UISwipeActionsConfiguration? SwipeConfiguration(
		NSIndexPath indexPath,
		SwipeSide side)
	{
		if (ItemAt(indexPath.Section, indexPath.Row) is not { } item)
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
					if (action.Command is { } command && command.CanExecute(item))
						command.Execute(item);

					// the diff must land before done resets the swipe, or a removed row slides
					// back into view for a beat before the queued snapshot takes it out
					FlushSnapshot();
					done(true);
				});

			if (action.Icon is { } icon)
				native.Image = UIImage.GetSystemImage(icon);

			if (action.Background is { } background)
				native.BackgroundColor = background.ToUIColor();

			actions.Add(native);
		}

		return actions.Count == 0
			? null
			: UISwipeActionsConfiguration.FromActions([.. actions]);
	}

	internal UIContextMenuConfiguration? MenuConfiguration(
		NSIndexPath indexPath)
	{
		if (ItemContextMenu.Count == 0 || ItemAt(indexPath.Section, indexPath.Row) is not { } item)
			return null;

		return UIContextMenuConfiguration.Create(
			null,
			null,
			_ =>
			{
				UIAction[] entries = new UIAction[ItemContextMenu.Count];

				for (int index = 0; index < ItemContextMenu.Count; index++)
				{
					MenuAction entry = ItemContextMenu[index];

					entries[index] = UIAction.Create(
						entry.Text,
						entry.Icon is { } icon ? UIImage.GetSystemImage(icon) : null,
						null,
						_ =>
						{
							if (entry.Command is { } command && command.CanExecute(item))
								command.Execute(item);
						});

					if (entry.IsDestructive)
						entries[index].Attributes = UIMenuElementAttributes.Destructive;
				}

				return UIMenu.Create(entries);
			});
	}

	private protected override void ApplyProperties() =>
		ReloadItems();

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
			if (cell is BareCell { Hosted: { } hosted })
				hosted.TintChanged();

		EmptyView?.TintChanged();
	}

	// the row tapped on the way out un-highlights on the way back
	internal override void PageAppeared()
	{
		if (!IsRealized)
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
				if (ItemAt(section, index) is { } item)
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
			cell.Attach(CreateItemView());

			// a plain cell has no selection visual of its own
			if (HighlightsSelection)
				cell.SelectedBackgroundView = new UIView
				{
					BackgroundColor = HighlightColor?.ToUIColor() ?? UIColor.SystemGray4
				};
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

		if (header.Hosted is null && (footer ? CreateFooterView() : CreateHeaderView()) is { } view)
			header.Attach(view);

		if (header.Hosted is ItemView<TSection> hosted)
			hosted.Item = SectionAt(indexPath.Section);

		return header;
	}

	void ICollectionHost.SyncEmptyState() =>
		SyncEmptyState();

	void ICollectionHost.SyncInsets() =>
		SyncInsets();

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
		if (EmptyView is not { } empty || !IsRealized)
			return;

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
		if (ItemAt(section, index) is not { } item || Selection is not { } command)
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
				return new UICollectionViewCompositionalLayout(GridSection(layout, headers, footers));

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
				if (SeparatorInsets is { } separator)
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

	static NSCollectionLayoutSection GridSection(
		CollectionLayout layout,
		bool headers,
		bool footers)
	{
		nfloat spacing = (nfloat)layout.Spacing;

		NSCollectionLayoutItem item = NSCollectionLayoutItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f / layout.Columns),
				NSCollectionLayoutDimension.CreateEstimated(160)));

		NSCollectionLayoutGroup group = NSCollectionLayoutGroup.CreateHorizontal(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateEstimated(160)),
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
			CarouselSnap.Item => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPaging,
			CarouselSnap.ItemCentered => UICollectionLayoutSectionOrthogonalScrollingBehavior.GroupPagingCentered,
			CarouselSnap.Page => UICollectionLayoutSectionOrthogonalScrollingBehavior.Paging,
			_ => UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous
		};
		section.ContentInsets = new(0, spacing, 0, spacing);

		return section;
	}
}

internal sealed class CollectionDelegate<TItem, TSection>(
	CollectionView<TItem, TSection> element) : UICollectionViewDelegate
	where TItem : class
	where TSection : class, ISection<TItem>
{
	// PageAppeared releases it, so it stays lit under a pushed page
	public override void ItemSelected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		if (!element.HighlightsSelection)
			collectionView.DeselectItem(indexPath, true);

		element.Select(indexPath.Section, indexPath.Row);
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

	// see LayoutHost
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

internal sealed class BareCell(
	NativeHandle handle) : UICollectionViewCell(handle)
{
	public View? Hosted { get; private set; }

	public void Attach(
		View view)
	{
		Hosted = view;

		ContentView.AddSubview(view.Realize());
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

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		Hosted?.Arrange(new(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height));
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
