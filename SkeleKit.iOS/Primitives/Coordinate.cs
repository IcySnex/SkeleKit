namespace SkeleKit;

/// <summary>
/// A geographic location in degrees.
/// </summary>
/// <param name="Latitude">Degrees north of the equator, negative for south.</param>
/// <param name="Longitude">Degrees east of the prime meridian, negative for west.</param>
public readonly record struct Coordinate(
	double Latitude,
	double Longitude);
