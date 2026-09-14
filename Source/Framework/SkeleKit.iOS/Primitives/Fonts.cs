namespace SkeleKit;

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
	
	// NaN = no bound; equal bounds produce a fixed point size.
	static UIFont Scale(
		UIFontMetrics metrics,
		UIFont font,
		double min,
		double max) =>
		Constrain(
			double.IsNaN(max)
				? metrics.GetScaledFont(font)
				: metrics.GetScaledFont(font, (nfloat)max),
			min,
			max);

	static UIFont Constrain(
		UIFont font,
		double min,
		double max)
	{
		double size = FontSpec.ConstrainSize((double)font.PointSize, min, max);

		return Math.Abs((double)font.PointSize - size) > 0.001
			? font.WithSize((nfloat)size)
			: font;
	}


	public static UIFont Scaled(
		double size,
		bool bold) =>
		Scaled(size, bold ? FontWeight.Bold : FontWeight.Regular, FontDesign.Default, double.NaN, double.NaN);

	public static UIFont Scaled(
		double size,
		FontWeight weight,
		FontDesign design,
		double min = double.NaN,
		double max = double.NaN)
	{
		UIFont font = UIFont.SystemFontOfSize((nfloat)size, Weight(weight));

		if (design is not FontDesign.Default
			&& font.FontDescriptor.CreateWithDesign(Design(design)) is UIFontDescriptor descriptor)
			font = UIFont.FromDescriptor(descriptor, (nfloat)size);

		return Scale(UIFontMetrics.DefaultMetrics, font, min, max);
	}

	public static UIFont Preferred(
		TextStyle style,
		FontWeight weight,
		FontDesign design,
		double min = double.NaN,
		double max = double.NaN)
	{
		UIFontTextStyle native = Style(style);

		if (weight is FontWeight.Regular
			&& design is FontDesign.Default
			&& double.IsNaN(min)
			&& double.IsNaN(max))
			return UIFont.GetPreferredFontForTextStyle(native);

		UIFontDescriptor descriptor = UIFontDescriptor.GetPreferredDescriptorForTextStyle(
			native,
			UITraitCollection.Create(UIContentSizeCategory.Large));

		UIFont font = UIFont.SystemFontOfSize(descriptor.PointSize, Weight(weight));

		if (design is not FontDesign.Default
			&& font.FontDescriptor.CreateWithDesign(Design(design)) is UIFontDescriptor designed)
			font = UIFont.FromDescriptor(designed, descriptor.PointSize);

		return Scale(UIFontMetrics.GetMetrics(native.GetConstant()!), font, min, max);
	}
}
