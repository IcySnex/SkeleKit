namespace SkeleKit;

/// <summary>
/// A z-stack: children are drawn atop one another, each given full bounds and placed by its own alignment.
/// </summary>
public class Overlay : Panel
{
	/// <inheritdoc/>
	protected override Size MeasureOverride(
		Size availableSize)
	{
		Thickness insets = ContentInsets;
		Size inner = availableSize.Deflate(insets);

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

		return new Size(width, height).Inflate(insets);
	}

	/// <inheritdoc/>
	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Thickness insets = ContentInsets;
		Rect bounds = new(new(insets.Left, insets.Top), finalSize.Deflate(insets));

		foreach (View child in Children)
			child.Arrange(bounds);

		return finalSize;
	}
}
