namespace BareUI;

// how a label picks its font: a text style brings the native Dynamic Type curve with it, an explicit
// size overrides it, and weight and design compose on top of whichever wins
static class FontSpec
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
