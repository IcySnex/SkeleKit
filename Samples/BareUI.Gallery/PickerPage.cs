using BareUI;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates <see cref="Picker"/> with sample items, a placeholder, different selections, and the <c>SelectionChanged</c> callback.
/// </summary>
public static class PickerPage
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
					new Label { Text = "With placeholder", FontSize = 13, TextColor = Secondary },
					new Picker
					{
						Items = ["Red", "Green", "Blue"],
						SelectedIndex = -1,
						Placeholder = "Select a color"
					},

					new Label { Text = "Pre-selected", FontSize = 13, TextColor = Secondary },
					new Picker
					{
						Items = ["Small", "Medium", "Large"],
						SelectedIndex = 1
					},

					new Label { Text = "Fruits", FontSize = 13, TextColor = Secondary },
					new Picker
					{
						Items = ["Apple", "Banana", "Cherry", "Date"],
						SelectedIndex = 0,
						Placeholder = "Choose a fruit"
					},

					new Label { Text = "With callback", FontSize = 13, TextColor = Secondary },
					new Picker
					{
						Items = ["Option A", "Option B", "Option C"],
						SelectedIndex = -1,
						Placeholder = "Select an option",
						SelectionChanged = index => Console.WriteLine($"PickerPage: selected index {index}")
					}
				}
			}
		};
}
