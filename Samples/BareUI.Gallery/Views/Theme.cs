namespace BareUI.Gallery.Views;

/// <summary>
/// Shared styling for the demo pages.
/// </summary>
public static class Theme
{
	/// <summary>
	/// The iOS secondary label color, dark-mode aware.
	/// </summary>
	public static readonly Color Secondary = Colors.SecondaryLabel;

	/// <summary>
	/// A small gray label that heads a demo section.
	/// </summary>
	public static Label Caption(
		string text) =>
		new()
		{
			Text = text,
			FontSize = 13,
			TextColor = Secondary
		};
}
