using BareUI.Primitives;

namespace BareUI;

/// <summary>
/// A z-stack: children are drawn atop one another, each given full bounds and placed by its own alignment.
/// </summary>
public class Overlay : Panel
{
	protected override Size MeasureOverride(
		Size availableSize)
	{
		double width = 0;
		double height = 0;

		foreach (View child in Children)
		{
			child.Measure(availableSize);

			if (!child.IsVisible)
				continue;

			width = Math.Max(width, child.DesiredSize.Width);
			height = Math.Max(height, child.DesiredSize.Height);
		}

		return new(width, height);
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Rect bounds = new(Point.Zero, finalSize);

		foreach (View child in Children)
			child.Arrange(bounds);

		return finalSize;
	}
}
