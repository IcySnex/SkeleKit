using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Image"/> with SF Symbols, URL images, and different stretch modes.
/// </summary>
public static class ImageDemo
{
	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Theme.Caption("SF Symbol"),
					new Image
					{
						Source = ImageSource.Symbol("star.fill"),
						Width = 60,
						Height = 60,
						Stretch = Stretch.Uniform
					},

					Theme.Caption("URL image"),
					new Image
					{
						Source = ImageSource.Url("https://picsum.photos/300/200"),
						Width = 300,
						Height = 200,
						Stretch = Stretch.UniformToFill
					},

					Theme.Caption("SF Symbol with fill stretch"),
					new Image
					{
						Source = ImageSource.Symbol("heart.fill"),
						Width = 80,
						Height = 80,
						Stretch = Stretch.Fill
					},

					Theme.Caption("SF Symbol no stretch"),
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
