using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class MapViewDemoViewModel(
	INavigator navigator)
{
	[RelayCommand]
	async Task Select(
		MapPin pin) =>
		await navigator.AlertAsync(pin.Title ?? "Pin", pin.Subtitle ?? "");
}
