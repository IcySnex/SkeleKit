using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class SwitchViewModel : ShowcaseViewModel
{
	public SwitchViewModel()
	{
		SelectedOnColor = OnColors[0];
		SelectedThumbColor = ThumbColors[0];
	}


	public List<ShowcaseOption<Color?>> OnColors { get; } =
	[
		new("Inherited tint", null),
		new("Pink", Colors.Pink),
		new("Indigo", Colors.Indigo),
		new("Purple", Colors.Purple)
	];

	public List<ShowcaseOption<Color?>> ThumbColors { get; } =
	[
		new("System", null),
		new("White", Colors.White),
		new("Pink", Colors.Pink)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateSummary))]
	[NotifyPropertyChangedFor(nameof(BindingCode))]
	bool isOn = true;

	[ObservableProperty]
	string toggleStatus = "Toggled has not fired yet.";

	int toggleCount;

	public string StateSummary =>
		IsOn ? "Notifications are enabled" : "Notifications are disabled";

	public IReadOnlyList<Span> BindingCode =>
		Code(
			"""
			new Switch
			{
				IsOn = Bind(
					model => model.IsOn,
					(model, value) => model.IsOn = value),
				Toggled = viewModel.RecordToggle
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	ShowcaseOption<Color?> selectedOnColor = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	ShowcaseOption<Color?> selectedThumbColor = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	bool previewOn = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	bool controlEnabled = true;

	public IReadOnlyList<Span> ConfigurationCode =>
		Code(
			$$"""
			new Switch
			{
				IsOn = {{Boolean(PreviewOn)}},
				OnColor = {{OnColorCode()}},
				ThumbColor = {{ThumbColorCode()}},
				IsEnabled = {{Boolean(ControlEnabled)}}
			};
			""");


	[RelayCommand]
	void ToggleFromViewModel()
	{
		IsOn = !IsOn;
		ToggleStatus = $"ViewModel set IsOn to {IsOn.ToString().ToLowerInvariant()}.";
	}

	internal void RecordToggle(
		bool value)
	{
		toggleCount++;
		ToggleStatus = $"Toggled · {value.ToString().ToLowerInvariant()} · {toggleCount}";
	}


	string OnColorCode() =>
		OnColors.IndexOf(SelectedOnColor) switch
		{
			1 => "Colors.Pink",
			2 => "Colors.Indigo",
			3 => "Colors.Purple",
			_ => "null"
		};

	string ThumbColorCode() =>
		ThumbColors.IndexOf(SelectedThumbColor) switch
		{
			1 => "Colors.White",
			2 => "Colors.Pink",
			_ => "null"
		};

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
