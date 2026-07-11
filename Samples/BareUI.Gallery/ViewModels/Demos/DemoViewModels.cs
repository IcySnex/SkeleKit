using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels.Demos;

// demos with no state still get a ViewModel: navigation is ViewModel-first
public class ProgressBarDemoViewModel;

public class ActivityIndicatorDemoViewModel;

public class DividerDemoViewModel;

public class ImageDemoViewModel;

public class NativeViewDemoViewModel;

public partial class ButtonDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Status { get; set; } = "Not tapped yet";

	[RelayCommand]
	void Tap() =>
		Status = $"Tapped at {DateTime.Now:HH:mm:ss}";
}

public partial class TextFieldDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Text { get; set; } = "";

	[ObservableProperty]
	public partial string Email { get; set; } = "";

	[ObservableProperty]
	public partial string Password { get; set; } = "";
}

public partial class TextEditorDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Text { get; set; } = "Type here";
}

public partial class SwitchDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial bool IsOn { get; set; } = true;
}

public partial class SliderDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial double Fraction { get; set; } = 0.5;

	[ObservableProperty]
	public partial double Percent { get; set; } = 50;
}

public partial class StepperDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial double Count { get; set; } = 1;
}

public partial class PickerDemoViewModel : ObservableObject
{
	public IReadOnlyList<string> Options { get; } =
		["Action", "Comedy", "Drama", "Science Fiction"];

	[ObservableProperty]
	public partial int SelectedIndex { get; set; } = -1;

	public string Selection =>
		SelectedIndex >= 0 && SelectedIndex < Options.Count
			? Options[SelectedIndex]
			: "Nothing selected";

	partial void OnSelectedIndexChanged(
		int value) =>
		OnPropertyChanged(nameof(Selection));
}
