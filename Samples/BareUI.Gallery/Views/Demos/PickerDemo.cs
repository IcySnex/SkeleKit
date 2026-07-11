using BareUI;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Picker"/> with sample items, a placeholder, different selections, and the <c>SelectionChanged</c> callback.
/// </summary>
public static class PickerDemo
{
	public static View Build() =>
		new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Theme.Caption("With placeholder"),
					new Picker
					{
						Items = ["Red", "Green", "Blue"],
						SelectedIndex = -1,
						Placeholder = "Select a color"
					},

					Theme.Caption("Pre-selected"),
					new Picker
					{
						Items = ["Small", "Medium", "Large"],
						SelectedIndex = 1
					},

					Theme.Caption("Fruits"),
					new Picker
					{
						Items = ["Apple", "Banana", "Cherry", "Date"],
						SelectedIndex = 0,
						Placeholder = "Choose a fruit"
					},

					Theme.Caption("With callback"),
					new Picker
					{
						Items = ["Option A", "Option B", "Option C"],
						SelectedIndex = -1,
						Placeholder = "Select an option",
						SelectionChanged = index => Console.WriteLine($"PickerDemo: selected index {index}")
					}
				}
			}
		};
}
