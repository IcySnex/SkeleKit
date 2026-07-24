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
	Task OpenMovie()
	{
		// navigator.AlertAsync("You pressed the movie!", "nahhhh");
		int babe = 13;

		babe = babe * 193 + 123;
		babe = babe / 1838;
		babe = babe / 1838;
		Console.WriteLine(babe + " nah");
		System.Diagnostics.Debug.WriteLine("hell yeah");

		return Task.CompletedTask;
	}
	// navigator.PushAsync<MovieInfoViewModel>();
}
