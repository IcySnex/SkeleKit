namespace BareUI;

/// <summary>
/// Displays an image from a symbol, bundle asset, or URL.
/// </summary>
public class Image : Control
{
	/// <summary>
	/// The loader used for URL sources. Set it through <c>BareApplicationBuilder.UseImageLoader(...)</c>.
	/// </summary>
	internal static IImageLoader Loader { get; set; } = new HttpImageLoader();

	/// <summary>
	/// Where the image is loaded from. URL sources load asynchronously, so give them an explicit Width/Height.
	/// </summary>
	public Bindable<ImageSource?> Source
	{
		get => source;
		set => sourceBinding = Register(sourceBinding, value, value => Set(ref source, value, ApplySource));
	}
	ImageSource? source;
	Binding<ImageSource?>? sourceBinding;

	/// <summary>
	/// How the image is scaled to fill its bounds.
	/// </summary>
	public Stretch Stretch
	{
		get => stretch;
		set => Set(ref stretch, value, ApplyStretch, affectsMeasure: false);
	}
	Stretch stretch = Stretch.Uniform;

	CancellationTokenSource? loadCancellation;


	private protected override UIView CreateNative() =>
		new UIImageView();

	private protected override void ApplyProperties()
	{
		ApplyStretch();
		ApplySource();
	}

	private protected override void OnUnrealized() =>
		CancelLoad();

	UIImageView Ui =>
		(UIImageView)Native;

	void ApplyStretch()
	{
		Ui.ContentMode = stretch switch
		{
			Stretch.None => UIViewContentMode.Center,
			Stretch.Fill => UIViewContentMode.ScaleToFill,
			Stretch.UniformToFill => UIViewContentMode.ScaleAspectFill,
			_ => UIViewContentMode.ScaleAspectFit
		};

		// aspect-fill spills outside the frame unless clipped
		if (stretch is Stretch.UniformToFill)
			Ui.ClipsToBounds = true;
	}

	void ApplySource()
	{
		CancelLoad();

		if (source is not { } current)
		{
			Ui.Image = null;
			return;
		}

		if (current.Kind is not ImageSourceKind.Url)
		{
			Ui.Image = ResolveSync(current);
			return;
		}

		Ui.Image = null;
		loadCancellation = new();

		LoadUrlAsync(current, loadCancellation.Token);
	}

	void CancelLoad()
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
		try
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

			MainThread.Post(() =>
			{
				// still realized, still same url?
				if (cancellationToken.IsCancellationRequested || !IsRealized)
					return;

				if (this.source is not { Kind: ImageSourceKind.Url } current || current.Value != source.Value)
					return;

				Ui.Image = image;
				InvalidateMeasure();
			});
		}
		catch
		{
			// ignore :3
		}
	}
}
