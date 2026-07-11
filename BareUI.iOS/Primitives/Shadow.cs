namespace BareUI;

/// <summary>
/// A drop shadow behind a view.
/// </summary>
public readonly record struct Shadow(
	double Opacity,
	double Radius,
	double OffsetX,
	double OffsetY,
	Color? Color)
{
	/// <summary>
	/// A shadow offset straight down, in the default shadow colour.
	/// </summary>
	public Shadow(
		double opacity,
		double radius,
		double offsetY = 0) : this(opacity, radius, 0, offsetY, null)
	{ }
}
