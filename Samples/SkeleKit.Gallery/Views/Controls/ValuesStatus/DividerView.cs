using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class DividerView : ShowcaseView<DividerViewModel>
{
	public DividerView(
		DividerViewModel viewModel) : base(viewModel, "Divider", Colors.Red)
	{
		AddDividerShowcase(viewModel);
	}


	void AddDividerShowcase(
		DividerViewModel viewModel)
	{
		Switch accent = new()
		{
			IsOn = Bind(
				model => model.UsesAccent,
				static (model, value) => model.UsesAccent = value)
		};

		AddShowcase(
			"Separator",
			"Show a native-scale hairline using the adaptive system separator or an explicit color.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 12,

						Children =
						{
							new Label
							{
								Text = "Above the divider",
								TextStyle = TextStyle.Body
							},
							new Divider
							{
								HorizontalAlignment = HorizontalAlignment.Stretch,
								Color = Bind(model => model.DividerColor)
							},
							new Label
							{
								Text = "Below the divider",
								TextStyle = TextStyle.Body
							}
						}
					},
					180),
				SettingRow("Accent color", accent)),
			Code(model => model.DividerCode));
	}
}
