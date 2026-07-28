using System.Windows.Input;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class AboutViewModel
{
	const string RepositoryUrl = "https://github.com/IcySnex/SkeleKit";

	readonly INavigator navigator;


	public AboutViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;

		DismissCommand = Command.From(Dismiss);
		OpenGitHubCommand = Command.From(OpenGitHub);
		ShowLicensesCommand = Command.From(ShowLicenses);
	}


	public ICommand DismissCommand { get; }
	public ICommand OpenGitHubCommand { get; }
	public ICommand ShowLicensesCommand { get; }


	void Dismiss() =>
		_ = navigator.DismissAsync();

	void OpenGitHub() =>
		_ = navigator.OpenUrlAsync(RepositoryUrl);

	void ShowLicenses() =>
		_ = navigator.AlertAsync(
			"Open-Source Licenses",
			"Microsoft.Extensions.DependencyInjection\nMIT License\n© Microsoft Corporation");
}
