namespace BareUI;

/// <summary>
/// The weight of a font.
/// </summary>
public enum FontWeight
{
	/// <summary>
	/// Ultra light.
	/// </summary>
	UltraLight,

	/// <summary>
	/// Thin.
	/// </summary>
	Thin,

	/// <summary>
	/// Light.
	/// </summary>
	Light,

	/// <summary>
	/// The default weight.
	/// </summary>
	Regular,

	/// <summary>
	/// Medium.
	/// </summary>
	Medium,

	/// <summary>
	/// Semibold.
	/// </summary>
	Semibold,

	/// <summary>
	/// Bold.
	/// </summary>
	Bold,

	/// <summary>
	/// Heavy.
	/// </summary>
	Heavy,

	/// <summary>
	/// Black.
	/// </summary>
	Black
}

/// <summary>
/// The design of a font: the system face, a rounded one, a serif, or monospaced.
/// </summary>
public enum FontDesign
{
	/// <summary>
	/// The system font.
	/// </summary>
	Default,

	/// <summary>
	/// The rounded system font.
	/// </summary>
	Rounded,

	/// <summary>
	/// A serif face.
	/// </summary>
	Serif,

	/// <summary>
	/// A monospaced face.
	/// </summary>
	Monospaced
}

/// <summary>
/// A step in the native type hierarchy. Each one carries its own Dynamic Type curve.
/// </summary>
public enum TextStyle
{
	/// <summary>
	/// The largest title, used once per screen.
	/// </summary>
	LargeTitle,

	/// <summary>
	/// The first title level.
	/// </summary>
	Title1,

	/// <summary>
	/// The second title level.
	/// </summary>
	Title2,

	/// <summary>
	/// The third title level.
	/// </summary>
	Title3,

	/// <summary>
	/// An emphasized heading above a block of body text.
	/// </summary>
	Headline,

	/// <summary>
	/// A heading below <see cref="Headline"/>.
	/// </summary>
	Subheadline,

	/// <summary>
	/// Running text.
	/// </summary>
	Body,

	/// <summary>
	/// A remark set slightly smaller than body text.
	/// </summary>
	Callout,

	/// <summary>
	/// A footnote.
	/// </summary>
	Footnote,

	/// <summary>
	/// The first caption level.
	/// </summary>
	Caption1,

	/// <summary>
	/// The second, smallest caption level.
	/// </summary>
	Caption2
}
