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
}
