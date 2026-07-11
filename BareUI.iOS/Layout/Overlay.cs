
namespace BareUI;

/// <summary>
/// A z-stack: children are drawn atop one another, each given full bounds and placed by its own alignment.
/// </summary>
public class Overlay : Panel
{
	protected override Size MeasureOverride(
		Size availableSize)
	{
		Size inner = availableSize.Deflate(Padding);

		double width = 0;
		double height = 0;

		foreach (View child in Children)
		{
			child.Measure(inner);

			if (!child.IsVisible.Value)
				continue;

			width = Math.Max(width, child.DesiredSize.Width);
			height = Math.Max(height, child.DesiredSize.Height);
		}

		return new Size(width, height).Inflate(Padding);
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Rect bounds = new(
			new Point(Padding.Left, Padding.Top),
			finalSize.Deflate(Padding));

		foreach (View child in Children)
			child.Arrange(bounds);

		return finalSize;
	}
}
