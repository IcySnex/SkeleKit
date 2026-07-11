using BareUI.Gallery.Models;

namespace BareUI.Gallery.ViewModels;

/// <summary>
/// One control demo, pushed by the menu.
/// </summary>
public class DemoViewModel(
	DemoEntry entry)
{
	public string Title { get; } = entry.Title;

	public Func<View> Content { get; } = entry.Build;
}
