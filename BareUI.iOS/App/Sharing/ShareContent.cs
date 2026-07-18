namespace BareUI;

/// <summary>
/// What to hand the share sheet: an optional text, link and image that compose into one coherent.
/// </summary>
public sealed class ShareContent
{
	/// <summary>
	/// The text to share, which also titles the share sheet.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// The link to share, so the sheet offers its link-specific actions.
	/// </summary>
	public Uri? Url { get; set; }

	/// <summary>
	/// The image to share, shown as the sheet's preview thumbnail.
	/// </summary>
	public ImageSource? Image { get; set; }


	/// <summary>
	/// Shares a string as plain text.
	/// </summary>
	/// <param name="text">The text to share.</param>
	public static implicit operator ShareContent(
		string text) =>
		new() { Text = text };

	/// <summary>
	/// Shares a web address as a link.
	/// </summary>
	/// <param name="url">The address to share.</param>
	public static implicit operator ShareContent(
		Uri url) =>
		new() { Url = url };

	/// <summary>
	/// Shares an image.
	/// </summary>
	/// <param name="image">The image to share.</param>
	public static implicit operator ShareContent(
		ImageSource image) =>
		new() { Image = image };
}
