namespace SkeleKit;

/// <summary>
/// How a view's background is filled: a solid color, a gradient, or a blurred material.
/// </summary>
public abstract class Brush
{
	internal static Brush? Lerp(
		Brush? a,
		Brush? b,
		double t)
	{
		if (ReferenceEquals(a, b))
			return a;

		if (a is SolidBrush solidA && b is SolidBrush solidB)
			return Color.Lerp(solidA.Color, solidB.Color, t) is Color color ? new SolidBrush(color) : null;

		if (a is not LinearGradient gradientA || b is not LinearGradient gradientB
			|| gradientA.Stops.Count != gradientB.Stops.Count)
			return null;

		List<GradientStop> stops = new(gradientA.Stops.Count);

		for (int index = 0; index < gradientA.Stops.Count; index++)
		{
			GradientStop from = gradientA.Stops[index];
			GradientStop to = gradientB.Stops[index];

			if (Color.Lerp(from.Color, to.Color, t) is not Color color)
				return null;

			stops.Add(new(color, from.Offset + (to.Offset - from.Offset) * t));
		}

		return new LinearGradient
		{
			Stops = stops,
			Start = new(
				gradientA.Start.X + (gradientB.Start.X - gradientA.Start.X) * t,
				gradientA.Start.Y + (gradientB.Start.Y - gradientA.Start.Y) * t),
			End = new(
				gradientA.End.X + (gradientB.End.X - gradientA.End.X) * t,
				gradientA.End.Y + (gradientB.End.Y - gradientA.End.Y) * t)
		};
	}


	private protected Brush()
	{ }


	/// <summary>
	/// Fills with a solid color.
	/// </summary>
	/// <param name="color">The solid color to wrap.</param>
	public static implicit operator Brush(
		Color color) =>
		new SolidBrush(color);
}


/// <summary>
/// A single flat color.
/// </summary>
/// <param name="color">The color painted.</param>
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
/// <param name="Color">The color at this step.</param>
/// <param name="Offset">The relative position from 0.0 to 1.0 along the axis.</param>
public readonly record struct GradientStop(
	Color Color,
	double Offset);

/// <summary>
/// A linear gradient between two points, given in unit space: (0,0) is the top-left corner, (1,1) the bottom-right.
/// </summary>
public sealed class LinearGradient : Brush
{
	static List<GradientStop> Spread(
		Color[] colors)
	{
		List<GradientStop> stops = new(colors.Length);

		for (int index = 0; index < colors.Length; index++)
			stops.Add(new(colors[index], colors.Length == 1 ? 0 : index / (double)(colors.Length - 1)));

		return stops;
	}


	/// <summary>
	/// The colors and where they sit along the gradient.
	/// </summary>
	public IList<GradientStop> Stops { get; init; } = [];

	/// <summary>
	/// Where the gradient starts, in unit space. Top-center by default.
	/// </summary>
	public Point Start { get; init; } = new(0.5, 0);

	/// <summary>
	/// Where the gradient ends, in unit space. Bottom-center by default.
	/// </summary>
	public Point End { get; init; } = new(0.5, 1);


	/// <summary>
	/// A top-to-bottom gradient through evenly spaced colors.
	/// </summary>
	/// <param name="colors">The sequence of colors to spread.</param>
	/// <returns>A new vertical linear gradient.</returns>
	public static LinearGradient Vertical(
		params Color[] colors) =>
		new() { Stops = Spread(colors) };

	/// <summary>
	/// A leading-to-trailing gradient through evenly spaced colors.
	/// </summary>
	/// <param name="colors">The sequence of colors to spread.</param>
	/// <returns>A new horizontal linear gradient.</returns>
	public static LinearGradient Horizontal(
		params Color[] colors) =>
		new()
		{
			Stops = Spread(colors),
			Start = new(0, 0.5),
			End = new(1, 0.5)
		};
}


/// <summary>
/// The thickness of a <see cref="Material"/>, mapping to the system blur styles.
/// </summary>
public enum MaterialKind
{
	/// <summary>
	/// The thinnest material; most of the content behind shows through.
	/// </summary>
	UltraThin,

	/// <summary>
	/// A thin material.
	/// </summary>
	Thin,

	/// <summary>
	/// The default material, as used behind sheets.
	/// </summary>
	Regular,

	/// <summary>
	/// A thick, mostly opaque material.
	/// </summary>
	Thick,

	/// <summary>
	/// The material used behind bars and toolbars.
	/// </summary>
	Chrome,

	/// <summary>
	/// The Liquid Glass surface; touches light it up. Renders as Chrome before iOS 26.
	/// </summary>
	Glass
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
