namespace SkeleKit;

/// <summary>
/// Presents the system share sheet.
/// </summary>
public interface ISharer
{
	/// <summary>
	/// Presents the share sheet for a piece of content.
	/// </summary>
	/// <remarks>
	/// A <c>string</c>, <c>Uri</c> or <see cref="ImageSource"/> converts to <see cref="ShareContent"/> implicitly for the common case.
	/// </remarks>
	/// <param name="content">The text, link and/or image to share.</param>
	/// <returns>A task that completes once the sheet is dismissed, whether shared or canceled.</returns>
	Task ShareAsync(
		ShareContent content);
}
