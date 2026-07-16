namespace BareUI;

/// <summary>
/// When the tab bar minimizes as content scrolls.
/// </summary>
public enum TabBarMinimize
{
	/// <summary>
	/// The bar always stays at full size.
	/// </summary>
	Never,

	/// <summary>
	/// Minimizes when scrolling down.
	/// </summary>
	OnScrollDown,

	/// <summary>
	/// Minimizes when scrolling up.
	/// </summary>
	OnScrollUp
}
