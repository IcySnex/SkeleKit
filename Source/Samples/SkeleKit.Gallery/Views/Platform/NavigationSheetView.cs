using SkeleKit.Gallery.ViewModels.Platform;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class NavigationSheetView : ContentView<NavigationSheetViewModel>
{
	public NavigationSheetView(
		NavigationSheetViewModel viewModel) : base(viewModel)
	{
		Title = "Navigation sheet";
		TitleStyle = TitleStyle.Inline;
		BackgroundStyle = PageBackground.Grouped;

		ToolbarItems.Add(new()
		{
			Text = "Done",
			Command = viewModel.DismissCommand
		});

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				Padding = new(24, 24, 24, 32),
				Spacing = 12,

				Children =
				{
					new Image
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Source = ImageSource.Symbol("rectangle.portrait.bottomhalf.filled"),
						SymbolSize = 38,
						SymbolWeight = FontWeight.Semibold,
						Tint = Colors.Green
					},

					new Label
					{
						Text = "A real modal page",
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold,
						TextAlignment = TextAlignment.Center
					},

					new Label
					{
						Text = "Dismiss this sheet with Done or swipe it down.",
						TextStyle = TextStyle.Subheadline,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center,
						MaxLines = 3
					},

					new Button
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Dismiss sheet",
						Icon = ImageSource.Symbol("xmark"),
						Kind = ButtonStyle.Tinted,
						Size = ButtonSize.Medium,
						Command = viewModel.DismissCommand
					}
				}
			}
		};
	}
}
