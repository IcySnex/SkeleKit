using BareUI.Gallery.Models;

namespace BareUI.Gallery.Services;

/// <summary>
/// Canned data with a small delay, so the page has a real loading state to bind against.
/// </summary>
public sealed class MovieService : IMovieService
{
	public async Task<Movie> GetFeaturedAsync()
	{
		await Task.Delay(600);

		return new(
			Title: "Interstellar",
			Year: 2014,
			Minutes: 169,
			Rating: "PG-13",
			Genres: ["Adventure", "Drama", "Science Fiction"],
			Overview: "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
			PosterUrl: "https://picsum.photos/id/1043/240/360",
			BackdropUrl: "https://picsum.photos/id/1036/800/450");
	}
}
