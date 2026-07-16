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
		// the wash sits over the glass and under the content: pressing lights the whole capsule
		Overlay highlight = new()
		{
			Background = Colors.White.WithAlpha(0.12),
			Opacity = 0
		};

		Overlay capsule = new()
		{
			Margin = new Thickness(16, 8),
			CornerRadius = 25,
			Background = new Material(MaterialKind.Glass),
			Children =
			{
				highlight,

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

		// the whole bar swells and lights under any touch; the buttons still fire
		capsule.Pressed = down => View.Animate(0.15, () =>
		{
			capsule.Scale = down ? 1.04 : 1.0;
			highlight.Opacity = down ? 1 : 0;
		});

		field.KeyboardAccessory = new Overlay
		{
			Height = 66,
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
