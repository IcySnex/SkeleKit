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
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	int selectedIndex = 1;

	public IReadOnlyList<Span> SelectionCode =>
	[
		new(
			$$"""
			SegmentedControl sections = new()
			{
				SelectedIndex = Bind(vm => vm.SelectedIndex)
					.TwoWay((vm, val) => vm.SelectedIndex = val)
			};
			sections.Items.Add("Overview");
			sections.Items.Add("Details");
			sections.Items.Add("Reviews");
			""")
	];

	[RelayCommand]
	void ResetSelection() =>
		SelectedIndex = 0;
}
