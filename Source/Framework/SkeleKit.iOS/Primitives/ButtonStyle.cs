namespace SkeleKit;

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
	FilledCapsule,

	/// <summary>
	/// A Liquid Glass capsule. Plain on earlier systems.
	/// </summary>
	Glass,

	/// <summary>
	/// A prominent, tinted Liquid Glass capsule. Filled on earlier systems.
	/// </summary>
	ProminentGlass,

	/// <summary>
	/// Invisible Liquid Glass: flat at rest, lights up and swells under the finger. For buttons on a glass bar. Plain on earlier systems.
	/// </summary>
	ClearGlass
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
