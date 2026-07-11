using System.Collections.Specialized;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BareUI;

public partial class CollectionView<TItem>
{
	internal const string CellId = "BareCell";
	internal const string HeaderId = "BareHeader";

	CollectionSource<TItem>? source;

	private protected override UIView CreateNative()
	{
		CollectionHost collection = new(this, CreateLayout(Layout, HeaderTemplate is not null))
		{
			BackgroundColor = UIColor.Clear
		};

		collection.RegisterClassForCell(typeof(BareCell), CellId);
		collection.RegisterClassForSupplementaryView(
			typeof(BareHeader),
			UICollectionElementKindSection.Header,
			HeaderId);

		source = new(this);
		collection.Source = source;

		return collection;
	}

	private protected override void ApplyProperties() =>
		ReloadItems();

	UICollectionView Ui =>
		(UICollectionView)Native;

	partial void ReloadItems()
	{
		if (!IsRealized)
			return;

		Ui.ReloadData();
	}

	// an ObservableCollection change becomes the matching animated batch update, not a full reload
	partial void ApplyChange(
		NotifyCollectionChangedEventArgs change)
	{
		if (!IsRealized)
			return;

		if (IsGrouped || change.Action is NotifyCollectionChangedAction.Reset)
		{
			ReloadItems();
			return;
		}

		Ui.PerformBatchUpdates(
			() =>
			{
				switch (change.Action)
				{
					case NotifyCollectionChangedAction.Add:
						Ui.InsertItems(Paths(change.NewStartingIndex, change.NewItems?.Count ?? 0));
						break;

					case NotifyCollectionChangedAction.Remove:
						Ui.DeleteItems(Paths(change.OldStartingIndex, change.OldItems?.Count ?? 0));
						break;

					case NotifyCollectionChangedAction.Replace:
						Ui.ReloadItems(Paths(change.NewStartingIndex, change.NewItems?.Count ?? 0));
						break;

					case NotifyCollectionChangedAction.Move:
						Ui.MoveItem(
							NSIndexPath.FromRowSection(change.OldStartingIndex, 0),
							NSIndexPath.FromRowSection(change.NewStartingIndex, 0));
						break;
				}
			},
			null);
	}

	static NSIndexPath[] Paths(
		int start,
		int count)
	{
		NSIndexPath[] paths = new NSIndexPath[Math.Max(0, count)];

		for (int offset = 0; offset < paths.Length; offset++)
			paths[offset] = NSIndexPath.FromRowSection(start + offset, 0);

		return paths;
	}

	// UIKit re-runs layout after every reload and batch update, so this is the one place that cannot
	// be missed by an update path or dropped by an interrupted animation
	void ICollectionHost.SyncEmptyState() =>
		SyncEmptyState();

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

	static UICollectionViewLayout CreateLayout(
		CollectionLayout layout,
		bool headers) =>
		layout.Kind switch
		{
			CollectionLayoutKind.Grid => new UICollectionViewCompositionalLayout(GridSection(layout, headers)),
			CollectionLayoutKind.Carousel => new UICollectionViewCompositionalLayout(CarouselSection(layout)),
			_ => UICollectionViewCompositionalLayout.GetLayout(
				new UICollectionLayoutListConfiguration(
					layout.Grouped
						? UICollectionLayoutListAppearance.InsetGrouped
						: UICollectionLayoutListAppearance.Plain)
				{
					HeaderMode = headers
						? UICollectionLayoutListHeaderMode.Supplementary
						: UICollectionLayoutListHeaderMode.None
				})
		};

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
		section.OrthogonalScrollingBehavior = UICollectionLayoutSectionOrthogonalScrollingBehavior.Continuous;
		section.ContentInsets = new(0, spacing, 0, spacing);

		return section;
	}
}

// the data source keeps the element side generic; cells stay plain UIKit
sealed class CollectionSource<TItem>(
	CollectionView<TItem> element) : UICollectionViewSource
	where TItem : class
{
	public override nint NumberOfSections(
		UICollectionView collectionView) =>
		element.SectionCount;

	public override nint GetItemsCount(
		UICollectionView collectionView,
		nint section) =>
		element.CountIn((int)section);

	public override UICollectionViewCell GetCell(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		BareCell cell = (BareCell)collectionView.DequeueReusableCell(
			CollectionView<TItem>.CellId,
			indexPath);

		// the tree is built once per recycled cell, then only rebound
		if (cell.Hosted is null)
			cell.Attach(element.CreateItemView());

		if (cell.Hosted is ItemView<TItem> view)
			view.Item = element.ItemAt(indexPath.Section, indexPath.Row);

		return cell;
	}

	public override UICollectionReusableView GetViewForSupplementaryElement(
		UICollectionView collectionView,
		NSString elementKind,
		NSIndexPath indexPath)
	{
		BareHeader header = (BareHeader)collectionView.DequeueReusableSupplementaryView(
			elementKind,
			CollectionView<TItem>.HeaderId,
			indexPath);

		if (header.Hosted is null && element.CreateHeaderView() is { } view)
			header.Attach(view);

		if (header.Hosted is ItemView<Section<TItem>> hosted)
			hosted.Item = element.SectionAt(indexPath.Section);

		return header;
	}

	public override void ItemSelected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		collectionView.DeselectItem(indexPath, true);

		element.Select(indexPath.Section, indexPath.Row);
	}
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
