namespace SkeleKit.Gallery.Views.Showcase;

internal sealed class ShowcaseContent : Panel
{
	View selected;


	public ShowcaseContent(
		View preview,
		View code)
	{
		selected = preview;

		Children.Add(preview);
		Children.Add(code);
	}


	public void Select(
		View view)
	{
		if (ReferenceEquals(selected, view))
			return;

		selected = view;
		InvalidateMeasure();
	}


	protected override Size MeasureOverride(
		Size availableSize)
	{
		foreach (View child in Children)
			child.Measure(availableSize);

		return selected.DesiredSize;
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
