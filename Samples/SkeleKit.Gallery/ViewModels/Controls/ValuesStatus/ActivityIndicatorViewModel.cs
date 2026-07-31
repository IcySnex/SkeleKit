using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class ActivityIndicatorViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateLabel))]
	[NotifyPropertyChangedFor(nameof(IndicatorCode))]
	bool isAnimating = true;

	public string StateLabel =>
		IsAnimating ? "Animating" : "Stopped · hidden";

	public IReadOnlyList<Span> IndicatorCode =>
	[
		new(
			$$"""
			new ActivityIndicator
			{
				IsAnimating = Bind(model => model.IsAnimating),
				IsLarge = true,
				Color = Colors.Red
			};
			""")
	];
}
