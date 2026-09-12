namespace SkeleKit;

/// <summary>
/// Wraps a single child with padding and an optional stroke; also the generic padding container.
/// </summary>
public partial class Border : Decorator
{
	Thickness Inset
	{
		get
		{
			Thickness insets = ContentInsets;
			return new(
				insets.Left + StrokeThickness,
				insets.Top + StrokeThickness,
				insets.Right + StrokeThickness,
				insets.Bottom + StrokeThickness);
		}
	}


	/// <summary>
	/// The stroke color, or null (default) for no stroke.
	/// </summary>
	public Color? Stroke
	{
		get;
		set => Set(ref field, value, ApplyStroke, affectsMeasure: false);
	}

	/// <summary>
	/// The stroke width in points.
	/// </summary>
	/// <remarks>
	/// Also insets the child so the stroke never overlaps content.
	/// </remarks>
	public double StrokeThickness
	{
		get;
		set => Set(ref field, value, ApplyStroke);
	}
	void ApplyStroke() =>
		ApplyStrokeCore();

	partial void ApplyStrokeCore();


	/// <inheritdoc/>
	protected override Size MeasureOverride(
		Size availableSize)
	{
		Thickness inset = Inset;
		View? child = Child;

		if (child is null)
			return new(inset.Horizontal, inset.Vertical);

		child.Measure(availableSize.Deflate(inset));
		return child.DesiredSize.Inflate(inset);
	}

	/// <inheritdoc/>
	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Child?.Arrange(new Rect(Point.Zero, finalSize).Deflate(Inset));
		return finalSize;
	}
}
