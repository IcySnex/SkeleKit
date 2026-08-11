using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class NavigationDetailViewModel(
	INavigator navigator,
	int depth) : ShowcaseViewModel
{
	public int Depth { get; } = depth;

	public string Title => $"Detail {Depth}";

	public string Summary =>
		$"This page is level {Depth} in the current navigation stack.";


	[RelayCommand]
	async Task PushNextAsync()
	{
		await navigator.PushAsync(
			new NavigationDetailViewModel(navigator, Depth + 1));
	}

	[RelayCommand]
	async Task PopAsync()
	{
		await navigator.PopAsync();
	}

	[RelayCommand]
	async Task PopToRootAsync()
	{
		await navigator.PopToRootAsync();
	}
}
