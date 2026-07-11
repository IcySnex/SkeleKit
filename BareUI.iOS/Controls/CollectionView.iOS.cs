using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BareUI;

public partial class CollectionView<TItem>
{
	const string CellId = "BareCell";

	CollectionSource<TItem>? source;

	private protected override UIView CreateNative()
	{
		UICollectionView collection = new(CGRect.Empty, CreateLayout(Layout))
		{
			BackgroundColor = UIColor.Clear
		};

		collection.RegisterClassForCell(typeof(BareCell), CellId);

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
		ApplyEmptyView();
	}

	void ApplyEmptyView()
	{
		if (EmptyView is not { } empty)
			return;

		Ui.BackgroundView = ItemCount == 0 ? empty.Realize() : null;
	}

	internal ItemView<TItem> CreateItemView() =>
		ItemTemplate?.Invoke()
		?? throw new InvalidOperationException(
			$"CollectionView<{typeof(TItem).Name}> needs an ItemTemplate.");

	internal void Select(
		int index)
	{
		if (ItemAt(index) is not { } item || SelectionCommand is not { } command)
			return;

		if (command.CanExecute(item))
			command.Execute(item);
	}

	static UICollectionViewLayout CreateLayout(
		CollectionLayout layout) =>
		layout.Kind switch
		{
			CollectionLayoutKind.Grid => new UICollectionViewCompositionalLayout(GridSection(layout)),
			CollectionLayoutKind.Carousel => new UICollectionViewCompositionalLayout(CarouselSection(layout)),
			_ => UICollectionViewCompositionalLayout.GetLayout(
				new UICollectionLayoutListConfiguration(
					layout.Grouped
						? UICollectionLayoutListAppearance.InsetGrouped
						: UICollectionLayoutListAppearance.Plain))
		};

	static NSCollectionLayoutSection GridSection(
		CollectionLayout layout)
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
	public override nint GetItemsCount(
		UICollectionView collectionView,
		nint section) =>
		element.ItemCount;

	public override UICollectionViewCell GetCell(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		BareCell cell = (BareCell)collectionView.DequeueReusableCell("BareCell", indexPath);

		// the tree is built once per recycled cell, then only rebound
		if (cell.Hosted is null)
			cell.Attach(element.CreateItemView());

		if (cell.Hosted is ItemView<TItem> view)
			view.Item = element.ItemAt(indexPath.Row);

		return cell;
	}

	public override void ItemSelected(
		UICollectionView collectionView,
		NSIndexPath indexPath)
	{
		collectionView.DeselectItem(indexPath, true);

		element.Select(indexPath.Row);
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
