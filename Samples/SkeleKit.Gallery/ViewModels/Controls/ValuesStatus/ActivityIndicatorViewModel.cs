using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class ActivityIndicatorViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateLabel))]
	[NotifyPropertyChangedFor(nameof(IndicatorCode))]
	bool isAnimating = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateLabel))]
	[NotifyPropertyChangedFor(nameof(IndicatorCode))]
	bool isLarge;

	public string StateLabel =>
		$"{(IsLarge ? "Large" : "Medium")} · {(IsAnimating ? "animating" : "stopped · hidden")}";

	public IReadOnlyList<Span> IndicatorCode =>
	[
		new(
			$$"""
			new ActivityIndicator
			{
				IsAnimating = Bind(model => model.IsAnimating),
				IsLarge = {{Boolean(IsLarge)}},
				Color = Colors.Red
			};
			""")
	];


	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
