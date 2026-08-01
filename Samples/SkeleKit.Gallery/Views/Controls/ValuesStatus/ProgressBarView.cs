using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class ProgressBarView : ShowcaseView<ProgressBarViewModel>
{
	public ProgressBarView(
		ProgressBarViewModel viewModel) : base(viewModel, "Progress Bar", Colors.Red)
	{
		AddProgressShowcase(viewModel);
	}


	void AddProgressShowcase(
		ProgressBarViewModel viewModel)
	{
		Picker<ShowcaseOption<double>> progress = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.ProgressValues,
			SelectedItem = Bind(
				model => model.SelectedProgress,
				static (model, value) => model.SelectedProgress = value!)
		};

		AddShowcase(
			"Progress",
			"Compare empty, partial, and completed determinate progress with custom fill and track colors.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						MaxWidth = 320,
						Spacing = 12,

						Children =
						{
							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.ProgressLabel),
								TextStyle = TextStyle.Title2,
								FontWeight = FontWeight.Semibold
							},
							new ProgressBar
							{
								HorizontalAlignment = HorizontalAlignment.Stretch,
								Progress = Bind(model => model.Progress),
								FillColor = Colors.Red,
								TrackColor = Colors.Red.WithAlpha(0.16)
							}
						}
					},
					160),
				SettingRow("Progress", progress)),
			ShowcaseBox.Code(Bind(model => model.ProgressBarCode)));
	}
}
