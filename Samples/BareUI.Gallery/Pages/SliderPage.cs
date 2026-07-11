using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Slider"/> with different ranges and the <c>ValueChanged</c> callback.
/// </summary>
public static class SliderPage
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
					Demo.Caption("0–1 range"),
					new Slider { Minimum = 0, Maximum = 1, Value = 0.5 },

					Demo.Caption("0–100 range"),
					new Slider { Minimum = 0, Maximum = 100, Value = 50 },

					Demo.Caption("1–10 range"),
					new Slider { Minimum = 1, Maximum = 10, Value = 5 },

					Demo.Caption("With callback"),
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
