using BareUI;
using BareUI.Primitives;
using BareUI.Tests.Elements;
using Xunit;

namespace BareUI.Tests.Layout;

public class StackPanelTests
{
	[Fact]
	public void Vertical_Measure_SumsHeightsMaxWidthPlusSpacing()
	{
		StackPanel stack = new()
		{
			Orientation = Orientation.Vertical,
			Spacing = 10,
			Children = { new StubLeaf(100, 40), new StubLeaf(60, 20) }
		};

		stack.Measure(Size.Infinity);

		// heights 40 + 20 + spacing 10 = 70; width max(100, 60) = 100
		Assert.Equal(new Size(100, 70), stack.DesiredSize);
	}

	[Fact]
	public void Vertical_Arrange_StacksChildrenWithSpacing()
	{
		StubLeaf first = new(100, 40);
		StubLeaf second = new(60, 20);
		StackPanel stack = new()
		{
			Spacing = 10,
			Children = { first, second }
		};

		stack.Measure(new Size(200, 500));
		stack.Arrange(new Rect(0, 0, 200, 500));

		Assert.Equal(new Rect(0, 0, 200, 40), first.ArrangedBounds);
		Assert.Equal(new Rect(0, 50, 200, 20), second.ArrangedBounds);
	}

	[Fact]
	public void Horizontal_Measure_SumsWidths()
	{
		StackPanel stack = new()
		{
			Orientation = Orientation.Horizontal,
			Spacing = 5,
			Children = { new StubLeaf(30, 40), new StubLeaf(20, 60) }
		};

		stack.Measure(Size.Infinity);

		// widths 30 + 20 + spacing 5 = 55; height max(40, 60) = 60
		Assert.Equal(new Size(55, 60), stack.DesiredSize);
	}

	[Fact]
	public void InvisibleChild_TakesNoSpaceOrSpacing()
	{
		StackPanel stack = new()
		{
			Spacing = 10,
			Children =
			{
				new StubLeaf(100, 40),
				new StubLeaf(100, 999) { IsVisible = false },
				new StubLeaf(100, 20)
			}
		};

		stack.Measure(Size.Infinity);

		// only two visible: 40 + 20 + one gap 10 = 70
		Assert.Equal(70, stack.DesiredSize.Height);
	}
}
