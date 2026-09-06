namespace SkeleKit;

/// <summary>
/// Describes where an image comes from, without touching UIKit.
/// </summary>
public readonly partial struct ImageSource
{
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
	/// <remarks>
	/// Supported by <see cref="Image.Source"/> and sharing. Compact control icons accept local sources only.
	/// </remarks>
	/// <param name="url">The full web address of the image.</param>
	/// <returns>An image source configured for a URL.</returns>
	public static ImageSource Url(
		string url) =>
		new(ImageSourceKind.Url, url);

	/// <summary>
	/// An image from raw encoded bytes.
	/// </summary>
	/// <param name="bytes">The encoded image data.</param>
	/// <returns>An image source configured for in-memory data.</returns>
	public static ImageSource Data(
		byte[] bytes) =>
		new(bytes);

	/// <summary>
	/// Treats a string as a URL when it looks like one, otherwise resolves it automatically.
	/// </summary>
	/// <param name="value">The string value to convert.</param>
	/// <returns>An image source for the string.</returns>
	public static implicit operator ImageSource(
		string value) =>
		new(value.Contains("://") ? ImageSourceKind.Url : ImageSourceKind.Auto, value);


	ImageSource(
		ImageSourceKind kind,
		string value)
	{
		Kind = kind;
		Value = value;
	}

	ImageSource(
		byte[] bytes) : this(ImageSourceKind.Data, "")
	{
		Bytes = bytes;
	}


	/// <summary>
	/// How <see cref="Value"/> should be resolved.
	/// </summary>
	public ImageSourceKind Kind { get; }

	/// <summary>
	/// The symbol name, bundle asset name, or URL.
	/// </summary>
	public string Value { get; }

	internal byte[]? Bytes { get; }
}

/// <summary>
/// Where an <c>Image</c> loads its content from.
/// </summary>
public enum ImageSourceKind
{
	/// <summary>
	/// Resolve from an SF Symbol first, then a bundle asset.
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
	Url,

	/// <summary>
	/// Raw encoded image bytes held in memory.
	/// </summary>
	Data
}
