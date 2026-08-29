using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;

internal sealed partial class DatePickerViewModel : ShowcaseViewModel
{
	static readonly DatePickerMode[] Modes =
	[
		DatePickerMode.Date,
		DatePickerMode.Time,
		DatePickerMode.DateAndTime
	];

	static readonly DatePickerStyle[] Styles =
	[
		DatePickerStyle.Compact,
		DatePickerStyle.Inline,
		DatePickerStyle.Wheels
	];

	internal static readonly DateTime ExampleDate = new(2026, 8, 12, 14, 30, 0);
	internal static readonly DateTime MinimumDate = new(2026, 8, 10, 9, 0, 0);
	internal static readonly DateTime MaximumDate = new(2026, 8, 14, 18, 0, 0);


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedMode))]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	int selectedModeIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedStyle))]
	[NotifyPropertyChangedFor(nameof(ConfigurationCode))]
	int selectedStyleIndex;

	public DatePickerMode SelectedMode =>
		Modes[Math.Clamp(SelectedModeIndex, 0, Modes.Length - 1)];

	public DatePickerStyle SelectedStyle =>
		Styles[Math.Clamp(SelectedStyleIndex, 0, Styles.Length - 1)];

	public IReadOnlyList<Span> ConfigurationCode =>
		Code(
			$$"""
			new DatePicker
			{
				Date = new DateTime(2026, 8, 12, 14, 30, 0),
				Mode = DatePickerMode.{{SelectedMode}},
				Kind = DatePickerStyle.{{SelectedStyle}}
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(DateSummary))]
	[NotifyPropertyChangedFor(nameof(RangeCode))]
	DateTime selectedDate = ExampleDate;

	[ObservableProperty]
	int rangePositionIndex = 1;

	public string DateSummary =>
		SelectedDate.ToString("ddd, MMM d · HH:mm", CultureInfo.CurrentCulture);

	public IReadOnlyList<Span> RangeCode =>
		Code(
			"""
			new DatePicker
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				Width = 215,
				Date = Bind(vm => vm.SelectedDate)
					.TwoWay((vm, val) => vm.SelectedDate = val),
				Mode = DatePickerMode.DateAndTime,
				Kind = DatePickerStyle.Compact,
				Minimum = new DateTime(2026, 8, 10, 9, 0, 0),
				Maximum = new DateTime(2026, 8, 14, 18, 0, 0)
			};
			""");


	partial void OnRangePositionIndexChanged(
		int value) =>
		SelectedDate = value switch
		{
			0 => MinimumDate,
			2 => MaximumDate,
			_ => ExampleDate
		};


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
