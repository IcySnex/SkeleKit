using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Switch"/> in on and off states, and the <c>Toggled</c> callback.
/// </summary>
public class SwitchDemo : StaticView
{
	public SwitchDemo()
	{
		Title = "Switch";

		Content =
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
}
