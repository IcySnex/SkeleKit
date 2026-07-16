using CoreGraphics;
using ObjCRuntime;

namespace BareUI;

// bridges an element tree into the auto-laid-out accessory slot
internal sealed class AccessoryHost : UIView
{
	readonly View? content;
	readonly IUITraitChangeRegistration? themeChange;

	public AccessoryHost(
		View content)
	{
		this.content = content;

		AddSubview(content.Realize());

		themeChange = RegisterForTraitChanges([typeof(UITraitUserInterfaceStyle)], (_, _) => this.content?.ReapplyVisuals());
	}

	// see LayoutHost
	public AccessoryHost(
		NativeHandle handle) : base(handle)
	{ }

	// the keyboard sizes an accessory by its frame; intrinsic size is never consulted
	internal static AccessoryHost ForKeyboard(
		View content)
	{
		AccessoryHost host = new(content);

		double width = UIScreen.MainScreen.Bounds.Width;
		content.Measure(new(width, double.PositiveInfinity));
		host.Frame = new(0, 0, width, content.DesiredSize.Height);

		return host;
	}


	public override CGSize IntrinsicContentSize
	{
		get
		{
			if (content is null)
				return CGSize.Empty;

			// unsized until the slot lays out: probe at screen width
			double width = Bounds.Width > 0 ? Bounds.Width : UIScreen.MainScreen.Bounds.Width;
			content.Measure(new(width, double.PositiveInfinity));

			return new(NoIntrinsicMetric, (nfloat)content.DesiredSize.Height);
		}
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		if (content is null)
			return;

		content.Measure(new(Bounds.Width, Bounds.Height));
		content.Arrange(new(0, 0, Bounds.Width, Bounds.Height));
	}
}
