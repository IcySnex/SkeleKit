namespace SkeleKit;

/// <summary>
/// Wraps a single child with padding and an optional stroke; also the generic padding container.
/// </summary>
public partial class Border : Panel
{
	Thickness Inset => new(Padding.Left + StrokeThickness, Padding.Top + StrokeThickness, Padding.Right + StrokeThickness, Padding.Bottom + StrokeThickness);


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

	/// <summary>
	/// The single wrapped child.
	/// </summary>
	public View? Child
	{
		get => Children.Count > 0 ? Children[0] : null;
		set
		{
			Children.Clear();
			if (value is not null)
				Children.Add(value);
		}
	}

	void ApplyStroke() =>
		ApplyStrokeCore();

	partial void ApplyStrokeCore();


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

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		Child?.Arrange(new Rect(Point.Zero, finalSize).Deflate(Inset));
		return finalSize;
	}
}
