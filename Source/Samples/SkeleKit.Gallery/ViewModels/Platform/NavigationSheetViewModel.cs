using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class NavigationSheetViewModel(
	INavigator navigator) : ShowcaseViewModel
{
	[RelayCommand]
	Task DismissAsync() =>
		navigator.DismissAsync();
}
