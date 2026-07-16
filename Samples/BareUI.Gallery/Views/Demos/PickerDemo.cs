using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Picker"/> with a bound selection.
/// </summary>
[Page]
public class PickerDemo : ContentView<PickerDemoViewModel>
{
	public PickerDemo(
		PickerDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Picker";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Menu-style selection" },
					new Picker<string>
					{
						Placeholder = "Pick a genre",
						ItemsSource = ViewModel.Options,
						SelectedItem = Bind(vm => vm.Genre, (vm, value) => vm.Genre = value),
						HorizontalAlignment = HorizontalAlignment.Start
					},

					new Label { Style = Styles.Caption, Text = "Selection" },
					new Label { Text = Bind(vm => vm.Selection), Bold = true }
				}
			}
		};
	}
}
