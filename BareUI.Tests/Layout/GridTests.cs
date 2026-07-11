using BareUI;
using BareUI.Tests.Elements;
using Xunit;

namespace BareUI.Tests.Layout;

public class GridTests
{
	[Fact]
	public void AutoAndStar_Columns_SizeAndPlace()
	{
		StubLeaf poster = new(80, 40);
		StubLeaf info = new(50, 40);
		Grid grid = new()
		{
			Columns = { GridLength.Auto, GridLength.Star },
			Children = { poster.Column(0), info.Column(1) }
		};

		grid.Measure(new Size(300, 200));
		grid.Arrange(new Rect(0, 0, 300, 200));

		// auto column fits poster (80); star column takes the rest (220)
		Assert.Equal(new Rect(0, 0, 80, 200), poster.ArrangedBounds);
		Assert.Equal(new Rect(80, 0, 220, 200), info.ArrangedBounds);
	}

	[Fact]
	public void FixedPlusStars_SplitRemainderByWeight()
	{
		StubLeaf a = new(10, 10);
		StubLeaf b = new(10, 10);
		StubLeaf c = new(10, 10);
		Grid grid = new()
		{
			Columns = { 100, GridLength.Star, GridLength.Star },
			Children = { a.Column(0), b.Column(1), c.Column(2) }
		};

		grid.Measure(new Size(300, 100));
		grid.Arrange(new Rect(0, 0, 300, 100));

		Assert.Equal(0, a.ArrangedBounds.X);
		Assert.Equal(100, a.ArrangedBounds.Width);
		Assert.Equal(new Rect(100, 0, 100, 100), b.ArrangedBounds);
		Assert.Equal(new Rect(200, 0, 100, 100), c.ArrangedBounds);
	}

	[Fact]
	public void AutoRows_SumChildHeights()
	{
		Grid grid = new()
		{
			Rows = { GridLength.Auto, GridLength.Auto },
			Children = { new StubLeaf(100, 40).Row(0), new StubLeaf(100, 20).Row(1) }
		};

		grid.Measure(Size.Infinity);

		Assert.Equal(60, grid.DesiredSize.Height);
	}

	[Fact]
	public void ColumnSpan_CoversMultipleTracks()
	{
		StubLeaf wide = new(10, 10);
		Grid grid = new()
		{
			Columns = { 100, 100 },
			ColumnSpacing = 20,
			Children = { wide.Column(0).ColumnSpan(2) }
		};

		grid.Measure(new Size(220, 100));
		grid.Arrange(new Rect(0, 0, 220, 100));

		// two 100 tracks + 20 gap between them
		Assert.Equal(new Rect(0, 0, 220, 100), wide.ArrangedBounds);
	}

	[Fact]
	public void ColumnSpacing_OffsetsSecondColumn()
	{
		StubLeaf a = new(10, 10);
		StubLeaf b = new(10, 10);
		Grid grid = new()
		{
			Columns = { 100, 100 },
			ColumnSpacing = 30,
			Children = { a.Column(0), b.Column(1) }
		};

		grid.Measure(new Size(230, 50));
		grid.Arrange(new Rect(0, 0, 230, 50));

		Assert.Equal(0, a.ArrangedBounds.X);
		Assert.Equal(130, b.ArrangedBounds.X);
	}
}
