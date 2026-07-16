using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="TextEditor"/> bound two-way inside a bordered box.
/// </summary>
[Page]
public class TextEditorDemo : ContentView<TextEditorDemoViewModel>
{
	static readonly Color Separator = Color.FromHex(0xC7C7CC);

	public TextEditorDemo(
		TextEditorDemoViewModel viewModel) : base(viewModel)
	{
		Title = "TextEditor";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Two-way, grows with its content" },
					Boxed(new TextEditor
					{
						FontSize = 16,
						Text = Bind(vm => vm.Text, (vm, value) => vm.Text = value ?? "")
					}),

					new Label { Style = Styles.Caption, Text = "Character count" },
					new Label { Text = Bind(vm => vm.Text, text => $"{text.Length} characters"), TextColor = Palette.Secondary }
				}
			}
		};
	}

	// box it so measured bounds are visible
	static Border Boxed(
		TextEditor editor) =>
		new()
		{
			Stroke = Separator,
			StrokeThickness = 1,
			CornerRadius = 8,
			Padding = new Thickness(4),
			Child = editor
		};
}
