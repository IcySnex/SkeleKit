namespace SkeleKit;

/// <summary>
/// A marker placed on a <c>MapView</c> at a coordinate.
/// </summary>
/// <param name="coordinate">Where the pin sits.</param>
public class MapPin(
	Coordinate coordinate)
{
	/// <summary>
	/// Where the pin sits.
	/// </summary>
	public Coordinate Coordinate { get; set; } = coordinate;

	/// <summary>
	/// The title shown in the pin's callout, or null for none.
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// The subtitle shown under the title in the pin's callout, or null for none.
	/// </summary>
	public string? Subtitle { get; set; }

	/// <summary>
	/// The SF Symbol drawn inside the marker, or null for the default dot.
	/// </summary>
	public string? Symbol { get; set; }

	/// <summary>
	/// The marker's fill color, or null to follow the map tint.
	/// </summary>
	public Color? Tint { get; set; }

	/// <summary>
	/// Builds a custom marker view, or null for the native marker styled by the properties above.
	/// </summary>
	/// <remarks>
	/// Called when the marker comes on screen; return a fresh tree each time.
	/// </remarks>
	public Func<View>? Marker { get; set; }

	/// <summary>
	/// Builds a custom callout view shown when the pin is tapped, or null for the native title and subtitle bubble.
	/// </summary>
	/// <remarks>
	/// Called when the marker comes on screen; return a fresh tree each time.
	/// </remarks>
	public Func<View>? Callout { get; set; }
}
