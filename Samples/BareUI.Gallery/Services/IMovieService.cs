using BareUI.Gallery.Models;

namespace BareUI.Gallery.Services;

/// <summary>
/// Fetches movies. Stands in for a real API client.
/// </summary>
public interface IMovieService
{
	Task<Movie> GetFeaturedAsync();
}
