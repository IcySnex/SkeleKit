using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;

internal sealed partial class SegmentedControlViewModel : ShowcaseViewModel
{
	static readonly string[] Sections =
	[
		"Overview",
		"Details",
		"Reviews"
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedTitle))]
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	int selectedIndex = 1;

	[ObservableProperty]
	string selectionStatus = "SelectionChanged has not fired yet.";

	int selectionCount;

	public string SelectedTitle =>
		Sections[Math.Clamp(SelectedIndex, 0, Sections.Length - 1)];

	public IReadOnlyList<Span> SelectionCode =>
	[
		new(
			$$"""
			SegmentedControl sections = new()
			{
				SelectedIndex = Bind(
					model => model.SelectedIndex,
					(model, value) => model.SelectedIndex = value),
				SelectionChanged = viewModel.RecordSelection
			};
			sections.Items.Add("Overview");
			sections.Items.Add("Details");
			sections.Items.Add("Reviews");
			""")
	];

	[RelayCommand]
	void ResetSelection()
	{
		SelectedIndex = 0;
		SelectionStatus = "Selection reset from the ViewModel.";
	}

	internal void RecordSelection(
		int index)
	{
		selectionCount++;
		SelectionStatus = $"SelectionChanged · {Sections[index]} · {selectionCount}";
	}
}
