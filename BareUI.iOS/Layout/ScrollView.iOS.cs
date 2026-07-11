using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace BareUI;

public partial class ScrollView
{
	private protected override UIView CreateNative() =>
		new ScrollHost(this);

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
