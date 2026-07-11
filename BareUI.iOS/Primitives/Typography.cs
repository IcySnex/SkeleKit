namespace BareUI;

/// <summary>
/// The weight of a font.
/// </summary>
public enum FontWeight
{
	/// <summary>Ultra light.</summary>
	UltraLight,

	/// <summary>Thin.</summary>
	Thin,

	/// <summary>Light.</summary>
	Light,

	/// <summary>The default weight.</summary>
	Regular,

	/// <summary>Medium.</summary>
	Medium,

	/// <summary>Semibold.</summary>
	Semibold,

	/// <summary>Bold.</summary>
	Bold,

	/// <summary>Heavy.</summary>
	Heavy,

	/// <summary>Black.</summary>
	Black
}

/// <summary>
/// The design of a font: the system face, a rounded one, a serif, or monospaced.
/// </summary>
public enum FontDesign
{
	/// <summary>The system font.</summary>
	Default,

	/// <summary>The rounded system font.</summary>
	Rounded,

	/// <summary>A serif face.</summary>
	Serif,

	/// <summary>A monospaced face.</summary>
	Monospaced
}

/// <summary>
/// How text is shortened when it does not fit.
/// </summary>
public enum Truncation
{
	/// <summary>Wrap onto the next line, up to MaxLines.</summary>
	None,

	/// <summary>An ellipsis at the end.</summary>
	Tail,

	/// <summary>An ellipsis at the start.</summary>
	Head,

	/// <summary>An ellipsis in the middle.</summary>
	Middle
}
