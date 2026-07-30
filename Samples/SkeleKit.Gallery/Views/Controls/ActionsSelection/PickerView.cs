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
		AddSelectionShowcase(viewModel);
		AddLiveItemsShowcase(viewModel);
	}


	void AddSelectionShowcase(
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

		AddShowcase(
			"Selection & labels",
			"Bind a selected model in both directions and format each menu title without changing the data.",
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
								Text = Bind(model => model.SelectionStatus),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					190),
				SettingRow("Selection", clear)),
			ShowcaseBox.Code(Bind(model => model.SelectionCode)));
	}

	void AddLiveItemsShowcase(
		PickerViewModel viewModel)
	{
		Picker<PickerDestination> picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			MinWidth = 180,
			ItemsSource = Bind(model => model.LiveDestinations),
			SelectedItem = Bind(
				model => model.LiveSelectedDestination,
				static (model, value) => model.LiveSelectedDestination = value),
			Placeholder = "Select an item",
			ItemTitle = DestinationTitle
		};

		SegmentedControl items = new()
		{
			SelectionChanged = viewModel.SetLiveItemsState
		};
		items.Items.Add("Base");
		items.Items.Add("Empty");
		items.Items.Add("Extended");

		AddShowcase(
			"Live items",
			"Mutate an ObservableCollection in place and keep the native menu, checkmark and intrinsic width current.",
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
								Text = Bind(model => model.LiveSelectionSummary),
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
							}
						}
					},
					190),
				LabeledControl("Collection contents", items)),
			ShowcaseBox.Code(Bind(model => model.LiveItemsCode)));
	}


	static string DestinationTitle(
		PickerDestination destination) =>
		$"{destination.City}, {destination.Country}";
}
