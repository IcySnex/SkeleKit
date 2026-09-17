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
