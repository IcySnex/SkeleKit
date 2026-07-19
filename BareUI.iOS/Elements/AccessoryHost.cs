using ObjCRuntime;

namespace BareUI;

internal sealed class AccessoryHost : UIView
{
	internal static AccessoryHost ForKeyboard(
		View content)
	{
		AccessoryHost host = new(content);

		double width = UIScreen.MainScreen.Bounds.Width;
		content.Measure(new(width, double.PositiveInfinity));
		host.Frame = new(0, 0, width, content.DesiredSize.Height);

		return host;
	}


	readonly View? content;

	public AccessoryHost(
		View content)
	{
		this.content = content;

		AddSubview(content.Realize());

		RegisterForTraitChanges([typeof(UITraitUserInterfaceStyle)], (_, _) => this.content?.ReapplyVisuals());
	}

	public AccessoryHost(
		NativeHandle handle) : base(handle)
	{ }


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
