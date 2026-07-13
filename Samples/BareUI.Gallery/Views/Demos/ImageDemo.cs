using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Image"/> with SF Symbols, URL images, and different stretch modes.
/// </summary>
public class ImageDemo : ContentView<ImageDemoViewModel>
{
	public ImageDemo(
		ImageDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Image";

		Content =
			new ScrollView
			{
				Content = new StackPanel
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Style = Styles.Caption, Text = "SF Symbol" },
						new Image
						{
							Source = ImageSource.Symbol("star.fill"),
							Width = 60,
							Height = 60,
							Stretch = Stretch.Uniform
						},

						new Label { Style = Styles.Caption, Text = "URL image" },
						new Image
						{
							Source = ImageSource.Url("https://picsum.photos/300/200"),
							Width = 300,
							Height = 200,
							Stretch = Stretch.UniformToFill
						},

						new Label { Style = Styles.Caption, Text = "SF Symbol with fill stretch" },
						new Image
						{
							Source = ImageSource.Symbol("heart.fill"),
							Width = 80,
							Height = 80,
							Stretch = Stretch.Fill
						},

						new Label { Style = Styles.Caption, Text = "SF Symbol no stretch" },
						new Image
						{
							Source = ImageSource.Symbol("gear"),
							Width = 50,
							Height = 50,
							Stretch = Stretch.None
						}
					}
				}
			};
	}
}
