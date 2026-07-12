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
