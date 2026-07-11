namespace BareUI.Gallery.Models;

/// <summary>
/// One control demo: its title and how to create its page.
/// </summary>
public record DemoEntry(
	string Title,
	Func<ContentView> Create);
