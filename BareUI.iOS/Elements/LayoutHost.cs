#if IOS
using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace BareUI;

/// <summary>
/// The native <c>UIView</c> hosting a BareUI panel, bridging UIKit's sizing protocol to the measure/arrange engine.
/// </summary>
sealed class LayoutHost : UIView
{
	readonly View? element;

	public LayoutHost(
		View element)
	{
		this.element = element;
	}

	// marshaller needs this; only hit if the peer was collected, so element is gone
	public LayoutHost(
		NativeHandle handle) : base(handle)
	{ }


	public override CGSize SizeThatFits(
		CGSize size)
	{
		if (element is null)
			return CGSize.Empty;

		Size desired = element.HostMeasure(new(size.Width, size.Height));
		return new CGSize(desired.Width, desired.Height);
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		element?.HostLayout(new(Bounds.Width, Bounds.Height));
	}
}
#endif
