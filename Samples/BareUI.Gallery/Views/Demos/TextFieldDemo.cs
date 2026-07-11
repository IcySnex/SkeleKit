using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="TextField"/> with a placeholder, the email keyboard, and
/// <see cref="SecureField"/>.
/// </summary>
public class TextFieldDemo : StaticView
{
	public TextFieldDemo()
	{
		Title = "TextField";

		Content =
			new ScrollView
			{
				Content = new VStack
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						Theme.Caption("Placeholder"),
						new TextField { Placeholder = "Enter some text" },

						Theme.Caption("Email keyboard"),
						new TextField { Placeholder = "you@example.com", Keyboard = KeyboardType.Email },

						Theme.Caption("Secure entry"),
						new SecureField { Placeholder = "Password" }
					}
				}
			};
	}
}
