namespace BareUI.Primitives;

/// <summary>
/// Describes the thickness of a frame around a rectangle: margins and paddings.
/// </summary>
public readonly record struct Thickness(
	double Left,
	double Top,
	double Right,
	double Bottom)
{
	/// <summary>
	/// Creates a uniform thickness: all four sides use the same value.
	/// </summary>
	public Thickness(
		double uniform) : this(uniform, uniform, uniform, uniform)
	{ }

	/// <summary>
	/// Creates a symmetric thickness: <paramref name="horizontal"/> for left/right, <paramref name="vertical"/> for top/bottom.
	/// </summary>
	public Thickness(
		double horizontal,
		double vertical) : this(horizontal, vertical, horizontal, vertical)
	{ }


	/// <summary>
	/// A thickness of zero on all sides.
	/// </summary>
	public static readonly Thickness Zero = new(0);


	/// <summary>
	/// The total thickness on the horizontal axis (<see cref="Left"/> + <see cref="Right"/>).
	/// </summary>
	public double Horizontal =>
		Left + Right;

	/// <summary>
	/// The total thickness on the vertical axis (<see cref="Top"/> + <see cref="Bottom"/>).
	/// </summary>
	public double Vertical =>
		Top + Bottom;


	public static implicit operator Thickness(
		double uniform) =>
		new(uniform);
}
