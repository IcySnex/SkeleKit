namespace SkeleKit.Gallery.Models;

/// <summary>
/// A row on a Home-style screen: a title, the movies in it, and which layout it uses.
/// </summary>
public record HomeRow(
	string Title,
	CollectionLayoutKind Layout,
	IReadOnlyList<Movie> Items) : ISection<Movie>;
