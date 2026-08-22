using CoreGraphics;
using UIKit;

namespace SkeleKit.Gallery.Views.Showcase;

internal sealed class DotGridView : UIView
{
	const double Spacing = 16;
	const double Radius = 1;


	public DotGridView()
	{
		BackgroundColor = UIColor.Clear;
		UserInteractionEnabled = false;
	}


	public override void Draw(
		CGRect rect)
	{
		CGContext? context = UIGraphics.GetCurrentContext();
		if (context is null)
			return;

		context.SetFillColor(UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 0.22f).CGColor);

		for (double x = Spacing / 2; x < Bounds.Width; x += Spacing)
		{
			for (double y = Spacing / 2; y < Bounds.Height; y += Spacing)
				context.FillEllipseInRect(new(x - Radius, y - Radius, Radius * 2, Radius * 2));
		}
	}
}
