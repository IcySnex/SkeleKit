using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Slider"/> with different ranges and the <c>ValueChanged</c> callback.
/// </summary>
public static class SliderPage
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
					new Label { Text = "0–1 range", FontSize = 13, TextColor = Secondary },
					new Slider { Minimum = 0, Maximum = 1, Value = 0.5 },

					new Label { Text = "0–100 range", FontSize = 13, TextColor = Secondary },
					new Slider { Minimum = 0, Maximum = 100, Value = 50 },

					new Label { Text = "1–10 range", FontSize = 13, TextColor = Secondary },
					new Slider { Minimum = 1, Maximum = 10, Value = 5 },

					new Label { Text = "With callback", FontSize = 13, TextColor = Secondary },
					new Slider
					{
						Minimum = 0,
						Maximum = 100,
						Value = 25,
						ValueChanged = value => Console.WriteLine($"SliderPage: value changed to {value}")
					}
				}
			}
		};
}
