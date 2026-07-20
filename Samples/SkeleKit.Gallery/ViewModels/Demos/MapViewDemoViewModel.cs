using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class MapViewDemoViewModel(
	INavigator navigator)
{
	[RelayCommand]
	async Task Select(
		MapPin pin)
	{
		if (pin.Title is null)
			return;

		await navigator.AlertAsync(pin.Title, pin.Subtitle ?? "");
	}
}
