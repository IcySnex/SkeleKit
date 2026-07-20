namespace SkeleKit;

/// <summary>
/// The base imagery a map draws.
/// </summary>
public enum MapKind
{
	/// <summary>
	/// The default road map.
	/// </summary>
	Standard,

	/// <summary>
	/// The road map with muted colors, so overlaid content stands out.
	/// </summary>
	Muted,

	/// <summary>
	/// Satellite imagery with no labels.
	/// </summary>
	Satellite,

	/// <summary>
	/// Satellite imagery with road and place labels.
	/// </summary>
	Hybrid
}
