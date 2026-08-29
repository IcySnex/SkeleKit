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
			"""
			new Switch
			{
				IsOn = Bind(vm => vm.IsOn)
					.TwoWay((vm, val) => vm.IsOn = val),
				IsEnabled = Bind(vm => vm.ControlEnabled)
			};
			""")
	];


	[RelayCommand]
	void ToggleFromViewModel() =>
		IsOn = !IsOn;
}
