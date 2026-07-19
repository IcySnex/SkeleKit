using System.ComponentModel;

namespace SkeleKit.Gallery.Models;

/// <summary>
/// One group of the settings-style list, collapsible behind its header. The section model is the app's own: the library only asks for <c>Items</c> and <c>IsExpanded</c>.
/// </summary>
public record SettingsSection(
	string Title,
	string Icon,
	string Footer,
	IReadOnlyList<SettingsEntry> Items) : IExpandableSection<SettingsEntry>, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public bool IsExpanded
	{
		get;
		set
		{
			if (field == value)
				return;

			field = value;
			PropertyChanged?.Invoke(this, new(nameof(IsExpanded)));
		}
	} = true;
}
