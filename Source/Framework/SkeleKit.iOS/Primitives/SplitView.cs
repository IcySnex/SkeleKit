namespace SkeleKit;

/// <summary>
/// The native column arrangement used by a split view controller.
/// </summary>
public enum SplitViewStyle
{
	/// <summary>
	/// A primary and secondary column.
	/// </summary>
	DoubleColumn,

	/// <summary>
	/// A primary, supplementary and secondary column.
	/// </summary>
	TripleColumn
}

/// <summary>
/// The edge that hosts a split view's primary column.
/// </summary>
public enum SplitViewEdge
{
	/// <summary>
	/// The leading edge of the interface.
	/// </summary>
	Leading,

	/// <summary>
	/// The trailing edge of the interface.
	/// </summary>
	Trailing
}

/// <summary>
/// The presentation used when split view columns share the available width.
/// </summary>
public enum SplitViewBehavior
{
	/// <summary>
	/// Lets the system choose the presentation for the available space.
	/// </summary>
	Automatic,

	/// <summary>
	/// Places visible columns side by side.
	/// </summary>
	SideBySide,

	/// <summary>
	/// Places leading columns over the secondary column when space is limited.
	/// </summary>
	Overlay,

	/// <summary>
	/// Lets leading columns displace the secondary column when space is limited.
	/// </summary>
	Displace
}

/// <summary>
/// The preferred number of visible split view columns.
/// </summary>
public enum SplitViewDisplay
{
	/// <summary>
	/// Lets the system choose the visible columns for the available space.
	/// </summary>
	Automatic,

	/// <summary>
	/// Shows only the secondary column.
	/// </summary>
	SecondaryOnly,

	/// <summary>
	/// Shows the secondary column and one adjacent column.
	/// </summary>
	TwoColumns,

	/// <summary>
	/// Shows every column in the selected split view style.
	/// </summary>
	AllColumns
}

/// <summary>
/// A column managed by a split view controller.
/// </summary>
public enum SplitViewColumn
{
	/// <summary>
	/// The leading primary column.
	/// </summary>
	Primary,

	/// <summary>
	/// The middle column of a triple-column split view.
	/// </summary>
	Supplementary,

	/// <summary>
	/// The secondary or detail column.
	/// </summary>
	Secondary,

	/// <summary>
	/// An optional replacement page used while the split view is collapsed.
	/// </summary>
	Compact,

	/// <summary>
	/// The trailing inspector column available on iOS 26 and later.
	/// </summary>
	Inspector
}
