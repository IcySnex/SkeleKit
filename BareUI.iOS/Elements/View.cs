using System.Windows.Input;

namespace BareUI;

/// <summary>
/// Base of every BareUI element: owns one lazily-created native view and takes part in the measure/arrange engine.
/// </summary>
public abstract partial class View
{
	/// <summary>
	/// Applies the app's implicit theme styles for this view's type.
	/// </summary>
	protected View() =>
		Theme.ApplyTo(this);


	/// <summary>
	/// A style applied the moment it is assigned: put it first in an object initializer, later assignments override it.
	/// </summary>
	public IStyle? Style
	{
		get => style;
		set
		{
			style = value;
			value?.Apply(this);
		}
	}
	IStyle? style;


	/// <summary>
	/// The panel this view sits in, or null when it is a root or unparented.
	/// </summary>
	public View? Parent { get; private set; }

	internal void SetParent(
		View? parent)
	{
		Parent = parent;
		OnBindingContextChanged();
	}


	readonly List<BindingBase> bindings = [];

	object? bindingContext;

	/// <summary>
	/// The object bindings resolve against. Inherited from the parent unless set explicitly.
	/// </summary>
	public object? BindingContext
	{
		get => bindingContext ?? Parent?.BindingContext;
		set
		{
			if (ReferenceEquals(bindingContext, value))
				return;

			bindingContext = value;
			OnBindingContextChanged();
		}
	}

	internal void OnBindingContextChanged()
	{
		object? context = BindingContext;

		foreach (BindingBase binding in bindings)
			binding.Attach(context);

		PropagateBindingContext();
	}

	private protected virtual void PropagateBindingContext()
	{ }

	// literal -> apply now; expression -> keep a live binding the context can feed
	private protected Binding<T>? Register<T>(
		Binding<T>? existing,
		Bindable<T> value,
		Action<T?> apply)
	{
		if (existing is not null)
		{
			existing.Detach();
			bindings.Remove(existing);
		}

		if (value.Expression is not { } expression)
		{
			apply(value.Value);
			return null;
		}

		Binding<T> binding = new(expression, apply);
		bindings.Add(binding);
		binding.Attach(BindingContext);

		return binding;
	}

	private protected void DetachBindings()
	{
		foreach (BindingBase binding in bindings)
			binding.Detach();
	}

	/// <summary>
	/// Drops the cached measurement here and up the tree, and asks the root host for a layout pass.
	/// </summary>
	public void InvalidateMeasure()
	{
		View view = this;

		while (true)
		{
			view.measureValid = false;
			view.previousValid = false;

			if (view.Parent is not { } parent)
				break;

			// an already-stale parent has stale ancestors too
			if (!parent.measureValid)
			{
				view = Root();
				break;
			}

			view = parent;
		}

		view.RequestLayout();
	}

	View Root()
	{
		View root = this;
		while (root.Parent is { } parent)
			root = parent;

		return root;
	}

	/// <summary>
	/// Drops every cached measurement in this subtree. For changes that hit leaves directly, like a dynamic-type resize.
	/// </summary>
	public void InvalidateSubtree()
	{
		measureValid = false;
		previousValid = false;

		InvalidateChildren();
		InvalidateMeasure();
	}

	private protected virtual void InvalidateChildren()
	{ }

	partial void RequestLayout();

	// stores a property, pushes it to the native view once realized, and relayouts if it can change size
	private protected void Set<T>(
		ref T field,
		T value,
		Action? apply = null,
		bool affectsMeasure = true)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return;

		field = value;

		if (apply is not null)
			ApplyIfRealized(apply);

		if (affectsMeasure)
			InvalidateMeasure();
	}

	partial void ApplyIfRealized(
		Action apply);

	// scroll views clip by default in UIKit; forcing ClipsToBounds=false would let their content
	// paint over everything around them
	private protected virtual bool ClipsByDefault =>
		false;

	// a scrolling view manages its own content insets, and a page lets it slide under the bars
	internal virtual bool Scrolls =>
		false;

	private protected void ApplyVisualState() =>
		ApplyVisualStateCore();

	partial void ApplyVisualStateCore();

	// theme change: dynamic UIColors adapt on their own, but CGColor snapshots (borders, shadows) do not
	internal virtual void ReapplyVisuals() =>
		ApplyVisualState();

	private protected void ApplyInteraction() =>
		ApplyInteractionCore();

	partial void ApplyInteractionCore();


	// Interaction

	/// <summary>
	/// Whether the view responds to touches.
	/// </summary>
	public bool IsEnabled
	{
		get => isEnabled;
		set => Set(ref isEnabled, value, ApplyInteraction, affectsMeasure: false);
	}
	bool isEnabled = true;

	/// <summary>
	/// Command invoked when the view is tapped.
	/// </summary>
	public Bindable<ICommand?> TapCommand
	{
		get => Bindable.From<ICommand?>(tapCommand);
		set => tapCommandBinding = Register(tapCommandBinding, value, value => Set(ref tapCommand, value, ApplyInteraction, affectsMeasure: false));
	}
	ICommand? tapCommand;
	Binding<ICommand?>? tapCommandBinding;

	/// <summary>
	/// The parameter passed to <see cref="TapCommand"/>.
	/// </summary>
	public object? TapCommandParameter { get; set; }

	// Accessibility

	/// <summary>
	/// Text VoiceOver reads for the view, or null for the control's own default.
	/// </summary>
	public Bindable<string?> AccessibilityLabel
	{
		get => accessibilityLabel;
		set => accessibilityLabelBinding = Register(accessibilityLabelBinding, value, value => Set(ref accessibilityLabel, value, ApplyAccessibility, affectsMeasure: false));
	}
	string? accessibilityLabel;
	Binding<string?>? accessibilityLabelBinding;

	/// <summary>
	/// Extra VoiceOver context describing what activating the view does, or null for none.
	/// </summary>
	public string? AccessibilityHint
	{
		get => accessibilityHint;
		set => Set(ref accessibilityHint, value, ApplyAccessibility, affectsMeasure: false);
	}
	string? accessibilityHint;

	/// <summary>
	/// The current value VoiceOver reads after the label (a slider's percentage), or null for the control's own default.
	/// </summary>
	public Bindable<string?> AccessibilityValue
	{
		get => accessibilityValue;
		set => accessibilityValueBinding = Register(accessibilityValueBinding, value, value => Set(ref accessibilityValue, value, ApplyAccessibility, affectsMeasure: false));
	}
	string? accessibilityValue;
	Binding<string?>? accessibilityValueBinding;

	/// <summary>
	/// Extra traits VoiceOver applies on top of the control's own (Header, Selected, ...).
	/// </summary>
	public AccessibilityTraits AccessibilityTraits
	{
		get => accessibilityTraits;
		set => Set(ref accessibilityTraits, value, ApplyAccessibility, affectsMeasure: false);
	}
	AccessibilityTraits accessibilityTraits;

	/// <summary>
	/// Identifier for UI tests. Never read to the user.
	/// </summary>
	public string? AccessibilityIdentifier
	{
		get => accessibilityIdentifier;
		set => Set(ref accessibilityIdentifier, value, ApplyAccessibility, affectsMeasure: false);
	}
	string? accessibilityIdentifier;

	/// <summary>
	/// Overrides whether VoiceOver treats the view as one element, or null for the control's own default.
	/// </summary>
	public bool? IsAccessibilityElement
	{
		get => isAccessibilityElement;
		set => Set(ref isAccessibilityElement, value, ApplyAccessibility, affectsMeasure: false);
	}
	bool? isAccessibilityElement;

	private protected void ApplyAccessibility() =>
		ApplyAccessibilityCore();

	partial void ApplyAccessibilityCore();


	/// <summary>
	/// Gives the view keyboard focus, raising the keyboard for a text control. No-op until realized.
	/// </summary>
	public void Focus() =>
		FocusCore();

	/// <summary>
	/// Takes keyboard focus away, dismissing the keyboard.
	/// </summary>
	public void Unfocus() =>
		UnfocusCore();

	partial void FocusCore();

	partial void UnfocusCore();


	// Layout properties

	/// <summary>
	/// Edges this view is allowed to extend past the safe area. A scrolling view still keeps its content inside it, so only the scroll passes under the bar.
	/// </summary>
	public SafeAreaEdges IgnoresSafeArea
	{
		get => ignoresSafeArea;
		set => Set(ref ignoresSafeArea, value);
	}
	SafeAreaEdges ignoresSafeArea = SafeAreaEdges.None;

	/// <summary>
	/// Empty space around the view, outside its bounds.
	/// </summary>
	public Thickness Margin
	{
		get => margin;
		set => Set(ref margin, value);
	}
	Thickness margin = Thickness.Zero;

	/// <summary>
	/// Explicit width in points, or NaN to size to content.
	/// </summary>
	public double Width
	{
		get => width;
		set => Set(ref width, value);
	}
	double width = double.NaN;

	/// <summary>
	/// Explicit height in points, or NaN to size to content.
	/// </summary>
	public double Height
	{
		get => height;
		set => Set(ref height, value);
	}
	double height = double.NaN;

	/// <summary>
	/// Minimum width in points.
	/// </summary>
	public double MinWidth
	{
		get => minWidth;
		set => Set(ref minWidth, value);
	}
	double minWidth = 0;

	/// <summary>
	/// Maximum width in points.
	/// </summary>
	public double MaxWidth
	{
		get => maxWidth;
		set => Set(ref maxWidth, value);
	}
	double maxWidth = double.PositiveInfinity;

	/// <summary>
	/// Minimum height in points.
	/// </summary>
	public double MinHeight
	{
		get => minHeight;
		set => Set(ref minHeight, value);
	}
	double minHeight = 0;

	/// <summary>
	/// Maximum height in points.
	/// </summary>
	public double MaxHeight
	{
		get => maxHeight;
		set => Set(ref maxHeight, value);
	}
	double maxHeight = double.PositiveInfinity;

	/// <summary>
	/// How the view is placed within the horizontal space its parent gives it.
	/// </summary>
	public HorizontalAlignment HorizontalAlignment
	{
		get => horizontalAlignment;
		set => Set(ref horizontalAlignment, value);
	}
	HorizontalAlignment horizontalAlignment = HorizontalAlignment.Stretch;

	/// <summary>
	/// How the view is placed within the vertical space its parent gives it.
	/// </summary>
	public VerticalAlignment VerticalAlignment
	{
		get => verticalAlignment;
		set => Set(ref verticalAlignment, value);
	}
	VerticalAlignment verticalAlignment = VerticalAlignment.Stretch;

	/// <summary>
	/// When false the view takes no space and is hidden natively.
	/// </summary>
	public Bindable<bool> IsVisible
	{
		get => isVisible;
		set => isVisibleBinding = Register(isVisibleBinding, value, value => Set(ref isVisible, value, ApplyVisualState));
	}
	bool isVisible = true;
	Binding<bool>? isVisibleBinding;


	// Visual properties

	/// <summary>
	/// The background fill — a color, a gradient or a material — or null for transparent.
	/// </summary>
	public Brush? Background
	{
		get => background;
		set => Set(ref background, value, ApplyVisualState, affectsMeasure: false);
	}
	Brush? background;

	/// <summary>
	/// Opacity from 0 (transparent) to 1 (opaque).
	/// </summary>
	public double Opacity
	{
		get => opacity;
		set => Set(ref opacity, value, ApplyVisualState, affectsMeasure: false);
	}
	double opacity = 1.0;

	/// <summary>
	/// Corner radius in points applied to the layer.
	/// </summary>
	public double CornerRadius
	{
		get => cornerRadius;
		set => Set(ref cornerRadius, value, ApplyVisualState, affectsMeasure: false);
	}
	double cornerRadius = 0;

	/// <summary>
	/// A drop shadow behind the view, or null for none. A shadow needs unclipped bounds: it stops a corner radius from clipping the content, and an explicit <see cref="ClipsToBounds"/> hides it.
	/// </summary>
	public Shadow? Shadow
	{
		get => shadow;
		set => Set(ref shadow, value, ApplyVisualState, affectsMeasure: false);
	}
	Shadow? shadow;

	/// <summary>
	/// When true, content is clipped to the bounds and corner radius.
	/// </summary>
	public bool ClipsToBounds
	{
		get => clipsToBounds;
		set => Set(ref clipsToBounds, value, ApplyVisualState, affectsMeasure: false);
	}
	bool clipsToBounds;


	// Transform: drawn-only, so it never disturbs layout. This is what a gesture drags and an animation moves.

	/// <summary>
	/// Shifts the view from where layout put it, in points. Does not affect layout.
	/// </summary>
	public Point Translation
	{
		get => translation;
		set => Set(ref translation, value, ApplyTransform, affectsMeasure: false);
	}
	Point translation = Point.Zero;

	/// <summary>
	/// Scales the view about its centre, 1 being its laid-out size. Does not affect layout.
	/// </summary>
	public double Scale
	{
		get => scale;
		set => Set(ref scale, value, ApplyTransform, affectsMeasure: false);
	}
	double scale = 1;

	/// <summary>
	/// Rotates the view about its centre, in degrees. Does not affect layout.
	/// </summary>
	public double Rotation
	{
		get => rotation;
		set => Set(ref rotation, value, ApplyTransform, affectsMeasure: false);
	}
	double rotation;

	// a transformed view must be positioned by bounds+centre: setting Frame under a transform is undefined
	private protected bool HasTransform =>
		translation != Point.Zero || scale != 1 || rotation != 0;

	private protected void ApplyTransform() =>
		ApplyTransformCore();

	partial void ApplyTransformCore();


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


	bool measureValid;
	Size lastAvailable;
	Size lastDesired;

	// a second slot: Grid measures an auto child unconstrained and then again at the resolved cell
	// size, so a single-slot cache would thrash on every pass
	bool previousValid;
	Size previousAvailable;
	Size previousDesired;

	/// <summary>
	/// First pass: computes <see cref="DesiredSize"/> for the space the parent offers.
	/// </summary>
	public void Measure(
		Size available)
	{
		// same slot and nothing changed since: DesiredSize still holds
		if (measureValid && lastAvailable == available)
		{
			DesiredSize = lastDesired;
			return;
		}

		if (previousValid && previousAvailable == available)
		{
			// swap the slots, so alternating between two sizes stays a cache hit
			(lastAvailable, previousAvailable) = (previousAvailable, lastAvailable);
			(lastDesired, previousDesired) = (previousDesired, lastDesired);
			(measureValid, previousValid) = (previousValid, measureValid);

			DesiredSize = lastDesired;
			return;
		}

		if (measureValid)
		{
			previousAvailable = lastAvailable;
			previousDesired = lastDesired;
			previousValid = true;
		}

		lastAvailable = available;
		measureValid = true;

		if (!isVisible)
		{
			DesiredSize = lastDesired = Size.Zero;
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

		DesiredSize = lastDesired = desired.Inflate(Margin);
	}

	/// <summary>
	/// Second pass: positions the view within its slot, honouring margin and alignment.
	/// </summary>
	public void Arrange(
		Rect finalRect)
	{
		if (!isVisible)
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


	// LayoutHost drives these

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

	// fold explicit length into min/max (WPF)
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

	partial void ApplyFrame(
		Rect frame);
}
