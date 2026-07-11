#if IOS
using CoreGraphics;
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// Base of every BareUI element: owns one lazily-created native view and takes part in the measure/arrange engine.
/// </summary>
public abstract class View
{
	// Layout properties

	/// <summary>
	/// Empty space around the view, outside its bounds.
	/// </summary>
	public Thickness Margin { get; set; } = Thickness.Zero;

	/// <summary>
	/// Explicit width in points, or NaN to size to content.
	/// </summary>
	public double Width { get; set; } = double.NaN;

	/// <summary>
	/// Explicit height in points, or NaN to size to content.
	/// </summary>
	public double Height { get; set; } = double.NaN;

	/// <summary>
	/// Minimum width in points.
	/// </summary>
	public double MinWidth { get; set; } = 0;

	/// <summary>
	/// Maximum width in points.
	/// </summary>
	public double MaxWidth { get; set; } = double.PositiveInfinity;

	/// <summary>
	/// Minimum height in points.
	/// </summary>
	public double MinHeight { get; set; } = 0;

	/// <summary>
	/// Maximum height in points.
	/// </summary>
	public double MaxHeight { get; set; } = double.PositiveInfinity;

	/// <summary>
	/// How the view is placed within the horizontal space its parent gives it.
	/// </summary>
	public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Stretch;

	/// <summary>
	/// How the view is placed within the vertical space its parent gives it.
	/// </summary>
	public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Stretch;

	/// <summary>
	/// When false the view takes no space and is hidden natively.
	/// </summary>
	public bool IsVisible { get; set; } = true;


	// Visual properties

	/// <summary>
	/// Solid background color, or null for transparent.
	/// </summary>
	public Color? Background { get; set; }

	/// <summary>
	/// Opacity from 0 (transparent) to 1 (opaque).
	/// </summary>
	public double Opacity { get; set; } = 1.0;

	/// <summary>
	/// Corner radius in points applied to the layer.
	/// </summary>
	public double CornerRadius { get; set; } = 0;

	/// <summary>
	/// When true, content is clipped to the bounds and corner radius.
	/// </summary>
	public bool ClipsToBounds { get; set; }


	/// <summary>
	/// Per-child data written by a parent panel (e.g. a Grid stores row/column here).
	/// </summary>
	internal object? LayoutParams { get; set; }


	// Layout results

	/// <summary>
	/// Size requested by the last measure pass, including <see cref="Margin"/>.
	/// </summary>
	public Size DesiredSize { get; private set; }

	/// <summary>
	/// Frame from the last arrange pass, in the parent's coordinates (margin excluded).
	/// </summary>
	public Rect ArrangedBounds { get; private set; }


	/// <summary>
	/// First pass: computes <see cref="DesiredSize"/> for the space the parent offers.
	/// </summary>
	public void Measure(
		Size available)
	{
		if (!IsVisible)
		{
			DesiredSize = Size.Zero;
			return;
		}

		Size slot = available.Deflate(Margin);

		(double minWidth, double maxWidth) = MinMax(Width, MinWidth, MaxWidth);
		(double minHeight, double maxHeight) = MinMax(Height, MinHeight, MaxHeight);

		Size contentAvailable = new(
			Clamp(slot.Width, minWidth, maxWidth),
			Clamp(slot.Height, minHeight, maxHeight));

		Size measured = MeasureOverride(contentAvailable);

		Size desired = new(
			Clamp(measured.Width, minWidth, maxWidth),
			Clamp(measured.Height, minHeight, maxHeight));

		DesiredSize = desired.Inflate(Margin);
	}

	/// <summary>
	/// Second pass: positions the view within its slot, honouring margin and alignment.
	/// </summary>
	public void Arrange(
		Rect finalRect)
	{
		if (!IsVisible)
		{
			ArrangedBounds = new(finalRect.Location, Size.Zero);
			ApplyFrame(ArrangedBounds);
			return;
		}

		Rect slot = finalRect.Deflate(Margin);

		(double minWidth, double maxWidth) = MinMax(Width, MinWidth, MaxWidth);
		(double minHeight, double maxHeight) = MinMax(Height, MinHeight, MaxHeight);

		Size desired = DesiredSize.Deflate(Margin);

		double width = HorizontalAlignment == HorizontalAlignment.Stretch
			? slot.Width
			: Math.Min(desired.Width, slot.Width);
		double height = VerticalAlignment == VerticalAlignment.Stretch
			? slot.Height
			: Math.Min(desired.Height, slot.Height);

		width = Clamp(width, minWidth, maxWidth);
		height = Clamp(height, minHeight, maxHeight);

		ArrangeOverride(new(width, height));

		double x = slot.X + HorizontalOffset(slot.Width, width);
		double y = slot.Y + VerticalOffset(slot.Height, height);

		ArrangedBounds = new(x, y, width, height);
		ApplyFrame(ArrangedBounds);
	}


	/// <summary>
	/// Content measurement. Panels recurse; controls delegate to the native SizeThatFits.
	/// </summary>
	protected virtual Size MeasureOverride(
		Size availableSize) =>
		Size.Zero;

	/// <summary>
	/// Content arrangement. Panels override to place their children.
	/// </summary>
	protected virtual Size ArrangeOverride(
		Size finalSize) =>
		finalSize;


	// Host hooks: the native LayoutHost drives these on the element it hosts.

	internal Size HostMeasure(
		Size available) =>
		MeasureOverride(available);

	internal void HostLayout(
		Size bounds)
	{
		MeasureOverride(bounds);
		ArrangeOverride(bounds);
	}


	static double Clamp(
		double value,
		double min,
		double max) =>
		Math.Max(min, Math.Min(value, max));

	// Folds an explicit length into the min/max window (WPF semantics).
	static (double Min, double Max) MinMax(
		double explicitLength,
		double min,
		double max)
	{
		double high = double.IsNaN(explicitLength) ? double.PositiveInfinity : explicitLength;
		double low = double.IsNaN(explicitLength) ? 0 : explicitLength;

		return (
			Math.Max(Math.Min(low, max), min),
			Math.Max(Math.Min(high, max), min));
	}

	double HorizontalOffset(
		double slotWidth,
		double childWidth) =>
		HorizontalAlignment switch
		{
			HorizontalAlignment.Center => (slotWidth - childWidth) / 2,
			HorizontalAlignment.End => slotWidth - childWidth,
			_ => 0
		};

	double VerticalOffset(
		double slotHeight,
		double childHeight) =>
		VerticalAlignment switch
		{
			VerticalAlignment.Center => (slotHeight - childHeight) / 2,
			VerticalAlignment.End => slotHeight - childHeight,
			_ => 0
		};

#if IOS
	UIView? native;

	/// <summary>
	/// The underlying UIKit view, created on first access. Primary escape hatch to UIKit.
	/// </summary>
	public UIView Native =>
		native ??= RealizeCore();

	/// <summary>
	/// Whether the native view has been created yet.
	/// </summary>
	public bool IsRealized =>
		native is not null;

	/// <summary>
	/// Creates the native view. Panels return a LayoutHost; controls return their control.
	/// </summary>
	private protected abstract UIView CreateNative();

	/// <summary>
	/// Whether Unrealize disposes the native view. False for wrappers around caller-owned views.
	/// </summary>
	private protected virtual bool OwnsNative =>
		true;

	/// <summary>
	/// Builds the native view (if needed) and realizes children and bindings.
	/// </summary>
	public UIView Realize() =>
		Native;

	UIView RealizeCore()
	{
		native = CreateNative();
		native.TranslatesAutoresizingMaskIntoConstraints = true;

		ApplyVisualState();
		OnRealized();

		return native;
	}

	/// <summary>
	/// Tears down the native view and children deterministically (no finalizers).
	/// </summary>
	public void Unrealize()
	{
		if (native is null)
			return;

		OnUnrealized();

		native.RemoveFromSuperview();
		if (OwnsNative)
			native.Dispose();
		native = null;
	}

	private protected virtual void OnRealized()
	{ }

	private protected virtual void OnUnrealized()
	{ }

	private protected void ApplyVisualState()
	{
		if (native is null)
			return;

		native.Hidden = !IsVisible;
		native.Alpha = (nfloat)Opacity;
		if (Background is { } background)
			native.BackgroundColor = background.ToUIColor();
		native.ClipsToBounds = ClipsToBounds || CornerRadius > 0;
		native.Layer.CornerRadius = (nfloat)CornerRadius;
	}
#endif

	void ApplyFrame(
		Rect frame)
	{
#if IOS
		if (native is not null)
			native.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
#endif
	}
}
