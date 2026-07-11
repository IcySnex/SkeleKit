using UIKit;

namespace BareUI;

static class Fonts
{
	// UIFontMetrics scales the point size by the user's text-size setting, so accessibility works
	public static UIFont Scaled(
		double size,
		bool bold) =>
		Scaled(size, bold ? FontWeight.Bold : FontWeight.Regular, FontDesign.Default);

	public static UIFont Scaled(
		double size,
		FontWeight weight,
		FontDesign design)
	{
		UIFont font = UIFont.SystemFontOfSize((nfloat)size, Weight(weight));

		if (design is not FontDesign.Default
			&& font.FontDescriptor.CreateWithDesign(Design(design)) is { } descriptor)
			font = UIFont.FromDescriptor(descriptor, (nfloat)size);

		return UIFontMetrics.DefaultMetrics.GetScaledFont(font);
	}

	static UIFontWeight Weight(
		FontWeight weight) =>
		weight switch
		{
			FontWeight.UltraLight => UIFontWeight.UltraLight,
			FontWeight.Thin => UIFontWeight.Thin,
			FontWeight.Light => UIFontWeight.Light,
			FontWeight.Medium => UIFontWeight.Medium,
			FontWeight.Semibold => UIFontWeight.Semibold,
			FontWeight.Bold => UIFontWeight.Bold,
			FontWeight.Heavy => UIFontWeight.Heavy,
			FontWeight.Black => UIFontWeight.Black,
			_ => UIFontWeight.Regular
		};

	static UIFontDescriptorSystemDesign Design(
		FontDesign design) =>
		design switch
		{
			FontDesign.Rounded => UIFontDescriptorSystemDesign.Rounded,
			FontDesign.Serif => UIFontDescriptorSystemDesign.Serif,
			FontDesign.Monospaced => UIFontDescriptorSystemDesign.Monospaced,
			_ => UIFontDescriptorSystemDesign.Default
		};
}
