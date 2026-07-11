using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="ActivityIndicator"/> in medium and large sizes, animating and stopped, with and without custom color.
/// </summary>
public static class ActivityIndicatorPage
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
					new Label { Text = "Medium, animating", FontSize = 13, TextColor = Secondary },
					new ActivityIndicator { IsAnimating = true, IsLarge = false },

					new Label { Text = "Medium, stopped", FontSize = 13, TextColor = Secondary },
					new ActivityIndicator { IsAnimating = false, IsLarge = false },

					new Label { Text = "Large, animating", FontSize = 13, TextColor = Secondary },
					new ActivityIndicator { IsAnimating = true, IsLarge = true },

					new Label { Text = "Large, stopped", FontSize = 13, TextColor = Secondary },
					new ActivityIndicator { IsAnimating = false, IsLarge = true },

					new Label { Text = "Custom color", FontSize = 13, TextColor = Secondary },
					new ActivityIndicator
					{
						IsAnimating = true,
						IsLarge = false,
						Color = Color.FromHex(0xFF9500)
					}
				}
			}
		};
}
