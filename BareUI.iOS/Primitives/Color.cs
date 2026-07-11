#if IOS
using UIKit;
#endif

namespace BareUI.Primitives;

/// <summary>
/// A straight (non-premultiplied) RGBA color with each channel in the range 0..1.
/// </summary>
public readonly record struct Color(
	double Red,
	double Green,
	double Blue,
	double Alpha)
{
	/// <summary>
	/// Creates an opaque color (alpha 1).
	/// </summary>
	public Color(
		double red,
		double green,
		double blue) : this(red, green, blue, 1.0)
	{ }


	/// <summary>
	/// Fully transparent.
	/// </summary>
	public static readonly Color Transparent = new(0, 0, 0, 0);


	/// <summary>
	/// Creates a color from 8-bit channel values (0..255).
	/// </summary>
	public static Color FromBytes(
		byte red,
		byte green,
		byte blue,
		byte alpha = 255) =>
		new(red / 255.0, green / 255.0, blue / 255.0, alpha / 255.0);

	/// <summary>
	/// Creates a color from a packed <c>0xRRGGBB</c> or <c>0xAARRGGBB</c> hex value.
	/// </summary>
	public static Color FromHex(
		uint hex)
	{
		byte a = (hex >> 24) == 0 ? (byte)255 : (byte)(hex >> 24);
		byte r = (byte)(hex >> 16);
		byte g = (byte)(hex >> 8);
		byte b = (byte)hex;

		return FromBytes(r, g, b, a);
	}


	/// <summary>
	/// Returns this color with a different <paramref name="alpha"/> (0..1).
	/// </summary>
	public Color WithAlpha(
		double alpha) =>
		this with { Alpha = alpha };
}

#if IOS
static class ColorInterop
{
	/// <summary>
	/// Converts a neutral <see cref="Color"/> to its <c>UIColor</c> equivalent.
	/// </summary>
	public static UIColor ToUIColor(
		this Color color) =>
		UIColor.FromRGBA(
			(nfloat)color.Red,
			(nfloat)color.Green,
			(nfloat)color.Blue,
			(nfloat)color.Alpha);
}
#endif
