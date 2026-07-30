using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class SwitchViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StateSummary))]
	bool isOn = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SwitchCode))]
	bool controlEnabled = true;

	[ObservableProperty]
	string toggleStatus = "Toggled has not fired yet.";

	int toggleCount;

	public string StateSummary =>
		$"Bound value · {IsOn.ToString().ToLowerInvariant()}";

	public IReadOnlyList<Span> SwitchCode =>
	[
		new(
			$$"""
			new Switch
			{
				IsOn = Bind(
					model => model.IsOn,
					(model, value) => model.IsOn = value),
				IsEnabled = {{Boolean(ControlEnabled)}},
				Toggled = viewModel.RecordToggle
			};
			""")
	];


	[RelayCommand]
	void ToggleFromViewModel() =>
		IsOn = !IsOn;

	internal void RecordToggle(
		bool value)
	{
		toggleCount++;
		ToggleStatus = $"Toggled · {value.ToString().ToLowerInvariant()} · {toggleCount}";
	}


	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
