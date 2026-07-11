using UIKit;

namespace BareUI;

/// <summary>
/// Displays an image from a symbol, bundle asset, or URL wrapping <c>UIImageView</c>.
/// </summary>
public class Image : Control
{
	/// <summary>
	/// Where the image is loaded from. URL sources load asynchronously and do not trigger re-layout yet, so give them an explicit Width/Height (or use UniformToFill inside a sized container).
	/// </summary>
	public ImageSource? Source { get; set; }

	/// <summary>
	/// How the image is scaled to fill its bounds.
	/// </summary>
	public Stretch Stretch { get; set; } = Stretch.Uniform;

	/// <summary>
	/// The loader used for URL sources. Replace to add caching.
	/// </summary>
	public static IImageLoader Loader { get; set; } = new HttpImageLoader();

	CancellationTokenSource? loadCancellation;

	private protected override UIView CreateNative()
	{
		// set on the element, not the native view: ApplyVisualState overwrites it after CreateNative
		ClipsToBounds |= Stretch is Stretch.UniformToFill;

		UIImageView imageView = new()
		{
			ContentMode = Stretch switch
			{
				Stretch.None => UIViewContentMode.Center,
				Stretch.Fill => UIViewContentMode.ScaleToFill,
				Stretch.UniformToFill => UIViewContentMode.ScaleAspectFill,
				_ => UIViewContentMode.ScaleAspectFit
			}
		};

		if (Source is { } source && source.Kind is not ImageSourceKind.Url)
			imageView.Image = ResolveSync(source);

		return imageView;
	}

	private protected override void OnRealized()
	{
		if (Source is not { Kind: ImageSourceKind.Url } source)
			return;

		loadCancellation = new();
		LoadUrlAsync(source, loadCancellation.Token);
	}

	private protected override void OnUnrealized()
	{
		loadCancellation?.Cancel();
		loadCancellation?.Dispose();
		loadCancellation = null;
	}

	// Auto: bundle asset beats symbol of same name
	static UIImage? ResolveSync(
		ImageSource source) =>
		source.Kind switch
		{
			ImageSourceKind.Symbol => UIImage.GetSystemImage(source.Value),
			ImageSourceKind.Bundle => UIImage.FromBundle(source.Value),
			_ => UIImage.FromBundle(source.Value) ?? UIImage.GetSystemImage(source.Value)
		};

	async void LoadUrlAsync(
		ImageSource source,
		CancellationToken cancellationToken)
	{
		UIImage? image;
		try
		{
			image = await Loader.LoadAsync(source.Value, cancellationToken);
		}
		catch (OperationCanceledException)
		{
			return;
		}
		catch
		{
			// custom loader must not kill the process
			return;
		}

		if (image is null || cancellationToken.IsCancellationRequested)
			return;

		UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
		{
			// still realized, still same url?
			if (cancellationToken.IsCancellationRequested || !IsRealized)
				return;
			if (Source is not { Kind: ImageSourceKind.Url } current || current.Value != source.Value)
				return;

			((UIImageView)Native).Image = image;
		});
	}
}
