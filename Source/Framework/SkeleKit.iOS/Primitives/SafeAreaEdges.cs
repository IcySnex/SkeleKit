namespace SkeleKit;

/// <summary>
/// The edges of a view that should be inset by the safe area during arrange.
/// </summary>
[Flags]
public enum SafeAreaEdges
{
	/// <summary>
	/// Ignore the safe area on all edges.
	/// </summary>
	None = 0,

	/// <summary>
	/// Inset the top edge.
	/// </summary>
	Top = 1 << 0,

	/// <summary>
	/// Inset the bottom edge.
	/// </summary>
	Bottom = 1 << 1,

	/// <summary>
	/// Inset the leading (left) edge.
	/// </summary>
	Leading = 1 << 2,

	/// <summary>
	/// Inset the trailing (right) edge.
	/// </summary>
	Trailing = 1 << 3,

	/// <summary>
	/// Inset all edges.
	/// </summary>
	All = Top | Bottom | Leading | Trailing
}
