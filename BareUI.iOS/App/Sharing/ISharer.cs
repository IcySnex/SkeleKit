namespace BareUI;

/// <summary>
/// Presents the system share sheet. Inject it into a ViewModel by constructor, or reach it from page
/// code through <c>ContentView.Sharer</c>.
/// </summary>
public interface ISharer
{
	/// <summary>
	/// Presents the share sheet for a coherent piece of content. A <c>string</c>, <c>Uri</c> or
	/// <see cref="ImageSource"/> converts to <see cref="ShareContent"/> implicitly for the common case.
	/// </summary>
	/// <param name="content">The text, link and/or image to share.</param>
	/// <returns>A task that completes once the sheet is dismissed, whether shared or cancelled.</returns>
	Task ShareAsync(
		ShareContent content);
}
