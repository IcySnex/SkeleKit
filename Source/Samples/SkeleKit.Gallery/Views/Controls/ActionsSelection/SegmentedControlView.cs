using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class SegmentedControlView : ShowcaseView<SegmentedControlViewModel>
{
	public SegmentedControlView(
		SegmentedControlViewModel viewModel) : base(viewModel, "Segmented Control", Colors.Pink)
	{
		AddSelectionShowcase(viewModel);
	}


	void AddSelectionShowcase(
		SegmentedControlViewModel viewModel)
	{
		SegmentedControl sections = new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			SelectedIndex = Bind(
				model => model.SelectedIndex,
				static (model, value) => model.SelectedIndex = value)
		};
		sections.Items.Add("Overview");
		sections.Items.Add("Details");
		sections.Items.Add("Reviews");

		Button reset = new()
		{
			Text = "Reset",
			Icon = ImageSource.Symbol("arrow.counterclockwise"),
			Kind = ButtonStyle.Tinted,
			Command = viewModel.ResetSelectionCommand
		};

		AddShowcase(
			"Selection & binding",
			"Keep the selected index and visible segment synchronized through two-way binding.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(sections, 150),
				SettingRow("Selection", reset)),
			Code(model => model.SelectionCode));
	}
}
