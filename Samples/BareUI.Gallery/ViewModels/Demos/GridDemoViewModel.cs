using System.Collections.ObjectModel;
using BareUI.Gallery.Models;
using BareUI.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels.Demos;

public partial class GridDemoViewModel(
	IMovieService movies,
	INavigator navigator) : ObservableObject
{
	// observable, so the CollectionView updates itself when the load finishes
	public ObservableCollection<Movie> Movies { get; } = [];

	[ObservableProperty]
	public partial bool IsLoading { get; set; } = true;

	[RelayCommand]
	async Task Open(
		Movie movie) =>
		await navigator.AlertAsync(movie.Title, $"{movie.Year} · {movie.Minutes} min");

	public async Task LoadAsync()
	{
		if (Movies.Count > 0)
			return;

		foreach (Movie movie in await movies.GetPopularAsync())
			Movies.Add(movie);

		IsLoading = false;
	}
}
