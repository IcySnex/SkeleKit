using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels;

public partial class MenuViewModel(
	INavigator navigator,
	IDemoCatalog catalog) : ObservableObject
{
	public ObservableCollection<DemoEntry> Demos { get; } = catalog.Demos;

	[RelayCommand]
	Task OpenDemo(
		DemoEntry demo) =>
		navigator.PushAsync(demo.ViewModel);

	[RelayCommand]
	Task OpenMovie() =>
		navigator.PushAsync<MovieInfoViewModel>();
}
