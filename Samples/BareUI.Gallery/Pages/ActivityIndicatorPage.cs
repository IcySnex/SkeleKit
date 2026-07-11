using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="ActivityIndicator"/> in medium and large sizes, animating and stopped, with and without custom color.
/// </summary>
public static class ActivityIndicatorPage
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
					Demo.Caption("Medium, animating"),
					new ActivityIndicator { IsAnimating = true, IsLarge = false },

					Demo.Caption("Medium, stopped"),
					new ActivityIndicator { IsAnimating = false, IsLarge = false },

					Demo.Caption("Large, animating"),
					new ActivityIndicator { IsAnimating = true, IsLarge = true },

					Demo.Caption("Large, stopped"),
					new ActivityIndicator { IsAnimating = false, IsLarge = true },

					Demo.Caption("Custom color"),
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
