
namespace BareUI;

/// <summary>
/// Wraps a single child with padding and an optional stroke; also the generic padding container.
/// </summary>
public class Border : Panel
{
	/// <summary>
	/// Empty space between the border edge and the child.
	/// </summary>
	public Thickness Padding { get; set; } = Thickness.Zero;

	/// <summary>
	/// The stroke color, or null (default) for no stroke.
	/// </summary>
	public Color? Stroke { get; set; }

	/// <summary>
	/// The stroke width in points. Also insets the child so the stroke never overlaps content.
	/// </summary>
	public double StrokeThickness { get; set; } = 0;


	/// <summary>
	/// The single wrapped child. Setting it replaces any previous child.
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


	Thickness Inset =>
		new(
			Padding.Left + StrokeThickness,
			Padding.Top + StrokeThickness,
			Padding.Right + StrokeThickness,
			Padding.Bottom + StrokeThickness);


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

#if IOS
	private protected override void OnRealized()
	{
		base.OnRealized();

		if (Stroke is { } stroke && StrokeThickness > 0)
		{
			Native.Layer.BorderWidth = (nfloat)StrokeThickness;
			Native.Layer.BorderColor = stroke.ToUIColor().CGColor;
		}
	}
#endif
}
