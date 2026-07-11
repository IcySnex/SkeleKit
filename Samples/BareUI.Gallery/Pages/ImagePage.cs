using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Image"/> with SF Symbols, URL images, and different stretch modes.
/// </summary>
public static class ImagePage
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
					Demo.Caption("SF Symbol"),
					new Image
					{
						Source = "star.fill",
						Width = 60,
						Height = 60,
						Stretch = Stretch.Uniform
					},

					Demo.Caption("URL image"),
					new Image
					{
						Source = "https://picsum.photos/300/200",
						Width = 300,
						Height = 200,
						Stretch = Stretch.UniformToFill
					},

					Demo.Caption("SF Symbol with fill stretch"),
					new Image
					{
						Source = "heart.fill",
						Width = 80,
						Height = 80,
						Stretch = Stretch.Fill
					},

					Demo.Caption("SF Symbol no stretch"),
					new Image
					{
						Source = "gear",
						Width = 50,
						Height = 50,
						Stretch = Stretch.None
					}
				}
			}
		};
}
