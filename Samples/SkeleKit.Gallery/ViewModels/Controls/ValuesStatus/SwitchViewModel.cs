using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class SwitchViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	bool isOn = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SwitchCode))]
	bool controlEnabled = true;

	public IReadOnlyList<Span> SwitchCode =>
	[
		new(
			$$"""
			new Switch
			{
				IsOn = Bind(
					model => model.IsOn,
					(model, value) => model.IsOn = value),
				IsEnabled = {{Boolean(ControlEnabled)}}
			};
			""")
	];


	[RelayCommand]
	void ToggleFromViewModel() =>
		IsOn = !IsOn;

	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
