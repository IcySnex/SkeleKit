using System.Collections.Specialized;

namespace SkeleKit;

/// <summary>
/// Wraps a single child with padding and an optional stroke; also the generic padding container.
/// </summary>
public partial class Border : Decorator
{
	bool strokeDashPatternHooked;

	Thickness Inset
	{
		get
		{
			Thickness insets = ContentInsets;
			return new(
				insets.Left + strokeThickness,
				insets.Top + strokeThickness,
				insets.Right + strokeThickness,
				insets.Bottom + strokeThickness);
		}
	}


	/// <summary>
	/// The stroke color, or null (default) for no stroke.
	/// </summary>
	public Bindable<Color?> Stroke
	{
		get => stroke;
		set => strokeBinding = Register(strokeBinding, value, value => Set(ref stroke, value, ApplyStroke, affectsMeasure: false));
	}
	Color? stroke;
	Binding<Color?>? strokeBinding;

	/// <summary>
	/// The stroke width in points.
	/// </summary>
	/// <remarks>
	/// Also insets the child so the stroke never overlaps content.
	/// </remarks>
	public Bindable<double> StrokeThickness
	{
		get => strokeThickness;
		set => strokeThicknessBinding = Register(strokeThicknessBinding, value, value => Set(ref strokeThickness, value, ApplyStroke));
	}
	double strokeThickness;
	Binding<double>? strokeThicknessBinding;

	/// <summary>
	/// Alternating painted and unpainted stroke lengths in points, or null (default) for a solid stroke.
	/// </summary>
	/// <example><c>[1, 5]</c> with a round line cap draws a dotted stroke.</example>
	public BindableList<double> StrokeDashPattern
	{
		get => new(strokeDashPattern);
		set => strokeDashPatternBinding = Register(
			strokeDashPatternBinding,
			value.Expression,
			value.Value,
			SetStrokeDashPattern);
	}
	IReadOnlyList<double>? strokeDashPattern;
	Binding<IReadOnlyList<double>?>? strokeDashPatternBinding;

	void SetStrokeDashPattern(
		IReadOnlyList<double>? value)
	{
		if (ReferenceEquals(strokeDashPattern, value))
			return;

		if (strokeDashPatternHooked && strokeDashPattern is INotifyCollectionChanged old)
			old.CollectionChanged -= OnStrokeDashPatternChanged;

		strokeDashPattern = value;

		if (strokeDashPatternHooked && strokeDashPattern is INotifyCollectionChanged live)
			live.CollectionChanged += OnStrokeDashPatternChanged;

		ApplyStrokeDashPatternCore();
	}

	void HookStrokeDashPattern()
	{
		if (strokeDashPatternHooked)
			return;

		strokeDashPatternHooked = true;
		if (strokeDashPattern is INotifyCollectionChanged live)
			live.CollectionChanged += OnStrokeDashPatternChanged;
	}

	void UnhookStrokeDashPattern()
	{
		if (!strokeDashPatternHooked)
			return;

		if (strokeDashPattern is INotifyCollectionChanged live)
			live.CollectionChanged -= OnStrokeDashPatternChanged;

		strokeDashPatternHooked = false;
	}

	void OnStrokeDashPatternChanged(
		object? sender,
		NotifyCollectionChangedEventArgs args) =>
		ApplyStroke();

	/// <summary>
	/// The shape at the ends of each painted dash. Flat by default.
	/// </summary>
	/// <remarks>
	/// This has no visible effect on a solid, closed stroke.
	/// </remarks>
	public Bindable<StrokeLineCap> StrokeLineCap
	{
		get => strokeLineCap;
		set => strokeLineCapBinding = Register(strokeLineCapBinding, value, value => Set(ref strokeLineCap, value, ApplyStroke, affectsMeasure: false));
	}
	StrokeLineCap strokeLineCap;
	Binding<StrokeLineCap>? strokeLineCapBinding;
	void ApplyStroke() =>
		ApplyStrokeCore();

	partial void ApplyStrokeCore();
	partial void ApplyStrokeDashPatternCore();
	partial void ArrangeStrokeCore(Size finalSize);


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
		ArrangeStrokeCore(finalSize);
		return finalSize;
	}
}
