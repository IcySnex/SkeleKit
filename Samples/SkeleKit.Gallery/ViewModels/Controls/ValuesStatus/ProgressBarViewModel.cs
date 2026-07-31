using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class ProgressBarViewModel : ShowcaseViewModel
{
	public ProgressBarViewModel()
	{
		SelectedProgress = ProgressValues[2];
	}


	public List<ShowcaseOption<double>> ProgressValues { get; } =
	[
		new("Empty", 0),
		new("25%", 0.25),
		new("65%", 0.65),
		new("Complete", 1)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ProgressLabel))]
	[NotifyPropertyChangedFor(nameof(ProgressBarCode))]
	double progress = 0.65;

	[ObservableProperty]
	ShowcaseOption<double> selectedProgress = null!;

	public string ProgressLabel =>
		$"{Progress:P0}";

	public IReadOnlyList<Span> ProgressBarCode =>
	[
		new(
			$$"""
			new ProgressBar
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Progress = Bind(model => model.Progress),
				FillColor = Colors.Red,
				TrackColor = Colors.Red.WithAlpha(0.16)
			};
			""")
	];
}
