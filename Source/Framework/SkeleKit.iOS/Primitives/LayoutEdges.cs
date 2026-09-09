namespace SkeleKit;

/// <summary>
/// Edges participating in a layout operation.
/// </summary>
[Flags]
public enum LayoutEdges
{
	/// <summary>
	/// No edges.
	/// </summary>
	None = 0,

	/// <summary>
	/// The top edge.
	/// </summary>
	Top = 1 << 0,

	/// <summary>
	/// The bottom edge.
	/// </summary>
	Bottom = 1 << 1,

	/// <summary>
	/// The leading edge.
	/// </summary>
	Leading = 1 << 2,

	/// <summary>
	/// The trailing edge.
	/// </summary>
	Trailing = 1 << 3,

	/// <summary>
	/// The leading and trailing edges.
	/// </summary>
	Horizontal = Leading | Trailing,

	/// <summary>
	/// The top and bottom edges.
	/// </summary>
	Vertical = Top | Bottom,

	/// <summary>
	/// All edges.
	/// </summary>
	All = Vertical | Horizontal
}
