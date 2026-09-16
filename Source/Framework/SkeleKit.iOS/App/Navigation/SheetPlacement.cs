namespace SkeleKit;

/// <summary>
/// Where a sheet is positioned within its presenting view.
/// </summary>
public enum SheetPlacement
{
	/// <summary>
	/// Lets the system choose the placement.
	/// </summary>
	Automatic,

	/// <summary>
	/// Places the sheet at the semantic leading edge.
	/// </summary>
	Leading,

	/// <summary>
	/// Centers the sheet.
	/// </summary>
	Center,

	/// <summary>
	/// Places the sheet at the semantic trailing edge.
	/// </summary>
	Trailing
}
