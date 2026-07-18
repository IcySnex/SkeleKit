namespace BareUI;

/// <summary>
/// Presents the system share sheet. Inject it into a ViewModel by constructor, or reach it from page
/// code through <c>ContentView.Sharer</c>.
/// </summary>
public interface ISharer
{
	/// <summary>
	/// Presents the share sheet for the given items, each a <c>string</c>, <c>Uri</c> or
	/// <see cref="ImageSource"/>. The first item is the primary one; remote images are fetched first.
	/// </summary>
	/// <param name="items">The things to share.</param>
	/// <returns>A task that completes once the sheet is dismissed, whether shared or cancelled.</returns>
	Task ShareAsync(
		params ShareItem[] items);
}
