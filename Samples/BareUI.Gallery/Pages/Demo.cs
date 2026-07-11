namespace BareUI.Gallery;

/// <summary>
/// Shared styling for the demo pages.
/// </summary>
static class Demo
{
	/// <summary>
	/// Muted gray matching the iOS secondary label color.
	/// </summary>
	public static readonly Color Secondary = Color.FromHex(0x8E8E93);

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
