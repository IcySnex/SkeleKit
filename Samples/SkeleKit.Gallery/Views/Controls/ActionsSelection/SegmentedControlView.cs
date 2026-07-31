using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class SegmentedControlView : ShowcaseView<SegmentedControlViewModel>
{
	public SegmentedControlView(
		SegmentedControlViewModel viewModel) : base(viewModel, "Segmented Control", Colors.Purple)
	{
		AddSelectionShowcase(viewModel);
	}


	void AddSelectionShowcase(
		SegmentedControlViewModel viewModel)
	{
		SegmentedControl sections = new()
		{
			SelectedIndex = Bind(
				model => model.SelectedIndex,
				static (model, value) => model.SelectedIndex = value),
			SelectionChanged = viewModel.RecordSelection
		};
		sections.Items.Add("Overview");
		sections.Items.Add("Details");
		sections.Items.Add("Reviews");

		Button reset = new()
		{
			Text = "Reset",
			Icon = "arrow.counterclockwise",
			Kind = ButtonStyle.Tinted,
			Command = viewModel.ResetSelectionCommand
		};

		AddShowcase(
			"Selection & binding",
			"Keep the selected index, visible segment and callback result synchronized.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 10,

						Children =
						{
							sections,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.SelectedTitle),
								TextStyle = TextStyle.Subheadline,
								FontWeight = FontWeight.Medium,
								TextAlignment = TextAlignment.Center
							},

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.SelectionStatus),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					190),
				SettingRow("Selection", reset)),
			ShowcaseBox.Code(Bind(model => model.SelectionCode)));
	}
}
