using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SkeleKit.Gallery.ViewModels;

public partial class MovieInfoViewModel(
	IMovieService movies) : ObservableObject
{
	[ObservableProperty]
	public partial Movie? Movie { get; set; }

	[ObservableProperty]
	public partial bool IsLoading { get; set; } = true;

	public string Title =>
		Movie?.Title ?? "";

	public string Overview =>
		Movie?.Overview ?? "";

	// year · runtime · rating
	public string Metadata =>
		Movie is { } movie
			? $"{movie.Year} · {movie.Minutes / 60}h {movie.Minutes % 60}m · {movie.Rating}"
			: "";

	public string GenreLine =>
		Movie is { } movie ? string.Join(" · ", movie.Genres) : "";

	public ImageSource? Poster =>
		Movie is { } movie ? ImageSource.Url(movie.PosterUrl) : (ImageSource?)null;

	public ImageSource? Backdrop =>
		Movie is { } movie ? ImageSource.Url(movie.BackdropUrl) : (ImageSource?)null;

	public async Task LoadAsync()
	{
		Movie = await movies.GetFeaturedAsync();
		IsLoading = false;
	}

	// the computed properties all read Movie, so they change with it
	partial void OnMovieChanged(
		Movie? value)
	{
		OnPropertyChanged(nameof(Title));
		OnPropertyChanged(nameof(Overview));
		OnPropertyChanged(nameof(Metadata));
		OnPropertyChanged(nameof(GenreLine));
		OnPropertyChanged(nameof(Poster));
		OnPropertyChanged(nameof(Backdrop));
	}
}
