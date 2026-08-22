using SkeleKit.Tests.Elements;
using Xunit;

namespace SkeleKit.Tests.Layout;

public class PaddingTests
{
	[Fact]
	public void StackPanel_Padding_GrowsDesiredSize()
	{
		StackPanel stack = new()
		{
			Padding = new Thickness(10),
			Children = { new StubLeaf(40, 20) }
		};

		stack.Measure(new(200, 200));

		Assert.Equal(60, stack.DesiredSize.Width);
		Assert.Equal(40, stack.DesiredSize.Height);
	}

	[Fact]
	public void StackPanel_Padding_OffsetsChildren()
	{
		StubLeaf leaf = new(40, 20);
		StackPanel stack = new()
		{
			Padding = new Thickness(10, 5),
			Children = { leaf }
		};

		stack.Measure(new(200, 200));
		stack.Arrange(new(0, 0, 200, 200));

		Assert.Equal(10, leaf.ArrangedBounds.X);
		Assert.Equal(5, leaf.ArrangedBounds.Y);
		Assert.Equal(180, leaf.ArrangedBounds.Width);
	}

	[Fact]
	public void StackPanel_Padding_ShrinksChildAvailableWidth()
	{
		StubLeaf leaf = new(40, 20);
		StackPanel stack = new()
		{
			Padding = new Thickness(25),
			Children = { leaf }
		};

		stack.Measure(new(100, 100));
		stack.Arrange(new(0, 0, 100, 100));

		Assert.Equal(50, leaf.ArrangedBounds.Width);
	}

	[Fact]
	public void Overlay_Padding_InsetsChildren()
	{
		StubLeaf leaf = new(10, 10);
		Overlay overlay = new()
		{
			Padding = new Thickness(8),
			Children = { leaf }
		};

		overlay.Measure(new(100, 100));
		overlay.Arrange(new(0, 0, 100, 100));

		Assert.Equal(8, leaf.ArrangedBounds.X);
		Assert.Equal(8, leaf.ArrangedBounds.Y);
		Assert.Equal(84, leaf.ArrangedBounds.Width);
	}

	[Fact]
	public void Grid_Padding_OffsetsCells()
	{
		StubLeaf leaf = new(10, 10);
		Grid grid = new()
		{
			Padding = new Thickness(12),
			Rows = { GridLength.Star },
			Columns = { GridLength.Star },
			Children = { leaf }
		};

		grid.Measure(new(100, 100));
		grid.Arrange(new(0, 0, 100, 100));

		Assert.Equal(12, leaf.ArrangedBounds.X);
		Assert.Equal(12, leaf.ArrangedBounds.Y);
		Assert.Equal(76, leaf.ArrangedBounds.Width);
	}

	[Fact]
	public void ScrollView_Padding_GrowsDesiredSize()
	{
		ScrollView scroll = new()
		{
			Padding = new Thickness(10),
			Content = new StubLeaf(40, 20)
		};

		scroll.Measure(Size.Infinity);

		Assert.Equal(new Size(60, 40), scroll.DesiredSize);
	}

	[Fact]
	public void ContentView_Padding_GrowsAndOffsetsContent()
	{
		StubLeaf leaf = new(40, 20);
		TestPage page = new()
		{
			Padding = new Thickness(10),
			Content = leaf
		};

		page.Measure(Size.Infinity);
		page.Arrange(new(0, 0, 60, 40));

		Assert.Equal(new Size(60, 40), page.DesiredSize);
		Assert.Equal(new Rect(10, 10, 40, 20), leaf.ArrangedBounds);
	}

	[Fact]
	public void Padding_Zero_ChangesNothing()
	{
		StubLeaf leaf = new(40, 20);
		StackPanel stack = new() { Children = { leaf } };

		stack.Measure(new(200, 200));
		stack.Arrange(new(0, 0, 200, 200));

		Assert.Equal(0, leaf.ArrangedBounds.X);
		Assert.Equal(200, leaf.ArrangedBounds.Width);
	}


	sealed class TestPage : ContentView;
}
