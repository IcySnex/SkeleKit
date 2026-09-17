namespace SkeleKit;

/// <summary>
/// How a tab takes part in adaptive tab and sidebar customization.
/// </summary>
public enum TabPlacement
{
	/// <summary>
	/// Lets the system choose a placement for the destination's context.
	/// </summary>
	Automatic,

	/// <summary>
	/// Shown in the tab bar by default and available for people to move or remove.
	/// </summary>
	Default,

	/// <summary>
	/// Hidden from the tab bar by default, but available for people to add and move.
	/// </summary>
	Optional,

	/// <summary>
	/// Shown in the tab bar and movable, but not removable.
	/// </summary>
	Movable,

	/// <summary>
	/// Anchored at the trailing end of the bar.
	/// </summary>
	Pinned,

	/// <summary>
	/// Shown at the leading edge of the tab bar and cannot be moved or removed.
	/// </summary>
	Fixed,

	/// <summary>
	/// Shown only in the sidebar, never in the tab bar.
	/// </summary>
	SidebarOnly,

	/// <summary>
	/// Not shown in the tab bar or sidebar, but still selectable programmatically
	/// through <see cref="INavigator.SelectTabAsync"/>.
	/// </summary>
	Hidden
}
