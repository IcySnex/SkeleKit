namespace BareUI;

/// <summary>
/// A drop shadow behind a view.
/// </summary>
/// <param name="Opacity">The shadow intensity from 0.0 (invisible) to 1.0 (fully opaque).</param>
/// <param name="Radius">The blur radius of the shadow edges.</param>
/// <param name="OffsetX">The horizontal displacement of the shadow.</param>
/// <param name="OffsetY">The vertical displacement of the shadow.</param>
/// <param name="Color">The color of the shadow, or null to use the system default.</param>
public readonly record struct Shadow(
	double Opacity,
	double Radius,
	double OffsetX,
	double OffsetY,
	Color? Color)
{
	/// <summary>
	/// A shadow offset straight down, in the default shadow color.
	/// </summary>
	public Shadow(
		double opacity,
		double radius,
		double offsetY = 0) : this(opacity, radius, 0, offsetY, null)
	{ }
}
