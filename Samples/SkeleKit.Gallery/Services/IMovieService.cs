using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Services;

/// <summary>
/// Fetches movies. Stands in for a real API client.
/// </summary>
public interface IMovieService
{
	Task<Movie> GetFeaturedAsync();

	Task<IReadOnlyList<Movie>> GetPopularAsync();
}
