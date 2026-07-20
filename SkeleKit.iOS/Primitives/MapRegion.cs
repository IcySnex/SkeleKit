namespace SkeleKit;

/// <summary>
/// A rectangular map extent, a center coordinate plus its span in degrees.
/// </summary>
/// <param name="Center">The coordinate at the middle of the extent.</param>
/// <param name="LatitudeSpan">The north-south height in degrees.</param>
/// <param name="LongitudeSpan">The east-west width in degrees.</param>
public readonly record struct MapRegion(
	Coordinate Center,
	double LatitudeSpan,
	double LongitudeSpan)
{
	/// <summary>
	/// Builds a region spanning roughly the given radius in meters around a center.
	/// </summary>
	/// <param name="center">The coordinate at the middle of the region.</param>
	/// <param name="radiusMeters">The distance from the center to each edge, in meters.</param>
	/// <returns>A region whose span covers the radius on both axes.</returns>
	public static MapRegion FromRadius(
		Coordinate center,
		double radiusMeters)
	{
		double latitudeSpan = radiusMeters * 2 / 111_320;
		double longitudeSpan = latitudeSpan / Math.Max(Math.Cos(center.Latitude * Math.PI / 180), 0.000_001);

		return new(center, latitudeSpan, longitudeSpan);
	}
}
