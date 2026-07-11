using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Stepper"/> with different ranges and steps, and the <c>ValueChanged</c> callback.
/// </summary>
public class StepperDemo : StaticView
{
	public StepperDemo()
	{
		Title = "Stepper";

		Content =
			new ScrollView
			{
				Content = new VStack
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						Theme.Caption("Default (0–9)"),
						new Stepper { Minimum = 0, Maximum = 9, Value = 5, Step = 1 },

						Theme.Caption("0–100 with step 10"),
						new Stepper { Minimum = 0, Maximum = 100, Value = 50, Step = 10 },

						Theme.Caption("0.0–1.0 with step 0.1"),
						new Stepper { Minimum = 0, Maximum = 1, Value = 0.5, Step = 0.1 },

						Theme.Caption("With callback"),
						new Stepper
						{
							Minimum = 0,
							Maximum = 10,
							Value = 5,
							Step = 1,
							ValueChanged = value => Console.WriteLine($"StepperDemo: value changed to {value}")
						}
					}
				}
			};
	}
}
