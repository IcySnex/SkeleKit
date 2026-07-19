using ObjCRuntime;

namespace SkeleKit;

internal sealed class LayoutHost : UIView
{
	readonly View? element;

	public LayoutHost(
		View element)
	{
		this.element = element;
	}

	public LayoutHost(
		NativeHandle handle) : base(handle)
	{ }


	public override CGSize SizeThatFits(
		CGSize size)
	{
		if (element is null)
			return CGSize.Empty;

		Size desired = element.HostMeasure(new(size.Width, size.Height));
		return new(desired.Width, desired.Height);
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		element?.HostLayout(new(Bounds.Width, Bounds.Height));
	}
}
