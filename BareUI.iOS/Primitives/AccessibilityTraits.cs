namespace BareUI;

/// <summary>
/// How VoiceOver describes and treats a view.
/// </summary>
/// <remarks>
/// Combines with the control's own traits.
/// </remarks>
[Flags]
public enum AccessibilityTraits
{
	/// <summary>
	/// No extra traits.
	/// </summary>
	None = 0,

	/// <summary>
	/// Acts like a button.
	/// </summary>
	Button = 1 << 0,

	/// <summary>
	/// Opens a link.
	/// </summary>
	Link = 1 << 1,

	/// <summary>
	/// A heading that divides content.
	/// </summary>
	Header = 1 << 2,

	/// <summary>
	/// An image with no text.
	/// </summary>
	Image = 1 << 3,

	/// <summary>
	/// Currently selected.
	/// </summary>
	Selected = 1 << 4,

	/// <summary>
	/// Static text that never changes.
	/// </summary>
	StaticText = 1 << 5,

	/// <summary>
	/// Adjustable with swipe up/down (a slider).
	/// </summary>
	Adjustable = 1 << 6,

	/// <summary>
	/// Updates its value on its own (a progress bar).
	/// </summary>
	UpdatesFrequently = 1 << 7,

	/// <summary>
	/// Present but not interactable.
	/// </summary>
	NotEnabled = 1 << 8,

	/// <summary>
	/// Plays a sound on activation.
	/// </summary>
	PlaysSound = 1 << 9,

	/// <summary>
	/// Starts a media session on activation.
	/// </summary>
	StartsMediaSession = 1 << 10
}
