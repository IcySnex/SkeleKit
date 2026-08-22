using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class AboutViewModel(
	INavigator navigator) : ObservableObject
{
	const string RepositoryUrl = "https://github.com/IcySnex/SkeleKit";


	[RelayCommand]
	Task DismissAsync() =>
		navigator.DismissAsync();

	[RelayCommand]
	Task OpenGitHubAsync() =>
		navigator.OpenUrlAsync(RepositoryUrl);

	[RelayCommand]
	Task ShowLicensesAsync() =>
		navigator.AlertAsync(
			"Open-Source Licenses",
			"CommunityToolkit.Mvvm\nMIT License\n© .NET Foundation and Contributors\n\n"
			+ "Microsoft.Extensions.DependencyInjection\nMIT License\n© Microsoft Corporation");
}
