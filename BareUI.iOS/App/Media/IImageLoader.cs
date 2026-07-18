namespace BareUI;

/// <summary>
/// Loads remote images for <c>Image</c>.
/// </summary>
public interface IImageLoader
{
	/// <summary>
	/// Loads the image at <paramref name="url"/>, returning null on failure.
	/// </summary>
	/// <param name="url">The image URL.</param>
	/// <param name="cancellationToken">The token to observe while downloading.</param>
	/// <returns>The downloaded image, or null on failure.</returns>
	Task<UIImage?> LoadAsync(
		string url,
		CancellationToken cancellationToken);
}
