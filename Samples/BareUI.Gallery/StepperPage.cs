using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Stepper"/> with different ranges and steps, and the <c>ValueChanged</c> callback.
/// </summary>
public static class StepperPage
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
					new Label { Text = "Default (0–9)", FontSize = 13, TextColor = Secondary },
					new Stepper { Minimum = 0, Maximum = 9, Value = 5, Step = 1 },

					new Label { Text = "0–100 with step 10", FontSize = 13, TextColor = Secondary },
					new Stepper { Minimum = 0, Maximum = 100, Value = 50, Step = 10 },

					new Label { Text = "0.0–1.0 with step 0.1", FontSize = 13, TextColor = Secondary },
					new Stepper { Minimum = 0, Maximum = 1, Value = 0.5, Step = 0.1 },

					new Label { Text = "With callback", FontSize = 13, TextColor = Secondary },
					new Stepper
					{
						Minimum = 0,
						Maximum = 10,
						Value = 5,
						Step = 1,
						ValueChanged = value => Console.WriteLine($"StepperPage: value changed to {value}")
					}
				}
			}
		};
}
