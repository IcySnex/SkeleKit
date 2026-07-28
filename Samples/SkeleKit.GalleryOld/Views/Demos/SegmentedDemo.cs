using SkeleKit.Gallery.ViewModels.Demos;
using SkeleKit.Gallery.Views;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="SegmentedControl"/> bound two-way.
/// </summary>
[Page]
public class SegmentedDemo : ContentView<SegmentedDemoViewModel>
{
	static readonly string[] Ranges = ["Day", "Week", "Month", "Year"];

	public SegmentedDemo(
		SegmentedDemoViewModel viewModel) : base(viewModel)
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
