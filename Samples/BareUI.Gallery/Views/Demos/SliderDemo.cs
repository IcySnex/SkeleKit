using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Slider"/> ranges, bound two-way.
/// </summary>
[Page]
public class SliderDemo : ContentView<SliderDemoViewModel>
{
	public SliderDemo(
		SliderDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Slider";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "0–1 range" },
					new Slider
					{
						Minimum = 0,
						Maximum = 1,
						Value = Bind(vm => vm.Fraction, (vm, value) => vm.Fraction = value)
					},
					new Label { Text = Bind(vm => vm.Fraction, value => $"{value:F2}"), TextColor = Palette.Secondary },

					new Label { Style = Styles.Caption, Text = "0–100 range" },
					new Slider
					{
						Minimum = 0,
						Maximum = 100,
						Value = Bind(vm => vm.Percent, (vm, value) => vm.Percent = value)
					},
					new Label { Text = Bind(vm => vm.Percent, value => $"{value:F0}%"), TextColor = Palette.Secondary },

					new Label { Style = Styles.Caption, Text = "Volume — icons, tinted track, snaps to 10" },
					new Slider
					{
						Minimum = 0,
						Maximum = 100,
						Step = 10,
						MinIcon = "speaker.fill",
						MaxIcon = "speaker.wave.3.fill",
						TrackColor = Colors.Indigo,
						Value = Bind(vm => vm.Percent, (vm, value) => vm.Percent = value)
					},

					new Label { Style = Styles.Caption, Text = "Commits on release, not during the drag" },
					new Slider
					{
						Minimum = 0,
						Maximum = 1,
						Continuous = false,
						Value = Bind(vm => vm.Settled, (vm, value) => vm.Settled = value)
					},
					new Label { Text = Bind(vm => vm.Settled, value => $"{value:F2}"), TextColor = Palette.Secondary }
				}
			}
		};
	}
}
