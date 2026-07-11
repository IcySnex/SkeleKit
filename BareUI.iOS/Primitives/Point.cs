namespace BareUI.Primitives;

/// <summary>
/// A point in the layout coordinate space (origin top-left, y grows downward).
/// </summary>
public readonly record struct Point(
	double X,
	double Y)
{
	/// <summary>
	/// The origin (0, 0).
	/// </summary>
	public static readonly Point Zero = new(0, 0);
}
