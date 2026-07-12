namespace BareUI;

/// <summary>
/// How a view's background is filled: a solid color, a gradient, or a blurred material.
/// </summary>
public abstract class Brush
{
	private protected Brush()
	{ }

	/// <summary>
	/// Fills with a solid color.
	/// </summary>
	public static implicit operator Brush(
		Color color) =>
		new SolidBrush(color);
}

/// <summary>
/// A single flat color.
/// </summary>
public sealed class SolidBrush(
	Color color) : Brush
{
	/// <summary>
	/// The color painted.
	/// </summary>
	public Color Color { get; } = color;
}

/// <summary>
/// One color of a gradient, at a position along it.
/// </summary>
public readonly record struct GradientStop(
	Color Color,
	double Offset);

/// <summary>
/// A linear gradient between two points, given in unit space: (0,0) is the top-left corner, (1,1) the bottom-right.
/// </summary>
public sealed class LinearGradient : Brush
{
	/// <summary>
	/// The colors and where they sit along the gradient.
	/// </summary>
	public IList<GradientStop> Stops { get; init; } = [];

	/// <summary>
	/// Where the gradient starts, in unit space. Top-centre by default.
	/// </summary>
	public Point Start { get; init; } = new(0.5, 0);

	/// <summary>
	/// Where the gradient ends, in unit space. Bottom-centre by default.
	/// </summary>
	public Point End { get; init; } = new(0.5, 1);


	/// <summary>
	/// A top-to-bottom gradient through evenly spaced colors.
	/// </summary>
	public static LinearGradient Vertical(
		params Color[] colors) =>
		new() { Stops = Spread(colors) };

	/// <summary>
	/// A leading-to-trailing gradient through evenly spaced colors.
	/// </summary>
	public static LinearGradient Horizontal(
		params Color[] colors) =>
		new()
		{
			Stops = Spread(colors),
			Start = new(0, 0.5),
			End = new(1, 0.5)
		};

	static List<GradientStop> Spread(
		Color[] colors)
	{
		List<GradientStop> stops = new(colors.Length);

		for (int index = 0; index < colors.Length; index++)
			stops.Add(new(colors[index], colors.Length == 1 ? 0 : index / (double)(colors.Length - 1)));

		return stops;
	}
}

/// <summary>
/// A blurred material, as used behind bars and sheets. Thinner materials let more of the content behind them through.
/// </summary>
public sealed class Material(
	MaterialKind kind) : Brush
{
	/// <summary>
	/// How much the material blurs what sits behind it.
	/// </summary>
	public MaterialKind Kind { get; } = kind;
}

/// <summary>
/// The thickness of a <see cref="Material"/>, mapping to the system blur styles.
/// </summary>
public enum MaterialKind
{
	/// <summary>The thinnest material; most of the content behind shows through.</summary>
	UltraThin,

	/// <summary>A thin material.</summary>
	Thin,

	/// <summary>The default material, as used behind sheets.</summary>
	Regular,

	/// <summary>A thick, mostly opaque material.</summary>
	Thick,

	/// <summary>The material used behind bars and toolbars.</summary>
	Chrome
}
