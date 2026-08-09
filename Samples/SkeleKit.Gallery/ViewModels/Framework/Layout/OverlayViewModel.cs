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

					new Border
					{
						Width = 168,
						Height = 112,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Background = Colors.Blue.WithAlpha(0.2),
						CornerRadius = 16,
						Child = new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Text = "Artwork",
							TextStyle = TextStyle.Headline,
							FontWeight = FontWeight.Semibold,
							TextColor = Colors.Blue
						}
					},

					new Border
					{
						Height = 58,
						VerticalAlignment = VerticalAlignment.End,
						Background = Colors.Blue.WithAlpha(0.82),
						Child = new Label
						{
							Margin = new Thickness(16, 0),
							VerticalAlignment = VerticalAlignment.Center,
							Text = "Caption",
							TextStyle = TextStyle.Subheadline,
							FontWeight = FontWeight.Semibold,
							TextColor = Colors.White
						}
					},

					new Border
					{
						Width = 42,
						Height = 30,
						Margin = 12,
						HorizontalAlignment = HorizontalAlignment.End,
						VerticalAlignment = VerticalAlignment.Start,
						Background = Colors.Blue,
						CornerRadius = 15,
						Child = new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Text = "3",
							TextStyle = TextStyle.Footnote,
							FontWeight = FontWeight.Bold,
							TextColor = Colors.White
						}
					}
				}
			};
			""");

	public IReadOnlyList<Span> AlignmentCode =>
		Code(
			$$"""
			Border child = new()
			{
				Width = 88,
				Height = 44,
				HorizontalAlignment = HorizontalAlignment.{{ChildHorizontalAlignment}},
				VerticalAlignment = VerticalAlignment.{{ChildVerticalAlignment}},
				Background = Colors.Blue,
				CornerRadius = 12,
				Child = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "Child",
					TextStyle = TextStyle.Subheadline,
					FontWeight = FontWeight.Semibold,
					TextColor = Colors.White
				}
			};

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
						Width = 1,
						HorizontalAlignment = HorizontalAlignment.Center,
						Background = Colors.Blue.WithAlpha(0.22)
					},
					new Border
					{
						Height = 1,
						VerticalAlignment = VerticalAlignment.Center,
						Background = Colors.Blue.WithAlpha(0.22)
					},
					child
				}
			};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
