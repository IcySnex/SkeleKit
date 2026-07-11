namespace BareUI;

/// <summary>
/// A width/height pair used throughout the measure/arrange layout engine.
/// </summary>
public readonly record struct Size(
	double Width,
	double Height)
{
	/// <summary>
	/// A size of zero width and height.
	/// </summary>
	public static readonly Size Zero = new(0, 0);

	/// <summary>
	/// A size unconstrained on both axes, used when measuring content that may grow without bound.
	/// </summary>
	public static readonly Size Infinity = new(double.PositiveInfinity, double.PositiveInfinity);


	/// <summary>
	/// True when both axes are finite (neither infinite nor NaN).
	/// </summary>
	public bool IsFinite =>
		double.IsFinite(Width) && double.IsFinite(Height);


	/// <summary>
	/// Returns this size shrunk by <paramref name="thickness"/> on both axes, clamped at zero.
	/// </summary>
	public Size Deflate(
		Thickness thickness) =>
		new(
			Math.Max(0, Width - thickness.Horizontal),
			Math.Max(0, Height - thickness.Vertical));

	/// <summary>
	/// Returns this size grown by <paramref name="thickness"/> on both axes.
	/// </summary>
	public Size Inflate(
		Thickness thickness) =>
		new(
			Width + thickness.Horizontal,
			Height + thickness.Vertical);
}
