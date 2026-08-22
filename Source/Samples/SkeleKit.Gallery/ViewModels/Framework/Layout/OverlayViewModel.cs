using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Layout;

internal sealed partial class OverlayViewModel : ShowcaseViewModel
{
	static readonly HorizontalAlignment[] HorizontalAlignments =
	[
		HorizontalAlignment.Start,
		HorizontalAlignment.Center,
		HorizontalAlignment.End
	];

	static readonly VerticalAlignment[] VerticalAlignments =
	[
		VerticalAlignment.Start,
		VerticalAlignment.Center,
		VerticalAlignment.End
	];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(AlignmentCode))]
	int horizontalIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(AlignmentCode))]
	int verticalIndex = 1;


	internal HorizontalAlignment ChildHorizontalAlignment =>
		HorizontalAlignments[Math.Clamp(HorizontalIndex, 0, HorizontalAlignments.Length - 1)];

	internal VerticalAlignment ChildVerticalAlignment =>
		VerticalAlignments[Math.Clamp(VerticalIndex, 0, VerticalAlignments.Length - 1)];

	public IReadOnlyList<Span> LayersCode { get; } =
		Code(
			"""
			Overlay artwork = new()
			{
				Width = 280,
				Height = 190,
				CornerRadius = 20,
				ClipsToBounds = true,

				Children =
				{
					new Border
					{
						Background = Colors.Blue.WithAlpha(0.1)
					},
					new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = "Artwork"
					},
					new Border
					{
						Height = 58,
						VerticalAlignment = VerticalAlignment.End,
						Background = Colors.Blue.WithAlpha(0.82),
						Child = new Label { Text = "Caption" }
					},
					new Label
					{
						Margin = 12,
						HorizontalAlignment = HorizontalAlignment.End,
						VerticalAlignment = VerticalAlignment.Start,
						Text = "3"
					}
				}
			};
			""");

	public IReadOnlyList<Span> AlignmentCode =>
		Code(
			$$"""
			Overlay overlay = new()
			{
				Width = 280,
				Height = 180,
				Padding = 14,
				Background = Colors.Blue.WithAlpha(0.1),
				CornerRadius = 18,
				Children =
				{
					new Border
					{
						Width = 88,
						Height = 44,
						HorizontalAlignment = HorizontalAlignment.{{ChildHorizontalAlignment}},
						VerticalAlignment = VerticalAlignment.{{ChildVerticalAlignment}},
						Background = Colors.Blue,
						Child = new Label { Text = "Child" }
					}
				}
			};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
