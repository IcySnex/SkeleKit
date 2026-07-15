namespace BareUI;

/// <summary>
/// One suggestion row under an active search field.
/// </summary>
public sealed class SearchSuggestion
{
	/// <summary>
	/// The suggested search text.
	/// </summary>
	public string Text { get; set; } = "";

	/// <summary>
	/// Secondary text for the suggestion, or null. iOS does not render it in every presentation.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// An SF Symbol name, or null for no icon.
	/// </summary>
	public string? Icon { get; set; }
}
