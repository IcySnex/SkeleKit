using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;

internal sealed partial class SliderViewModel : ShowcaseViewModel
{
	public SliderViewModel()
	{
		SelectedStep = Steps[0];
	}


	public List<ShowcaseOption<double>> Steps { get; } =
	[
		new("Continuous", 0),
		new("1", 1),
		new("5", 5),
		new("10", 10)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ValueLabel))]
	double value = 50;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SliderCode))]
	ShowcaseOption<double> selectedStep = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SliderCode))]
	bool continuous = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SliderCode))]
	bool showsIcons = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SliderCode))]
	bool controlEnabled = true;

	[ObservableProperty]
	string changeStatus = "ValueChanged has not fired yet.";

	int changeCount;

	public string ValueLabel => Number(Value);

	public IReadOnlyList<Span> SliderCode =>
	[
		new(
			$$"""
			new Slider
			{
				Value = Bind(
					model => model.Value,
					(model, value) => model.Value = value),
				Minimum = 0,
				Maximum = 100,
				Step = {{Number(SelectedStep.Value)}},
				Continuous = {{Boolean(Continuous)}},
				MinIcon = {{(ShowsIcons ? "\"speaker.fill\"" : "null")}},
				MaxIcon = {{(ShowsIcons ? "\"speaker.wave.3.fill\"" : "null")}},
				IsEnabled = {{Boolean(ControlEnabled)}},
				ValueChanged = viewModel.RecordChange
			};
			""")
	];


	[RelayCommand]
	void ResetValue() =>
		Value = 50;

	internal void RecordChange(
		double value)
	{
		changeCount++;
		ChangeStatus = $"ValueChanged · {Number(value)} · {changeCount}";
	}


	static string Boolean(
		bool value) =>
		value ? "true" : "false";

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
