namespace SkeleKit.Gallery.Models;

/// <summary>
/// One control demo: its title and the ViewModel that opens it.
/// </summary>
public record DemoEntry(
	string Title,
	Type ViewModel);
