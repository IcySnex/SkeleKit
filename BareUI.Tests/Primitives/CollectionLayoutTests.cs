using Xunit;

namespace BareUI.Tests.Primitives;

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
	}

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
