using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class CarouselDemoViewModel(
	IMovieService movies,
	INavigator navigator) : ObservableObject
{
	public ObservableCollection<Movie> Movies { get; } = [];

	[RelayCommand]
	async Task Open(
		Movie movie) =>
		await navigator.AlertAsync(movie.Title, $"{movie.Year}");

	public async Task LoadAsync()
	{
		if (Movies.Count > 0)
			return;

		foreach (Movie movie in await movies.GetPopularAsync())
			Movies.Add(movie);
	}
}
