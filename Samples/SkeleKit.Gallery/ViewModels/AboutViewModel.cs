using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class AboutViewModel : ObservableObject
{
	const string RepositoryUrl = "https://github.com/IcySnex/SkeleKit";

	readonly INavigator navigator;


	public AboutViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;
	}


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
