using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Divider"/> with default and custom colors between labeled sections.
/// </summary>
public static class DividerPage
{
	static readonly Color Secondary = Color.FromHex(0x8E8E93);

	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Text = "Section 1", FontSize = 17, Bold = true },
					new Label { Text = "Content here", FontSize = 13, TextColor = Secondary },

					new Divider { },

					new Label { Text = "Section 2", FontSize = 17, Bold = true },
					new Label { Text = "More content", FontSize = 13, TextColor = Secondary },

					new Divider { Color = Color.FromHex(0x8E8E93) },

					new Label { Text = "Section 3", FontSize = 17, Bold = true },
					new Label { Text = "Even more content", FontSize = 13, TextColor = Secondary },

					new Divider { Color = Color.FromHex(0xFF3B30) },

					new Label { Text = "Section 4", FontSize = 17, Bold = true },
					new Label { Text = "Final section", FontSize = 13, TextColor = Secondary }
				}
			}
		};
}
