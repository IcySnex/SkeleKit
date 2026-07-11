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
				Content = new VStack
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Text = "Section 1", FontSize = 17, Bold = true },
						Theme.Caption("Content here"),

						new Divider { },

						new Label { Text = "Section 2", FontSize = 17, Bold = true },
						Theme.Caption("More content"),

						new Divider { Color = Color.FromHex(0x8E8E93) },

						new Label { Text = "Section 3", FontSize = 17, Bold = true },
						Theme.Caption("Even more content"),

						new Divider { Color = Color.FromHex(0xFF3B30) },

						new Label { Text = "Section 4", FontSize = 17, Bold = true },
						Theme.Caption("Final section")
					}
				}
			};
	}
}
