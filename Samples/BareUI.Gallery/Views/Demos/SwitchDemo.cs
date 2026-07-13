using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Switch"/> bound two-way to the ViewModel.
/// </summary>
public class SwitchDemo : ContentView<SwitchDemoViewModel>
{
	public SwitchDemo(
		SwitchDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Switch";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Two-way" },
					new Switch { IsOn = Bind(vm => vm.IsOn, (vm, value) => vm.IsOn = value) },

					new Label { Style = Styles.Caption, Text = "Mirrors the switch" },
					new Label { Text = Bind(vm => vm.IsOn, on => on ? "On" : "Off"), Bold = true },

					new Label { Style = Styles.Caption, Text = "Disabled" },
					new Switch { IsOn = true, IsEnabled = false },

					new Label { Style = Styles.Caption, Text = "Tinted" },
					new Switch { IsOn = true, OnColor = Colors.Indigo }
				}
			}
		};
	}
}
