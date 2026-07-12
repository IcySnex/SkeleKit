namespace BareUI;

/// <summary>
/// What the field holds, so the system can offer autofill (passwords, one-time codes, contacts).
/// </summary>
public enum ContentKind
{
	/// <summary>
	/// No autofill hint.
	/// </summary>
	None,

	/// <summary>
	/// A login user name; the QuickType bar offers saved credentials.
	/// </summary>
	Username,

	/// <summary>
	/// An existing password; offers the saved credential.
	/// </summary>
	Password,

	/// <summary>
	/// A password being created; the system suggests a strong one and saves it.
	/// </summary>
	NewPassword,

	/// <summary>
	/// A one-time code; autofills from incoming messages.
	/// </summary>
	OneTimeCode,

	/// <summary>
	/// An email address.
	/// </summary>
	Email,

	/// <summary>
	/// A person's full name.
	/// </summary>
	Name,

	/// <summary>
	/// A phone number.
	/// </summary>
	PhoneNumber,

	/// <summary>
	/// A street address.
	/// </summary>
	StreetAddress,

	/// <summary>
	/// A web address.
	/// </summary>
	Url
}

/// <summary>
/// When typing is automatically capitalized.
/// </summary>
public enum Capitalization
{
	/// <summary>
	/// The start of every sentence. The system default for plain text.
	/// </summary>
	Sentences,

	/// <summary>
	/// Never.
	/// </summary>
	None,

	/// <summary>
	/// The start of every word.
	/// </summary>
	Words,

	/// <summary>
	/// Every character.
	/// </summary>
	Characters
}

/// <summary>
/// When a text field shows its built-in clear button.
/// </summary>
public enum ClearButton
{
	/// <summary>
	/// Never. The default.
	/// </summary>
	Never,

	/// <summary>
	/// While the field is being edited.
	/// </summary>
	WhileEditing,

	/// <summary>
	/// Only while the field is not being edited.
	/// </summary>
	UnlessEditing,

	/// <summary>
	/// Always.
	/// </summary>
	Always
}
