using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace BareUI;

public abstract partial class View
{
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
		ApplyInteraction();
		ApplyAccessibility();
		ApplyProperties();
		OnBindingContextChanged();
		OnRealized();

		return native;
	}

	// controls push their whole property set here; CreateNative only constructs
	private protected virtual void ApplyProperties()
	{ }

	/// <summary>
	/// Tears down the native view and children deterministically (no finalizers).
	/// </summary>
	public void Unrealize()
	{
		if (native is null)
			return;

		OnUnrealized();
		DetachBindings();

		DropGradient();
		DropMaterial();

		native.RemoveFromSuperview();
		if (OwnsNative)
			native.Dispose();
		native = null;
		defaultTraits = null;
	}

	private protected virtual void OnRealized()
	{ }

	private protected virtual void OnUnrealized()
	{ }

	partial void ApplyIfRealized(
		Action apply)
	{
		if (native is not null)
			apply();
	}

	partial void RequestLayout() =>
		native?.SetNeedsLayout();

	UITapGestureRecognizer? tapRecognizer;

	partial void ApplyInteractionCore()
	{
		if (native is null)
			return;

		ApplyGestures();

		native.UserInteractionEnabled = IsEnabled;

		if (tapCommand is not null && tapRecognizer is null)
		{
			tapRecognizer = new(OnTapped);
			native.AddGestureRecognizer(tapRecognizer);
		}

		if (tapRecognizer is not null)
			tapRecognizer.Enabled = tapCommand is not null && IsEnabled;
	}

	void OnTapped()
	{
		if (tapCommand is { } command && command.CanExecute(TapCommandParameter))
			command.Execute(TapCommandParameter);
	}

	// the control's own traits, captured before we layer ours on top
	UIAccessibilityTrait? defaultTraits;

	partial void ApplyAccessibilityCore()
	{
		if (native is null)
			return;

		// null falls back to the control's own default (a UILabel reads its text)
		native.AccessibilityLabel = accessibilityLabel;
		native.AccessibilityHint = accessibilityHint;
		native.AccessibilityValue = accessibilityValue;
		native.AccessibilityIdentifier = accessibilityIdentifier;

		defaultTraits ??= native.AccessibilityTraits;
		native.AccessibilityTraits = defaultTraits.Value | Traits(accessibilityTraits);

		if (isAccessibilityElement is { } element)
			native.IsAccessibilityElement = element;
	}

	static UIAccessibilityTrait Traits(
		AccessibilityTraits traits)
	{
		UIAccessibilityTrait native = UIAccessibilityTrait.None;

		if (traits.HasFlag(AccessibilityTraits.Button))
			native |= UIAccessibilityTrait.Button;
		if (traits.HasFlag(AccessibilityTraits.Link))
			native |= UIAccessibilityTrait.Link;
		if (traits.HasFlag(AccessibilityTraits.Header))
			native |= UIAccessibilityTrait.Header;
		if (traits.HasFlag(AccessibilityTraits.Image))
			native |= UIAccessibilityTrait.Image;
		if (traits.HasFlag(AccessibilityTraits.Selected))
			native |= UIAccessibilityTrait.Selected;
		if (traits.HasFlag(AccessibilityTraits.StaticText))
			native |= UIAccessibilityTrait.StaticText;
		if (traits.HasFlag(AccessibilityTraits.Adjustable))
			native |= UIAccessibilityTrait.Adjustable;
		if (traits.HasFlag(AccessibilityTraits.UpdatesFrequently))
			native |= UIAccessibilityTrait.UpdatesFrequently;
		if (traits.HasFlag(AccessibilityTraits.NotEnabled))
			native |= UIAccessibilityTrait.NotEnabled;
		if (traits.HasFlag(AccessibilityTraits.PlaysSound))
			native |= UIAccessibilityTrait.PlaysSound;
		if (traits.HasFlag(AccessibilityTraits.StartsMediaSession))
			native |= UIAccessibilityTrait.StartsMediaSession;

		return native;
	}

	partial void ApplyVisualStateCore()
	{
		if (native is null)
			return;

		native.Hidden = !isVisible;
		native.Alpha = (nfloat)Opacity;

		ApplyBackground();

		// a clipped layer cannot draw a shadow: the shadow is outside the bounds. A corner radius alone
		// still clips (an Image must round its content), a shadow turns that off
		native.ClipsToBounds = ClipsToBounds || (CornerRadius > 0 && Shadow is null) || ClipsByDefault;

		ApplyShadow();
		native.Layer.CornerRadius = (nfloat)CornerRadius;
	}

	CAGradientLayer? gradientLayer;
	UIVisualEffectView? materialView;

	// a material is a real subview, so the panel's child diff has to know to leave it alone
	internal UIView? BackgroundView =>
		materialView;

	void ApplyBackground()
	{
		if (native is null)
			return;

		switch (Background)
		{
			case SolidBrush solid:
				DropGradient();
				DropMaterial();

				native.BackgroundColor = solid.Color.ToUIColor();
				break;

			case LinearGradient gradient:
				DropMaterial();
				RequirePanel("A gradient");

				native.BackgroundColor = UIColor.Clear;
				ApplyGradient(gradient);
				break;

			case Material material:
				DropGradient();
				RequirePanel("A material");

				native.BackgroundColor = UIColor.Clear;
				ApplyMaterial(material);
				break;

			// no brush leaves the native background alone: a control paints its own
			default:
				DropGradient();
				DropMaterial();
				break;
		}
	}

	// both fills sit under the view's *subviews*, but over a control's own drawing — a UILabel renders
	// its text into the layer, so a gradient behind it would cover the text
	void RequirePanel(
		string fill)
	{
		if (this is not Panel)
			throw new InvalidOperationException(
				$"{fill} background needs a panel (Border, Overlay, VStack, ...); {GetType().Name} draws its own content, which the fill would cover.");
	}

	void ApplyGradient(
		LinearGradient gradient)
	{
		gradientLayer ??= new();

		if (gradientLayer.SuperLayer is null)
			native!.Layer.InsertSublayer(gradientLayer, 0);

		CGColor[] colors = new CGColor[gradient.Stops.Count];
		NSNumber[] locations = new NSNumber[gradient.Stops.Count];

		for (int index = 0; index < gradient.Stops.Count; index++)
		{
			GradientStop stop = gradient.Stops[index];

			colors[index] = stop.Color.ToUIColor().CGColor;
			locations[index] = NSNumber.FromDouble(stop.Offset);
		}

		gradientLayer.Colors = colors;
		gradientLayer.Locations = locations;
		gradientLayer.StartPoint = new(gradient.Start.X, gradient.Start.Y);
		gradientLayer.EndPoint = new(gradient.End.X, gradient.End.Y);

		SyncGradientFrame();
	}

	// a CALayer does not autoresize, and its implicit animation would lag a frame behind a scroll
	void SyncGradientFrame()
	{
		if (gradientLayer is null || native is null)
			return;

		CATransaction.Begin();
		CATransaction.DisableActions = true;

		gradientLayer.Frame = native.Bounds;
		gradientLayer.CornerRadius = (nfloat)CornerRadius;

		CATransaction.Commit();
	}

	void ApplyMaterial(
		Material material)
	{
		UIBlurEffect effect = UIBlurEffect.FromStyle(material.Kind switch
		{
			MaterialKind.UltraThin => UIBlurEffectStyle.SystemUltraThinMaterial,
			MaterialKind.Thin => UIBlurEffectStyle.SystemThinMaterial,
			MaterialKind.Thick => UIBlurEffectStyle.SystemThickMaterial,
			MaterialKind.Chrome => UIBlurEffectStyle.SystemChromeMaterial,
			_ => UIBlurEffectStyle.SystemMaterial
		});

		if (materialView is null)
		{
			materialView = new(effect)
			{
				Frame = native!.Bounds,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
				UserInteractionEnabled = false
			};

			native.InsertSubview(materialView, 0);
		}
		else
			materialView.Effect = effect;

		materialView.Layer.CornerRadius = (nfloat)CornerRadius;
		materialView.ClipsToBounds = CornerRadius > 0;
	}

	void DropGradient()
	{
		gradientLayer?.RemoveFromSuperLayer();
		gradientLayer?.Dispose();
		gradientLayer = null;
	}

	void DropMaterial()
	{
		materialView?.RemoveFromSuperview();
		materialView?.Dispose();
		materialView = null;
	}

	// the page's insets, walked up the tree
	Thickness PageSafeArea
	{
		get
		{
			for (View? view = this; view is not null; view = view.Parent)
				if (view is ContentView page)
					return page.PageSafeArea;

			return Thickness.Zero;
		}
	}

	// how far this view has grown past the safe area. A scrolling view insets its own content by
	// exactly this, so the scroll passes under the bar but the content never does
	internal Thickness BledInsets
	{
		get
		{
			if (IgnoresSafeArea is SafeAreaEdges.None)
				return Thickness.Zero;

			Thickness insets = PageSafeArea;

			return new(
				IgnoresSafeArea.HasFlag(SafeAreaEdges.Leading) ? insets.Left : 0,
				IgnoresSafeArea.HasFlag(SafeAreaEdges.Top) ? insets.Top : 0,
				IgnoresSafeArea.HasFlag(SafeAreaEdges.Trailing) ? insets.Right : 0,
				IgnoresSafeArea.HasFlag(SafeAreaEdges.Bottom) ? insets.Bottom : 0);
		}
	}

	Rect Bleed(
		Rect frame)
	{
		Thickness bled = BledInsets;

		return new(
			frame.X - bled.Left,
			frame.Y - bled.Top,
			frame.Width + bled.Left + bled.Right,
			frame.Height + bled.Top + bled.Bottom);
	}

	/// <summary>
	/// Whether this view currently holds keyboard focus.
	/// </summary>
	public bool IsFocused =>
		native?.IsFirstResponder is true;

	partial void FocusCore() =>
		native?.BecomeFirstResponder();

	partial void UnfocusCore() =>
		native?.ResignFirstResponder();

	/// <summary>
	/// Animates the property changes made inside <paramref name="changes"/>.
	/// </summary>
	public static void Animate(
		double seconds,
		Action changes) =>
		UIView.Animate(seconds, changes);

	/// <summary>
	/// Adds a native gesture recognizer. An escape hatch for gestures the library does not wrap.
	/// </summary>
	public void AddGesture(
		UIGestureRecognizer gesture)
	{
		gestures.Add(gesture);

		native?.AddGestureRecognizer(gesture);
	}

	// kept managed-side too: UIKit retains the recognizer, but the peer needs a root
	readonly List<UIGestureRecognizer> gestures = [];

	void ApplyGestures()
	{
		foreach (UIGestureRecognizer gesture in gestures)
			if (gesture.View is null)
				native?.AddGestureRecognizer(gesture);
	}

	void ApplyShadow()
	{
		if (native is null)
			return;

		if (Shadow is not { } shadow)
		{
			native.Layer.ShadowOpacity = 0;
			return;
		}

		native.Layer.ShadowOpacity = (float)shadow.Opacity;
		native.Layer.ShadowRadius = (nfloat)shadow.Radius;
		native.Layer.ShadowOffset = new(shadow.OffsetX, shadow.OffsetY);
		native.Layer.ShadowColor = (shadow.Color ?? Colors.Black).ToUIColor().CGColor;
	}

	partial void ApplyFrame(
		Rect frame)
	{
		if (native is null)
			return;

		frame = Bleed(frame);

		CGRect next = new(frame.X, frame.Y, frame.Width, frame.Height);
		bool resized = native.Frame.Size != next.Size;

		native.Frame = next;

		if (resized)
		{
			SyncGradientFrame();
			native.SetNeedsLayout();
		}
	}
}
