namespace BareUI;

/// <summary>
/// A point in the layout coordinate space (origin top-left, y grows downward).
/// </summary>
/// <param name="X">The horizontal coordinate.</param>
/// <param name="Y">The vertical coordinate.</param>
public readonly record struct Point(
	double X,
	double Y)
{
	/// <summary>
	/// The origin (0, 0).
	/// </summary>
	public static readonly Point Zero = new(0, 0);
}
