using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class PageControlViewModel : ShowcaseViewModel
{
	public PageControlViewModel()
	{
		SelectedCount = Counts[2];
	}


	public List<ShowcaseOption<int>> Counts { get; } =
	[
		new("1", 1),
		new("3", 3),
		new("5", 5),
		new("10", 10)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateLabel))]
	[NotifyPropertyChangedFor(nameof(PageControlCode))]
	int count = 5;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateLabel))]
	int current = 2;

	[ObservableProperty]
	ShowcaseOption<int> selectedCount = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageControlCode))]
	bool hidesForSinglePage = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageControlCode))]
	bool allowsScrubbing = true;

	public string StateLabel =>
		$"Page {Current + 1} of {Count}";

	public IReadOnlyList<Span> PageControlCode =>
	[
		new(
			$$"""
			new PageControl
			{
				Count = Bind(model => model.Count),
				Current = Bind(
					model => model.Current,
					(model, value) => model.Current = value),
				DotColor = Colors.Red.WithAlpha(0.25),
				CurrentDotColor = Colors.Red,
				HidesForSinglePage = {{Boolean(HidesForSinglePage)}},
				AllowsScrubbing = {{Boolean(AllowsScrubbing)}}
			};
			""")
	];


	[RelayCommand]
	void AdvancePage() =>
		Current = (Current + 1) % Count;

	internal void SelectCount(
		ShowcaseOption<int> option)
	{
		SelectedCount = option;
		Count = option.Value;
		Current = Math.Min(Current, Count - 1);
	}

	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
