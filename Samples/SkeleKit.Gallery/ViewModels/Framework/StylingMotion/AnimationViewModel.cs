using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed partial class AnimationViewModel : ShowcaseViewModel
{
	static readonly Animation[] Timings =
	[
		Animation.Ease(0.45, Easing.EaseInOut),
		Animation.Ease(0.45, Easing.EaseOut),
		Animation.Spring(0.55, damping: 0.72)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SelectedTiming))]
	[NotifyPropertyChangedFor(nameof(AnimationCode))]
	int timingIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ActionTitle))]
	bool isExpanded;

	internal Animation SelectedTiming =>
		Timings[Math.Clamp(TimingIndex, 0, Timings.Length - 1)];

	public string ActionTitle =>
		IsExpanded ? "Collapse" : "Expand";

	public IReadOnlyList<Span> AnimationCode =>
		[new(
			$$"""
			bool expanded = false;

			Label detail = new()
			{
				Text = "12 works",
				TextStyle = TextStyle.Footnote,
				TextColor = Colors.SecondaryLabel,
				Opacity = 0
			};

			Border card = new()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 210,
				Height = 96,
				Padding = 12,
				Background = Colors.SecondaryGroupedBackground,
				CornerRadius = 18,
				Stroke = Colors.Separator,
				StrokeThickness = 0.5,

				Child = new Grid
				{
					ColumnSpacing = 12,
					Columns =
					{
						64,
						GridLength.Star
					},
					Children =
					{
						Artwork(),
						new StackPanel
						{
							VerticalAlignment = VerticalAlignment.Center,
							Spacing = 3,
							Children =
							{
								new Label
								{
									Text = "Collection",
									TextStyle = TextStyle.Headline,
									MaxLines = 1
								},
								detail
							}
						}.Column(1)
					}
				}
			};

			void Toggle()
			{
				expanded = !expanded;

				View.Animate(
					{{TimingCode()}},
					() =>
					{
						card.Width = expanded ? 280 : 210;
						card.Height = expanded ? 132 : 96;
						card.CornerRadius = expanded ? 28 : 18;
						detail.Opacity = expanded ? 1 : 0;
					});
			}

			static Grid Artwork() =>
				new()
				{
					Width = 64,
					Height = 64,
					Padding = 5,
					ColumnSpacing = 4,
					RowSpacing = 4,
					Background = Colors.SecondaryBackground,
					CornerRadius = 14,
					Columns =
					{
						GridLength.Star,
						GridLength.Star
					},
					Rows =
					{
						GridLength.Star,
						GridLength.Star
					},
					Children =
					{
						Tile(Colors.Cyan.WithAlpha(0.34)).Row(0).Column(0),
						Tile(Colors.Blue.WithAlpha(0.28)).Row(0).Column(1),
						Tile(Colors.Teal.WithAlpha(0.26)).Row(1).Column(0),
						Tile(Colors.Indigo.WithAlpha(0.2)).Row(1).Column(1)
					}
				};

			static Border Tile(Color color) =>
				new()
				{
					Background = color,
					CornerRadius = 4
				};
			""")];


	string TimingCode() =>
		Math.Clamp(TimingIndex, 0, Timings.Length - 1) switch
		{
			0 => "Animation.Ease(0.45, Easing.EaseInOut)",
			1 => "Animation.Ease(0.45, Easing.EaseOut)",
			_ => "Animation.Spring(0.55, damping: 0.72)"
		};
}
