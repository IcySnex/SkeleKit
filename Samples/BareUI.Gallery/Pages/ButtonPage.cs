using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates every <see cref="ButtonStyle"/>, an icon-only button, an icon+text combo, and the
/// <c>Clicked</c> callback (logged to the console — properties are create-only pre-M3, so there's
/// no live label to update here).
/// </summary>
public static class ButtonPage
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
					new Label { Text = "Styles", FontSize = 20, Bold = true },

					Demo.Caption("Plain"),
					new Button { Text = "Plain", Style = ButtonStyle.Plain },

					Demo.Caption("Gray"),
					new Button { Text = "Gray", Style = ButtonStyle.Gray },

					Demo.Caption("Tinted"),
					new Button { Text = "Tinted", Style = ButtonStyle.Tinted },

					Demo.Caption("Filled"),
					new Button { Text = "Filled", Style = ButtonStyle.Filled },

					Demo.Caption("FilledCapsule"),
					new Button { Text = "Filled Capsule", Style = ButtonStyle.FilledCapsule },

					new Label { Text = "Icon only", FontSize = 20, Bold = true },
					new Button { Icon = "play.fill", Style = ButtonStyle.Filled },

					new Label { Text = "Text + icon", FontSize = 20, Bold = true },
					new Button { Text = "Play", Icon = "play.fill", Style = ButtonStyle.Tinted },

					new Label { Text = "Clicked", FontSize = 20, Bold = true },
					new Button
					{
						Text = "Tap me (check console)",
						Style = ButtonStyle.Filled,
						Clicked = () => Console.WriteLine("ButtonPage: tapped")
					}
				}
			}
		};
}
