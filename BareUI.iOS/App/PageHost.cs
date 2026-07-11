using Foundation;
using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace BareUI;

/// <summary>
/// The hidden <c>UIViewController</c> that hosts one <see cref="ContentView"/>. App code never sees it.
/// </summary>
sealed class PageHost : UIViewController
{
	readonly ContentView? page;

	UITapGestureRecognizer? dismissKeyboard;
	UIView? keyboardFocus;
	nfloat keyboardCover;

	public PageHost(
		ContentView page)
	{
		this.page = page;
		page.Host = this;

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

		// dynamic type resizes the fonts under us, so every cached measurement in the tree is wrong
		NSNotificationCenter.DefaultCenter.AddObserver(
			this,
			new Selector("contentSizeChanged:"),
			UIApplication.ContentSizeCategoryChangedNotification,
			null);
	}

	[Export("contentSizeChanged:")]
	void ContentSizeChanged(
		NSNotification notification)
	{
		page?.InvalidateSubtree();
		View?.SetNeedsLayout();
	}

	// marshaller needs this; Navigator keeps the managed ref so it stays unused
	public PageHost(
		NativeHandle handle) : base(handle)
	{ }

	/// <summary>
	/// The page this controller hosts.
	/// </summary>
	public ContentView? Page =>
		page;

	// UIViewController.Title also rewrites the tab bar item, which would clobber the tab's own label
	public void SetTitle(
		string? title) =>
		NavigationItem.Title = title;

	// a scrolling root fills the bounds so UIKit can blur the bars behind it, and insets the content itself
	UIScrollView? ScrollRoot =>
		page?.IsRealized is true ? page.Native.Subviews.FirstOrDefault() as UIScrollView : null;

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		if (page is null)
			return;

		View!.BackgroundColor = UIColor.SystemBackground;
		SetTitle(page.Title.Value);

		View.AddSubview(page.Realize());

		// numeric keyboards have no return key, so tapping outside is the only way out
		dismissKeyboard = new(() => View.EndEditing(true))
		{
			CancelsTouchesInView = false
		};
		View.AddGestureRecognizer(dismissKeyboard);

		if (ScrollRoot is { } scroll)
			scroll.ContentInsetAdjustmentBehavior = page.SafeAreaEdges is SafeAreaEdges.None
				? UIScrollViewContentInsetAdjustmentBehavior.Never
				: UIScrollViewContentInsetAdjustmentBehavior.ScrollableAxes;
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (page is null)
			return;

		// a scrolling root insets its own content for the keyboard; anything else has to shrink
		if (ScrollRoot is not null)
		{
			// frame set drives measure/arrange via LayoutSubviews
			page.Native.Frame = View!.Bounds;
			return;
		}

		CGRect frame = Inset(View!.Bounds, View.SafeAreaInsets, page.SafeAreaEdges);
		CGRect shrunk = new(
			frame.X,
			frame.Y,
			frame.Width,
			(nfloat)Math.Max(0, frame.Height - keyboardCover));

		page.Native.Frame = shrunk;

		if (keyboardCover <= 0 || keyboardFocus is not { } focused)
			return;

		// shrinking reflows a layout that can adapt; a top-anchored field just gets clipped, so
		// slide the page up until the focused control clears the keyboard
		page.Native.LayoutIfNeeded();

		CGRect target = focused.ConvertRectToView(focused.Bounds, View);
		nfloat hidden = target.GetMaxY() + 8 - shrunk.GetMaxY();

		if (hidden > 0)
			page.Native.Frame = new(
				shrunk.X,
				shrunk.Y - (nfloat)Math.Min(hidden, keyboardCover),
				shrunk.Width,
				shrunk.Height);
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

	// SwiftUI treats the keyboard as a shrink of the safe area, so any layout adapts — not just scrolling ones
	[Export("keyboardFrameChanged:")]
	void KeyboardFrameChanged(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: false);

	[Export("keyboardHidden:")]
	void KeyboardHidden(
		NSNotification notification) =>
		ApplyKeyboard(notification, hiding: true);

	void ApplyKeyboard(
		NSNotification notification,
		bool hiding)
	{
		// the scroll view handles its own case, and doubling up would inset twice
		if (page is null || ScrollRoot is not null || View?.Window is null)
			return;

		// resolved once here, not on every layout pass during the animation
		keyboardFocus = hiding ? null : FirstResponder(page.Native);

		nfloat cover = 0;

		if (!hiding)
		{
			CGRect keyboard = UIKeyboard.FrameEndFromNotification(notification);
			CGRect pageInWindow = View.ConvertRectToView(View.Bounds, null);

			// the safe-area bottom is already deducted by Inset, so do not count it twice
			cover = (nfloat)Math.Max(
				0,
				pageInWindow.GetMaxY() - keyboard.GetMinY() - View.SafeAreaInsets.Bottom);
		}

		if (cover == keyboardCover)
			return;

		keyboardCover = cover;

		double duration = UIKeyboard.AnimationDurationFromNotification(notification);

		UIView.Animate(duration, () =>
		{
			View.SetNeedsLayout();
			View.LayoutIfNeeded();
		});
	}

	public override void ViewDidAppear(
		bool animated)
	{
		base.ViewDidAppear(animated);

		page?.NotifyAppearing();
	}

	public override void ViewDidDisappear(
		bool animated)
	{
		base.ViewDidDisappear(animated);

		page?.NotifyDisappearing();

		// popped for good, not just covered
		if (IsMovingFromParentViewController)
			page?.Unrealize();
	}

	protected override void Dispose(
		bool disposing)
	{
		if (disposing)
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

		base.Dispose(disposing);
	}

	static CGRect Inset(
		CGRect bounds,
		UIEdgeInsets insets,
		SafeAreaEdges edges)
	{
		nfloat top = edges.HasFlag(SafeAreaEdges.Top) ? insets.Top : 0;
		nfloat bottom = edges.HasFlag(SafeAreaEdges.Bottom) ? insets.Bottom : 0;
		nfloat leading = edges.HasFlag(SafeAreaEdges.Leading) ? insets.Left : 0;
		nfloat trailing = edges.HasFlag(SafeAreaEdges.Trailing) ? insets.Right : 0;

		return new(
			bounds.X + leading,
			bounds.Y + top,
			bounds.Width - leading - trailing,
			bounds.Height - top - bottom);
	}
}
