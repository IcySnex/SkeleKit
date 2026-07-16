using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A page with no ScrollView and an input pinned to the bottom: the keyboard must still not cover it.
/// </summary>
[Page]
public class KeyboardDemo : ContentView<KeyboardDemoViewModel>
{
	public KeyboardDemo(
		KeyboardDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Keyboard";

		TextField field = new()
		{
			Placeholder = "Type here",
			ReturnKey = ReturnKeyType.Done,
			Text = Bind(vm => vm.Message, (vm, value) => vm.Message = value ?? "")
		};

		// a custom keyboard bar: an inset glass capsule, Safari-style
		Overlay capsule = new()
		{
			Margin = new Thickness(16, 8),
			CornerRadius = 23,
			Background = new Material(MaterialKind.Glass),
			Children =
			{
				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Spacing = 8,
					Margin = new Thickness(10, 0),
					HorizontalAlignment = HorizontalAlignment.Start,
					VerticalAlignment = VerticalAlignment.Center,
					Children =
					{
						new Button { Text = "👍", Kind = ButtonStyle.Plain, Command = Command.From(() => ViewModel.Message += "👍") },
						new Button { Text = "🎬", Kind = ButtonStyle.Plain, Command = Command.From(() => ViewModel.Message += "🎬") }
					}
				},

				new Button
				{
					Text = "Done",
					Kind = ButtonStyle.Plain,
					Margin = new Thickness(0, 0, 10, 0),
					HorizontalAlignment = HorizontalAlignment.End,
					VerticalAlignment = VerticalAlignment.Center,
					Command = Command.From(field.Unfocus)
				}
			}
		};

		capsule.Pressed = down => View.Animate(Animation.Spring(0.45, damping: 0.45), () => capsule.Scale = down ? 1.002 : 1.0);

		field.KeyboardAccessory = new Overlay
		{
			Height = 62,
			Children = { capsule }
		};

		Content = new StackPanel
		{
			Margin = new Thickness(16),
			Spacing = 12,
			VerticalAlignment = VerticalAlignment.End,
			Children =
			{
				new Label { Style = Styles.Caption, Text = "No ScrollView on this page. The field below sits at the bottom." },

				new Label
				{
					Text = Bind(vm => vm.Message, message => message.Length > 0 ? message : "Nothing typed yet"),
					VerticalAlignment = VerticalAlignment.Center,
					TextAlignment = TextAlignment.Center,
					Bold = true
				},

				field
			}
		};
	}
}
