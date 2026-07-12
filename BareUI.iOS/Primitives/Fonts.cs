using UIKit;

namespace BareUI;

internal static class Fonts
{
	static UIFontTextStyle Style(
		TextStyle style) =>
		style switch
		{
			TextStyle.LargeTitle => UIFontTextStyle.LargeTitle,
			TextStyle.Title1 => UIFontTextStyle.Title1,
			TextStyle.Title2 => UIFontTextStyle.Title2,
			TextStyle.Title3 => UIFontTextStyle.Title3,
			TextStyle.Headline => UIFontTextStyle.Headline,
			TextStyle.Subheadline => UIFontTextStyle.Subheadline,
			TextStyle.Callout => UIFontTextStyle.Callout,
			TextStyle.Footnote => UIFontTextStyle.Footnote,
			TextStyle.Caption1 => UIFontTextStyle.Caption1,
			TextStyle.Caption2 => UIFontTextStyle.Caption2,
			_ => UIFontTextStyle.Body
		};

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

	public static UIFont Preferred(
		TextStyle style,
		FontWeight weight,
		FontDesign design)
	{
		UIFontTextStyle native = Style(style);

		if (weight is FontWeight.Regular && design is FontDesign.Default)
			return UIFont.GetPreferredFontForTextStyle(native);

		UIFontDescriptor descriptor = UIFontDescriptor.GetPreferredDescriptorForTextStyle(
			native,
			UITraitCollection.Create(UIContentSizeCategory.Large));

		UIFont font = UIFont.SystemFontOfSize(descriptor.PointSize, Weight(weight));

		if (design is not FontDesign.Default
			&& font.FontDescriptor.CreateWithDesign(Design(design)) is { } designed)
			font = UIFont.FromDescriptor(designed, descriptor.PointSize);

		return UIFontMetrics.GetMetrics(native.GetConstant()!).GetScaledFont(font);
	}
}
