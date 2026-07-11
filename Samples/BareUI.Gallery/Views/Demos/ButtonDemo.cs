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
					Theme.Caption("Styles"),
					new Button { Text = "Plain", Style = ButtonStyle.Plain },
					new Button { Text = "Gray", Style = ButtonStyle.Gray },
					new Button { Text = "Tinted", Style = ButtonStyle.Tinted },
					new Button { Text = "Filled", Style = ButtonStyle.Filled },
					new Button { Text = "FilledCapsule", Style = ButtonStyle.FilledCapsule },

					Theme.Caption("With an SF Symbol"),
					new Button { Text = "Play", Icon = "play.fill", Style = ButtonStyle.Filled },

					Theme.Caption("Bound command"),
					new Button
					{
						Text = "Tap me",
						Style = ButtonStyle.Filled,
						Command = Bind<ICommand?>(vm => vm.TapCommand)
					},
					new Label { Text = Bind(vm => vm.Status), TextColor = Theme.Secondary }
				}
			}
		};
	}
}
