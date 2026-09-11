using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Layout;

internal sealed partial class BorderViewModel : ShowcaseViewModel
{
	public BorderViewModel()
	{
		SelectedSystemCornerRadius = SystemCornerRadii[0];
		SelectedCornerCurve = CornerCurves[0];
	}


	public List<ShowcaseOption<double?>> SystemCornerRadii { get; } =
	[
		new("None", null),
		new("GroupedList", SystemCornerRadius.GroupedList)
	];

	public List<ShowcaseOption<CornerCurve>> CornerCurves { get; } =
	[
		new("Circular", CornerCurve.Circular),
		new("Continuous", CornerCurve.Continuous)
	];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(UsesCustomCornerRadius))]
	[NotifyPropertyChangedFor(nameof(EffectiveCornerRadius))]
	[NotifyPropertyChangedFor(nameof(FrameCode))]
	ShowcaseOption<double?> selectedSystemCornerRadius = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FrameCode))]
	ShowcaseOption<CornerCurve> selectedCornerCurve = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(CornerRadiusLabel))]
	[NotifyPropertyChangedFor(nameof(EffectiveCornerRadius))]
	[NotifyPropertyChangedFor(nameof(FrameCode))]
	double cornerRadius = 20;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StrokeLabel))]
	[NotifyPropertyChangedFor(nameof(FrameCode))]
	double strokeThickness = 2;

	public bool UsesCustomCornerRadius =>
		SelectedSystemCornerRadius.Value is null;

	public double EffectiveCornerRadius =>
		SelectedSystemCornerRadius.Value ?? CornerRadius;

	public string CornerRadiusLabel =>
		$"{Number(CornerRadius)} pt";

	public string StrokeLabel =>
		$"{Number(StrokeThickness)} pt";

	public IReadOnlyList<Span> FrameCode =>
		Code(
			$$"""
			Border frame = new()
			{
				Width = 280,
				Height = 130,
				Stroke = Colors.Blue,
				StrokeThickness = {{Number(StrokeThickness)}},
				Background = Colors.Blue.WithAlpha(0.16),
				CornerRadius = {{CornerRadiusCode}},
				CornerCurve = CornerCurve.{{SelectedCornerCurve.Value}},

				Child = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "Border",
					TextStyle = TextStyle.Title2,
					FontWeight = FontWeight.Bold,
					TextColor = Colors.Blue
				}
			};
			""");

	string CornerRadiusCode =>
		SelectedSystemCornerRadius.Value is not null
			? "SystemCornerRadius.GroupedList"
			: Number(CornerRadius);


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
