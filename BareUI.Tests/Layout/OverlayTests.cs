using BareUI;
using BareUI.Primitives;
using BareUI.Tests.Elements;
using Xunit;

namespace BareUI.Tests.Layout;

public class OverlayTests
{
	[Fact]
	public void Measure_TakesLargestChild()
	{
		Overlay overlay = new()
		{
			Children = { new StubLeaf(100, 40), new StubLeaf(60, 120) }
		};

		overlay.Measure(Size.Infinity);

		Assert.Equal(new Size(100, 120), overlay.DesiredSize);
	}

	[Fact]
	public void Arrange_GivesEachChildFullBounds_AlignmentPositions()
	{
		StubLeaf backdrop = new(300, 200);
		StubLeaf badge = new(40, 40)
		{
			HorizontalAlignment = HorizontalAlignment.End,
			VerticalAlignment = VerticalAlignment.Start
		};
		Overlay overlay = new() { Children = { backdrop, badge } };

		overlay.Measure(new Size(300, 200));
		overlay.Arrange(new Rect(0, 0, 300, 200));

		Assert.Equal(new Rect(0, 0, 300, 200), backdrop.ArrangedBounds);
		Assert.Equal(new Rect(260, 0, 40, 40), badge.ArrangedBounds);
	}
}
