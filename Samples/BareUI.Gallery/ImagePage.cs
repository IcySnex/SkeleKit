using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Image"/> with SF Symbols, URL images, and different stretch modes.
/// </summary>
public static class ImagePage
{
	static readonly Color Secondary = Color.FromHex(0x8E8E93);

	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Text = "SF Symbol", FontSize = 13, TextColor = Secondary },
					new Image
					{
						Source = "star.fill",
						Width = 60,
						Height = 60,
						Stretch = Stretch.Uniform
					},

					new Label { Text = "URL image", FontSize = 13, TextColor = Secondary },
					new Image
					{
						Source = "https://picsum.photos/300/200",
						Width = 300,
						Height = 200,
						Stretch = Stretch.UniformToFill
					},

					new Label { Text = "SF Symbol with fill stretch", FontSize = 13, TextColor = Secondary },
					new Image
					{
						Source = "heart.fill",
						Width = 80,
						Height = 80,
						Stretch = Stretch.Fill
					},

					new Label { Text = "SF Symbol no stretch", FontSize = 13, TextColor = Secondary },
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
