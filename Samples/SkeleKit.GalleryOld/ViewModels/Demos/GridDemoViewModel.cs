using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

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

	// fresh instances per page: items are keyed by reference
	[RelayCommand]
	async Task LoadMore()
	{
		if (Movies.Count == 0 || page >= 4)
			return;

		page++;

		foreach (Movie movie in await movies.GetPopularAsync())
			Movies.Add(movie with { Title = $"{movie.Title} ({page})" });
	}
	int page = 1;

	public async Task LoadAsync()
	{
		if (Movies.Count > 0)
			return;

		foreach (Movie movie in await movies.GetPopularAsync())
			Movies.Add(movie);

		IsLoading = false;
	}
}
