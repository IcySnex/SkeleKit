using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Switch"/> in on and off states, and the <c>Toggled</c> callback.
/// </summary>
public static class SwitchPage
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
					Demo.Caption("Off"),
					new Switch { IsOn = false },

					Demo.Caption("On"),
					new Switch { IsOn = true },

					Demo.Caption("With callback"),
					new Switch
					{
						IsOn = false,
						Toggled = isOn => Console.WriteLine($"SwitchPage: toggled to {isOn}")
					}
				}
			}
		};
}
