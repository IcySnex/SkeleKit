using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Switch"/> in on and off states, and the <c>Toggled</c> callback.
/// </summary>
public static class SwitchPage
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
					new Label { Text = "Off", FontSize = 13, TextColor = Secondary },
					new Switch { IsOn = false },

					new Label { Text = "On", FontSize = 13, TextColor = Secondary },
					new Switch { IsOn = true },

					new Label { Text = "With callback", FontSize = 13, TextColor = Secondary },
					new Switch
					{
						IsOn = false,
						Toggled = isOn => Console.WriteLine($"SwitchPage: toggled to {isOn}")
					}
				}
			}
		};
}
