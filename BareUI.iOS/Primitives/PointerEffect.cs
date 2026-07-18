namespace BareUI;

/// <summary>
/// How a view reacts to a hovering trackpad or mouse pointer on iPad (iPadOS pointer effects). No
/// effect on iPhone, which has no pointer.
/// </summary>
public enum PointerEffect
{
	/// <summary>
	/// No pointer effect (the default).
	/// </summary>
	None,

	/// <summary>
	/// The system effect for the view's size and role — a highlight for small controls, a lift for
	/// larger tiles. The explicit variants are not exposed: Microsoft.iOS 26.0 binds only the automatic
	/// effect factory, so distinguishing highlight/lift/hover is not possible yet.
	/// </summary>
	Automatic
}
