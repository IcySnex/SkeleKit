namespace SkeleKit;

/// <summary>
/// How a tab takes part in iPad user customization.
/// </summary>
public enum TabPlacement
{
	/// <summary>
	/// The system default: fully customizable.
	/// </summary>
	Automatic,

	/// <summary>
	/// Exempt from customization: cannot be hidden or moved.
	/// </summary>
	Locked,

	/// <summary>
	/// Anchored at the trailing end of the bar.
	/// </summary>
	Pinned,

	/// <summary>
	/// Shown only in the sidebar, never in the tab bar.
	/// </summary>
	SidebarOnly,

	/// <summary>
	/// Hidden until the user adds it through Edit.
	/// </summary>
	Optional
}
