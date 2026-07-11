using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BareUI.Gallery;

/// <summary>
/// Demonstrates one-way, two-way, converted and command bindings against a plain INPC ViewModel.
/// </summary>
public class BindingPage : ContentView<BindingViewModel>
{
	protected override View Build()
	{
		BindingViewModel viewModel = ViewModel!;

		RelayCommand clear = new(
			() => viewModel.Name = "",
			() => viewModel.Name.Length > 0);

		viewModel.PropertyChanged += (sender, e) => clear.RaiseCanExecuteChanged();

		return new ScrollView
		{
			Content = new VStack
			{
				Spacing = 20,
				Margin = new Thickness(16),
				Children =
				{
					Demo.Caption("Two-way: type here"),
					new TextField
					{
						Placeholder = "Name",
						Text = Bind(vm => vm.Name, (vm, value) => vm.Name = value ?? "")
					},

					Demo.Caption("One-way: mirrors the field above"),
					new Label { Text = Bind(vm => vm.Name), Bold = true },

					Demo.Caption("Converter: length as text"),
					new Label { Text = Bind(vm => vm.Name, name => $"{name.Length} characters") },

					Demo.Caption("Two-way switch, one-way label"),
					new Switch { IsOn = Bind(vm => vm.IsSubscribed, (vm, value) => vm.IsSubscribed = value) },
					new Label { Text = Bind(vm => vm.IsSubscribed, on => on ? "Subscribed" : "Not subscribed") },

					Demo.Caption("Two-way slider, converted label"),
					new Slider
					{
						Minimum = 0,
						Maximum = 100,
						Value = Bind(vm => vm.Volume, (vm, value) => vm.Volume = value)
					},
					new Label { Text = Bind(vm => vm.Volume, volume => $"Volume {volume:F0}") },

					Demo.Caption("Command: disabled while the name is empty"),
					new Button
					{
						Text = "Clear name",
						Style = ButtonStyle.Filled,
						Command = clear
					}
				}
			}
		};
	}
}

public class BindingViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	string name = "Kevin";
	public string Name
	{
		get => name;
		set
		{
			name = value;
			Raise();
		}
	}

	bool isSubscribed = true;
	public bool IsSubscribed
	{
		get => isSubscribed;
		set
		{
			isSubscribed = value;
			Raise();
		}
	}

	double volume = 42;
	public double Volume
	{
		get => volume;
		set
		{
			volume = value;
			Raise();
		}
	}

	void Raise(
		[CallerMemberName] string? property = null) =>
		PropertyChanged?.Invoke(this, new(property));
}

// stand-in until M4 brings CommunityToolkit.Mvvm into the sample
class RelayCommand(
	Action execute,
	Func<bool> canExecute) : ICommand
{
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute(
		object? parameter) =>
		canExecute();

	public void Execute(
		object? parameter) =>
		execute();

	public void RaiseCanExecuteChanged() =>
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
