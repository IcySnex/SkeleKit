namespace BareUI;

/// <summary>
/// How scrolling dismisses the on-screen keyboard.
/// </summary>
public enum KeyboardDismiss
{
	/// <summary>Scrolling never dismisses the keyboard.</summary>
	None,

	/// <summary>The keyboard is dismissed as soon as a drag starts.</summary>
	OnDrag,

	/// <summary>The keyboard follows the drag and can be pulled away.</summary>
	Interactive
}
