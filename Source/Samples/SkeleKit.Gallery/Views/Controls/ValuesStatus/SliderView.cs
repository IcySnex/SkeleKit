using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class SliderView : ShowcaseView<SliderViewModel>
{
	public SliderView(
		SliderViewModel viewModel) : base(viewModel, "Slider", Colors.Red)
	{
		AddSliderShowcase(viewModel);
	}


	void AddSliderShowcase(
		SliderViewModel viewModel)
	{
		Slider slider = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Value = Bind(
				model => model.Value,
				static (model, value) => model.Value = value),
			Minimum = 0,
			Maximum = 100,
			Step = viewModel.SelectedStep.Value,
			Continuous = viewModel.Continuous,
			MinIcon = ImageSource.Symbol("speaker.fill"),
			MaxIcon = ImageSource.Symbol("speaker.wave.3.fill"),
			IsEnabled = Bind(model => model.ControlEnabled)
		};

		Picker<ShowcaseOption<double>> step = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Steps,
			SelectedItem = Bind(
				model => model.SelectedStep,
				static (model, value) => model.SelectedStep = value!),
			SelectionChanged = option => slider.Step = option.Value
		};

		Switch continuous = new()
		{
			IsOn = Bind(
				model => model.Continuous,
				static (model, value) => model.Continuous = value),
			Toggled = value =>
			{
				slider.Continuous = value;
			}
		};

		Switch icons = new()
		{
			IsOn = Bind(
				model => model.ShowsIcons,
				static (model, value) => model.ShowsIcons = value),
			Toggled = value =>
			{
			slider.MinIcon = value ? ImageSource.Symbol("speaker.fill") : (ImageSource?)null;
			slider.MaxIcon = value ? ImageSource.Symbol("speaker.wave.3.fill") : (ImageSource?)null;
			}
		};

		Switch enabled = new()
		{
			IsOn = Bind(
				model => model.ControlEnabled,
				static (model, value) => model.ControlEnabled = value)
		};

		AddShowcase(
			"Value & behavior",
			"Drag a two-way 0–100 slider, choose snapping, and compare continuous and release-only updates.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 10,

						Children =
						{
							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.ValueLabel),
								TextStyle = TextStyle.Title2,
								FontWeight = FontWeight.Semibold
							},
							slider
						}
					},
					190),
				SettingRow(
					"Bound value",
					new Button
					{
						Text = "Reset",
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Small,
						Command = viewModel.ResetValueCommand
					}),
				SettingRow("Step", step),
				SettingRow("Continuous updates", continuous),
				SettingRow("Endpoint symbols", icons),
				SettingRow("Enabled", enabled)),
			Code(model => model.SliderCode));
	}

}
