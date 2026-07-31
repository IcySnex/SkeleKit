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
		Switch animating = new()
		{
			IsOn = viewModel.IsAnimating,
			Toggled = value => viewModel.IsAnimating = value
		};

		AddShowcase(
			"Loading state",
			"Compare the native medium and large styles, then stop and restart their animation.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 12,

						Children =
						{
							new StackPanel
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Orientation = Orientation.Horizontal,
								Spacing = 48,

								Children =
								{
									Indicator("Medium", new ActivityIndicator
									{
										IsAnimating = Bind(model => model.IsAnimating)
									}),
									Indicator("Large", new ActivityIndicator
									{
										IsAnimating = Bind(model => model.IsAnimating),
										IsLarge = true,
										Color = Colors.Red
									})
								}
							},
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
				SettingRow("Animating", animating)),
			ShowcaseBox.Code(Bind(model => model.IndicatorCode)));
	}


	static StackPanel Indicator(
		string title,
		ActivityIndicator indicator) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Spacing = 10,

			Children =
			{
				indicator,
				new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = title,
					TextStyle = TextStyle.Caption1,
					TextColor = Colors.SecondaryLabel
				}
			}
		};
}
