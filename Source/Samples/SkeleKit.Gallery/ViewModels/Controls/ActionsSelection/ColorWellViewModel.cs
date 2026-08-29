using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;

internal sealed partial class ColorWellViewModel : ShowcaseViewModel
{
	static readonly Color DefaultColor = Color.FromHex(0xaf52de);


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ColorSummary))]
	Color selectedColor = DefaultColor;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	bool showsTitle = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectionCode))]
	bool supportsAlpha = true;

	public string ColorSummary =>
		$"#{Channel(SelectedColor.Red):X2}{Channel(SelectedColor.Green):X2}{Channel(SelectedColor.Blue):X2} · {SelectedColor.Alpha:P0} opacity";

	public IReadOnlyList<Span> SelectionCode =>
	[
		new(
			$$"""
			new ColorWell
			{
				Selected = Bind(vm => vm.SelectedColor)
					.TwoWay((vm, val) => vm.SelectedColor = val),
				Title = {{(ShowsTitle ? "\"Gallery accent\"" : "null")}},
				SupportsAlpha = {{(SupportsAlpha ? "true" : "false")}}
			};
			""")
	];


	[RelayCommand]
	void ResetColor() =>
		SelectedColor = DefaultColor;


	static byte Channel(
		double value) =>
		(byte)Math.Round(Math.Clamp(value, 0, 1) * byte.MaxValue);
}
