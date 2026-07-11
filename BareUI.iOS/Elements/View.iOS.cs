using CoreGraphics;
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

		native.RemoveFromSuperview();
		if (OwnsNative)
			native.Dispose();
		native = null;
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

	partial void ApplyVisualStateCore()
	{
		if (native is null)
			return;

		native.Hidden = !isVisible;
		native.Alpha = (nfloat)Opacity;
		if (Background is { } background)
			native.BackgroundColor = background.ToUIColor();
		native.ClipsToBounds = ClipsToBounds || CornerRadius > 0 || ClipsByDefault;
		native.Layer.CornerRadius = (nfloat)CornerRadius;
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
			native.SetNeedsLayout();
	}
}
