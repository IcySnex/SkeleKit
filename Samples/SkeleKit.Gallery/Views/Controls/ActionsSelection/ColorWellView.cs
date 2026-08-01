using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ActionsSelection;

[Page]
internal sealed class ColorWellView : ShowcaseView<ColorWellViewModel>
{
	public ColorWellView(
		ColorWellViewModel viewModel) : base(viewModel, "Color Well", Colors.Purple)
	{
		AddSelectionShowcase(viewModel);
	}


	void AddSelectionShowcase(
		ColorWellViewModel viewModel)
	{
		ColorWell well = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Selected = Bind(
				model => model.SelectedColor,
				static (model, value) => model.SelectedColor = value),
			Title = "Gallery accent",
			SupportsAlpha = viewModel.SupportsAlpha
		};

		Switch title = new()
		{
			IsOn = Bind(
				model => model.ShowsTitle,
				static (model, value) => model.ShowsTitle = value),
			Toggled = value =>
			{
				well.Title = value ? "Gallery accent" : null;
			}
		};

		Switch alpha = new()
		{
			IsOn = Bind(
				model => model.SupportsAlpha,
				static (model, value) => model.SupportsAlpha = value),
			Toggled = value =>
			{
				well.SupportsAlpha = value;
			}
		};

		Button reset = new()
		{
			Text = "Reset",
			Icon = "arrow.counterclockwise",
			Kind = ButtonStyle.Tinted,
			Command = viewModel.ResetColorCommand
		};

		AddShowcase(
			"Selection & presentation",
			"Open the system picker, bind its live color, and configure title and opacity support.",
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
							well,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.ColorSummary),
								TextStyle = TextStyle.Subheadline,
								FontWeight = FontWeight.Medium,
								TextAlignment = TextAlignment.Center
							},
						}
					},
					190),
				SettingRow("Picker title", title),
				SettingRow("Opacity slider", alpha),
				SettingRow("Bound value", reset)),
			ShowcaseBox.Code(Bind(model => model.SelectionCode)));
	}
}
