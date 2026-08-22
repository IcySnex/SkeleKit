using SkeleKit.Tests.Elements;
using Xunit;

namespace SkeleKit.Tests.Layout;

public class LayoutInvalidationTests
{
	[Fact]
	public void StackPanel_SpacingChange_InvalidatesMeasurement()
	{
		StackPanel stack = new()
		{
			Children = { new StubLeaf(10, 10), new StubLeaf(10, 10) }
		};
		stack.Measure(Size.Infinity);

		stack.Spacing = 20;
		stack.Measure(Size.Infinity);

		Assert.Equal(40, stack.DesiredSize.Height);
	}

	[Fact]
	public void StackPanel_OrientationChange_InvalidatesMeasurement()
	{
		StackPanel stack = new()
		{
			Children = { new StubLeaf(40, 10), new StubLeaf(20, 20) }
		};
		stack.Measure(Size.Infinity);

		stack.Orientation = Orientation.Horizontal;
		stack.Measure(Size.Infinity);

		Assert.Equal(new Size(60, 20), stack.DesiredSize);
	}

	[Fact]
	public void ScrollView_OrientationChange_InvalidatesMeasurement()
	{
		ScrollView scroll = new()
		{
			Content = new StubLeaf(40, 20)
		};
		scroll.Measure(new(100, 80));

		scroll.Orientation = Orientation.Horizontal;
		scroll.Measure(new(100, 80));

		Assert.Equal(new Size(40, 80), scroll.DesiredSize);
	}

	[Fact]
	public void Border_StrokeThicknessChange_InvalidatesMeasurement()
	{
		Border border = new()
		{
			Child = new StubLeaf(10, 10)
		};
		border.Measure(Size.Infinity);

		border.StrokeThickness = 5;
		border.Measure(Size.Infinity);

		Assert.Equal(new Size(20, 20), border.DesiredSize);
	}

	[Fact]
	public void Grid_TrackMutation_InvalidatesMeasurement()
	{
		Grid grid = new()
		{
			Rows = { 10 },
			Children = { new StubLeaf(10, 10) }
		};
		grid.Measure(Size.Infinity);

		grid.Rows.Add(20);
		grid.Measure(Size.Infinity);

		Assert.Equal(30, grid.DesiredSize.Height);
	}

	[Fact]
	public void Grid_SpacingChange_InvalidatesMeasurement()
	{
		Grid grid = new()
		{
			Rows = { 10, 10 },
			Children = { new StubLeaf(10, 10) }
		};
		grid.Measure(Size.Infinity);

		grid.RowSpacing = 5;
		grid.Measure(Size.Infinity);

		Assert.Equal(25, grid.DesiredSize.Height);
	}
}
