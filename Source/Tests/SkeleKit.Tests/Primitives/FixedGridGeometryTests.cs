using Xunit;

namespace SkeleKit.Tests.Primitives;

public class FixedGridGeometryTests
{
	static FixedGridGeometry Build(
		int[] counts,
		int columns = 7,
		double spacing = 6,
		double itemHeight = 10,
		double headerHeight = 20,
		double footerHeight = 0,
		Thickness? insets = null,
		double width = 100) =>
		new(
			counts.Length,
			section => counts[section],
			columns,
			spacing,
			itemHeight,
			headerHeight,
			footerHeight,
			insets ?? Thickness.Zero,
			width);


	[Fact]
	public void ContentHeight_SumsSectionHeightsAndSectionGaps()
	{
		FixedGridGeometry geometry = Build([31, 28]);

		// 6 + 20 + (5 * 10 + 4 * 6) + 6 = 106
		Assert.Equal(106, geometry.SectionHeight(0));

		// 6 + 20 + (4 * 10 + 3 * 6) + 6 = 90
		Assert.Equal(90, geometry.SectionHeight(1));

		Assert.Equal(106 + 6 + 90, geometry.ContentHeight);
		Assert.Equal(0, geometry.SectionTop(0));
		Assert.Equal(112, geometry.SectionTop(1));
	}

	[Fact]
	public void ItemFrame_PlacesColumnsAndRows()
	{
		FixedGridGeometry geometry = Build([31, 28]);
		double itemWidth = geometry.ItemWidth;
		Rect first = geometry.ItemFrame(1, 0);
		Rect second = geometry.ItemFrame(1, 1);
		Rect nextRow = geometry.ItemFrame(1, 7);

		Assert.Equal(6, first.X);
		Assert.Equal(112 + 6 + 20, first.Y);
		Assert.Equal(6 + itemWidth + 6, second.X);
		Assert.Equal(first.Y + 10 + 6, nextRow.Y);
	}

	[Fact]
	public void HeaderAndFooterFrames_UseTheSectionInsets()
	{
		FixedGridGeometry geometry = Build([31], headerHeight: 20, footerHeight: 15);

		Rect header = geometry.HeaderFrame(0);
		Rect footer = geometry.FooterFrame(0, 31);

		Assert.Equal(6, header.Y);
		Assert.Equal(20, header.Height);
		Assert.Equal(6 + 20 + 5 * 10 + 4 * 6, footer.Y);
		Assert.Equal(15, footer.Height);
	}

	[Fact]
	public void OuterInsets_MoveTheFirstHeaderAndTheContentHeight()
	{
		FixedGridGeometry geometry = Build([31], insets: new Thickness(10, 5, 20, 30));

		Assert.Equal(5 + 6, geometry.HeaderFrame(0).Y);
		Assert.Equal(6 + 20 + 74 + 6 + 5 + 30, geometry.ContentHeight);
	}

	[Fact]
	public void VisibleSections_FindsOnlyTheIntersectingRange()
	{
		FixedGridGeometry geometry = Build([31, 28]);

		Assert.True(geometry.VisibleSections(0, 100, out int first, out int last));
		Assert.Equal(0, first);
		Assert.Equal(1, last);

		Assert.True(geometry.VisibleSections(113, 200, out first, out last));
		Assert.Equal(1, first);
		Assert.Equal(2, last);

		Assert.False(geometry.VisibleSections(202, 300, out _, out _));
	}

	[Fact]
	public void Build_ReadsEachSectionCountOneTime()
	{
		int calls = 0;
		FixedGridGeometry geometry = new(
			3,
			section =>
			{
				calls++;
				return 30;
			},
			7,
			6,
			10,
			20,
			0,
			Thickness.Zero,
			100);

		Assert.Equal(3, calls);

		geometry.ItemFrame(2, 4);
		geometry.VisibleSections(0, 5000, out _, out _);
		geometry.HeaderFrame(1);
		geometry.FooterFrame(2, 30);

		Assert.Equal(3, calls);
	}

	[Fact]
	public void EmptySection_KeepsTheHeaderTheFooterAndTheSpacing()
	{
		FixedGridGeometry geometry = Build([0], headerHeight: 20, footerHeight: 15);

		Assert.Equal(6 + 20 + 15 + 6, geometry.SectionHeight(0));
	}

	[Fact]
	public void NoSections_HasNoHeight()
	{
		FixedGridGeometry geometry = Build([]);

		Assert.Equal(0, geometry.ContentHeight);
	}

	[Fact]
	public void ItemWidth_SubtractsTheInsetsAndTheGaps()
	{
		FixedGridGeometry geometry = Build([31], columns: 7, spacing: 6, insets: new Thickness(10, 0, 20, 0), width: 390);

		Assert.Equal((390 - 30 - 6 * 8) / 7.0, geometry.ItemWidth, 6);
	}

	[Fact]
	public void LargeSource_BuildsAndFindsASmallVisibleRange()
	{
		int[] counts = new int[24_309];
		Array.Fill(counts, 35);

		FixedGridGeometry geometry = Build(counts, columns: 7, spacing: 6, itemHeight: 46, headerHeight: 44, width: 390);

		Assert.Equal(24_309, geometry.SectionCount);
		Assert.True(geometry.ContentHeight > 7_000_000);

		double top = geometry.SectionTop(24_300);

		Assert.True(geometry.VisibleSections(top + 1, top + 400, out int first, out int last));
		Assert.InRange(first, 24_295, 24_300);
		Assert.InRange(last - first, 1, 4);
	}
}
