using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Layout;

internal sealed partial class BorderViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(CornerRadiusLabel))]
	[NotifyPropertyChangedFor(nameof(FrameCode))]
	double cornerRadius = 20;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(StrokeLabel))]
	[NotifyPropertyChangedFor(nameof(FrameCode))]
	double strokeThickness = 2;

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
				CornerRadius = {{Number(CornerRadius)}},

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


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
