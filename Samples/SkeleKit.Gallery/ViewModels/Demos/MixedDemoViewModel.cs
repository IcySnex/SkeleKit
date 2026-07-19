using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

public partial class MixedDemoViewModel(
	IMovieService movies,
	INavigator navigator) : ObservableObject
{
	public ObservableCollection<HomeRow> Rows { get; } = [];

	[RelayCommand]
	async Task Open(
		Movie movie) =>
		await navigator.AlertAsync(movie.Title, $"{movie.Year} · {movie.Minutes} min");

	public async Task LoadAsync()
	{
		if (Rows.Count > 0)
			return;

		IReadOnlyList<Movie> all = await movies.GetPopularAsync();

		// the diffable data source keys items by reference, so an item may live in only one section: clone per row
		Rows.Add(new("Featured", CollectionLayoutKind.Carousel, [.. all.Take(6).Select(movie => movie with { })]));
		Rows.Add(new("Popular", CollectionLayoutKind.Grid, [.. all.Skip(6).Take(9).Select(movie => movie with { })]));
		Rows.Add(new("All", CollectionLayoutKind.List, [.. all.Select(movie => movie with { })]));
	}
}
