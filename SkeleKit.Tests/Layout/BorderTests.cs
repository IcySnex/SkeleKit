using SkeleKit;
using SkeleKit.Tests.Elements;
using Xunit;

namespace SkeleKit.Tests.Layout;

public class BorderTests
{
	[Fact]
	public void Measure_AddsPaddingAndStrokeAroundChild()
	{
		Border border = new()
		{
			Padding = new Thickness(10),
			StrokeThickness = 2,
			Child = new StubLeaf(100, 40)
		};

		border.Measure(Size.Infinity);

		// child 100x40 + padding 10 each side + stroke 2 each side = +24 / +24
		Assert.Equal(new Size(124, 64), border.DesiredSize);
	}

	[Fact]
	public void Arrange_InsetsChildByPaddingAndStroke()
	{
		StubLeaf child = new(100, 40);
		Border border = new()
		{
			Padding = new Thickness(10),
			StrokeThickness = 2,
			Child = child
		};

		border.Measure(new Size(200, 100));
		border.Arrange(new Rect(0, 0, 124, 64));

		Assert.Equal(new Rect(12, 12, 100, 40), child.ArrangedBounds);
	}

	[Fact]
	public void ChildSetter_ReplacesPreviousChild()
	{
		Border border = new() { Child = new StubLeaf(1, 1) };
		StubLeaf replacement = new(50, 50);

		border.Child = replacement;

		Assert.Equal(1, border.Children.Count);
		Assert.Same(replacement, border.Child);
	}
}
