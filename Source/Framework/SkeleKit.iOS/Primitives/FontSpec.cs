namespace SkeleKit;

// a text style brings its Dynamic Type curve, an explicit size overrides it, weight and design compose on top
internal static class FontSpec
{
	public const double DefaultSize = 17;


	public static bool UsesTextStyle(
		TextStyle? textStyle,
		double size) =>
		textStyle is not null && double.IsNaN(size);

	public static double SizeOf(
		double size) =>
		double.IsNaN(size) ? DefaultSize : size;

	public static double ConstrainSize(
		double size,
		double min,
		double max)
	{
		if (!double.IsNaN(min) && !double.IsNaN(max) && min > max)
			throw new ArgumentOutOfRangeException(nameof(min), "Minimum font size cannot exceed maximum font size.");

		if (!double.IsNaN(min))
			size = Math.Max(size, min);
		if (!double.IsNaN(max))
			size = Math.Min(size, max);

		return size;
	}
}
