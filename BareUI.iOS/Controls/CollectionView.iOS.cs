using System.Collections.Specialized;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BareUI;

public partial class CollectionView<TItem>
{
	internal const string CellId = "BareCell";
	internal const string HeaderId = "BareHeader";

	// one key per item, reused across snapshots: no allocation per update, and it roots the managed
	// peers so the GC cannot collect an identifier the data source still holds
	readonly Dictionary<object, ItemKey> keys = new(ReferenceEqualityComparer.Instance);
	readonly List<NSNumber> sectionKeys = [];

	UICollectionViewDiffableDataSource<NSNumber, ItemKey>? data;
	CollectionDelegate<TItem>? selection;

	bool snapshotQueued;

	private protected override UIView CreateNative()
	{
		bool carousel = Layout.Kind is CollectionLayoutKind.Carousel;

		CollectionHost collection = new(this, CreateLayout(Layout, HeaderTemplate is not null))
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

		data = new(collection, CellFor)
		{
			SupplementaryViewProvider = HeaderFor
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
		refresh.ValueChanged += async (sender, e) =>
		{
			try
			{
				if (RefreshCommand is { } command)
					await command();
			}
			finally
			{
				refresh.EndRefreshing();
			}
		};

		collection.RefreshControl = refresh;
	}

	/// <summary>
	/// Scrolls the item into view.
	/// </summary>
	public void ScrollTo(
		TItem item,
		bool animated = true)
	{
		if (!IsRealized || data is null || !keys.TryGetValue(item, out ItemKey? key))
			return;

		if (data.GetIndexPath(key) is { } path)
			Ui.ScrollToItem(path, UICollectionViewScrollPosition.Top, animated);
	}

	internal void OnScrolled(
		double offset) =>
		Scrolled?.Invoke(offset);

	// swipe actions and context menus are per-row, so they resolve the item behind the index path
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
		if (ContextMenu.Count == 0 || ItemAt(indexPath.Section, indexPath.Row) is not { } item)
			return null;

		return UIContextMenuConfiguration.Create(
			null,
			null,
			_ =>
			{
				UIAction[] entries = new UIAction[ContextMenu.Count];

				for (int index = 0; index < ContextMenu.Count; index++)
				{
					MenuAction entry = ContextMenu[index];

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

	// UIKit does the diffing. A burst of changes (an Add loop over an ObservableCollection) collapses
	// into one snapshot on the next turn of the run loop instead of N separate updates.
	partial void ReloadItems() =>
		QueueSnapshot();

	partial void ApplyChange(
		NotifyCollectionChangedEventArgs change) =>
		QueueSnapshot();

	void QueueSnapshot()
	{
		if (!IsRealized || snapshotQueued)
			return;

		snapshotQueued = true;

		DispatchQueue.MainQueue.DispatchAsync(() =>
		{
			snapshotQueued = false;

			if (IsRealized)
				ApplySnapshot();
		});
	}

	void ApplySnapshot()
	{
		if (data is null)
			return;

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
		data.ApplySnapshot(snapshot, Ui.Window is not null);
	}

	ItemKey KeyFor(
		object item)
	{
		if (!keys.TryGetValue(item, out ItemKey? key))
			keys[item] = key = new(item);

		return key;
	}

	// drop keys for items that left, so the cache cannot grow without bound
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

	UICollectionViewCell CellFor(
		UICollectionView collectionView,
		NSIndexPath indexPath,
		NSObject identifier)
	{
		BareCell cell = (BareCell)collectionView.DequeueReusableCell(CellId, indexPath);

		// the tree is built once per recycled cell, then only rebound
		if (cell.Hosted is null)
			cell.Attach(CreateItemView());

		if (cell.Hosted is ItemView<TItem> view && identifier is ItemKey { Item: TItem item })
			view.Item = item;

		return cell;
	}

	UICollectionReusableView HeaderFor(
		UICollectionView collectionView,
		string kind,
		NSIndexPath indexPath)
	{
		BareHeader header = (BareHeader)collectionView.DequeueReusableSupplementaryView(
			new NSString(kind),
			HeaderId,
			indexPath);

		if (header.Hosted is null && CreateHeaderView() is { } view)
			header.Attach(view);

		if (header.Hosted is ItemView<Section<TItem>> hosted)
			hosted.Item = SectionAt(indexPath.Section);

		return header;
	}

	// UIKit re-runs layout after every snapshot, so this is the one place that cannot be missed by an
	// update path or dropped by an interrupted animation
	void ICollectionHost.SyncEmptyState() =>
		SyncEmptyState();

	void ICollectionHost.SyncInsets() =>
		SyncInsets();

	// the bleed becomes a content inset along the scroll axis: items start inside the safe area and
	// scroll under the bar. Nothing is ever inset across the axis, so the view cannot drift.
	void SyncInsets()
	{
		if (!IsRealized)
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

	internal ItemView<Section<TItem>>? CreateHeaderView() =>
		HeaderTemplate?.Invoke();

	internal void Select(
		int section,
		int index)
	{
		if (ItemAt(section, index) is not { } item || SelectionCommand is not { } command)
			return;

		if (command.CanExecute(item))
			command.Execute(item);
	}

	UICollectionViewLayout CreateLayout(
		CollectionLayout layout,
		bool headers)
	{
		switch (layout.Kind)
		{
			case CollectionLayoutKind.Grid:
				return new UICollectionViewCompositionalLayout(GridSection(layout, headers));

			case CollectionLayoutKind.Carousel:
				return new UICollectionViewCompositionalLayout(CarouselSection(layout));

			default:
				// a list configuration paints its own opaque background, which would cover the empty view
				UICollectionLayoutListConfiguration configuration = new(
					layout.Grouped
						? UICollectionLayoutListAppearance.InsetGrouped
						: UICollectionLayoutListAppearance.Plain)
				{
					BackgroundColor = UIColor.Clear,
					HeaderMode = headers
						? UICollectionLayoutListHeaderMode.Supplementary
						: UICollectionLayoutListHeaderMode.None
				};

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

	static NSCollectionLayoutBoundarySupplementaryItem Header() =>
		NSCollectionLayoutBoundarySupplementaryItem.Create(
			NSCollectionLayoutSize.Create(
				NSCollectionLayoutDimension.CreateFractionalWidth(1f),
				NSCollectionLayoutDimension.CreateEstimated(44)),
			UICollectionElementKindSectionKey.Header.ToString(),
			NSRectAlignment.Top);

	static NSCollectionLayoutSection GridSection(
		CollectionLayout layout,
		bool headers)
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

		if (headers)
			section.BoundarySupplementaryItems = [Header()];

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

// only selection: the diffable data source owns the data side
sealed class CollectionDelegate<TItem>(
	CollectionView<TItem> element) : UICollectionViewDelegate
	where TItem : class
{
	public override void ItemSelected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		collectionView.DeselectItem(indexPath, true);

		element.Select(indexPath.Section, indexPath.Row);
	}

	public override void Scrolled(
		UIScrollView scrollView) =>
		element.OnScrolled(scrollView.ContentOffset.Y + scrollView.AdjustedContentInset.Top);

	public override UIContextMenuConfiguration? GetContextMenuConfiguration(
		UICollectionView collectionView,
		NSIndexPath indexPath,
		CGPoint point) =>
		element.MenuConfiguration(indexPath);
}

/// <summary>
/// The native <c>UICollectionView</c>, which drives the empty state from its own layout pass.
/// </summary>
sealed class CollectionHost : UICollectionView
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

/// <summary>
/// A recycled cell that hosts one BareUI item tree.
/// </summary>
sealed class BareCell : UICollectionViewCell
{
	View? hosted;

	public BareCell(
		NativeHandle handle) : base(handle)
	{ }

	public View? Hosted =>
		hosted;

	public void Attach(
		View view)
	{
		hosted = view;

		ContentView.AddSubview(view.Realize());
	}

	// self-sizing: the compositional layout asks, our engine answers
	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		if (hosted is null)
			return layoutAttributes;

		hosted.Measure(new(layoutAttributes.Frame.Width, double.PositiveInfinity));

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)hosted.DesiredSize.Height;
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		hosted?.Arrange(new(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height));
	}
}

/// <summary>
/// A recycled section header hosting one BareUI tree.
/// </summary>
sealed class BareHeader : UICollectionReusableView
{
	View? hosted;

	public BareHeader(
		NativeHandle handle) : base(handle)
	{ }

	public View? Hosted =>
		hosted;

	public void Attach(
		View view)
	{
		hosted = view;

		AddSubview(view.Realize());
	}

	public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
		UICollectionViewLayoutAttributes layoutAttributes)
	{
		if (hosted is null)
			return layoutAttributes;

		hosted.Measure(new(layoutAttributes.Frame.Width, double.PositiveInfinity));

		CGRect frame = layoutAttributes.Frame;
		frame.Height = (nfloat)hosted.DesiredSize.Height;
		layoutAttributes.Frame = frame;

		return layoutAttributes;
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		hosted?.Arrange(new(0, 0, Bounds.Width, Bounds.Height));
	}
}
