using System.Collections.Concurrent;
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
/// Default loader: shared HttpClient, in-memory cache, one download per url, pre-decoded images.
/// </summary>
sealed class HttpImageLoader : IImageLoader
{
	// not NSUrlSessionHandler: its delegate peer dies mid-redirect and kills the process
	static readonly HttpClient client = new(new SocketsHttpHandler());

	// decoded images; UIKit evicts on memory pressure, cost is the decoded byte size
	static readonly NSCache cache = new() { TotalCostLimit = 64 * 1024 * 1024 };

	static readonly ConcurrentDictionary<string, Task<UIImage?>> inflight = new();

	public async Task<UIImage?> LoadAsync(
		string url,
		CancellationToken cancellationToken)
	{
		using NSString key = new(url);
		if (cache.ObjectForKey(key) is UIImage cached)
			return cached;

		// one download per url no matter how many cells ask; a caller cancelling only
		// abandons its wait, the fetch still completes and fills the cache
		Task<UIImage?> fetch = inflight.GetOrAdd(url, Fetch);

		try
		{
			return await fetch.WaitAsync(cancellationToken);
		}
		catch (Exception e) when (e is not OperationCanceledException)
		{
			return null;
		}
	}

	static async Task<UIImage?> Fetch(
		string url)
	{
		try
		{
			byte[] data = await client.GetByteArrayAsync(url);

			if (UIImage.LoadFromData(NSData.FromArray(data)) is not { } image)
				return null;

			// decode now, off the scroll's critical path, instead of lazily at first draw
			image = await image.PrepareForDisplayAsync() ?? image;

			nuint cost = (nuint)(image.Size.Width * image.Size.Height * image.CurrentScale * image.CurrentScale * 4);
			cache.SetCost(image, new NSString(url), cost);

			return image;
		}
		catch
		{
			return null;
		}
		finally
		{
			inflight.TryRemove(url, out _);
		}
	}
}
