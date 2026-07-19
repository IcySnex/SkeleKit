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
	protected View()
	{
		Theme.ApplyTo(this);
	}


	/// <summary>
	/// A style applied the moment it is assigned.
	/// </summary>
	/// <remarks>
	/// Put it first in an object initializer; later assignments override it.
	/// </remarks>
	public IStyle? Style
	{
		get;
		set
		{
			field = value;
			value?.Apply(this);
		}
	}


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

	/// <summary>
	/// The object bindings resolve against. Inherited from the parent unless set explicitly.
	/// </summary>
	public object? BindingContext
	{
		get => field ?? Parent?.BindingContext;
		set
		{
			if (ReferenceEquals(field, value))
				return;

			field = value;
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
		Action<T?> apply) =>
		Register(existing, value.Expression, value.Value, apply);

	private protected Binding<T>? Register<T>(
		Binding<T>? existing,
		BindingExpression<T>? expression,
		T? value,
		Action<T?> apply)
	{
		if (existing is not null)
		{
			existing.Detach();
			bindings.Remove(existing);
		}

		if (expression is null)
		{
			apply(value);
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

			if (view.Parent is not View parent)
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
		while (root.Parent is View parent)
			root = parent;

		return root;
	}

	/// <summary>
	/// Drops every cached measurement in this subtree.
	/// </summary>
	/// <remarks>
	/// For changes that hit leaves directly, like a dynamic-type resize.
	/// </remarks>
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

		// inside an animation's changes: remember where this view was, in case UIKit reverts it
		AnimationCapture.Record(this);

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

	// the page came (back) on screen: a list uses this to release its still-selected row
	internal virtual void PageAppeared()
	{ }

	private protected void ApplyInteraction() =>
		ApplyInteractionCore();

	partial void ApplyInteractionCore();


	// Interaction

	/// <summary>
	/// Whether the view responds to touches.
	/// </summary>
	public bool IsEnabled
	{
		get;
		set => Set(ref field, value, ApplyInteraction, affectsMeasure: false);
	} = true;

	/// <summary>
	/// The iPad pointer effect shown when a trackpad or mouse hovers this view, or None (the default).
	/// </summary>
	public PointerEffect PointerEffect
	{
		get;
		set => Set(ref field, value, ApplyInteraction, affectsMeasure: false);
	}

	/// <summary>
	/// Entries in the view's long-press context menu. Empty for none.
	/// </summary>
	public IList<MenuAction> ContextMenu { get; } = [];

	/// <summary>
	/// Command invoked when the view is tapped.
	/// </summary>
	public ICommand? TapCommand
	{
		get => tapCommand;
		set => Set(ref tapCommand, value, ApplyInteraction, affectsMeasure: false);
	}
	ICommand? tapCommand;

	/// <summary>
	/// The parameter passed to <see cref="TapCommand"/>.
	/// </summary>
	public object? TapCommandParameter { get; set; }

	/// <summary>
	/// Command invoked when the view is double-tapped.
	/// </summary>
	public ICommand? DoubleTapCommand
	{
		get => doubleTapCommand;
		set => Set(ref doubleTapCommand, value, ApplyInteraction, affectsMeasure: false);
	}
	ICommand? doubleTapCommand;

	/// <summary>
	/// Command invoked when the view is held down for <see cref="LongPressDuration"/>.
	/// </summary>
	public ICommand? LongPressCommand
	{
		get => longPressCommand;
		set => Set(ref longPressCommand, value, ApplyInteraction, affectsMeasure: false);
	}
	ICommand? longPressCommand;

	/// <summary>
	/// How long a press must be held to count as a long press, in seconds.
	/// </summary>
	public double LongPressDuration { get; set; } = 0.5;

	/// <summary>
	/// Invoked with true on touch-down anywhere in the view, false on release or cancel.
	/// </summary>
	/// <remarks>
	/// Child controls still receive their touches.
	/// </remarks>
	public Action<bool>? Pressed
	{
		get => pressed;
		set => Set(ref pressed, value, ApplyInteraction, affectsMeasure: false);
	}
	Action<bool>? pressed;

	/// <summary>
	/// Invoked as the view is dragged.
	/// </summary>
	/// <remarks>
	/// Drive an <see cref="Animator"/> from it to make an animation interactive.
	/// </remarks>
	public Action<PanGesture>? Panned
	{
		get => panned;
		set => Set(ref panned, value, ApplyInteraction, affectsMeasure: false);
	}
	Action<PanGesture>? panned;

	/// <summary>
	/// Invoked as the view is pinched.
	/// </summary>
	/// <remarks>
	/// Feed the scale into <see cref="Scale"/> to zoom it.
	/// </remarks>
	public Action<PinchGesture>? Pinched
	{
		get => pinched;
		set => Set(ref pinched, value, ApplyInteraction, affectsMeasure: false);
	}
	Action<PinchGesture>? pinched;

	/// <summary>
	/// Invoked as the view is rotated with two fingers.
	/// </summary>
	/// <remarks>
	/// Feed the degrees into <see cref="Rotation"/> to turn it.
	/// </remarks>
	public Action<RotateGesture>? Rotated
	{
		get => rotated;
		set => Set(ref rotated, value, ApplyInteraction, affectsMeasure: false);
	}
	Action<RotateGesture>? rotated;

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
	/// Edges this view is allowed to extend past the safe area.
	/// </summary>
	/// <remarks>
	/// A scrolling view still keeps its content inside it, so only the scroll passes under the bar.
	/// </remarks>
	public SafeAreaEdges IgnoresSafeArea
	{
		get;
		set => Set(ref field, value);
	} = SafeAreaEdges.None;

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
		get;
		set => Set(ref field, value);
	}

	/// <summary>
	/// Maximum width in points.
	/// </summary>
	public double MaxWidth
	{
		get;
		set => Set(ref field, value);
	} = double.PositiveInfinity;

	/// <summary>
	/// Minimum height in points.
	/// </summary>
	public double MinHeight
	{
		get;
		set => Set(ref field, value);
	}

	/// <summary>
	/// Maximum height in points.
	/// </summary>
	public double MaxHeight
	{
		get;
		set => Set(ref field, value);
	} = double.PositiveInfinity;

	/// <summary>
	/// How the view is placed within the horizontal space its parent gives it.
	/// </summary>
	public HorizontalAlignment HorizontalAlignment
	{
		get;
		set => Set(ref field, value);
	} = HorizontalAlignment.Stretch;

	/// <summary>
	/// How the view is placed within the vertical space its parent gives it.
	/// </summary>
	public VerticalAlignment VerticalAlignment
	{
		get;
		set => Set(ref field, value);
	} = VerticalAlignment.Stretch;

	/// <summary>
	/// When false the view takes no space and is hidden natively.
	/// </summary>
	public Bindable<bool> IsVisible
	{
		get => isVisible;
		set => isVisibleBinding = Register(isVisibleBinding, value, value => Set(ref isVisible, value, ApplyVisibility));
	}
	bool isVisible = true;
	Binding<bool>? isVisibleBinding;

	// set only on the tab accessory root: the shell removes the slot when it hides
	internal Action? VisibilityChanged;

	void ApplyVisibility()
	{
		ApplyVisualState();
		VisibilityChanged?.Invoke();
	}


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
	/// The accent color for this view and everything under it.
	/// </summary>
	/// <remarks>
	/// Inherited from the parent unless set here, falling back to the app accent.
	/// </remarks>
	public Color? Tint
	{
		get => tint ?? (Parent ?? TintHost)?.Tint ?? AppAccent;
		set => Set(ref tint, value, ApplyTint, affectsMeasure: false);
	}
	Color? tint;

	// UseAccent's app-wide fallback
	internal static Color? AppAccent;

	// bridges inheritance into hosted trees (collection cells), where Parent is null
	internal View? TintHost;

	// set on this view itself, not inherited
	internal Color? LocalTint =>
		tint;

	void ApplyTint()
	{
		ApplyVisualState();
		TintChanged();
	}

	internal virtual void TintChanged()
	{ }

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
	double cornerRadius;

	/// <summary>
	/// A drop shadow behind the view, or null for none.
	/// </summary>
	/// <remarks>
	/// A shadow needs unclipped bounds: it stops a corner radius from clipping the content, and an explicit <see cref="ClipsToBounds"/> hides it.
	/// </remarks>
	public Shadow? Shadow
	{
		get;
		set => Set(ref field, value, ApplyVisualState, affectsMeasure: false);
	}

	/// <summary>
	/// When true, content is clipped to the bounds and corner radius.
	/// </summary>
	public bool ClipsToBounds
	{
		get;
		set => Set(ref field, value, ApplyVisualState, affectsMeasure: false);
	}


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
	/// Scales the view about its center, 1 being its laid-out size. Does not affect layout.
	/// </summary>
	public double Scale
	{
		get => scale;
		set => Set(ref scale, value, ApplyTransform, affectsMeasure: false);
	}
	double scale = 1;

	/// <summary>
	/// Rotates the view about its <see cref="AnchorPoint"/>, in degrees. Does not affect layout.
	/// </summary>
	public double Rotation
	{
		get => rotation;
		set => Set(ref rotation, value, ApplyTransform, affectsMeasure: false);
	}
	double rotation;

	/// <summary>
	/// The pivot for <see cref="Rotation"/> and <see cref="Scale"/>, in unit coordinates.
	/// </summary>
	/// <remarks>
	/// (0.5, 0.5) is the center (the default), (0, 0) the top-left corner, (1, 1) the bottom-right.
	/// </remarks>
	public Point AnchorPoint
	{
		get;
		set => Set(ref field, value, ApplyAnchor, affectsMeasure: false);
	} = new(0.5, 0.5);

	// the pivot lives in the transform matrix (baked around the center), so a change re-derives it
	void ApplyAnchor() =>
		ApplyTransform();

	// a transformed view must be positioned by bounds+center: setting Frame under a transform is undefined
	private protected bool HasTransform => translation != Point.Zero || Math.Abs(scale - 1) > 0.00001 || Math.Abs(rotation) > 0.00001;

	internal ViewState Capture() =>
		new(translation, scale, rotation, opacity, cornerRadius, background, width, height, margin);

	// unconditional, past Set's equality check: an animation block must write natively even when the
	// model already holds these values, or the animation would capture nothing
	internal void Apply(
		ViewState state)
	{
		translation = state.Translation;
		scale = state.Scale;
		rotation = state.Rotation;
		opacity = state.Opacity;
		cornerRadius = state.CornerRadius;
		background = state.Background;

		ApplyIfRealized(ApplyTransform);
		ApplyIfRealized(ApplyVisualState);

		if (!width.Equals(state.Width) || !height.Equals(state.Height) || margin != state.Margin)
		{
			width = state.Width;
			height = state.Height;
			margin = state.Margin;

			InvalidateMeasure();
		}
	}

	private protected void ApplyTransform() =>
		ApplyTransformCore();

	partial void ApplyTransformCore();


	// per-child data written by a parent panel (a Grid stores row/column here)
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
	/// Second pass: positions the view within its slot, honoring margin and alignment.
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
