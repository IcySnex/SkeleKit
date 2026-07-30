using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class DatePickerView : ShowcaseView<DatePickerViewModel>
{
	public DatePickerView(
		DatePickerViewModel viewModel) : base(viewModel, "Date Picker", Colors.Purple)
	{
		AddConfigurationShowcase(viewModel);
		AddRangeShowcase(viewModel);
	}


	void AddConfigurationShowcase(
		DatePickerViewModel viewModel)
	{
		DatePicker picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Date = DatePickerViewModel.ExampleDate,
			Mode = viewModel.SelectedMode,
			Kind = viewModel.SelectedStyle
		};

		View canvas = ShowcaseBox.Canvas(picker, StyleHeight(viewModel.SelectedStyle));

		SegmentedControl mode = new()
		{
			SelectedIndex = viewModel.SelectedModeIndex,
			SelectionChanged = index =>
			{
				viewModel.SelectedModeIndex = index;
				picker.Mode = viewModel.SelectedMode;
			}
		};
		mode.Items.Add("Date");
		mode.Items.Add("Time");
		mode.Items.Add("Both");

		SegmentedControl style = new()
		{
			SelectedIndex = viewModel.SelectedStyleIndex,
			SelectionChanged = index =>
			{
				viewModel.SelectedStyleIndex = index;
				picker.Kind = viewModel.SelectedStyle;
				canvas.Height = StyleHeight(viewModel.SelectedStyle);
			}
		};
		style.Items.Add("Compact");
		style.Items.Add("Inline");
		style.Items.Add("Wheels");

		AddShowcase(
			"Mode & presentation",
			"Compare every native input mode and presentation style on one deterministic value.",
			PreviewWithSettings(
				canvas,
				LabeledControl("Mode", mode),
				LabeledControl("Style", style)),
			ShowcaseBox.Code(Bind(model => model.ConfigurationCode)));
	}

	void AddRangeShowcase(
		DatePickerViewModel viewModel)
	{
		DatePicker picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Date = Bind(
				model => model.SelectedDate,
				static (model, value) => model.SelectedDate = value),
			Mode = DatePickerMode.DateAndTime,
			Kind = DatePickerStyle.Compact,
			Minimum = DatePickerViewModel.MinimumDate,
			Maximum = DatePickerViewModel.MaximumDate,
			DateChanged = viewModel.RecordDateChanged
		};

		SegmentedControl position = new()
		{
			SelectedIndex = 1,
			SelectionChanged = viewModel.SetRangePosition
		};
		position.Items.Add("Start");
		position.Items.Add("Middle");
		position.Items.Add("End");

		AddShowcase(
			"Range & binding",
			"Constrain the available interval and keep picker, ViewModel and callback output synchronized.",
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
							picker,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.DateSummary),
								TextStyle = TextStyle.Subheadline,
								FontWeight = FontWeight.Medium,
								TextAlignment = TextAlignment.Center
							},

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.DateChangedStatus),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					210),
				LabeledControl("Bound value", position)),
			ShowcaseBox.Code(Bind(model => model.RangeCode)));
	}


	static double StyleHeight(
		DatePickerStyle style) =>
		style switch
		{
			DatePickerStyle.Inline => 370,
			DatePickerStyle.Wheels => 250,
			_ => 156
		};
}
