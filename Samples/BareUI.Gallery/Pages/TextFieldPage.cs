using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="TextField"/> with a placeholder, the email keyboard, and
/// <see cref="SecureField"/>.
/// </summary>
public static class TextFieldPage
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
					Demo.Caption("Placeholder"),
					new TextField { Placeholder = "Enter some text" },

					Demo.Caption("Email keyboard"),
					new TextField { Placeholder = "you@example.com", Keyboard = KeyboardType.Email },

					Demo.Caption("Secure entry"),
					new SecureField { Placeholder = "Password" }
				}
			}
		};
}
