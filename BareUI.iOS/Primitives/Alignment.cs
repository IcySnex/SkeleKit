namespace BareUI;

/// <summary>
/// How a view is placed within the horizontal space its parent gives it.
/// </summary>
public enum HorizontalAlignment
{
	/// <summary>Fills the available width.</summary>
	Stretch,

	/// <summary>Sized to content, pinned to the leading (left) edge.</summary>
	Start,

	/// <summary>Sized to content, centered.</summary>
	Center,

	/// <summary>Sized to content, pinned to the trailing (right) edge.</summary>
	End
}

/// <summary>
/// How a view is placed within the vertical space its parent gives it.
/// </summary>
public enum VerticalAlignment
{
	/// <summary>Fills the available height.</summary>
	Stretch,

	/// <summary>Sized to content, pinned to the top edge.</summary>
	Start,

	/// <summary>Sized to content, centered.</summary>
	Center,

	/// <summary>Sized to content, pinned to the bottom edge.</summary>
	End
}

/// <summary>
/// Horizontal alignment of text within a control.
/// </summary>
public enum TextAlignment
{
	/// <summary>Aligned to the leading (left) edge.</summary>
	Leading,

	/// <summary>Centered.</summary>
	Center,

	/// <summary>Aligned to the trailing (right) edge.</summary>
	Trailing
}

/// <summary>
/// The stacking axis of a <c>StackPanel</c>.
/// </summary>
public enum Orientation
{
	/// <summary>Children stacked top to bottom.</summary>
	Vertical,

	/// <summary>Children laid out leading to trailing.</summary>
	Horizontal
}

/// <summary>
/// The edges of a view that should be inset by the safe area during arrange.
/// </summary>
[Flags]
public enum SafeAreaEdges
{
	/// <summary>Ignore the safe area on all edges.</summary>
	None = 0,

	/// <summary>Inset the top edge.</summary>
	Top = 1 << 0,

	/// <summary>Inset the bottom edge.</summary>
	Bottom = 1 << 1,

	/// <summary>Inset the leading (left) edge.</summary>
	Leading = 1 << 2,

	/// <summary>Inset the trailing (right) edge.</summary>
	Trailing = 1 << 3,

	/// <summary>Inset all edges.</summary>
	All = Top | Bottom | Leading | Trailing
}
