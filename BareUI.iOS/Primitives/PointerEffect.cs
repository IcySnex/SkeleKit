namespace BareUI;

/// <summary>
/// How a view reacts to a hovering trackpad or mouse pointer on iPad.
/// </summary>
/// <remarks>
/// No effect on iPhone, which has no pointer.
/// </remarks>
public enum PointerEffect
{
	/// <summary>
	/// No pointer effect (the default).
	/// </summary>
	None,

	/// <summary>
	/// The system effect matched to the view's size and role, highlighting small controls and lifting larger tiles.
	/// </summary>
	/// <remarks>
	/// The explicit variants are not exposed: Microsoft.iOS 26.0 binds only the automatic effect factory, so distinguishing highlight/lift/hover is not possible.
	/// </remarks>
	Automatic
}
