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
	/// The system picks the effect for the view's size and role.
	/// </summary>
	Automatic,

	/// <summary>
	/// The view highlights under the pointer, which also morphs onto it. Best for small controls.
	/// </summary>
	Highlight,

	/// <summary>
	/// The view lifts toward the pointer with a shadow. Best for larger tappable tiles.
	/// </summary>
	Lift,

	/// <summary>
	/// The pointer keeps its shape while the view scales, tints or shadows under it.
	/// </summary>
	Hover
}
