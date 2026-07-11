namespace BareUI.Gallery.Models;

/// <summary>
/// One control demo: its title and the tree that shows it off.
/// </summary>
public record DemoEntry(
	string Title,
	Func<View> Build);
