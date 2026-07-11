using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="ActivityIndicator"/> in medium and large sizes, animating and stopped, with and without custom color.
/// </summary>
public static class ActivityIndicatorDemo
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
					Theme.Caption("Medium, animating"),
					new ActivityIndicator { IsAnimating = true, IsLarge = false },

					Theme.Caption("Medium, stopped"),
					new ActivityIndicator { IsAnimating = false, IsLarge = false },

					Theme.Caption("Large, animating"),
					new ActivityIndicator { IsAnimating = true, IsLarge = true },

					Theme.Caption("Large, stopped"),
					new ActivityIndicator { IsAnimating = false, IsLarge = true },

					Theme.Caption("Custom color"),
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
