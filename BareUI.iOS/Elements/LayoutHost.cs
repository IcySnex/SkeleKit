#if IOS
using BareUI.Primitives;
using CoreGraphics;
using UIKit;

namespace BareUI;

/// <summary>
/// The native <c>UIView</c> hosting a BareUI panel, bridging UIKit's sizing protocol to the measure/arrange engine.
/// </summary>
sealed class LayoutHost : UIView
{
	readonly View element;

	public LayoutHost(
		View element)
	{
		this.element = element;
	}


	public override CGSize SizeThatFits(
		CGSize size)
	{
		Size desired = element.HostMeasure(new(size.Width, size.Height));
		return new CGSize(desired.Width, desired.Height);
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();
		element.HostLayout(new(Bounds.Width, Bounds.Height));
	}
}
#endif
