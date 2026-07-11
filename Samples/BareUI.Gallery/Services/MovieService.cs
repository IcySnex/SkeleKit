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

	public async Task<IReadOnlyList<Movie>> GetPopularAsync()
	{
		await Task.Delay(400);

		string[] titles =
		[
			"Interstellar", "Dune", "Arrival", "Tenet", "Blade Runner 2049", "The Martian",
			"Gravity", "Ex Machina", "Annihilation", "Moon", "Sunshine", "Looper",
			"Edge of Tomorrow", "Oblivion", "Prometheus", "Passengers", "Ad Astra", "Solaris"
		];

		int[] posters = [1043, 1036, 1024, 1015, 1016, 1018, 1019, 1020, 1021, 1022, 1023, 1025, 1026, 1027, 1028, 1029, 1031, 1033];

		List<Movie> movies = [];
		for (int index = 0; index < titles.Length; index++)
			movies.Add(new(
				Title: titles[index],
				Year: 2010 + index % 12,
				Minutes: 100 + index * 3,
				Rating: "PG-13",
				Genres: ["Science Fiction"],
				Overview: "",
				PosterUrl: $"https://picsum.photos/id/{posters[index]}/200/300",
				BackdropUrl: ""));

		return movies;
	}
}
