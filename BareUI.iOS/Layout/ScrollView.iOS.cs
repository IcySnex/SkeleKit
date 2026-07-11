using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace BareUI;

public partial class ScrollView
{
	private protected override UIView CreateNative() =>
		new ScrollHost(this);

	private protected override void ApplyProperties() =>
		ApplyKeyboardDismiss();

	partial void ApplyKeyboardDismissCore() =>
		((UIScrollView)Native).KeyboardDismissMode = keyboardDismiss switch
		{
			KeyboardDismiss.OnDrag => UIScrollViewKeyboardDismissMode.OnDrag,
			KeyboardDismiss.Interactive => UIScrollViewKeyboardDismissMode.Interactive,
			_ => UIScrollViewKeyboardDismissMode.None
		};

	partial void ArrangeContent(
		Size viewport) =>
		LayoutContent(viewport);

	// lay out content, report scrollable size
	internal void LayoutContent(
		Size viewport)
	{
		UIScrollView host = (UIScrollView)Native;

		View? content = Content;
		if (content is null)
		{
			host.ContentSize = CGSize.Empty;
			return;
		}

		// the scroll view fills the bounds, but UIKit insets the content by the safe area — laying
		// out against the raw bounds makes the content wider than what is visible
		UIEdgeInsets inset = host.AdjustedContentInset;
		viewport = new(
			Math.Max(0, viewport.Width - inset.Left - inset.Right),
			Math.Max(0, viewport.Height - inset.Top - inset.Bottom));

		bool vertical = Orientation == Orientation.Vertical;
		Size probe = vertical
			? new(viewport.Width, double.PositiveInfinity)
			: new(double.PositiveInfinity, viewport.Height);

		content.Measure(probe);
		Size desired = content.DesiredSize;

		double width = vertical ? viewport.Width : desired.Width;
		double height = vertical ? desired.Height : viewport.Height;

		content.Arrange(new(0, 0, width, height));
		host.ContentSize = new CGSize(width, height);
	}

	// UIKit never moves anything for the keyboard: inset the content by however much it covers,
	// then scroll the focused control back into view
	internal void OnKeyboardChanged(
		NSNotification notification,
		bool hiding)
	{
		if (!AvoidsKeyboard || !IsRealized)
			return;

		UIScrollView host = (UIScrollView)Native;
		if (host.Window is null)
			return;

		// the keyboard may belong to a field in some other scroll view
		UIView? focused = FirstResponder(host);
		if (!hiding && focused is null)
			return;

		nfloat covered = 0;

		if (!hiding)
		{
			CGRect keyboard = UIKeyboard.FrameEndFromNotification(notification);
			CGRect hostInWindow = host.ConvertRectToView(host.Bounds, null);

			// the safe-area bottom is already in the adjusted inset, so do not count it twice
			covered = (nfloat)Math.Max(
				0,
				hostInWindow.GetMaxY() - keyboard.GetMinY() - host.SafeAreaInsets.Bottom);
		}

		UIEdgeInsets content = host.ContentInset;
		content.Bottom = covered;

		UIEdgeInsets indicator = host.VerticalScrollIndicatorInsets;
		indicator.Bottom = covered;

		double duration = UIKeyboard.AnimationDurationFromNotification(notification);

		UIView.Animate(duration, () =>
		{
			host.ContentInset = content;
			host.VerticalScrollIndicatorInsets = indicator;
		});

		if (hiding || focused is null)
			return;

		CGRect target = focused.ConvertRectToView(focused.Bounds, host);
		host.ScrollRectToVisible(target.Inset(0, -8), true);
	}

	static UIView? FirstResponder(
		UIView view)
	{
		if (view.IsFirstResponder)
			return view;

		foreach (UIView child in view.Subviews)
			if (FirstResponder(child) is { } found)
				return found;

		return null;
	}
}

/// <summary>
/// The native <c>UIScrollView</c> that hosts a <see cref="ScrollView"/> and drives its content layout.
/// </summary>
sealed class ScrollHost : UIScrollView
{
	readonly ScrollView? element;

	public ScrollHost(
		ScrollView element)
	{
		this.element = element;

		// selector-based, not block-based: a block observer's dispatcher peer can be collected
		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new Selector("keyboardFrameChanged:"),
			UIKeyboard.WillChangeFrameNotification,
			null);

		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new Selector("keyboardHidden:"),
			UIKeyboard.WillHideNotification,
			null);
	}

	// see LayoutHost
	public ScrollHost(
		NativeHandle handle) : base(handle)
	{ }

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		element?.LayoutContent(new(Bounds.Width, Bounds.Height));
	}

	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		element?.OnKeyboardChanged(notification, hiding: false);

	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		element?.OnKeyboardChanged(notification, hiding: true);

	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
	}
}
