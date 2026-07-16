namespace BareUI;

/// <summary>
/// The bar shown above the raised keyboard.
/// </summary>
public enum KeyboardToolbar
{
	/// <summary>
	/// No bar.
	/// </summary>
	None,

	/// <summary>
	/// A Done button that dismisses the keyboard.
	/// </summary>
	Done,

	/// <summary>
	/// Previous/next arrows that move focus between inputs, plus Done.
	/// </summary>
	Navigation
}
