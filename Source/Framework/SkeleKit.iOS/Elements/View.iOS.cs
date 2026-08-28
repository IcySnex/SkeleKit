using System.Windows.Input;
using CoreAnimation;
using ObjCRuntime;

namespace SkeleKit;

public abstract partial class View
{
	// ReSharper disable once RedundantAssignment
	static partial void GetApplicationTint(
		ref Color? tint) =>
		tint = SkeleApplication.Current?.Tint;


	static void Run(
		ICommand? command,
		object? parameter)
	{
		if (command is not null && command.CanExecute(parameter))
			command.Execute(parameter);
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

	static UIViewAnimationOptions Options(
		Easing easing) =>
		UIViewAnimationOptions.AllowUserInteraction | easing switch
		{
			Easing.Linear => UIViewAnimationOptions.CurveLinear,
			Easing.EaseIn => UIViewAnimationOptions.CurveEaseIn,
			Easing.EaseOut => UIViewAnimationOptions.CurveEaseOut,
			_ => UIViewAnimationOptions.CurveEaseInOut
		};

	static GestureState StateOf(
		UIGestureRecognizer recognizer) =>
		recognizer.State switch
		{
			UIGestureRecognizerState.Began => GestureState.Began,
			UIGestureRecognizerState.Changed => GestureState.Changed,
			UIGestureRecognizerState.Ended => GestureState.Ended,
			_ => GestureState.Canceled
		};


	readonly List<UIGestureRecognizer> gestures = [];

	UIView? native;

	UITapGestureRecognizer? tapRecognizer;
	UITapGestureRecognizer? doubleTapRecognizer;
	UILongPressGestureRecognizer? longPressRecognizer;
	UILongPressGestureRecognizer? pressRecognizer;
	UIPanGestureRecognizer? panRecognizer;
	UIPinchGestureRecognizer? pinchRecognizer;
	UIRotationGestureRecognizer? rotationRecognizer;

	UIPointerInteraction? pointerInteraction;
	PointerInteractionDelegate? pointerDelegate;

	ContextMenuDelegate? contextMenuDelegate;
	UIContextMenuInteraction? contextMenuInteraction;
	UIAction[]? contextMenuActions;

	UIAccessibilityTrait? defaultTraits;
	CAGradientLayer? gradientLayer;
	UIVisualEffectView? materialView;

	Thickness PageSafeArea
	{
		get
		{
			for (View? view = this; view is not null; view = view.Parent)
			{
				if (view is ContentView page)
					return page.PageSafeArea;
			}

			return Thickness.Zero;
		}
	}


	private protected virtual bool OwnsNative => true;

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

	internal UIView? BackgroundView => materialView;

	internal UIView ChildHost => materialView is UIVisualEffectView material && Background is Material { Kind: MaterialKind.Glass }
		? material.ContentView
		: Native;


	/// <summary>
	/// The underlying UIKit view, created on first access. Primary escape hatch to UIKit.
	/// </summary>
	public UIView Native => native ??= RealizeCore();

	/// <summary>
	/// Whether the native view has been created yet.
	/// </summary>
	public bool IsRealized => native is not null;

	/// <summary>
	/// Whether this view currently holds keyboard focus.
	/// </summary>
	public bool IsFocused => native?.IsFirstResponder is true;



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

	void DropInteractions()
	{
		DropRecognizer(ref tapRecognizer);
		DropRecognizer(ref doubleTapRecognizer);
		DropRecognizer(ref longPressRecognizer);
		DropRecognizer(ref pressRecognizer);
		DropRecognizer(ref panRecognizer);
		DropRecognizer(ref pinchRecognizer);
		DropRecognizer(ref rotationRecognizer);

		if (contextMenuInteraction is not null)
		{
			native?.RemoveInteraction(contextMenuInteraction);
			contextMenuInteraction.Dispose();
			contextMenuInteraction = null;
		}

		contextMenuDelegate?.Dispose();
		contextMenuDelegate = null;
		contextMenuActions = null;
	}

	void DropRecognizer<T>(
		ref T? recognizer) where T : UIGestureRecognizer
	{
		if (recognizer is null)
			return;

		native?.RemoveGestureRecognizer(recognizer);
		recognizer.Dispose();
		recognizer = null;
	}

	void RequirePanel(
		string fill)
	{
		if (this is not Panel)
		{
			throw new InvalidOperationException(
				$"{fill} background needs a panel (Border, Overlay, StackPanel, ...); {GetType().Name} draws its own content, which the fill would cover.");
		}
	}

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

	partial void ApplyIfRealized(
		Action apply)
	{
		if (native is not null)
			apply();
	}

	partial void RequestLayout()
	{
		native?.SetNeedsLayout();

		if (this is ContentView { Host: PageHost host })
			host.ContentMeasureInvalidated();
	}

	void ApplyContextMenu()
	{
		if (native is null || ContextMenu.Count == 0 || contextMenuInteraction is not null)
			return;

		contextMenuDelegate = new(this);
		contextMenuInteraction = new(contextMenuDelegate);

		native.AddInteraction(contextMenuInteraction);
	}

	void ApplyGestures()
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		foreach (UIGestureRecognizer gesture in gestures.Where(gesture => gesture.View is null))
			native?.AddGestureRecognizer(gesture);
	}

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

		ChildHostChanged();
	}

	void ApplyShadow()
	{
		if (native is null)
			return;

		if (Shadow is not Shadow shadow)
		{
			native.Layer.ShadowOpacity = 0;
			return;
		}

		native.Layer.ShadowOpacity = (float)shadow.Opacity;
		native.Layer.ShadowRadius = (nfloat)shadow.Radius;
		native.Layer.ShadowOffset = new(shadow.OffsetX, shadow.OffsetY);
		native.Layer.ShadowColor = (shadow.Color ?? Colors.Black).ToUIColor().CGColor;
	}

	void ApplyGradient(
		LinearGradient gradient)
	{
		// negative z keeps the fill under the children even if the sublayer order shifts
		gradientLayer ??= new() { ZPosition = -1 };

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

	void ApplyMaterial(
		Material material)
	{
		UIVisualEffect effect = material.Kind is MaterialKind.Glass && OperatingSystem.IsIOSVersionAtLeast(26)
			? new UIGlassEffect { Interactive = true }
			: UIBlurEffect.FromStyle(material.Kind switch
			{
				MaterialKind.UltraThin => UIBlurEffectStyle.SystemUltraThinMaterial,
				MaterialKind.Thin => UIBlurEffectStyle.SystemThinMaterial,
				MaterialKind.Thick => UIBlurEffectStyle.SystemThickMaterial,
				MaterialKind.Chrome or MaterialKind.Glass => UIBlurEffectStyle.SystemChromeMaterial,
				_ => UIBlurEffectStyle.SystemMaterial
			});

		if (materialView is null)
		{
			materialView = new(effect)
			{
				Frame = native!.Bounds,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,

				// interactive glass lights up under the finger, so it has to receive touches
				UserInteractionEnabled = material.Kind is MaterialKind.Glass
			};

			native.InsertSubview(materialView, 0);
		}
		else
			materialView.Effect = effect;

		if (material.Kind is MaterialKind.Glass && OperatingSystem.IsIOSVersionAtLeast(26))
		{
			// the glass renderer draws its rim against this shape; a layer clip would flatten it
			materialView.CornerConfiguration = UICornerConfiguration.CreateUniformCorners(UICornerRadius.CreateFixed((nfloat)CornerRadius));
			materialView.ClipsToBounds = false;
			return;
		}

		materialView.Layer.CornerRadius = (nfloat)CornerRadius;
		materialView.ClipsToBounds = CornerRadius > 0;
	}

	partial void ApplyFrame(
		Rect frame)
	{
		if (native is null)
			return;

		frame = Bleed(frame);

		CGRect next = new(frame.X, frame.Y, frame.Width, frame.Height);
		bool resized = native.Bounds.Size != next.Size;

		// always bounds+center, never Frame: an animation can leave the native transform non-identity
		// while the model reads as untransformed, and setting Frame under a transform is undefined.
		// The origin stays — a scroll view keeps its content offset there
		native.Bounds = new(native.Bounds.X, native.Bounds.Y, next.Width, next.Height);
		native.Center = new(next.X + next.Width / 2, next.Y + next.Height / 2);

		if (resized)
		{
			SyncGradientFrame();
			native.SetNeedsLayout();

			// the pivot offset scales with the bounds, so a resize re-bakes it into the transform
			ApplyTransform();
		}
	}

	partial void ApplyInteractionCore()
	{
		if (native is null)
			return;

		ApplyGestures();
		ApplyContextMenu();

		native.UserInteractionEnabled = isEnabled;

		if (tapCommand is not null && tapRecognizer is null)
		{
			tapRecognizer = new(() => Run(tapCommand, TapCommandParameter));
			native.AddGestureRecognizer(tapRecognizer);
		}

		if (doubleTapCommand is not null && doubleTapRecognizer is null)
		{
			doubleTapRecognizer = new(() => Run(doubleTapCommand, null)) { NumberOfTapsRequired = 2 };
			native.AddGestureRecognizer(doubleTapRecognizer);

			// a single tap must wait, or it fires on the double's first tap
			tapRecognizer?.RequireGestureRecognizerToFail(doubleTapRecognizer);
		}

		if (longPressCommand is not null && longPressRecognizer is null)
		{
			UILongPressGestureRecognizer recognizer = null!;
			recognizer = new(() =>
			{
				if (recognizer.State is UIGestureRecognizerState.Began)
					Run(longPressCommand, null);
			});

			longPressRecognizer = recognizer;
			native.AddGestureRecognizer(recognizer);
		}

		longPressRecognizer?.MinimumPressDuration = LongPressDuration;

		if (pressed is not null && pressRecognizer is null)
		{
			UILongPressGestureRecognizer recognizer = null!;
			recognizer = new(() =>
			{
				switch (recognizer.State)
				{
					case UIGestureRecognizerState.Began:
						pressed?.Invoke(true);
						break;
					case UIGestureRecognizerState.Ended or UIGestureRecognizerState.Cancelled or UIGestureRecognizerState.Failed:
						pressed?.Invoke(false);
						break;
				}
			});

			// zero duration turns it into a touch-down tracker; not canceling keeps child controls live
			recognizer.MinimumPressDuration = 0;
			recognizer.CancelsTouchesInView = false;

			pressRecognizer = recognizer;
			native.AddGestureRecognizer(recognizer);
		}

		if (panned is not null && panRecognizer is null)
		{
			UIPanGestureRecognizer recognizer = null!;
			recognizer = new(() =>
			{
				// measured in the parent: the view's own space rotates and scales under the gesture
				CGPoint translation = recognizer.TranslationInView(recognizer.View.Superview);
				CGPoint velocity = recognizer.VelocityInView(recognizer.View.Superview);

				panned?.Invoke(new(
					StateOf(recognizer),
					new(translation.X, translation.Y),
					new(velocity.X, velocity.Y)));
			});

			panRecognizer = recognizer;
			native.AddGestureRecognizer(recognizer);
		}

		if (pinched is not null && pinchRecognizer is null)
		{
			UIPinchGestureRecognizer recognizer = null!;
			recognizer = new(() =>
				pinched?.Invoke(new(
					StateOf(recognizer),
					recognizer.Scale,
					recognizer.Velocity)));

			pinchRecognizer = recognizer;
			native.AddGestureRecognizer(recognizer);
		}

		if (rotated is not null && rotationRecognizer is null)
		{
			UIRotationGestureRecognizer recognizer = null!;
			recognizer = new(() =>
				rotated?.Invoke(new(
					StateOf(recognizer),
					recognizer.Rotation * 180 / Math.PI,
					recognizer.Velocity * 180 / Math.PI)));

			rotationRecognizer = recognizer;
			native.AddGestureRecognizer(recognizer);
		}

		tapRecognizer?.Enabled = tapCommand is not null && isEnabled;
		doubleTapRecognizer?.Enabled = doubleTapCommand is not null && isEnabled;
		longPressRecognizer?.Enabled = longPressCommand is not null && isEnabled;
		pressRecognizer?.Enabled = pressed is not null && isEnabled;
		panRecognizer?.Enabled = panned is not null && isEnabled;
		pinchRecognizer?.Enabled = pinched is not null && isEnabled;
		rotationRecognizer?.Enabled = rotated is not null && isEnabled;

		// a hovered pointer picks up the effect through the delegate, which reads the live PointerEffect
		if (PointerEffect is not PointerEffect.None && pointerInteraction is null)
		{
			pointerDelegate = new(this);
			pointerInteraction = new(pointerDelegate);
			native.AddInteraction(pointerInteraction);
		}
	}

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

		if (isAccessibilityElement is bool element)
			native.IsAccessibilityElement = element;
	}

	partial void ApplyVisualStateCore()
	{
		if (native is null)
			return;

		native.Hidden = !isVisible;
		native.Alpha = (nfloat)Opacity;

		// local only: nil lets UIKit inherit down the native tree
		native.TintColor = LocalTint?.ToUIColor();

		ApplyBackground();

		// a clipped layer cannot draw a shadow: the shadow is outside the bounds. A corner radius alone
		// still clips (an Image must round its content), a shadow turns that off. Glass rounds by
		// corner configuration instead: a layer clip would scissor its rim lensing
		bool glass = Background is Material { Kind: MaterialKind.Glass } && OperatingSystem.IsIOSVersionAtLeast(26);

		native.ClipsToBounds = ClipsToBounds || CornerRadius > 0 && Shadow is null && !glass || ClipsByDefault;

		ApplyShadow();
		native.Layer.CornerRadius = glass ? 0 : (nfloat)CornerRadius;

		ApplyTransform();
	}

	partial void ApplyTransformCore()
	{
		if (native is null)
			return;

		if (!HasTransform)
		{
			native.Transform = CGAffineTransform.MakeIdentity();
			return;
		}

		// linear part: scale then rotate (no translation yet)
		CGAffineTransform transform = CGAffineTransform.MakeScale((nfloat)Scale, (nfloat)Scale);
		transform = CGAffineTransform.Multiply(transform, CGAffineTransform.MakeRotation((nfloat)(Rotation * Math.PI / 180)));

		// UIKit applies the transform about the center; to pivot at AnchorPoint instead, add the
		// translation (I - L)·(P - C) that a change of pivot introduces, then the caller's own translation.
		// Baked into the matrix rather than set on layer.AnchorPoint, which UIView's frame system resets.
		nfloat px = (nfloat)((AnchorPoint.X - 0.5) * native.Bounds.Width);
		nfloat py = (nfloat)((AnchorPoint.Y - 0.5) * native.Bounds.Height);

		transform.Tx = px - (transform.A * px + transform.C * py) + (nfloat)Translation.X;
		transform.Ty = py - (transform.B * px + transform.D * py) + (nfloat)Translation.Y;

		native.Transform = transform;
	}

	partial void FocusCore() =>
		native?.BecomeFirstResponder();

	partial void UnfocusCore() =>
		native?.ResignFirstResponder();


	private protected abstract UIView CreateNative();

	private protected virtual void ApplyProperties()
	{ }

	private protected virtual void OnRealized()
	{ }

	private protected virtual void OnUnrealized()
	{ }

	private protected virtual void ChildHostChanged()
	{ }


	internal UIPointerStyle? PointerStyle()
	{
		if (native is null || PointerEffect is PointerEffect.None)
			return null;

		// only the automatic effect is bound in Microsoft.iOS 26.0 (see PointerEffect)
		return UIPointerStyle.Create(UIPointerEffect.Create(new(native)), null);
	}

	internal UIContextMenuConfiguration? MenuConfiguration()
	{
		if (ContextMenu.Count == 0)
			return null;

		return UIContextMenuConfiguration.Create(
			null,
			null,
			_ =>
			{
				contextMenuActions = new UIAction[ContextMenu.Count];

				for (int index = 0; index < ContextMenu.Count; index++)
				{
					MenuAction entry = ContextMenu[index];

					contextMenuActions[index] = UIAction.Create(
						entry.Text,
						entry.Icon is string icon ? UIImage.GetSystemImage(icon) : null,
						null,
						_ => Run(entry.Command, entry.CommandParameter));

					if (entry.IsDestructive)
						contextMenuActions[index].Attributes = UIMenuElementAttributes.Destructive;
				}

				return UIMenu.Create(contextMenuActions);
			});
	}


	/// <summary>
	/// Builds the native view (if needed) and realizes children and bindings.
	/// </summary>
	public UIView Realize() =>
		Native;

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
		DropInteractions();

		native.RemoveFromSuperview();
		if (OwnsNative)
			native.Dispose();
		native = null;
		defaultTraits = null;
	}

	/// <summary>
	/// Runs any pending layout right now. Call it inside an <see cref="Animator"/>'s changes to animate a layout property.
	/// </summary>
	/// <remarks>A layout property (Width, Margin, ...) only reaches the native frame on the next layout pass, which lands after an animation block has closed — so it would snap instead of animating. <see cref="Animate(Animation, Action, Action{bool})"/> does this for you.</remarks>
	public static void LayoutNow() =>
		UIApplication.SharedApplication
			.ConnectedScenes
			.OfType<UIWindowScene>()
			.SelectMany(scene => scene.Windows)
			.FirstOrDefault(window => window.IsKeyWindow)?
			.LayoutIfNeeded();

	/// <summary>
	/// Adds a native gesture recognizer without forcing the view to realize.
	/// </summary>
	/// <remarks>
	/// This is an escape hatch for gestures the library does not wrap. The recognizer is retained and
	/// attached whenever the view's native peer is realized. The caller owns any separate target or
	/// delegate objects used by the recognizer.
	/// </remarks>
	public void AddNativeGesture(
		UIGestureRecognizer gesture)
	{
		gestures.Add(gesture);

		native?.AddGestureRecognizer(gesture);
	}

	/// <summary>
	/// Animates the property changes made inside <paramref name="changes"/>.
	/// </summary>
	public static void Animate(
		double seconds,
		Action changes) =>
		Animate(Animation.Ease(seconds), changes);

	/// <summary>
	/// Animates the property changes made inside <paramref name="changes"/>, following <paramref name="animation"/>.
	/// </summary>
	/// <remarks>For an animation the user can grab mid-flight, create an <see cref="Animator"/> instead.</remarks>
	public static void Animate(
		Animation animation,
		Action changes,
		Action<bool>? completed = null)
	{
		UICompletionHandler done = finished => completed?.Invoke(finished);

		Action animated = () =>
		{
			changes();
			LayoutNow();
		};

		if (animation.SpringDamping is double damping)
		{
			UIView.AnimateNotify(
				animation.Duration,
				animation.Delay,
				(nfloat)damping,
				0,
				UIViewAnimationOptions.AllowUserInteraction,
				animated,
				done);
		}
		else
		{
			UIView.AnimateNotify(
				animation.Duration,
				animation.Delay,
				Options(animation.Easing),
				animated,
				done);
		}
	}

}

internal sealed class ContextMenuDelegate : NSObject, IUIContextMenuInteractionDelegate
{
	readonly View? element;

	public ContextMenuDelegate(
		View element)
	{
		this.element = element;
	}

	public ContextMenuDelegate(
		NativeHandle handle) : base(handle)
	{ }


	public UIContextMenuConfiguration? GetConfigurationForMenu(
		UIContextMenuInteraction interaction,
		CGPoint location) =>
		element?.MenuConfiguration();
}

internal sealed class PointerInteractionDelegate : UIPointerInteractionDelegate
{
	readonly View? element;

	public PointerInteractionDelegate(
		View element)
	{
		this.element = element;
	}

	public PointerInteractionDelegate(
		NativeHandle handle) : base(handle)
	{ }


	public override UIPointerStyle? GetStyleForRegion(
		UIPointerInteraction interaction,
		UIPointerRegion region) =>
		element?.PointerStyle();
}
