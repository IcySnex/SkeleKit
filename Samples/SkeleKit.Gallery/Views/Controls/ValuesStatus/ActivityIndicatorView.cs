using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.ValuesStatus;

[Page]
internal sealed class ActivityIndicatorView : ShowcaseView<ActivityIndicatorViewModel>
{
	public ActivityIndicatorView(
		ActivityIndicatorViewModel viewModel) : base(viewModel, "Activity Indicator", Colors.Red)
	{
		AddActivityShowcase(viewModel);
	}


	void AddActivityShowcase(
		ActivityIndicatorViewModel viewModel)
	{
		ActivityIndicator indicator = new()
		{
			IsAnimating = Bind(model => model.IsAnimating),
			IsLarge = viewModel.IsLarge,
			Color = Colors.Red
		};

		Switch animating = new()
		{
			IsOn = viewModel.IsAnimating,
			Toggled = value => viewModel.IsAnimating = value
		};

		Switch size = new()
		{
			IsOn = viewModel.IsLarge,
			Toggled = value =>
			{
				viewModel.IsLarge = value;
				indicator.IsLarge = value;
			}
		};

		AddShowcase(
			"Loading state",
			"Switch the native indicator between medium and large, then stop and restart its animation.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 12,

						Children =
						{
							indicator,
							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.StateLabel),
								TextStyle = TextStyle.Footnote,
								TextColor = Colors.SecondaryLabel
							}
						}
					},
					180),
				SettingRow("Large", size),
				SettingRow("Animating", animating)),
			ShowcaseBox.Code(Bind(model => model.IndicatorCode)));
	}
}
