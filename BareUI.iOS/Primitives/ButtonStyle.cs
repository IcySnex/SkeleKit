namespace BareUI;

/// <summary>
/// The visual treatment of a <c>Button</c>.
/// </summary>
public enum ButtonStyle
{
	/// <summary>
	/// Borderless button with tinted text and no background.
	/// </summary>
	Plain,

	/// <summary>
	/// Gray translucent background.
	/// </summary>
	Gray,

	/// <summary>
	/// Tinted translucent background.
	/// </summary>
	Tinted,

	/// <summary>
	/// Solid filled background.
	/// </summary>
	Filled,

	/// <summary>
	/// Solid filled background with fully rounded (capsule) corners.
	/// </summary>
	FilledCapsule
}

/// <summary>
/// The built-in size classes of a <c>Button</c>.
/// </summary>
public enum ButtonSize
{
	/// <summary>
	/// The standard size.
	/// </summary>
	Medium,

	/// <summary>
	/// The smallest size.
	/// </summary>
	Mini,

	/// <summary>
	/// Slightly smaller than standard.
	/// </summary>
	Small,

	/// <summary>
	/// A prominent, call-to-action size.
	/// </summary>
	Large
}

/// <summary>
/// Where a <c>Button</c>'s icon sits relative to its text.
/// </summary>
public enum IconPlacement
{
	/// <summary>
	/// Before the text.
	/// </summary>
	Leading,

	/// <summary>
	/// After the text.
	/// </summary>
	Trailing,

	/// <summary>
	/// Above the text.
	/// </summary>
	Top,

	/// <summary>
	/// Below the text.
	/// </summary>
	Bottom
}
