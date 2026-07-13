using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels.Demos;

// demos with no state still get a ViewModel: navigation is ViewModel-first
public class ProgressBarDemoViewModel;

public class ActivityIndicatorDemoViewModel;

public class DividerDemoViewModel;

public class ImageDemoViewModel;

public class StylingDemoViewModel;

public class AnimationDemoViewModel;

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

public partial class SegmentedDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial int Selected { get; set; }
}

public partial class DatePickerDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial DateTime Birthday { get; set; } = new(2000, 1, 1);
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
	public partial string? Genre { get; set; }

	public string Selection =>
		Genre ?? "Nothing selected";

	partial void OnGenreChanged(
		string? value) =>
		OnPropertyChanged(nameof(Selection));
}
