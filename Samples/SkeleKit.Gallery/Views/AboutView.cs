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
			Text = "Done",
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
						Background = LinearGradient.Horizontal(Colors.Indigo, Colors.Purple),

						Child = new Image
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Source = ImageSource.Symbol("square.stack.3d.up.fill"),
							SymbolSize = 40,
							SymbolWeight = FontWeight.Semibold,
							Tint = Colors.White
						}
					},

					new Label
					{
						Text = "SkeleKit",
						TextStyle = TextStyle.Title1,
						FontWeight = FontWeight.Bold,
						TextAlignment = TextAlignment.Center
					},

					new Label
					{
						Text = "Native iOS experiences from clean, composable C#.",
						TextStyle = TextStyle.Body,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center,
						MaxLines = 2
					},

					new Border
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Padding = new(10, 5),
						CornerRadius = 12,
						Background = Colors.Indigo.WithAlpha(0.12),

						Child = new Label
						{
							Text = "NATIVE UI · MVVM · NO XAML",
							TextStyle = TextStyle.Caption1,
							FontWeight = FontWeight.Semibold,
							TextColor = Colors.Indigo
						}
					},

					new Divider
					{
						Margin = new(0, 8)
					},

					new Button
					{
						Text = "View on GitHub",
						Icon = "arrow.up.right",
						Kind = ButtonStyle.FilledCapsule,
						Size = ButtonSize.Large,
						Tint = Colors.Indigo,
						Command = viewModel.OpenGitHubCommand
					},

					new Button
					{
						Text = "Open-Source Licenses",
						Icon = "doc.text",
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
