using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="TextEditor"/> with different font sizes and the <c>TextChanged</c> callback.
/// </summary>
public static class TextEditorPage
{
	static readonly Color Separator = Color.FromHex(0xC7C7CC);

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

	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Demo.Caption("Default"),
					Boxed(new TextEditor { }),

					Demo.Caption("Custom font size"),
					Boxed(new TextEditor { FontSize = 14 }),

					Demo.Caption("With callback"),
					Boxed(new TextEditor
					{
						Text = "Type here",
						FontSize = 16,
						TextChanged = text => Console.WriteLine($"TextEditorPage: text changed to '{text}'")
					})
				}
			}
		};
}
