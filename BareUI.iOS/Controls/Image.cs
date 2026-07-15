using Symbols;

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
	/// A symbol or bundle image shown while a URL source is still loading, or null for none.
	/// </summary>
	public ImageSource? Placeholder
	{
		get;
		set => Set(ref field, value, affectsMeasure: false);
	}

	/// <summary>
	/// A symbol or bundle image shown when a URL source fails to load, or null to keep the placeholder.
	/// </summary>
	public ImageSource? Fallback
	{
		get;
		set => Set(ref field, value, affectsMeasure: false);
	}

	/// <summary>
	/// Whether a URL image cross-dissolves in once it arrives, instead of popping.
	/// </summary>
	public bool FadesIn
	{
		get => fadesIn;
		set => Set(ref fadesIn, value, affectsMeasure: false);
	}
	bool fadesIn;

	/// <summary>
	/// How the image is scaled to fill its bounds.
	/// </summary>
	public Stretch Stretch
	{
		get => stretch;
		set => Set(ref stretch, value, ApplyStretch, affectsMeasure: false);
	}
	Stretch stretch = Stretch.Uniform;

	// Symbol styling: all of it only affects SF Symbol sources

	/// <summary>
	/// The symbol's point size, or NaN for its natural size.
	/// </summary>
	public double SymbolSize
	{
		get;
		set => Set(ref field, value, ApplySymbolConfiguration);
	} = double.NaN;

	/// <summary>
	/// The symbol's stroke weight, or null for its default.
	/// </summary>
	public FontWeight? SymbolWeight
	{
		get;
		set => Set(ref field, value, ApplySymbolConfiguration);
	}

	/// <summary>
	/// The symbol's relative scale within its font metrics.
	/// </summary>
	public SymbolScale SymbolScale
	{
		get;
		set => Set(ref field, value, ApplySymbolConfiguration);
	}

	/// <summary>
	/// Colors for the symbol's layers: one gives the hierarchical look, several assign the palette explicitly.
	/// </summary>
	public IList<Color> SymbolColors { get; } = [];

	/// <summary>
	/// Whether a symbol with a built-in multicolor rendition uses it.
	/// </summary>
	public bool PrefersMulticolor
	{
		get;
		set => Set(ref field, value, ApplySymbolConfiguration, affectsMeasure: false);
	}

	/// <summary>
	/// The value 0–1 driving a variable symbol's layers (a wifi or speaker level), or NaN for none.
	/// </summary>
	public Bindable<double> SymbolValue
	{
		get => symbolValue;
		set => symbolValueBinding = Register(symbolValueBinding, value, value => Set(ref symbolValue, value, ApplySource, affectsMeasure: false));
	}
	double symbolValue = double.NaN;
	Binding<double>? symbolValueBinding;

	/// <summary>
	/// An ambient effect the symbol performs continuously while set.
	/// </summary>
	public SymbolEffect SymbolEffect
	{
		get;
		set => Set(ref field, value, ApplySymbolEffect, affectsMeasure: false);
	}

	CancellationTokenSource? loadCancellation;
	UIImage? displayed;


	/// <summary>
	/// Plays a symbol effect once, on top of any ambient <see cref="SymbolEffect"/>. No-op until realized.
	/// </summary>
	/// <param name="effect">The effect to perform.</param>
	public void PlaySymbolEffect(
		SymbolEffect effect)
	{
		if (!IsRealized || effect is SymbolEffect.None)
			return;

		Ui.AddSymbolEffect(
			Effect(effect),
			NSSymbolEffectOptions.Create(NSSymbolEffectOptionsRepeatBehavior.CreatePeriodic(1)),
			animated: true);
	}


	private protected override UIView CreateNative() =>
		new UIImageView();

	private protected override void ApplyProperties()
	{
		ApplyStretch();
		ApplySymbolConfiguration();
		ApplySource();
		ApplySymbolEffect();
	}

	// only an explicit Tint renders a raster as a template — an inherited one would flatten a photo
	internal override void TintChanged()
	{
		if (IsRealized)
			Show(displayed, animated: false);
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

	void ApplySymbolConfiguration()
	{
		UIImageSymbolConfiguration? configuration = null;

		void Add(UIImageSymbolConfiguration next) =>
			configuration = configuration is null
				? next
				: (UIImageSymbolConfiguration)configuration.GetConfiguration(next);

		if (!double.IsNaN(SymbolSize))
			Add(UIImageSymbolConfiguration.Create((nfloat)SymbolSize));

		if (SymbolWeight is { } weight)
			Add(UIImageSymbolConfiguration.Create(Weight(weight)));

		if (SymbolScale is not SymbolScale.Default)
			Add(UIImageSymbolConfiguration.Create(SymbolScale switch
			{
				SymbolScale.Small => UIImageSymbolScale.Small,
				SymbolScale.Large => UIImageSymbolScale.Large,
				_ => UIImageSymbolScale.Medium
			}));

		if (SymbolColors.Count == 1)
			Add(UIImageSymbolConfiguration.Create(SymbolColors[0].ToUIColor()));
		else if (SymbolColors.Count > 1)
			Add(UIImageSymbolConfiguration.Create(SymbolColors.Select(color => color.ToUIColor()).ToArray()));

		if (PrefersMulticolor)
			Add(UIImageSymbolConfiguration.ConfigurationPreferringMulticolor);

		Ui.PreferredSymbolConfiguration = configuration;
	}

	void ApplySymbolEffect()
	{
		Ui.RemoveAllSymbolEffects();

		if (SymbolEffect is SymbolEffect.None)
			return;

		Ui.AddSymbolEffect(
			Effect(SymbolEffect),
			NSSymbolEffectOptions.Create(NSSymbolEffectOptionsRepeatBehavior.CreateContinuous()),
			animated: true);
	}

	void ApplySource()
	{
		CancelLoad();

		if (source is not { } current)
		{
			Show(null, animated: false);
			return;
		}

		if (current.Kind is not ImageSourceKind.Url)
		{
			Show(ResolveSync(current), animated: false);
			return;
		}

		Show(Placeholder is { } waiting ? ResolveSync(waiting) : null, animated: false);
		loadCancellation = new();

		LoadUrlAsync(current, loadCancellation.Token);
	}

	// a template render leaves layered symbol colors alone only when no tint asks for it
	void Show(
		UIImage? image,
		bool animated)
	{
		displayed = image;

		UIImage? rendered = LocalTint is not null
			? image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)
			: image;

		if (animated && fadesIn)
			UIView.Transition(Ui, 0.3, UIViewAnimationOptions.TransitionCrossDissolve, () => Ui.Image = rendered, static () => { });
		else
			Ui.Image = rendered;
	}

	void CancelLoad()
	{
		loadCancellation?.Cancel();
		loadCancellation?.Dispose();
		loadCancellation = null;
	}

	// Auto: bundle asset beats symbol of same name
	UIImage? ResolveSync(
		ImageSource source) =>
		source.Kind switch
		{
			ImageSourceKind.Symbol => Symbol(source.Value),
			ImageSourceKind.Bundle => UIImage.FromBundle(source.Value),
			ImageSourceKind.Url => null,
			_ => UIImage.FromBundle(source.Value) ?? Symbol(source.Value)
		};

	UIImage? Symbol(
		string name) =>
		double.IsNaN(symbolValue)
			? UIImage.GetSystemImage(name)
			: UIImage.GetSystemImage(name, Math.Clamp(symbolValue, 0, 1), UIImageSymbolConfiguration.UnspecifiedConfiguration);

	static NSSymbolEffect Effect(
		SymbolEffect effect) =>
		effect switch
		{
			SymbolEffect.Bounce => NSSymbolBounceEffect.Create(),
			SymbolEffect.Pulse => NSSymbolPulseEffect.Create(),
			SymbolEffect.VariableColor => NSSymbolVariableColorEffect.Create(),
			SymbolEffect.Breathe => NSSymbolBreatheEffect.Create(),
			SymbolEffect.Wiggle => NSSymbolWiggleEffect.Create(),
			_ => NSSymbolRotateEffect.Create()
		};

	static UIImageSymbolWeight Weight(
		FontWeight weight) =>
		weight switch
		{
			FontWeight.UltraLight => UIImageSymbolWeight.UltraLight,
			FontWeight.Thin => UIImageSymbolWeight.Thin,
			FontWeight.Light => UIImageSymbolWeight.Light,
			FontWeight.Medium => UIImageSymbolWeight.Medium,
			FontWeight.Semibold => UIImageSymbolWeight.Semibold,
			FontWeight.Bold => UIImageSymbolWeight.Bold,
			FontWeight.Heavy => UIImageSymbolWeight.Heavy,
			FontWeight.Black => UIImageSymbolWeight.Black,
			_ => UIImageSymbolWeight.Regular
		};

	async void LoadUrlAsync(
		ImageSource source,
		CancellationToken cancellationToken)
	{
		UIImage? image = null;
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
		}

		if (cancellationToken.IsCancellationRequested)
			return;

		MainThread.Post(() =>
		{
			// still realized, still same url?
			if (cancellationToken.IsCancellationRequested || !IsRealized)
				return;

			if (this.source is not { Kind: ImageSourceKind.Url } current || current.Value != source.Value)
				return;

			if (image is null)
			{
				// a load can fail without an exception; the placeholder stays unless a fallback exists
				if (Fallback is { } failed)
					Show(ResolveSync(failed), animated: true);

				return;
			}

			Show(image, animated: true);
			InvalidateMeasure();
		});
	}
}
