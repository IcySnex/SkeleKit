using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class PickerView : ShowcaseView<PickerViewModel>
{
	public PickerView(
		PickerViewModel viewModel) : base(viewModel, "Picker", Colors.Purple)
	{
		AddPickerShowcase(viewModel);
	}


	void AddPickerShowcase(
		PickerViewModel viewModel)
	{
		Picker<PickerDestination> picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			MinWidth = 220,
			ItemsSource = Bind(model => model.Destinations),
			SelectedItem = Bind(
				model => model.SelectedDestination,
				static (model, value) => model.SelectedDestination = value),
			Placeholder = "Choose a destination",
			ItemTitle = DestinationTitle,
			SelectionChanged = viewModel.RecordSelection
		};

		Button clear = new()
		{
			Text = "Clear",
			Icon = "xmark",
			Kind = ButtonStyle.Tinted,
			Command = viewModel.ClearSelectionCommand
		};

		SegmentedControl items = new()
		{
			SelectionChanged = viewModel.SetItemsState
		};
		items.Items.Add("Base");
		items.Items.Add("Empty");
		items.Items.Add("Extended");

		AddShowcase(
			"Selection & items",
			"Bind and format the selected model while the observable collection changes in place.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 300,
						Spacing = 10,

						Children =
						{
							picker,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.SelectedSummary),
								TextStyle = TextStyle.Subheadline,
								FontWeight = FontWeight.Medium,
								TextAlignment = TextAlignment.Center
							},

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.ItemsSummary),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center
							},

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.SelectionStatus),
								TextStyle = TextStyle.Caption1,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					210),
				SettingRow("Selection", clear),
				LabeledControl("Collection contents", items)),
			ShowcaseBox.Code(Bind(model => model.PickerCode)));
	}


	static string DestinationTitle(
		PickerDestination destination) =>
		$"{destination.City}, {destination.Country}";
}
