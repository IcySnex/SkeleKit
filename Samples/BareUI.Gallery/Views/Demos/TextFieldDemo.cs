using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="TextField"/> and <see cref="SecureField"/>, bound two-way.
/// </summary>
public class TextFieldDemo : ContentView<TextFieldDemoViewModel>
{
	public TextFieldDemo(
		TextFieldDemoViewModel viewModel) : base(viewModel)
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

					new Label { Style = Styles.Caption, Text = "SecureField — autofills a saved password" },
					new SecureField
					{
						Placeholder = "Password",
						ContentKind = ContentKind.Password,
						ReturnKey = ReturnKeyType.Done,
						Text = Bind(vm => vm.Password, (vm, value) => vm.Password = value ?? "")
					},

					new Label { Style = Styles.Caption, Text = "One-time code — autofills from Messages" },
					new TextField
					{
						Placeholder = "123456",
						ContentKind = ContentKind.OneTimeCode,
						Keyboard = KeyboardType.Numeric
					},

					new Label { Style = Styles.Caption, Text = "Clear button, no autocorrection, mono" },
					new TextField
					{
						Placeholder = "SKU-0000",
						ClearButton = ClearButton.WhileEditing,
						Autocorrection = false,
						Capitalization = Capitalization.Characters,
						FontDesign = FontDesign.Monospaced,
						RequiresText = true
					},

					new Label { Style = Styles.Caption, Text = "Dark keyboard, whatever the system appearance" },
					new TextField
					{
						Placeholder = "Tap me in light mode",
						KeyboardLook = KeyboardLook.Dark
					}
				}
			}
		};
	}
}
