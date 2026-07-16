using BareUI;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates the tab accessory: the app-global bar floating above the tabs.
/// </summary>
[Page]
public class AccessoryDemo : ContentView<AccessoryDemoViewModel>
{
	public AccessoryDemo(
		AccessoryDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Tab accessory";

		Content = new StackPanel
		{
			Spacing = 20,
			Margin = new Thickness(16),
			Children =
			{
				new Label { Style = Styles.Caption, Text = "The player bar above the tab bar is the accessory — registered once on the shell, like Music's mini player" },

				new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Spacing = 12,
					Children =
					{
						new Switch { IsOn = Bind(vm => vm.ShowsAccessory, (vm, value) => vm.ShowsAccessory = value) },
						new Label
						{
							VerticalAlignment = VerticalAlignment.Center,
							Text = Bind(vm => vm.ShowsAccessory, shown => shown ? "Accessory shown" : "Accessory hidden")
						}
					}
				},

				new Label { Style = Styles.Caption, Text = "Pages that hide the tab bar (Page chrome) take the accessory with it" }
			}
		};
	}


	protected override void OnAppearing() =>
		ViewModel.Entered();

	protected override void OnDisappearing() =>
		ViewModel.Left();
}
