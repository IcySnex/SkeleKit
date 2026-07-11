using BareUI.Gallery.Models;
using BareUI.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels;

public partial class MenuViewModel(
	INavigator navigator,
	IDemoCatalog catalog) : ObservableObject
{
	public IReadOnlyList<DemoEntry> Demos { get; } = catalog.Demos;

	[RelayCommand]
	Task OpenDemo(
		DemoEntry demo) =>
		navigator.PushAsync(demo.ViewModel);

	[RelayCommand]
	Task OpenMovie() =>
		navigator.PushAsync<MovieInfoViewModel>();
}
