namespace BareUI;

/// <summary>
/// What kind of value a <see cref="ShareItem"/> carries.
/// </summary>
public enum ShareItemKind
{
	/// <summary>
	/// Plain text.
	/// </summary>
	Text,

	/// <summary>
	/// A web address.
	/// </summary>
	Url,

	/// <summary>
	/// An image.
	/// </summary>
	Image
}

/// <summary>
/// One thing handed to the share sheet. Never spelled out at a call site: a <c>string</c>, <c>Uri</c>
/// or <see cref="ImageSource"/> converts to it implicitly, so <c>ShareAsync("caption", url, image)</c>
/// reads as native types while still typing each item exactly.
/// </summary>
public readonly struct ShareItem
{
	ShareItem(
		ShareItemKind kind,
		string? text,
		Uri? url,
		ImageSource? image)
	{
		Kind = kind;
		Text = text;
		Url = url;
		Image = image;
	}


	internal ShareItemKind Kind { get; }

	internal string? Text { get; }

	internal Uri? Url { get; }

	internal ImageSource? Image { get; }


	/// <summary>
	/// Shares a string as plain text.
	/// </summary>
	/// <param name="text">The text to share.</param>
	public static implicit operator ShareItem(
		string text) =>
		new(ShareItemKind.Text, text, null, null);

	/// <summary>
	/// Shares a web address as a link, so the sheet offers its link-specific actions.
	/// </summary>
	/// <param name="url">The address to share.</param>
	public static implicit operator ShareItem(
		Uri url) =>
		new(ShareItemKind.Url, null, url, null);

	/// <summary>
	/// Shares an image; a remote source is fetched before the sheet appears.
	/// </summary>
	/// <param name="image">The image to share.</param>
	public static implicit operator ShareItem(
		ImageSource image) =>
		new(ShareItemKind.Image, null, null, image);
}
