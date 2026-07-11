using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="TextField"/> with a placeholder, the email keyboard, and
/// <see cref="SecureField"/>.
/// </summary>
public static class TextFieldPage
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
					new Label { Text = "Placeholder", FontSize = 13, TextColor = Secondary },
					new TextField { Placeholder = "Enter some text" },

					new Label { Text = "Email keyboard", FontSize = 13, TextColor = Secondary },
					new TextField { Placeholder = "you@example.com", Keyboard = KeyboardType.Email },

					new Label { Text = "Secure entry", FontSize = 13, TextColor = Secondary },
					new SecureField { Placeholder = "Password" }
				}
			}
		};
}
