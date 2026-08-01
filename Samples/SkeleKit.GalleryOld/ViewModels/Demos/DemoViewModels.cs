using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkeleKit.Gallery.ViewModels.Demos;

// demos with no state still get a ViewModel: navigation is ViewModel-first
public class ProgressBarDemoViewModel;

public class ActivityIndicatorDemoViewModel;

public class DividerDemoViewModel;

public partial class ImageDemoViewModel(
	INavigator navigator) : ObservableObject
{
	[ObservableProperty]
	public partial double Level { get; set; } = 0.6;

	[RelayCommand]
	Task CopyLink() =>
		navigator.AlertAsync("Copied", "The poster's link is on the clipboard.");

	[RelayCommand]
	Task Save() =>
		navigator.AlertAsync("Saved", "The poster went to your library.");

	[RelayCommand]
	Task Remove() =>
		navigator.AlertAsync("Removed", "The poster is gone.");
}

public class StylingDemoViewModel;

public partial class ChromeDemoViewModel(
	INavigator navigator) : ObservableObject
{
	[ObservableProperty]
	public partial bool GuardLeave { get; set; } = false;

	[ObservableProperty]
	public partial string SearchStatus { get; set; } = "Nothing yet";

	[ObservableProperty]
	public partial string SearchText { get; set; } = "";

	[ObservableProperty]
	public partial int SearchScopeIndex { get; set; }

	partial void OnSearchTextChanged(
		string value) =>
		SearchStatus = $"Typing: {value}";

	partial void OnSearchScopeIndexChanged(
		int value) =>
		SearchStatus = $"Scope {value} selected";

	public void CancelSearch()
	{
		SearchText = "";
		SearchStatus = "Search cancelled";
	}

	public async Task<bool> ConfirmLeaveAsync() =>
		!GuardLeave || await navigator.ConfirmAsync("Leave this page?", "The guard switch is on.", "Leave", "Stay", destructive: true);

	[RelayCommand]
	Task Present(
		string style) =>
		navigator.PresentAsync<ChromeDemoViewModel>(style switch
		{
			"full" => ModalStyle.FullScreen,
			"form" => ModalStyle.FormSheet,
			"medium" => ModalStyle.Sheet(Detent.Medium),
			"resizable" => ModalStyle.Sheet(Detent.Medium, Detent.Large),
			_ => ModalStyle.Sheet()
		});

	[RelayCommand]
	Task PresentPopover(
		ModalStyle style) =>
		navigator.PresentAsync<ChromeDemoViewModel>(style);

	[RelayCommand]
	Task Dismiss() =>
		navigator.DismissAsync();
}

public class AnimationDemoViewModel;

public partial class SearchTabDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Status { get; set; } = "Nothing searched yet";

	[ObservableProperty]
	public partial string Query { get; set; } = "";

	partial void OnQueryChanged(
		string value) =>
		Status = $"Searching: {value}";

	public void CancelSearch()
	{
		Query = "";
		Status = "Cancelled";
	}
}

public partial class PlayerBarViewModel : ObservableObject
{
	[ObservableProperty]
	public partial bool Visible { get; set; } = false;

	[RelayCommand]
	void Play() =>
		Haptics.Impact();
}

public partial class AccessoryDemoViewModel(
	PlayerBarViewModel player) : ObservableObject
{
	[ObservableProperty]
	public partial bool ShowsAccessory { get; set; } = true;

	partial void OnShowsAccessoryChanged(
		bool value) =>
		player.Visible = value;

	// the accessory only accompanies this page: the rest of the gallery stays clean
	public void Entered() =>
		player.Visible = ShowsAccessory;

	public void Left() =>
		player.Visible = false;
}

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

public partial class TintDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial Color Accent { get; set; } = Colors.Indigo;
}

public partial class PageControlDemoViewModel : ObservableObject
{
	const int Pages = 5;

	[ObservableProperty]
	public partial int Page { get; set; }

	[RelayCommand]
	void Previous() =>
		Page = Math.Max(0, Page - 1);

	[RelayCommand]
	void Next() =>
		Page = Math.Min(Pages - 1, Page + 1);
}

public partial class SliderDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial double Fraction { get; set; } = 0.5;

	[ObservableProperty]
	public partial double Percent { get; set; } = 50;

	[ObservableProperty]
	public partial double Settled { get; set; } = 0.5;
}

public partial class StepperDemoViewModel : ObservableObject
{
	[ObservableProperty]
	public partial double Count { get; set; } = 1;
}

public partial class PickerDemoViewModel : ObservableObject
{
	public ObservableCollection<string> Options { get; } =
		["Action", "Comedy", "Drama", "Science Fiction"];

	[ObservableProperty]
	public partial string? Genre { get; set; }

	public string Selection =>
		Genre ?? "Nothing selected";

	partial void OnGenreChanged(
		string? value) =>
		OnPropertyChanged(nameof(Selection));
}
