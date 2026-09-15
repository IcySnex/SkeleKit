namespace SkeleKit;

/// <summary>
/// When the navigation bar minimizes as content scrolls.
/// </summary>
public enum NavigationBarMinimize
{
	/// <summary>
	/// The system determines the minimization behavior.
	/// </summary>
	Automatic,

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

/// <summary>
/// Whether the safe area changes while the navigation bar minimizes.
/// </summary>
public enum NavigationBarMinimizeSafeArea
{
	/// <summary>
	/// The system determines the safe-area behavior.
	/// </summary>
	Automatic,

	/// <summary>
	/// The safe area changes with the bar so content can reflow.
	/// </summary>
	Enabled,

	/// <summary>
	/// The safe area stays at its expanded size while the bar minimizes.
	/// </summary>
	Disabled
}

/// <summary>
/// When a minimized navigation bar returns to full size.
/// </summary>
public enum NavigationBarMinimizeRestore
{
	/// <summary>
	/// Restores when the scroll direction reverses.
	/// </summary>
	Automatic,

	/// <summary>
	/// Restores only when scrolling reaches the content edge.
	/// </summary>
	AtScrollEdge
}
