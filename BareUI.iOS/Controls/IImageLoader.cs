#if IOS
using Foundation;
using UIKit;

namespace BareUI;

/// <summary>
/// Loads remote images for <c>Image</c>. Plug a custom implementation via <c>Image.Loader</c> to add caching.
/// </summary>
public interface IImageLoader
{
	/// <summary>
	/// Loads the image at <paramref name="url"/>, returning null on failure.
	/// </summary>
	Task<UIImage?> LoadAsync(
		string url,
		CancellationToken cancellationToken);
}

/// <summary>
/// Default loader: fetches over a shared HttpClient with no caching.
/// </summary>
sealed class HttpImageLoader : IImageLoader
{
	// not NSUrlSessionHandler: its delegate peer dies mid-redirect and kills the process
	static readonly HttpClient client = new(new SocketsHttpHandler());

	public async Task<UIImage?> LoadAsync(
		string url,
		CancellationToken cancellationToken)
	{
		try
		{
			byte[] data = await client.GetByteArrayAsync(url, cancellationToken);
			return UIImage.LoadFromData(NSData.FromArray(data));
		}
		catch
		{
			return null;
		}
	}
}
#endif
