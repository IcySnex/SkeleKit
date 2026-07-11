using System.Windows.Input;

namespace BareUI.Gallery;

/// <summary>
/// Lists every control demo; tapping one pushes it. Navigation goes through <see cref="INavigator"/>,
/// so no UIKit is involved.
/// </summary>
public class MenuViewModel(
	INavigator navigator)
{
	public IReadOnlyList<DemoViewModel> Demos { get; } =
	[
		new("MovieInfo", MovieInfoPage.Build),
		new("Button", ButtonPage.Build),
		new("TextField", TextFieldPage.Build),
		new("TextEditor", TextEditorPage.Build),
		new("Switch", SwitchPage.Build),
		new("Slider", SliderPage.Build),
		new("Stepper", StepperPage.Build),
		new("ProgressBar", ProgressBarPage.Build),
		new("ActivityIndicator", ActivityIndicatorPage.Build),
		new("Divider", DividerPage.Build),
		new("Picker", PickerPage.Build),
		new("Image", ImagePage.Build),
		new("NativeView", NativeViewPage.Build)
	];

	public ICommand OpenCommand { get; } =
		new OpenDemoCommand(navigator);
}

class OpenDemoCommand(
	INavigator navigator) : ICommand
{
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute(
		object? parameter) =>
		parameter is DemoViewModel;

	public async void Execute(
		object? parameter)
	{
		if (parameter is DemoViewModel demo)
			await navigator.PushAsync(demo);
	}
}

public class MenuPage : ContentView<MenuViewModel>
{
	protected override View Build()
	{
		Title = "BareUI Gallery";

		VStack list = new()
		{
			Spacing = 12,
			Margin = new Thickness(16)
		};

		foreach (DemoViewModel demo in ViewModel!.Demos)
			list.Children.Add(new Button
			{
				Text = demo.Title,
				Style = ButtonStyle.Gray,
				Command = ViewModel.OpenCommand,
				CommandParameter = demo
			});

		return new ScrollView { Content = list };
	}
}
