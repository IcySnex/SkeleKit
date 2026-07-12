using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="SegmentedControl"/> bound two-way.
/// </summary>
public class SegmentedDemo : ContentView<SegmentedDemoViewModel>
{
	static readonly string[] Ranges = ["Day", "Week", "Month", "Year"];

	public SegmentedDemo()
	{
		Title = "SegmentedControl";

		Content = new StackPanel
		{
			Spacing = 20,
			Margin = new Thickness(16),
			Children =
			{
				new Label { Style = Styles.Caption, Text = "Two-way" },
				new SegmentedControl
				{
					Items = { "Day", "Week", "Month", "Year" },
					SelectedIndex = Bind(vm => vm.Selected, (vm, value) => vm.Selected = value)
				},
				new Label
				{
					Text = Bind(vm => vm.Selected, index => $"Showing: {Ranges[Math.Clamp(index, 0, Ranges.Length - 1)]}"),
					TextColor = Palette.Secondary
				}
			}
		};
	}
}
