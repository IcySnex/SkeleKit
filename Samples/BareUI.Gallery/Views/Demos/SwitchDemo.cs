using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Switch"/> bound two-way to the ViewModel.
/// </summary>
public class SwitchDemo : ContentView<SwitchDemoViewModel>
{
	public SwitchDemo()
	{
		Title = "Switch";

		Content = new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Theme.Caption("Two-way"),
					new Switch { IsOn = Bind(vm => vm.IsOn, (vm, value) => vm.IsOn = value) },

					Theme.Caption("Mirrors the switch"),
					new Label { Text = Bind(vm => vm.IsOn, on => on ? "On" : "Off"), Bold = true },

					Theme.Caption("Disabled"),
					new Switch { IsOn = true, IsEnabled = false }
				}
			}
		};
	}
}
