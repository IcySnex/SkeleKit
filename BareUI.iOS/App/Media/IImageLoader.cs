namespace BareUI;

/// <summary>
/// Loads remote images for <c>Image</c>.
/// </summary>
public interface IImageLoader
{
	/// <summary>
	/// Loads the image at <paramref name="url"/>, returning null on failure.
	/// </summary>
	/// <param name="url">The remote uniform resource locator pointing to the target image asset.</param>
	/// <param name="cancellationToken">The cancellation token to observe while downloading the asset.</param>
	/// <returns>A task representing the asynchronous load operation, containing the downloaded image or null on failure.</returns>
	Task<UIImage?> LoadAsync(
		string url,
		CancellationToken cancellationToken);
}
