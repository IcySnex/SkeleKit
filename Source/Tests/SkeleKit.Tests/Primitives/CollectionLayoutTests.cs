using Xunit;

namespace SkeleKit.Tests.Primitives;

public class CollectionLayoutTests
{
	[Fact]
	public void List_Plain_IsNotGrouped()
	{
		CollectionLayout layout = CollectionLayout.List();

		Assert.Equal(CollectionLayoutKind.List, layout.Kind);
		Assert.False(layout.Grouped);
	}

	[Fact]
	public void List_Grouped_UsesInsetGrouped()
	{
		CollectionLayout layout = CollectionLayout.List(grouped: true);

		Assert.True(layout.Grouped);
	}

	[Fact]
	public void Grid_KeepsColumnsAndSpacing()
	{
		CollectionLayout layout = CollectionLayout.Grid(columns: 3, spacing: 12);

		Assert.Equal(CollectionLayoutKind.Grid, layout.Kind);
		Assert.Equal(3, layout.Columns);
		Assert.Equal(12, layout.Spacing);
		Assert.Null(layout.ItemAspectRatio);
	}

	[Fact]
	public void Grid_KeepsItemAspectRatio()
	{
		CollectionLayout layout = CollectionLayout.Grid(columns: 3, itemAspectRatio: 16.0 / 9.0);

		Assert.Equal(16.0 / 9.0, layout.ItemAspectRatio);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	public void Grid_RejectsInvalidItemAspectRatio(
		double itemAspectRatio) =>
		Assert.Throws<ArgumentOutOfRangeException>(() => CollectionLayout.Grid(3, itemAspectRatio: itemAspectRatio));

	[Theory]
	[InlineData(0)]
	[InlineData(-4)]
	public void Grid_ClampsColumnsToAtLeastOne(
		int columns)
	{
		CollectionLayout layout = CollectionLayout.Grid(columns);

		Assert.Equal(1, layout.Columns);
	}

	[Fact]
	public void Carousel_KeepsItemWidthAndSnap()
	{
		CollectionLayout layout = CollectionLayout.Carousel(itemWidth: 130, spacing: 8, snap: CarouselSnap.ItemCentered);

		Assert.Equal(CollectionLayoutKind.Carousel, layout.Kind);
		Assert.Equal(130, layout.ItemWidth);
		Assert.Equal(CarouselSnap.ItemCentered, layout.Snap);
	}

	[Fact]
	public void Carousel_DefaultsToFreeScrolling()
	{
		CollectionLayout layout = CollectionLayout.Carousel(itemWidth: 100);

		Assert.Equal(CarouselSnap.None, layout.Snap);
	}

	[Fact]
	public void Carousel_KeepsLeadingBoundaryPeek()
	{
		CollectionLayout layout = CollectionLayout.Carousel(itemWidth: 120, spacing: 10, snap: CarouselSnap.LeadingBoundaryPeek);

		Assert.Equal(CarouselSnap.LeadingBoundaryPeek, layout.Snap);
	}

	[Fact]
	public void FixedGrid_MarksFixedGeometry()
	{
		CollectionLayout layout = CollectionLayout.FixedGrid(columns: 7, spacing: 6, itemAspectRatio: 1);

		Assert.Equal(CollectionLayoutKind.Grid, layout.Kind);
		Assert.True(layout.IsFixedGeometry);
		Assert.Equal(7, layout.Columns);
		Assert.Equal(6, layout.Spacing);
		Assert.Equal(1, layout.ItemAspectRatio);
	}

	[Fact]
	public void Grid_IsNotFixedGeometry()
	{
		CollectionLayout layout = CollectionLayout.Grid(columns: 3);

		Assert.False(layout.IsFixedGeometry);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	public void FixedGrid_RejectsInvalidItemAspectRatio(
		double itemAspectRatio) =>
		Assert.Throws<ArgumentOutOfRangeException>(() => CollectionLayout.FixedGrid(7, itemAspectRatio: itemAspectRatio));

	[Theory]
	[InlineData(0)]
	[InlineData(-4)]
	public void FixedGrid_ClampsColumnsToAtLeastOne(
		int columns)
	{
		CollectionLayout layout = CollectionLayout.FixedGrid(columns);

		Assert.Equal(1, layout.Columns);
	}

	[Fact]
	public void Section_IsWhateverTheAppModelSays()
	{
		ISection<string> section = new Group("General", "settings", ["Appearance", "Language"]);

		Assert.Equal(2, section.Items.Count);
		Assert.Equal("Appearance", section.Items[0]);
	}

	record Group(
		string Title,
		string Icon,
		IReadOnlyList<string> Items) : ISection<string>;
}
