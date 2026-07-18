namespace BareUI;

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

	// when set, the native side resolves the live UIKit color; the channels are a light-mode fallback
	internal SystemColor? System { get; init; }

	// dark-appearance channels; ignored when System is set
	internal (double Red, double Green, double Blue, double Alpha)? Dark { get; init; }

	/// <summary>
	/// A color that resolves per appearance: <paramref name="light"/> normally, <paramref name="dark"/> in dark mode.
	/// </summary>
	public static Color Dynamic(
		Color light,
		Color dark) =>
		light with { System = null, Dark = (dark.Red, dark.Green, dark.Blue, dark.Alpha) };


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
	/// Returns this color with a different <paramref name="alpha"/> (0..1). A system color flattens to its light-mode value.
	/// </summary>
	public Color WithAlpha(
		double alpha) =>
		this with
		{
			System = null,
			Alpha = alpha,
			Dark = Dark.HasValue ? (Dark.Value.Red, Dark.Value.Green, Dark.Value.Blue, alpha) : null
		};

	// straight-channel mix, both appearances of a dynamic pair; a system color resolves natively
	// and cannot be mixed here, so null tells the animation to snap instead
	internal static Color? Lerp(
		Color a,
		Color b,
		double t)
	{
		if (a.System is not null || b.System is not null)
			return null;

		Color mixed = new(
			Mix(a.Red, b.Red),
			Mix(a.Green, b.Green),
			Mix(a.Blue, b.Blue),
			Mix(a.Alpha, b.Alpha));

		if (a.Dark is null && b.Dark is null)
			return mixed;

		(double Red, double Green, double Blue, double Alpha) darkA = a.Dark ?? (a.Red, a.Green, a.Blue, a.Alpha);
		(double Red, double Green, double Blue, double Alpha) darkB = b.Dark ?? (b.Red, b.Green, b.Blue, b.Alpha);

		return mixed with
		{
			Dark = (Mix(darkA.Red, darkB.Red), Mix(darkA.Green, darkB.Green), Mix(darkA.Blue, darkB.Blue), Mix(darkA.Alpha, darkB.Alpha))
		};

		double Mix(
			double from,
			double to) =>
			Math.Clamp(from + ((to - from) * t), 0, 1);
	}
}

/// <summary>
/// The UIKit colors that adapt to appearance, contrast and vibrancy on their own.
/// </summary>
enum SystemColor
{
	Red,
	Orange,
	Yellow,
	Green,
	Mint,
	Teal,
	Cyan,
	Blue,
	Indigo,
	Purple,
	Pink,
	Brown,
	Gray,
	Gray2,
	Gray3,
	Gray4,
	Gray5,
	Gray6,
	Label,
	SecondaryLabel,
	TertiaryLabel,
	PlaceholderText,
	Separator,
	Link,
	Background,
	SecondaryBackground,
	TertiaryBackground,
	GroupedBackground,
	SecondaryGroupedBackground,
	TertiaryGroupedBackground
}
