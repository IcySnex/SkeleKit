namespace SkeleKit;

/// <summary>
/// How a <c>TextField</c> draws its frame.
/// </summary>
public enum TextFieldStyle
{
	/// <summary>
	/// A filled rounded rectangle. The default.
	/// </summary>
	Rounded,

	/// <summary>
	/// No frame or background, for fields placed on a custom background.
	/// </summary>
	Plain,

	/// <summary>
	/// A thin rectangular outline.
	/// </summary>
	Line,

	/// <summary>
	/// A beveled, inset rectangle.
	/// </summary>
	Bezel
}
