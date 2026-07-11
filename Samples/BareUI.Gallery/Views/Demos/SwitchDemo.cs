using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Switch"/> in on and off states, and the <c>Toggled</c> callback.
/// </summary>
public static class SwitchDemo
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
					Theme.Caption("Off"),
					new Switch { IsOn = false },

					Theme.Caption("On"),
					new Switch { IsOn = true },

					Theme.Caption("With callback"),
					new Switch
					{
						IsOn = false,
						Toggled = isOn => Console.WriteLine($"SwitchDemo: toggled to {isOn}")
					}
				}
			}
		};
}
