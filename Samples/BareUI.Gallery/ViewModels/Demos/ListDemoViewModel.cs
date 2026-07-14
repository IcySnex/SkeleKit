using System.Collections.ObjectModel;
using BareUI.Gallery.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels.Demos;

public partial class ListDemoViewModel : ObservableObject
{
	readonly INavigator navigator;

	int extra = 1;


	// a section's own items are a live source too, not just the list of sections
	public ObservableCollection<SettingsEntry> General { get; } =
	[
		new("Appearance", "paintbrush", "System"),
		new("Language", "globe", "English"),
		new("Notifications", "bell", "On")
	];

	public ObservableCollection<SettingsSection> Sections { get; }


	[RelayCommand]
	void AddSetting() =>
		General.Add(new($"Extra {extra++}", "sparkles", "Off"));

	[RelayCommand]
	async Task Open(
		SettingsEntry entry) =>
		await navigator.AlertAsync(entry.Title, entry.Detail.Length > 0 ? entry.Detail : "No detail");

	public ListDemoViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;

		Sections =
		[
			new("General", "gearshape", "Follows the system setting unless you pick one here.", General),

			new("Playback", "play.rectangle", "Downloads over cellular are off by default.",
			[
				new("Quality", "play.circle", "Auto"),
				new("Downloads", "arrow.down.circle", "Wi-Fi only"),
				new("Storage", "internaldrive", "2.4 GB")
			]),

			new("About", "info.square", "Built with BareUI — no XAML, no MAUI.",
			[
				new("Privacy", "hand.raised", ""),
				new("Version", "info.circle", "1.0.0")
			])
		];
	}
}
