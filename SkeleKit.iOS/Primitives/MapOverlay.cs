namespace SkeleKit;

/// <summary>
/// A shape drawn onto a <see cref="MapView"/> beneath its pins.
/// </summary>
/// <remarks>
/// Sits in the map's <see cref="MapView.Overlays"/> list; a change to the list or one of its items redraws the shapes.<br/>
/// The concrete shapes are <see cref="MapPolyline"/>, <see cref="MapPolygon"/>, and <see cref="MapCircle"/>; reach for the map's <see cref="View.Native"/> handle to draw anything else.
/// </remarks>
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
	/// A <see cref="MapPolyline"/> has no interior, so its fill is ignored.
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
public sealed class MapPolyline : MapOverlay
{
	/// <summary>
	/// Creates a polyline through a sequence of coordinates.
	/// </summary>
	/// <param name="points">The coordinates the line passes through, in order.</param>
	public MapPolyline(
		Coordinate[] points)
	{
		Points = points;
	}


	/// <summary>
	/// The coordinates the line passes through, in order.
	/// </summary>
	public Coordinate[] Points { get; set; }
}


/// <summary>
/// A closed area bounded by coordinates.
/// </summary>
public sealed class MapPolygon : MapOverlay
{
	/// <summary>
	/// Creates a polygon from its boundary coordinates.
	/// </summary>
	/// <param name="points">The boundary coordinates, in order.</param>
	public MapPolygon(
		Coordinate[] points)
	{
		Points = points;
	}


	/// <summary>
	/// The boundary coordinates, in order.
	/// </summary>
	public Coordinate[] Points { get; set; }
}


/// <summary>
/// A circular area of a fixed radius around a center.
/// </summary>
public sealed class MapCircle : MapOverlay
{
	/// <summary>
	/// Creates a circle from a center and radius.
	/// </summary>
	/// <param name="center">The coordinate at the middle of the circle.</param>
	/// <param name="radiusMeters">The radius in meters.</param>
	public MapCircle(
		Coordinate center,
		double radiusMeters)
	{
		Center = center;
		RadiusMeters = radiusMeters;
	}


	/// <summary>
	/// The coordinate at the middle of the circle.
	/// </summary>
	public Coordinate Center { get; set; }

	/// <summary>
	/// The radius in meters.
	/// </summary>
	public double RadiusMeters { get; set; }
}
