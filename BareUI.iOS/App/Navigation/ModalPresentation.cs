namespace BareUI;

/// <summary>
/// How a modal page is structured and presented on screen.
/// </summary>
public enum ModalPresentation
{
	/// <summary>
	/// Let the system choose the best presentation style dynamically.
	/// </summary>
	Automatic,

	/// <summary>
	/// Fills the screen height but restricts width on large screens.
	/// </summary>
	PageSheet,

	/// <summary>
	/// Covers the entire screen and unloads background views.
	/// </summary>
	FullScreen,

	/// <summary>
	/// Covers the whole screen but keeps the background loaded for transparency.
	/// </summary>
	OverFullScreen,

	/// <summary>
	/// Presents inside the current view controller bounds instead of the full screen.
	/// </summary>
	CurrentContext,

	/// <summary>
	/// Presents inside the parent view controller context while keeping its background visible.
	/// </summary>
	OverCurrentContext,

	/// <summary>
	/// A contextual floating bubble modal on large displays.
	/// </summary>
	Popover,

	/// <summary>
	/// A centered card layout on iPad/desktop, and a full sheet on iPhone.
	/// </summary>
	FormSheet
}
