using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class StepperView : ShowcaseView<StepperViewModel>
{
	public StepperView(
		StepperViewModel viewModel) : base(viewModel, "Stepper", Colors.Red)
	{
		AddStepperShowcase(viewModel);
	}


	void AddStepperShowcase(
		StepperViewModel viewModel)
	{
		Stepper stepper = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Value = Bind(vm => vm.Value)
				.TwoWay((vm, val) => vm.Value = val),
			Minimum = 0,
			Maximum = 20,
			Step = viewModel.SelectedStep.Value,
			IsEnabled = Bind(vm => vm.ControlEnabled)
		};

		Picker<ShowcaseOption<double>> increment = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Steps,
			SelectedItem = Bind(vm => vm.SelectedStep)
				.TwoWay((vm, val) => vm.SelectedStep = val!),
			SelectionChanged = option => stepper.Step = option.Value
		};

		Switch enabled = new()
		{
			IsOn = Bind(vm => vm.ControlEnabled)
				.TwoWay((vm, val) => vm.ControlEnabled = val)
		};

		AddShowcase(
			"Value & range",
			"Increment and decrement a bounded two-way value, then compare different step sizes.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 10,

						Children =
						{
							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(vm => vm.ValueLabel),
								TextStyle = TextStyle.Title2,
								FontWeight = FontWeight.Semibold
							},
							stepper
						}
					},
					170),
				SettingRow(
					"Bound value",
					new Button
					{
						Text = "Reset",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = viewModel.ResetValueCommand
					}),
				SettingRow("Increment", increment),
				SettingRow("Enabled", enabled)),
			Code(vm => vm.StepperCode));
	}

}
