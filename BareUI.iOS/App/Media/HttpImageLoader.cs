using System.Collections.Concurrent;

namespace BareUI;

internal sealed class HttpImageLoader : IImageLoader
{
	static readonly HttpClient Client = new(new SocketsHttpHandler());
	static readonly NSCache Cache = new() { TotalCostLimit = 64 * 1024 * 1024 };
	static readonly ConcurrentDictionary<string, Task<UIImage?>> Inflight = new();


	static async Task<UIImage?> Fetch(
		string url)
	{
		try
		{
			byte[] data = await Client.GetByteArrayAsync(url);

			if (UIImage.LoadFromData(NSData.FromArray(data)) is not UIImage image)
				return null;

			image = (UIImage?)await image.PrepareForDisplayAsync() ?? image;

			nuint cost = (nuint)(image.Size.Width * image.Size.Height * image.CurrentScale * image.CurrentScale * 4);
			Cache.SetCost(image, new NSString(url), cost);

			return image;
		}
		catch
		{
			return null;
		}
		finally
		{
			Inflight.TryRemove(url, out _);
		}
	}

	
	public async Task<UIImage?> LoadAsync(
		string url,
		CancellationToken cancellationToken)
	{
		using NSString key = new(url);
		if (Cache.ObjectForKey(key) is UIImage cached)
			return cached;

		Task<UIImage?> fetch = Inflight.GetOrAdd(url, Fetch);

		try
		{
			return await fetch.WaitAsync(cancellationToken);
		}
		catch (Exception e) when (e is not OperationCanceledException)
		{
			return null;
		}
	}
}
