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
			MinIcon = "speaker.fill",
			MaxIcon = "speaker.wave.3.fill",
			IsEnabled = viewModel.ControlEnabled,
			ValueChanged = viewModel.RecordChange
		};

		Picker<ShowcaseOption<double>> step = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Steps,
			SelectedItem = viewModel.SelectedStep,
			SelectionChanged = option =>
			{
				viewModel.SelectedStep = option;
				slider.Step = option.Value;
			}
		};

		Switch continuous = new()
		{
			IsOn = viewModel.Continuous,
			Toggled = value =>
			{
				viewModel.Continuous = value;
				slider.Continuous = value;
			}
		};

		Switch icons = new()
		{
			IsOn = viewModel.ShowsIcons,
			Toggled = value =>
			{
				viewModel.ShowsIcons = value;
				slider.MinIcon = value ? "speaker.fill" : null;
				slider.MaxIcon = value ? "speaker.wave.3.fill" : null;
			}
		};

		Switch enabled = new()
		{
			IsOn = viewModel.ControlEnabled,
			Toggled = value =>
			{
				viewModel.ControlEnabled = value;
				slider.IsEnabled = value;
			}
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
							slider,
							Status(Bind(model => model.ChangeStatus))
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
			ShowcaseBox.Code(Bind(model => model.SliderCode)));
	}


	static Label Status(
		BindingExpression<string?> text) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel,
			MaxLines = 2,
			TextAlignment = TextAlignment.Center
		};
}
