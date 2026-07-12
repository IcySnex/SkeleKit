namespace BareUI;

/// <summary>
/// An axis-aligned rectangle (location plus size), produced by the arrangement pass.
/// </summary>
public readonly record struct Rect(
	double X,
	double Y,
	double Width,
	double Height)
{
	/// <summary>
	/// Creates a rectangle from a location and a size.
	/// </summary>
	public Rect(
		Point location,
		Size size) : this(location.X, location.Y, size.Width, size.Height)
	{ }


	/// <summary>
	/// A rectangle at the origin with zero size.
	/// </summary>
	public static readonly Rect Zero = new(0, 0, 0, 0);


	/// <summary>
	/// The left edge (<see cref="X"/>).
	/// </summary>
	public double Left => X;

	/// <summary>
	/// The top edge (<see cref="Y"/>).
	/// </summary>
	public double Top => Y;

	/// <summary>
	/// The right edge (<see cref="X"/> + <see cref="Width"/>).
	/// </summary>
	public double Right => X + Width;

	/// <summary>
	/// The bottom edge (<see cref="Y"/> + <see cref="Height"/>).
	/// </summary>
	public double Bottom => Y + Height;

	/// <summary>
	/// The top-left corner.
	/// </summary>
	public Point Location => new(X, Y);

	/// <summary>
	/// The width/height of the rectangle.
	/// </summary>
	public Size Size => new(Width, Height);


	/// <summary>
	/// Returns this rectangle inset by <paramref name="thickness"/>, clamped so size never goes negative.
	/// </summary>
	public Rect Deflate(
		Thickness thickness) =>
		new(X + thickness.Left, Y + thickness.Top, Math.Max(0, Width - thickness.Horizontal), Math.Max(0, Height - thickness.Vertical));
}
