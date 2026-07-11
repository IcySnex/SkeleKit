using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="TextEditor"/> with different font sizes and the <c>TextChanged</c> callback.
/// </summary>
public static class TextEditorPage
{
	static readonly Color Secondary = Color.FromHex(0x8E8E93);
	static readonly Color Separator = Color.FromHex(0xC7C7CC);

	// Wraps an editor in a visible bordered box so its measured bounds are apparent.
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
					new Label { Text = "Default", FontSize = 13, TextColor = Secondary },
					Boxed(new TextEditor { }),

					new Label { Text = "Custom font size", FontSize = 13, TextColor = Secondary },
					Boxed(new TextEditor { FontSize = 14 }),

					new Label { Text = "With callback", FontSize = 13, TextColor = Secondary },
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
