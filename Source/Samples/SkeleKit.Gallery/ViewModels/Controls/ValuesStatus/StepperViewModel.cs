using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class StepperViewModel : ShowcaseViewModel
{
	public StepperViewModel()
	{
		SelectedStep = Steps[1];
	}


	public List<ShowcaseOption<double>> Steps { get; } =
	[
		new("0.5", 0.5),
		new("1", 1),
		new("2", 2),
		new("5", 5)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ValueLabel))]
	double value = 10;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StepperCode))]
	ShowcaseOption<double> selectedStep = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StepperCode))]
	bool controlEnabled = true;

	public string ValueLabel =>
		Number(Value);

	public IReadOnlyList<Span> StepperCode =>
	[
		new(
			$$"""
			new Stepper
			{
				Value = Bind(vm => vm.Value)
					.TwoWay((vm, val) => vm.Value = val),
				Minimum = 0,
				Maximum = 20,
				Step = {{Number(SelectedStep.Value)}},
				IsEnabled = Bind(vm => vm.ControlEnabled)
			};
			""")
	];


	[RelayCommand]
	void ResetValue() =>
		Value = 10;

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
