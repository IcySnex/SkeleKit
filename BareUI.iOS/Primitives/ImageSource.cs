namespace BareUI;

/// <summary>
/// Where an <c>Image</c> loads its content from.
/// </summary>
public enum ImageSourceKind
{
	/// <summary>
	/// Resolve from a bundle asset first, then an SF Symbol.
	/// </summary>
	Auto,

	/// <summary>
	/// An SF Symbol name.
	/// </summary>
	Symbol,

	/// <summary>
	/// A bundle asset name.
	/// </summary>
	Bundle,

	/// <summary>
	/// A remote URL loaded asynchronously.
	/// </summary>
	Url
}

/// <summary>
/// Describes where an image comes from, without touching UIKit.
/// </summary>
/// <param name="kind">How the value should be resolved.</param>
/// <param name="value">The symbol name, bundle asset name, or URL.</param>
public readonly struct ImageSource(
	ImageSourceKind kind,
	string value)
{
	/// <summary>
	/// How <see cref="Value"/> should be resolved.
	/// </summary>
	public ImageSourceKind Kind { get; } = kind;

	/// <summary>
	/// The symbol name, bundle asset name, or URL.
	/// </summary>
	public string Value { get; } = value;


	/// <summary>
	/// An image from an SF Symbol name.
	/// </summary>
	/// <param name="name">The name of the system symbol.</param>
	/// <returns>An image source configured for a symbol.</returns>
	public static ImageSource Symbol(
		string name) =>
		new(ImageSourceKind.Symbol, name);

	/// <summary>
	/// An image from a bundle asset name.
	/// </summary>
	/// <param name="name">The name of the asset in the bundle.</param>
	/// <returns>An image source configured for a bundle asset.</returns>
	public static ImageSource Bundle(
		string name) =>
		new(ImageSourceKind.Bundle, name);

	/// <summary>
	/// An image from a remote URL, loaded asynchronously.
	/// </summary>
	/// <param name="url">The full web address of the image.</param>
	/// <returns>An image source configured for a URL.</returns>
	public static ImageSource Url(
		string url) =>
		new(ImageSourceKind.Url, url);


	/// <summary>
	/// Treats a string as a URL when it looks like one, otherwise resolves it automatically.
	/// </summary>
	/// <param name="value">The string value to convert.</param>
	public static implicit operator ImageSource(
		string value) =>
		new(value.Contains("://") ? ImageSourceKind.Url : ImageSourceKind.Auto, value);
}
