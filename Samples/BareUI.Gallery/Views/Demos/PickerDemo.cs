using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="Picker"/> with a bound selection.
/// </summary>
public class PickerDemo : ContentView<PickerDemoViewModel>
{
	readonly Picker<string> picker = new()
	{
		Placeholder = "Pick a genre",
		SelectedItem = Bind(vm => vm.Genre, (vm, value) => vm.Genre = value),
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
					new Label { Style = Styles.Caption, Text = "Menu-style selection" },
					picker,

					new Label { Style = Styles.Caption, Text = "Selection" },
					new Label { Text = Bind(vm => vm.Selection), Bold = true }
				}
			}
		};
	}

	// interface-typed, so a literal needs Bindable.From
	protected override void OnViewModelAttached() =>
		picker.ItemsSource = Bindable.From<IReadOnlyList<string>?>(ViewModel!.Options);
}
