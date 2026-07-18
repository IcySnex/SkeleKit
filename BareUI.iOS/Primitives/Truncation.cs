namespace BareUI;

/// <summary>
/// How text is shortened when it does not fit.
/// </summary>
public enum Truncation
{
	/// <summary>
	/// Wrap onto the next line, up to MaxLines.
	/// </summary>
	None,

	/// <summary>
	/// An ellipsis at the end.
	/// </summary>
	Tail,

	/// <summary>
	/// An ellipsis at the start.
	/// </summary>
	Head,

	/// <summary>
	/// An ellipsis in the middle.
	/// </summary>
	Middle
}
