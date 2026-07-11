using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Picker"/> with a bound selection.
/// </summary>
public class PickerDemo : ContentView<PickerDemoViewModel>
{
	readonly Picker picker = new()
	{
		Placeholder = "Pick a genre",
		SelectedIndex = Bind(vm => vm.SelectedIndex, (vm, value) => vm.SelectedIndex = value),
		HorizontalAlignment = HorizontalAlignment.Start
	};

	public PickerDemo()
	{
		Title = "Picker";

		Content = new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Theme.Caption("Menu-style selection"),
					picker,

					Theme.Caption("Selection"),
					new Label { Text = Bind(vm => vm.Selection), Bold = true }
				}
			}
		};
	}

	// Items is a plain property: C# forbids implicit conversion from an interface, so it cannot be Bindable
	protected override void OnViewModelAttached() =>
		picker.Items = ViewModel!.Options;
}
