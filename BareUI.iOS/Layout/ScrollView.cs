#if IOS
using CoreGraphics;
using ObjCRuntime;
using UIKit;
#endif

namespace BareUI;

/// <summary>
/// A scrolling container for a single child, backed by <c>UIScrollView</c>.
/// </summary>
public class ScrollView : Panel
{
	/// <summary>
	/// The scroll axis.
	/// </summary>
	public Orientation Orientation { get; set; } = Orientation.Vertical;

	/// <summary>
	/// The single scrollable child.
	/// </summary>
	public View? Content
	{
		get => Children.Count > 0 ? Children[0] : null;
		set
		{
			Children.Clear();
			if (value is not null)
				Children.Add(value);
		}
	}


	protected override Size MeasureOverride(
		Size availableSize)
	{
		View? content = Content;
		if (content is null)
			return Size.Zero;

		bool vertical = Orientation == Orientation.Vertical;
		Size probe = vertical
			? new(availableSize.Width, double.PositiveInfinity)
			: new(double.PositiveInfinity, availableSize.Height);

		content.Measure(probe);
		Size desired = content.DesiredSize;

		// fill finite dimension, else size to content
		double width = vertical
			? Fill(availableSize.Width, desired.Width)
			: desired.Width;
		double height = vertical
			? desired.Height
			: Fill(availableSize.Height, desired.Height);

		return new(width, height);
	}

	static double Fill(
		double available,
		double desired) =>
		double.IsFinite(available) ? available : desired;

#if IOS
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
#endif
}

#if IOS
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
#endif
