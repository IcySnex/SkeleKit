using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Divider"/> with default and custom colors between labeled sections.
/// </summary>
public class DividerDemo : ContentView<DividerDemoViewModel>
{
	public DividerDemo()
	{
		Title = "Divider";

		Content =
			new ScrollView
			{
				Content = new StackPanel
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Style = Styles.Title, Text = "Section 1" },
						new Label { Style = Styles.Caption, Text = "Content here" },

						new Divider { },

						new Label { Style = Styles.Title, Text = "Section 2" },
						new Label { Style = Styles.Caption, Text = "More content" },

						new Divider { Color = Color.FromHex(0x8E8E93) },

						new Label { Style = Styles.Title, Text = "Section 3" },
						new Label { Style = Styles.Caption, Text = "Even more content" },

						new Divider { Color = Color.FromHex(0xFF3B30) },

						new Label { Style = Styles.Title, Text = "Section 4" },
						new Label { Style = Styles.Caption, Text = "Final section" }
					}
				}
			};
	}
}
