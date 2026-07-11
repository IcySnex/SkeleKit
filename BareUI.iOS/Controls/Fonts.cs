using UIKit;

namespace BareUI;

static class Fonts
{
	// UIFontMetrics scales the point size by the user's text-size setting, so accessibility works
	public static UIFont Scaled(
		double size,
		bool bold)
	{
		UIFont font = bold
			? UIFont.BoldSystemFontOfSize((nfloat)size)
			: UIFont.SystemFontOfSize((nfloat)size);

		return UIFontMetrics.DefaultMetrics.GetScaledFont(font);
	}
}
