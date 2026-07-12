using System.Windows.Input;
using BareUI.Gallery.ViewModels;

namespace BareUI.Gallery.Views;

/// <summary>
/// One-way, two-way, converted and command bindings against a CommunityToolkit.Mvvm ViewModel.
/// </summary>
public class BindingView : ContentView<BindingViewModel>
{
	public BindingView()
	{
		Title = "Bindings";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					new Label { Style = Styles.Caption, Text = "Two-way: type here" },
					new TextField
					{
						Placeholder = "Name",
						Text = Bind(vm => vm.Name, (vm, value) => vm.Name = value ?? "")
					},

					new Label { Style = Styles.Caption, Text = "One-way: mirrors the field above" },
					new Label { Text = Bind(vm => vm.Name), Bold = true },

					new Label { Style = Styles.Caption, Text = "Converter: length as text" },
					new Label { Text = Bind(vm => vm.Name, name => $"{name.Length} characters") },

					new Label { Style = Styles.Caption, Text = "Two-way switch, one-way label" },
					new Switch { IsOn = Bind(vm => vm.IsSubscribed, (vm, value) => vm.IsSubscribed = value) },
					new Label { Text = Bind(vm => vm.IsSubscribed, on => on ? "Subscribed" : "Not subscribed") },

					new Label { Style = Styles.Caption, Text = "Two-way slider, converted label" },
					new Slider
					{
						Minimum = 0,
						Maximum = 100,
						Value = Bind(vm => vm.Volume, (vm, value) => vm.Volume = value)
					},
					new Label { Text = Bind(vm => vm.Volume, volume => $"Volume {volume:F0}") },

					new Label { Style = Styles.Caption, Text = "Command: disabled while the name is empty" },
					new Button
					{
						Text = "Clear name",
						Kind = ButtonStyle.Filled,
						Command = Bind<ICommand?>(vm => vm.ClearNameCommand)
					}
				}
			}
		};
	}
}
