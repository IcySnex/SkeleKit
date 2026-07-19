namespace SkeleKit.Gallery.Models;

/// <summary>
/// One group of the settings-style list. The section model is the app's own: the library only asks for <c>Items</c>.
/// </summary>
public record SettingsSection(
	string Title,
	string Icon,
	string Footer,
	IReadOnlyList<SettingsEntry> Items) : ISection<SettingsEntry>;
