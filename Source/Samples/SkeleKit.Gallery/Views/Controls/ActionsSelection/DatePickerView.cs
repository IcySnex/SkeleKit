using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class DatePickerView : ShowcaseView<DatePickerViewModel>
{
	public DatePickerView(
		DatePickerViewModel viewModel) : base(viewModel, "Date Picker", Colors.Pink)
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

		View canvas = ShowcaseBox.FittingCanvas(picker);

		SegmentedControl mode = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedModeIndex)
				.TwoWay((vm, val) => vm.SelectedModeIndex = val),
			SelectionChanged = index =>
			{
				picker.Mode = viewModel.SelectedMode;
			}
		};
		mode.Items.Add("Date");
		mode.Items.Add("Time");
		mode.Items.Add("Both");

		SegmentedControl style = new()
		{
			SelectedIndex = Bind(vm => vm.SelectedStyleIndex)
				.TwoWay((vm, val) => vm.SelectedStyleIndex = val),
			SelectionChanged = index =>
			{
				picker.Kind = viewModel.SelectedStyle;
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
			Code(vm => vm.ConfigurationCode));
	}

	void AddRangeShowcase(
		DatePickerViewModel viewModel)
	{
		DatePicker picker = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Width = 215,
			Date = Bind(vm => vm.SelectedDate)
				.TwoWay((vm, val) => vm.SelectedDate = val),
			Mode = DatePickerMode.DateAndTime,
			Kind = DatePickerStyle.Compact,
			Minimum = DatePickerViewModel.MinimumDate,
			Maximum = DatePickerViewModel.MaximumDate
		};

		SegmentedControl position = new()
		{
			SelectedIndex = Bind(vm => vm.RangePositionIndex)
				.TwoWay((vm, val) => vm.RangePositionIndex = val)
		};
		position.Items.Add("Start");
		position.Items.Add("Middle");
		position.Items.Add("End");

		AddShowcase(
			"Range & binding",
			"Constrain the available interval and keep the picker synchronized with a two-way bound value.",
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
								Text = Bind(vm => vm.DateSummary),
								TextStyle = TextStyle.Subheadline,
								FontWeight = FontWeight.Medium,
								TextAlignment = TextAlignment.Center
							},
						}
					},
					190),
				LabeledControl("Bound value", position)),
			Code(vm => vm.RangeCode));
	}
}
