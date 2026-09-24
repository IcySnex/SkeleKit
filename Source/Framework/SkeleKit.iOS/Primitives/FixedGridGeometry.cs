namespace SkeleKit;

/// <summary>
/// The arithmetic for <see cref="CollectionLayout.FixedGrid"/>: section tops, item frames,
/// and the visible range.
/// </summary>
/// <remarks>
/// The build reads one item count per section through a delegate and never creates an item.
/// Every lookup after the build is arithmetic. The layout can therefore answer UIKit without
/// touching an offscreen section model.
/// </remarks>
internal sealed class FixedGridGeometry
{
	readonly double[] sectionTops;
	readonly double[] sectionEnds;

	public FixedGridGeometry(
		int sectionCount,
		Func<int, int> itemCount,
		int columns,
		double spacing,
		double itemHeight,
		double headerHeight,
		double footerHeight,
		Thickness insets,
		double width)
	{
		ArgumentNullException.ThrowIfNull(itemCount);
		ArgumentOutOfRangeException.ThrowIfNegative(sectionCount);

		Columns = Math.Max(1, columns);
		Spacing = Math.Max(0, spacing);
		ItemHeight = Math.Max(1, itemHeight);
		HeaderHeight = Math.Max(0, headerHeight);
		FooterHeight = Math.Max(0, footerHeight);
		LeftInset = insets.Left;
		RightInset = insets.Right;
		TopInset = insets.Top;
		BottomInset = insets.Bottom;
		Width = Math.Max(0, width);
		ItemWidth = Math.Max(1, (Width - insets.Horizontal - Spacing * (Columns + 1)) / Columns);

		sectionTops = new double[sectionCount];
		sectionEnds = new double[sectionCount];

		double running = 0;

		for (int section = 0; section < sectionCount; section++)
		{
			sectionTops[section] = running;

			int count = Math.Max(0, itemCount(section));
			int rows = Rows(count);
			double content = rows > 0 ? rows * ItemHeight + (rows - 1) * Spacing : 0;

			double height = Spacing
				+ HeaderHeight
				+ content
				+ FooterHeight
				+ Spacing
				+ (section == 0 ? TopInset : 0)
				+ (section == sectionCount - 1 ? BottomInset : 0);

			sectionEnds[section] = running + height;
			running += height;

			if (section < sectionCount - 1)
				running += Spacing;
		}

		ContentHeight = running;
	}


	/// <summary>
	/// Columns per row.
	/// </summary>
	public int Columns { get; }

	/// <summary>
	/// The gap between items, between item rows, and around a section, in points.
	/// </summary>
	public double Spacing { get; }

	/// <summary>
	/// The width of one item, in points.
	/// </summary>
	public double ItemWidth { get; }

	/// <summary>
	/// The height of one item, in points.
	/// </summary>
	public double ItemHeight { get; }

	/// <summary>
	/// The section header height, in points. Zero when there is no header template.
	/// </summary>
	public double HeaderHeight { get; }

	/// <summary>
	/// The section footer height, in points. Zero when there is no footer template.
	/// </summary>
	public double FooterHeight { get; }

	/// <summary>
	/// The left content inset of the collection.
	/// </summary>
	public double LeftInset { get; }

	/// <summary>
	/// The right content inset of the collection.
	/// </summary>
	public double RightInset { get; }

	/// <summary>
	/// The top content inset of the first section.
	/// </summary>
	public double TopInset { get; }

	/// <summary>
	/// The bottom content inset of the last section.
	/// </summary>
	public double BottomInset { get; }

	/// <summary>
	/// The collection width used for this geometry.
	/// </summary>
	public double Width { get; }

	/// <summary>
	/// The full content height, in points.
	/// </summary>
	public double ContentHeight { get; }

	/// <summary>
	/// The number of sections used for this geometry.
	/// </summary>
	public int SectionCount => sectionTops.Length;


	/// <summary>
	/// The number of item rows in a section.
	/// </summary>
	public int Rows(
		int itemCount) =>
		(itemCount + Columns - 1) / Columns;

	/// <summary>
	/// The top edge of a section, including its outer inset.
	/// </summary>
	public double SectionTop(
		int section) =>
		sectionTops[section];

	/// <summary>
	/// The height of a section, including its header, footer, and outer inset.
	/// </summary>
	public double SectionHeight(
		int section) =>
		sectionEnds[section] - sectionTops[section];

	/// <summary>
	/// The top edge of a section's first item row.
	/// </summary>
	public double SectionContentTop(
		int section) =>
		SectionTop(section) + (section == 0 ? TopInset : 0) + Spacing + HeaderHeight;

	/// <summary>
	/// The height of a section's item area.
	/// </summary>
	public double SectionItemsHeight(
		int itemCount)
	{
		int rows = Rows(itemCount);
		return rows > 0 ? rows * ItemHeight + (rows - 1) * Spacing : 0;
	}

	/// <summary>
	/// The frame of a section header.
	/// </summary>
	public Rect HeaderFrame(
		int section) =>
		new(
			LeftInset + Spacing,
			SectionTop(section) + (section == 0 ? TopInset : 0) + Spacing,
			HeaderWidth,
			HeaderHeight);

	/// <summary>
	/// The frame of a section footer.
	/// </summary>
	public Rect FooterFrame(
		int section,
		int itemCount) =>
		new(
			LeftInset + Spacing,
			SectionContentTop(section) + SectionItemsHeight(itemCount),
			HeaderWidth,
			FooterHeight);

	/// <summary>
	/// The frame of one item.
	/// </summary>
	public Rect ItemFrame(
		int section,
		int item)
	{
		int row = item / Columns;
		int column = item % Columns;

		return new(
			LeftInset + Spacing + column * (ItemWidth + Spacing),
			SectionContentTop(section) + row * (ItemHeight + Spacing),
			ItemWidth,
			ItemHeight);
	}

	/// <summary>
	/// The first section that intersects a vertical range.
	/// </summary>
	public int FirstVisibleSection(
		double minY)
	{
		int low = 0;
		int high = sectionEnds.Length;

		while (low < high)
		{
			int middle = (low + high) / 2;

			if (sectionEnds[middle] <= minY)
				low = middle + 1;
			else
				high = middle;
		}

		return low;
	}

	/// <summary>
	/// The section range that intersects a vertical range. The end is exclusive.
	/// </summary>
	public bool VisibleSections(
		double minY,
		double maxY,
		out int first,
		out int lastExclusive)
	{
		first = FirstVisibleSection(minY);

		int low = first;
		int high = sectionTops.Length;

		while (low < high)
		{
			int middle = (low + high) / 2;

			if (sectionTops[middle] < maxY)
				low = middle + 1;
			else
				high = middle;
		}

		lastExclusive = low;
		return first < lastExclusive;
	}


	double HeaderWidth =>
		Math.Max(0, Width - LeftInset - RightInset - Spacing * 2);
}
