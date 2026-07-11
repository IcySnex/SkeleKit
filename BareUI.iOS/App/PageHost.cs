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

	public PageHost(
		ContentView page)
	{
		this.page = page;
		page.Host = this;
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

	// a scrolling root fills the bounds so UIKit can blur the bars behind it, and insets the content itself
	UIScrollView? ScrollRoot =>
		page?.IsRealized is true ? page.Native.Subviews.FirstOrDefault() as UIScrollView : null;

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		if (page is null)
			return;

		View!.BackgroundColor = UIColor.SystemBackground;
		Title = page.Title.Value;

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
				: UIScrollViewContentInsetAdjustmentBehavior.Always;
	}

	public override void ViewDidLayoutSubviews()
	{
		base.ViewDidLayoutSubviews();

		if (page is null)
			return;

		// frame set drives measure/arrange via LayoutSubviews
		page.Native.Frame = ScrollRoot is not null
			? View!.Bounds
			: Inset(View!.Bounds, View.SafeAreaInsets, page.SafeAreaEdges);
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
