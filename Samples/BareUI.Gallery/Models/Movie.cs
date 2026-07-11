namespace BareUI.Gallery.Models;

/// <summary>
/// A movie as the catalog returns it.
/// </summary>
public record Movie(
	string Title,
	int Year,
	int Minutes,
	string Rating,
	IReadOnlyList<string> Genres,
	string Overview,
	string PosterUrl,
	string BackdropUrl);
