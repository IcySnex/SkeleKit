using System.Windows.Input;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Button"/> styles, an icon, and a bound command.
/// </summary>
public class ButtonDemo : ContentView<ButtonDemoViewModel>
{
	public ButtonDemo()
	{
		Title = "Button";

		Content = new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Kinds" },
					new Button { Text = "Plain", Kind = ButtonStyle.Plain },
					new Button { Text = "Gray", Kind = ButtonStyle.Gray },
					new Button { Text = "Tinted", Kind = ButtonStyle.Tinted },
					new Button { Text = "Filled", Kind = ButtonStyle.Filled },
					new Button { Text = "FilledCapsule", Kind = ButtonStyle.FilledCapsule },

					new Label { Style = Styles.Caption, Text = "With an SF Symbol" },
					new Button { Text = "Play", Icon = "play.fill", Kind = ButtonStyle.Filled },

					new Label { Style = Styles.Caption, Text = "Bound command" },
					new Button
					{
						Text = "Tap me",
						Kind = ButtonStyle.Filled,
						Command = Bind<ICommand?>(vm => vm.TapCommand)
					},
					new Label { Text = Bind(vm => vm.Status), TextColor = Palette.Secondary }
				}
			}
		};
	}
}
