using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="TextField"/> and <see cref="SecureField"/>, bound two-way.
/// </summary>
public class TextFieldDemo : ContentView<TextFieldDemoViewModel>
{
	public TextFieldDemo()
	{
		Title = "TextField";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Two-way" },
					new TextField
					{
						Placeholder = "Name",
						Text = Bind(vm => vm.Text, (vm, value) => vm.Text = value ?? "")
					},
					new Label { Text = Bind(vm => vm.Text), TextColor = Palette.Secondary },

					new Label { Style = Styles.Caption, Text = "Email keyboard" },
					new TextField
					{
						Placeholder = "you@example.com",
						Keyboard = KeyboardType.Email,
						ReturnKey = ReturnKeyType.Next,
						Text = Bind(vm => vm.Email, (vm, value) => vm.Email = value ?? "")
					},

					new Label { Style = Styles.Caption, Text = "SecureField" },
					new SecureField
					{
						Placeholder = "Password",
						ReturnKey = ReturnKeyType.Done,
						Text = Bind(vm => vm.Password, (vm, value) => vm.Password = value ?? "")
					}
				}
			}
		};
	}
}
