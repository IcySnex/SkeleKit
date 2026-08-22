namespace SkeleKit;

/// <summary>
/// The thickness of a frame around a rectangle, as used for margins and padding.
/// </summary>
/// <param name="Left">The thickness on the left side.</param>
/// <param name="Top">The thickness on the top side.</param>
/// <param name="Right">The thickness on the right side.</param>
/// <param name="Bottom">The thickness on the bottom side.</param>
public readonly record struct Thickness(
	double Left,
	double Top,
	double Right,
	double Bottom)
{
	/// <summary>
	/// Creates a uniform thickness: all four sides use the same value.
	/// </summary>
	/// <param name="uniform">The thickness value for all four sides.</param>
	public Thickness(
		double uniform) : this(uniform, uniform, uniform, uniform)
	{ }

	/// <summary>
	/// Creates a symmetric thickness: <paramref name="horizontal"/> for left/right, <paramref name="vertical"/> for top/bottom.
	/// </summary>
	/// <param name="horizontal">The thickness for the left and right sides.</param>
	/// <param name="vertical">The thickness for the top and bottom sides.</param>
	public Thickness(
		double horizontal,
		double vertical) : this(horizontal, vertical, horizontal, vertical)
	{ }


	/// <summary>
	/// A thickness of zero on all sides.
	/// </summary>
	public static readonly Thickness Zero = new(0);

	/// <summary>
	/// Creates a uniform thickness from a single numeric value.
	/// </summary>
	/// <param name="uniform">The uniform thickness value.</param>
	public static implicit operator Thickness(
		double uniform) =>
		new(uniform);


	/// <summary>
	/// The total thickness on the horizontal axis (<see cref="Left"/> + <see cref="Right"/>).
	/// </summary>
	public double Horizontal => Left + Right;

	/// <summary>
	/// The total thickness on the vertical axis (<see cref="Top"/> + <see cref="Bottom"/>).
	/// </summary>
	public double Vertical => Top + Bottom;
}
