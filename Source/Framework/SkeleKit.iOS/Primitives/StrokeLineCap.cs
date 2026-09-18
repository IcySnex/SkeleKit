namespace SkeleKit;

/// <summary>
/// The shape drawn at the ends of a stroked line or dash.
/// </summary>
public enum StrokeLineCap
{
	/// <summary>
	/// Ends the stroke at its exact endpoint.
	/// </summary>
	Flat,

	/// <summary>
	/// Extends the endpoint with a semicircle whose radius is half the stroke width.
	/// </summary>
	Round,

	/// <summary>
	/// Extends the endpoint with a square whose depth is half the stroke width.
	/// </summary>
	Square
}
