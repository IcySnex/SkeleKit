namespace SkeleKit;

/// <summary>
/// A shape drawn onto a <c>MapView</c> beneath its pins.
/// </summary>
public abstract class MapOverlay
{
	private protected MapOverlay()
	{ }


	/// <summary>
	/// The outline color, or null for none.
	/// </summary>
	public Color? StrokeColor { get; set; }

	/// <summary>
	/// The outline width in points.
	/// </summary>
	public double StrokeWidth { get; set; } = 2;

	/// <summary>
	/// The fill color, or null for none.
	/// </summary>
	/// <remarks>
	/// A <c>MapPolyline</c> has no interior, so its fill is ignored.
	/// </remarks>
	public Color? FillColor { get; set; }

	/// <summary>
	/// The dash lengths of the outline, alternating on and off, or null for a solid line.
	/// </summary>
	public double[]? LineDash { get; set; }
}


/// <summary>
/// An open path connecting coordinates in order.
/// </summary>
/// <param name="points">The coordinates the line passes through, in order.</param>
public sealed class MapPolyline(
	Coordinate[] points) : MapOverlay
{
	/// <summary>
	/// The coordinates the line passes through, in order.
	/// </summary>
	public Coordinate[] Points { get; set; } = points;
}

/// <summary>
/// A closed area bounded by coordinates.
/// </summary>
/// <param name="points">The boundary coordinates, in order.</param>
public sealed class MapPolygon(
	Coordinate[] points) : MapOverlay
{
	/// <summary>
	/// The boundary coordinates, in order.
	/// </summary>
	public Coordinate[] Points { get; set; } = points;
}


/// <summary>
/// A circular area of a fixed radius around a center.
/// </summary>
/// <param name="center">The coordinate at the middle of the circle.</param>
/// <param name="radiusMeters">The radius in meters.</param>
public sealed class MapCircle(
	Coordinate center,
	double radiusMeters) : MapOverlay
{
	/// <summary>
	/// The coordinate at the middle of the circle.
	/// </summary>
	public Coordinate Center { get; set; } = center;

	/// <summary>
	/// The radius in meters.
	/// </summary>
	public double RadiusMeters { get; set; } = radiusMeters;
}
