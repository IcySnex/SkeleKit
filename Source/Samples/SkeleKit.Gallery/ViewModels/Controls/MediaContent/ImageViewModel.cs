using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal sealed partial class ImageViewModel : ShowcaseViewModel
{
	const string StockImageUrl = "https://picsum.photos/500";
	const string FailureUrl = "https://example.invalid/image.png";

	public ImageViewModel()
	{
		SelectedSource = Sources[0];
		SelectedStretch = Stretches[2];
		SelectedWeight = Weights[1];
		SelectedScale = Scales[2];
		SelectedEffect = Effects[0];
	}


	public List<ShowcaseOption<string>> Sources { get; } =
	[
		new("Remote", StockImageUrl),
		new("Failure", FailureUrl)
	];

	public List<ShowcaseOption<Stretch>> Stretches { get; } =
	[
		new("None", Stretch.None),
		new("Fill", Stretch.Fill),
		new("Uniform", Stretch.Uniform),
		new("Uniform to fill", Stretch.UniformToFill)
	];

	public List<ShowcaseOption<FontWeight>> Weights { get; } =
	[
		new("Regular", FontWeight.Regular),
		new("Semibold", FontWeight.Semibold),
		new("Bold", FontWeight.Bold)
	];

	public List<ShowcaseOption<SymbolScale>> Scales { get; } =
	[
		new("Small", SymbolScale.Small),
		new("Medium", SymbolScale.Medium),
		new("Large", SymbolScale.Large)
	];

	public List<ShowcaseOption<SymbolEffect>> Effects { get; } =
	[
		new("None", SymbolEffect.None),
		new("Pulse", SymbolEffect.Pulse),
		new("Variable color", SymbolEffect.VariableColor),
		new("Breathe", SymbolEffect.Breathe),
		new("Wiggle", SymbolEffect.Wiggle),
		new("Rotate", SymbolEffect.Rotate)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SourceCode))]
	ShowcaseOption<string> selectedSource = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SourceCode))]
	ShowcaseOption<Stretch> selectedStretch = null!;

	[ObservableProperty]
	ImageSource? remoteSource;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(RenderingCode))]
	double symbolSize = 72;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(RenderingCode))]
	ShowcaseOption<FontWeight> selectedWeight = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(RenderingCode))]
	ShowcaseOption<SymbolScale> selectedScale = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(RenderingCode))]
	bool prefersMulticolor = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EffectsCode))]
	double symbolValue = 0.6;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EffectsCode))]
	ShowcaseOption<SymbolEffect> selectedEffect = null!;

	public IReadOnlyList<Span> SourceCode =>
	[
		new(
			$$"""
			new Image
			{
				Width = 280,
				Height = 180,
				Source = ImageSource.Url("{{SelectedSource.Value}}"),
				Placeholder = ImageSource.Symbol("photo"),
				Fallback = ImageSource.Symbol("exclamationmark.triangle.fill"),
				FadesIn = true,
				Stretch = Stretch.{{SelectedStretch.Value}}
			};
			""")
	];

	public IReadOnlyList<Span> RenderingCode =>
	[
		new(
			$$"""
			new Image
			{
				Source = ImageSource.Symbol("cloud.sun.rain.fill"),
				SymbolSize = {{SymbolSize:0}},
				SymbolWeight = FontWeight.{{SelectedWeight.Value}},
				SymbolScale = SymbolScale.{{SelectedScale.Value}},
				PrefersMulticolor = {{Boolean(PrefersMulticolor)}}
			};
			""")
	];

	public IReadOnlyList<Span> EffectsCode =>
	[
		new(
			$$"""
			new Image
			{
				Source = ImageSource.Symbol("speaker.wave.3.fill"),
				SymbolSize = 72,
				SymbolValue = Bind(vm => vm.SymbolValue),
				SymbolEffect = SymbolEffect.{{SelectedEffect.Value}}
			};

			image.PlaySymbolEffect(SymbolEffect.Bounce);
			""")
	];


	partial void OnSelectedSourceChanged(
		ShowcaseOption<string> value) =>
		RemoteSource = ImageSource.Url(value.Value);


	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
