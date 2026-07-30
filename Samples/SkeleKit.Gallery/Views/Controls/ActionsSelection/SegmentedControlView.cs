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
		AddDensityShowcase();
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

	void AddDensityShowcase()
	{
		AddShowcase(
			"Content density",
			"Compare how the native control distributes short titles across different segment counts.",
			ShowcaseBox.Canvas(
				new StackPanel
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					MaxWidth = 320,
					Spacing = 14,

					Children =
					{
						Variant("Two options", Segments("Day", "Week")),
						Variant("Three options", Segments("Day", "Week", "Month")),
						Variant("Five options", Segments("1D", "1W", "1M", "6M", "1Y"))
					}
				},
				260),
			ShowcaseBox.Code(Bind(model => model.DensityCode)));
	}


	static View Variant(
		string title,
		SegmentedControl control) =>
		new StackPanel
		{
			Spacing = 5,

			Children =
			{
				new Label
				{
					Text = title,
					TextStyle = TextStyle.Caption1,
					TextColor = Colors.SecondaryLabel
				},

				control
			}
		};

	static SegmentedControl Segments(
		params string[] items)
	{
		SegmentedControl control = new();

		foreach (string item in items)
			control.Items.Add(item);

		return control;
	}
}
