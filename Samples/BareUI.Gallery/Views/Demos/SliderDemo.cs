using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Slider"/> ranges, bound two-way.
/// </summary>
public class SliderDemo : ContentView<SliderDemoViewModel>
{
	public SliderDemo()
	{
		Title = "Slider";

		Content = new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Theme.Caption("0–1 range"),
					new Slider
					{
						Minimum = 0,
						Maximum = 1,
						Value = Bind(vm => vm.Fraction, (vm, value) => vm.Fraction = value)
					},
					new Label { Text = Bind(vm => vm.Fraction, value => $"{value:F2}"), TextColor = Theme.Secondary },

					Theme.Caption("0–100 range"),
					new Slider
					{
						Minimum = 0,
						Maximum = 100,
						Value = Bind(vm => vm.Percent, (vm, value) => vm.Percent = value)
					},
					new Label { Text = Bind(vm => vm.Percent, value => $"{value:F0}%"), TextColor = Theme.Secondary }
				}
			}
		};
	}
}
