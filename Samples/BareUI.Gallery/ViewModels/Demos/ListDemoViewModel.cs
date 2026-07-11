using BareUI.Gallery.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels.Demos;

public partial class ListDemoViewModel(
	INavigator navigator) : ObservableObject
{
	public IReadOnlyList<Section<SettingsEntry>> Sections { get; } =
	[
		new("General",
		[
			new("Appearance", "paintbrush", "System"),
			new("Language", "globe", "English"),
			new("Notifications", "bell", "On")
		]),

		new("Playback",
		[
			new("Quality", "play.circle", "Auto"),
			new("Downloads", "arrow.down.circle", "Wi-Fi only"),
			new("Storage", "internaldrive", "2.4 GB")
		]),

		new("About",
		[
			new("Privacy", "hand.raised", ""),
			new("Version", "info.circle", "1.0.0")
		])
	];

	[RelayCommand]
	async Task Open(
		SettingsEntry entry) =>
		await navigator.AlertAsync(entry.Title, entry.Detail.Length > 0 ? entry.Detail : "No detail");
}
