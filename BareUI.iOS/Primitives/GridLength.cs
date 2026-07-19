namespace BareUI;

/// <summary>
/// How a <see cref="GridLength"/> is interpreted by the grid layout.
/// </summary>
public enum GridUnitType
{
	/// <summary>
	/// Size to the content of the row or column (the largest child's desired size).
	/// </summary>
	Auto,

	/// <summary>
	/// A fixed size in points.
	/// </summary>
	Pixel,

	/// <summary>
	/// A weighted share of the remaining space after Auto and Pixel tracks are placed.
	/// </summary>
	Star
}

/// <summary>
/// The size of a grid row or column: absolute (points), auto-sized, or a weighted star share.
/// </summary>
public readonly record struct GridLength
{
	/// <summary>
	/// An auto-sized track.
	/// </summary>
	public static readonly GridLength Auto = new(0, GridUnitType.Auto);

	/// <summary>
	/// A single star track (weight 1).
	/// </summary>
	public static readonly GridLength Star = new(1, GridUnitType.Star);


	/// <summary>
	/// A fixed track of <paramref name="points"/> points.
	/// </summary>
	/// <param name="points">The fixed size in layout points.</param>
	/// <returns>A new fixed-size grid length configuration.</returns>
	public static GridLength Pixels(
		double points) =>
		new(points, GridUnitType.Pixel);

	/// <summary>
	/// A star track with the given <paramref name="weight"/> of the remaining space.
	/// </summary>
	/// <param name="weight">The proportional allocation factor.</param>
	/// <returns>A new proportional grid length configuration.</returns>
	public static GridLength Stars(
		double weight) =>
		new(weight, GridUnitType.Star);

	/// <summary>
	/// A fixed track from a point value (so <c>Columns = { 200, GridLength.Star }</c> compiles).
	/// </summary>
	/// <param name="points">The absolute size in layout points.</param>
	public static implicit operator GridLength(
		double points) =>
		Pixels(points);


	GridLength(
		double value,
		GridUnitType type)
	{
		Value = value;
		Type = type;
	}


	/// <summary>
	/// Points for a pixel track, the weight for a star track, ignored for auto.
	/// </summary>
	public double Value { get; }

	/// <summary>
	/// How <see cref="Value"/> is interpreted.
	/// </summary>
	public GridUnitType Type { get; }

	/// <summary>
	/// True for an <see cref="GridUnitType.Auto"/> track.
	/// </summary>
	public bool IsAuto => Type == GridUnitType.Auto;

	/// <summary>
	/// True for a <see cref="GridUnitType.Pixel"/> track.
	/// </summary>
	public bool IsAbsolute => Type == GridUnitType.Pixel;

	/// <summary>
	/// True for a <see cref="GridUnitType.Star"/> track.
	/// </summary>
	public bool IsStar => Type == GridUnitType.Star;
}
