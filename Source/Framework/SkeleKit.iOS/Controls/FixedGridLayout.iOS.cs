using Foundation;
using UIKit;

namespace SkeleKit;

/// <summary>
/// A visible-only layout for <see cref="CollectionLayout.FixedGrid"/>.
/// </summary>
/// <remarks>
/// The layout writes one section top per section and answers UIKit with arithmetic. UIKit asks for the
/// visible rectangle only, so the collection reads only the visible section models.
/// </remarks>
internal sealed class FixedGridLayout<TItem, TSection> : UICollectionViewLayout
	where TItem : class
	where TSection : class, ISection<TItem>
{
	static readonly string HeaderKind = UICollectionElementKindSectionKey.Header.ToString();
	static readonly string FooterKind = UICollectionElementKindSectionKey.Footer.ToString();

	readonly CollectionView<TItem, TSection> host;

	FixedGridGeometry? geometry;
	bool geometryDirty = true;
	nfloat measuredWidth = -1;

	internal FixedGridLayout(
		CollectionView<TItem, TSection> host) =>
		this.host = host;


	internal double ContentHeight =>
		geometry?.ContentHeight ?? 0;


	internal void MarkGeometryDirty() =>
		geometryDirty = true;

	internal bool TrySectionTop(
		int section,
		out double top)
	{
		if (geometry is null || section < 0 || section >= host.SectionCount)
		{
			top = 0;
			return false;
		}

		top = geometry.SectionTop(section);
		return true;
	}


	public override void PrepareLayout()
	{
		if (!geometryDirty)
			return;

		double width = CollectionView?.Bounds.Width ?? 0;

		measuredWidth = (nfloat)width;
		geometry = host.BuildFixedGridGeometry(width);
		geometryDirty = false;
	}

	public override CGSize CollectionViewContentSize =>
		new(CollectionView?.Bounds.Width ?? 0, (nfloat)ContentHeight);

	// an offset change does not move a frame; only a width change rebuilds the geometry
	public override bool ShouldInvalidateLayoutForBoundsChange(
		CGRect newBounds)
	{
		if (newBounds.Width == measuredWidth)
			return false;

		measuredWidth = newBounds.Width;
		geometryDirty = true;
		return true;
	}

	public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(
		CGRect rect)
	{
		if (geometry is null)
			return [];

		double minY = (double)rect.GetMinY();
		double maxY = (double)rect.GetMaxY();

		if (double.IsNaN(minY) || double.IsNaN(maxY) || maxY < minY)
			return [];

		if (!geometry.VisibleSections(Math.Max(0, minY), Math.Min(geometry.ContentHeight, maxY), out int first, out int last))
			return [];

		List<UICollectionViewLayoutAttributes> attributes = [];

		for (int section = first; section < last; section++)
		{
			if (host.HasSectionHeader)
				attributes.Add(SupplementaryAttributes(HeaderKind, section, geometry.HeaderFrame(section)));

			if (host.Expanded(section))
				AddItems(attributes, section, minY, maxY);

			if (host.HasSectionFooter)
				attributes.Add(SupplementaryAttributes(FooterKind, section, geometry.FooterFrame(section, host.CountIn(section))));
		}

		return [.. attributes];
	}

	public override UICollectionViewLayoutAttributes LayoutAttributesForItem(
		NSIndexPath indexPath)
	{
		if (geometry is null
			|| indexPath.Section < 0 || indexPath.Section >= host.SectionCount
			|| !host.Expanded((int)indexPath.Section)
			|| indexPath.Row < 0 || indexPath.Row >= host.CountIn((int)indexPath.Section))
			return null!;

		return CellAttributes(
			indexPath,
			geometry.ItemFrame((int)indexPath.Section, (int)indexPath.Row));
	}

	public override UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView(
		NSString elementKind,
		NSIndexPath indexPath)
	{
		if (geometry is null || indexPath.Section < 0 || indexPath.Section >= host.SectionCount)
			return null!;

		string kind = elementKind.ToString();

		if (kind == HeaderKind && host.HasSectionHeader)
			return SupplementaryAttributes(kind, (int)indexPath.Section, geometry.HeaderFrame((int)indexPath.Section));

		if (kind == FooterKind && host.HasSectionFooter)
			return SupplementaryAttributes(
				kind,
				(int)indexPath.Section,
				geometry.FooterFrame((int)indexPath.Section, host.CountIn((int)indexPath.Section)));

		return null!;
	}


	void AddItems(
		List<UICollectionViewLayoutAttributes> attributes,
		int section,
		double minY,
		double maxY)
	{
		if (geometry is null)
			return;

		int count = host.CountIn(section);
		if (count == 0)
			return;

		double step = geometry.ItemHeight + geometry.Spacing;
		double top = geometry.SectionContentTop(section);
		int rows = geometry.Rows(count);
		int firstRow = Math.Clamp((int)Math.Floor((minY - top) / step), 0, rows);
		int lastRow = Math.Clamp((int)Math.Ceiling((maxY - top) / step), 0, rows);

		for (int row = firstRow; row < lastRow; row++)
		{
			int start = row * geometry.Columns;
			int end = Math.Min(start + geometry.Columns, count);

			for (int item = start; item < end; item++)
			{
				attributes.Add(CellAttributes(
					NSIndexPath.FromRowSection(item, section),
					geometry.ItemFrame(section, item)));
			}
		}
	}

	static UICollectionViewLayoutAttributes CellAttributes(
		NSIndexPath indexPath,
		Rect frame)
	{
		UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
		attributes.Frame = ToNative(frame);
		return attributes;
	}

	static UICollectionViewLayoutAttributes SupplementaryAttributes(
		string kind,
		int section,
		Rect frame)
	{
		UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForSupplementaryView(
			new NSString(kind),
			NSIndexPath.FromRowSection(0, section));
		attributes.Frame = ToNative(frame);
		return attributes;
	}

	static CGRect ToNative(
		Rect frame) =>
		new(frame.X, frame.Y, frame.Width, frame.Height);
}
