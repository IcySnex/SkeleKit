using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Stepper"/> bound two-way.
/// </summary>
public class StepperDemo : ContentView<StepperDemoViewModel>
{
	public StepperDemo(
		StepperDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Stepper";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "1–10, step 1" },
					new Stepper
					{
						Minimum = 1,
						Maximum = 10,
						Step = 1,
						Value = Bind(vm => vm.Count, (vm, value) => vm.Count = value),
						HorizontalAlignment = HorizontalAlignment.Start
					},
					new Label { Text = Bind(vm => vm.Count, count => $"Count: {count:F0}"), Bold = true }
				}
			}
		};
	}
}
