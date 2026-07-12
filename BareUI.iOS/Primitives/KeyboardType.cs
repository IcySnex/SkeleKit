namespace BareUI;

/// <summary>
/// The on-screen keyboard shown while editing a text input.
/// </summary>
public enum KeyboardType
{
	/// <summary>
	/// The standard keyboard.
	/// </summary>
	Default,

	/// <summary>
	/// A numeric keypad (digits only).
	/// </summary>
	Numeric,

	/// <summary>
	/// A numeric keypad with a decimal point.
	/// </summary>
	Decimal,

	/// <summary>
	/// A keypad for entering phone numbers.
	/// </summary>
	Phone,

	/// <summary>
	/// A keyboard optimized for entering email addresses.
	/// </summary>
	Email,

	/// <summary>
	/// A keyboard optimized for entering URLs.
	/// </summary>
	Url
}
