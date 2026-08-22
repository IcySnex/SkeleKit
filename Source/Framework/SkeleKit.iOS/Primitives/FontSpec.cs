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
}
