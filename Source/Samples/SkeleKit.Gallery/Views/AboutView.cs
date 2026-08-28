using SkeleKit.Gallery.ViewModels;

namespace SkeleKit.Gallery.Views;

[Page]
internal sealed class AboutView : ContentView<AboutViewModel>
{
	public AboutView(
		AboutViewModel viewModel) : base(viewModel)
	{
		Title = "SkeleKit";
		BackgroundStyle = PageBackground.Grouped;

		ToolbarItems.Add(new()
		{
			Icon = ImageSource.Symbol("xmark"),
			Tint = Colors.Indigo,
			Command = viewModel.DismissCommand
		});

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Padding = new(24, 20, 24, 28),
				Spacing = 12,

				Children =
				{
					new Border
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Width = 86,
						Height = 86,
						CornerRadius = 22,

						Child = new Image
						{
							Source = ImageSource.Bundle("AppIcon60x60")
						}
					},

					new Label
					{
						Margin = new(0, 0, 0, 4),
						Text = "SkeleKit",
						TextStyle = TextStyle.Title1,
						FontWeight = FontWeight.Bold,
						TextAlignment = TextAlignment.Center
					},

					new Border
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Padding = new(10, 5),
						CornerRadius = 12,
						Background = Colors.Indigo.WithAlpha(0.12),

						Child = new Label
						{
							Text = "C# UI FRAMEWORK FOR NATIVE IOS",
							TextStyle = TextStyle.Footnote,
							FontWeight = FontWeight.Semibold,
							TextColor = Colors.Indigo
						}
					},

					new Label
					{
						Margin = new(0, -8, 0, 0),
						Text = "Real UIKit controls, no storyboards or constraints, zero boilerplate.",
						TextStyle = TextStyle.Callout,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center,
						MaxLines = 3
					},

					new Divider
					{
						Margin = new(0, 8)
					},

					new Button
					{
						Text = "View on GitHub",
			Icon = ImageSource.Symbol("arrow.up.right"),
						Kind = ButtonStyle.Filled,
						Size = ButtonSize.Large,
						Tint = Colors.Indigo,
						Command = viewModel.OpenGitHubCommand
					},

					new Button
					{
						Text = "Open-Source Licenses",
			Icon = ImageSource.Symbol("doc.text"),
						Kind = ButtonStyle.Gray,
						Size = ButtonSize.Large,
						Tint = Colors.Indigo,
						Command = viewModel.ShowLicensesCommand
					}
				}
			}
		};
	}
}
