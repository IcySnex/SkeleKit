namespace SkeleKit;

/// <summary>
/// How a view is placed within the horizontal space its parent gives it.
/// </summary>
public enum HorizontalAlignment
{
	/// <summary>
	/// Fills the available width.
	/// </summary>
	Stretch,

	/// <summary>
	/// Sized to content, pinned to the leading (left) edge.
	/// </summary>
	Start,

	/// <summary>
	/// Sized to content, centered.
	/// </summary>
	Center,

	/// <summary>
	/// Sized to content, pinned to the trailing (right) edge.
	/// </summary>
	End
}

/// <summary>
/// How a view is placed within the vertical space its parent gives it.
/// </summary>
public enum VerticalAlignment
{
	/// <summary>
	/// Fills the available height.
	/// </summary>
	Stretch,

	/// <summary>
	/// Sized to content, pinned to the top edge.
	/// </summary>
	Start,

	/// <summary>
	/// Sized to content, centered.
	/// </summary>
	Center,

	/// <summary>
	/// Sized to content, pinned to the bottom edge.
	/// </summary>
	End
}
