namespace SkeleKit.Gallery.Views;

/// <summary>
/// The app's shared values. In a C#-only tree a resource dictionary is just a static class.
/// </summary>
public static class Palette
{
	/// <summary>
	/// Dimmed text: captions, detail lines, footnotes. Dark-mode aware.
	/// </summary>
	public static readonly Color Secondary = Colors.SecondaryLabel;

	/// <summary>
	/// The surface a card sits on.
	/// </summary>
	public static readonly Color Card = Colors.SecondaryGroupedBackground;

	/// <summary>
	/// The plate behind a tapped row.
	/// </summary>
	public static readonly Color Highlight = Colors.Blue.WithAlpha(0.15);
}
