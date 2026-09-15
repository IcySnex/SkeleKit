using ObjCRuntime;

namespace SkeleKit;

internal sealed class AccessoryHost : UIView
{
	internal static AccessoryHost ForKeyboard(
		View content,
		UIView? widthSource = null)
	{
		AccessoryHost host = new(content, widthSource)
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth
		};

		double width = host.ResolveWidth();
		double height = host.MeasureHeight(width);
		host.Frame = new(0, 0, double.IsFinite(width) ? width : 0, height);

		return host;
	}


	readonly View? content;
	readonly WeakReference<UIView>? widthSource;
	double measuredHeight = double.NaN;

	public AccessoryHost(
		View content)
		: this(content, null)
	{ }

	AccessoryHost(
		View content,
		UIView? widthSource)
	{
		this.content = content;
		this.widthSource = widthSource is null ? null : new(widthSource);

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

			measuredHeight = MeasureHeight(ResolveWidth());
			return new(NoIntrinsicMetric, (nfloat)measuredHeight);
		}
	}

	public override CGSize SizeThatFits(
		CGSize size)
	{
		if (content is null)
			return CGSize.Empty;

		double width = ResolveWidth(size.Width);
		measuredHeight = MeasureHeight(width);

		return new(
			double.IsFinite(width) ? width : content.DesiredSize.Width,
			measuredHeight);
	}

	public override void MovedToWindow()
	{
		base.MovedToWindow();

		InvalidateIntrinsicContentSize();
		SetNeedsLayout();
	}

	
	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		if (content is null || Bounds.Width <= 0)
			return;

		double previousHeight = measuredHeight;
		measuredHeight = MeasureHeight(Bounds.Width);
		if (!double.IsNaN(previousHeight) && Math.Abs(measuredHeight - previousHeight) > 0.5)
			InvalidateIntrinsicContentSize();

		content.Arrange(new(0, 0, Bounds.Width, Bounds.Height));
	}


	double ResolveWidth(
		double proposed = double.NaN)
	{
		if (Valid(proposed))
			return proposed;

		if (Valid(Bounds.Width))
			return Bounds.Width;

		if (Superview is UIView superview && Valid(superview.Bounds.Width))
			return superview.Bounds.Width;

		if (Window is UIWindow window && Valid(window.Bounds.Width))
			return window.Bounds.Width;

		if (widthSource?.TryGetTarget(out UIView? source) is true)
		{
			if (source.Window is UIWindow sourceWindow && Valid(sourceWindow.Bounds.Width))
				return sourceWindow.Bounds.Width;

			UIView outermost = source;
			while (outermost.Superview is UIView parent)
				outermost = parent;

			if (Valid(outermost.Bounds.Width))
				return outermost.Bounds.Width;

			if (Valid(source.Bounds.Width))
				return source.Bounds.Width;
		}

		return double.PositiveInfinity;
	}

	double MeasureHeight(
		double width)
	{
		if (content is null)
			return 0;

		content.Measure(new(width, double.PositiveInfinity));
		return content.DesiredSize.Height;
	}

	static bool Valid(
		double width) =>
		double.IsFinite(width) && width > 0;
}
