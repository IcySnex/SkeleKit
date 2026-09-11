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
	/// An image from an SF Symbol name with rendering configuration.
	/// </summary>
	/// <param name="name">The name of the system symbol.</param>
	/// <param name="size">The symbol's point size, or NaN to let the consuming control choose.</param>
	/// <param name="weight">The symbol's stroke weight, or null to let the consuming control choose.</param>
	/// <param name="scale">The symbol's relative scale within its font metrics.</param>
	/// <param name="prefersMulticolor">Whether to prefer the symbol's built-in multicolor rendition.</param>
	/// <param name="colors">Colors for hierarchical or palette rendering.</param>
	/// <returns>An image source configured for a symbol.</returns>
	public static ImageSource Symbol(
		string name,
		double size = double.NaN,
		FontWeight? weight = null,
		SymbolScale scale = SymbolScale.Default,
		bool prefersMulticolor = false,
		params Color[] colors) =>
		new(ImageSourceKind.Symbol, name, size, weight, scale, prefersMulticolor, colors);

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
		string value,
		double symbolSize = double.NaN,
		FontWeight? symbolWeight = null,
		SymbolScale symbolScale = SymbolScale.Default,
		bool prefersMulticolor = false,
		Color[]? symbolColors = null)
	{
		Kind = kind;
		Value = value;
		SymbolSize = symbolSize;
		SymbolWeight = symbolWeight;
		SymbolScale = symbolScale;
		PrefersMulticolor = prefersMulticolor;
		this.symbolColors = symbolColors is null or { Length: 0 }
			? null
			: Array.AsReadOnly(symbolColors.ToArray());
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

	/// <summary>
	/// The symbol's point size, or NaN to let the consuming control choose.
	/// </summary>
	public double SymbolSize { get; }

	/// <summary>
	/// The symbol's stroke weight, or null to let the consuming control choose.
	/// </summary>
	public FontWeight? SymbolWeight { get; }

	/// <summary>
	/// The symbol's relative scale within its font metrics.
	/// </summary>
	public SymbolScale SymbolScale { get; }

	/// <summary>
	/// Colors for the symbol's layers: one gives the hierarchical look, while several define a palette.
	/// </summary>
	public IReadOnlyList<Color> SymbolColors => symbolColors ?? Array.Empty<Color>();
	readonly IReadOnlyList<Color>? symbolColors;

	/// <summary>
	/// Whether a symbol with a built-in multicolor rendition should use it.
	/// </summary>
	public bool PrefersMulticolor { get; }

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
