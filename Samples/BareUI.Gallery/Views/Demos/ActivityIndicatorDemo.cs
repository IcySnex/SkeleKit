using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="ActivityIndicator"/> in medium and large sizes, animating and stopped, with and without custom color.
/// </summary>
public class ActivityIndicatorDemo : ContentView<ActivityIndicatorDemoViewModel>
{
	public ActivityIndicatorDemo()
	{
		Title = "ActivityIndicator";

		Content =
			new ScrollView
			{
				Content = new StackPanel
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Style = Styles.Caption, Text = "Medium, animating" },
						new ActivityIndicator { IsAnimating = true, IsLarge = false },

						new Label { Style = Styles.Caption, Text = "Medium, stopped" },
						new ActivityIndicator { IsAnimating = false, IsLarge = false },

						new Label { Style = Styles.Caption, Text = "Large, animating" },
						new ActivityIndicator { IsAnimating = true, IsLarge = true },

						new Label { Style = Styles.Caption, Text = "Large, stopped" },
						new ActivityIndicator { IsAnimating = false, IsLarge = true },

						new Label { Style = Styles.Caption, Text = "Custom color" },
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
}
