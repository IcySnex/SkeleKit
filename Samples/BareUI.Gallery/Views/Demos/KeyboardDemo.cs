using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A page with no ScrollView and an input pinned to the bottom: the keyboard must still not cover it.
/// </summary>
public class KeyboardDemo : ContentView<KeyboardDemoViewModel>
{
	public KeyboardDemo(
		KeyboardDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Keyboard";

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

				new TextField
				{
					Placeholder = "Type here",
					ReturnKey = ReturnKeyType.Done,
					Text = Bind(vm => vm.Message, (vm, value) => vm.Message = value ?? "")
				}
			}
		};
	}
}
