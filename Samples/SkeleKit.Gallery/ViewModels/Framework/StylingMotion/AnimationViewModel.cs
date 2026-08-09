using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.StylingMotion;

internal sealed partial class AnimationViewModel : ShowcaseViewModel
{
	static readonly Animation[] Timings =
	[
		Animation.Spring(0.5, damping: 0.72),
		Animation.Ease(0.3, Easing.EaseInOut),
		Animation.Ease(0.3, Easing.EaseOut)
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

			Grid artwork = Artwork();

			StackPanel details = new()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 110,
				Spacing = 3,
				Translation = new(24, 0),
				Opacity = 0,
				Children =
				{
					new Label
					{
						Text = "Collection",
						TextStyle = TextStyle.Headline,
						FontWeight = FontWeight.Semibold
					},
					new Label
					{
						Text = "12 works",
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel
					}
				}
			};

			Border card = new()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 84,
				Height = 84,
				Background = Colors.SecondaryGroupedBackground,
				CornerRadius = 20,
				Stroke = Colors.Separator,
				StrokeThickness = 0.5
			};
			artwork.Scale = 0.82;

			Overlay preview = new()
			{
				Width = 300,
				Height = 156,
				Children =
				{
					card,
					artwork,
					details
				}
			};

			void Toggle()
			{
				expanded = !expanded;

				View.Animate(
					{{TimingCode()}},
					() =>
					{
						card.Width = expanded ? 280 : 84;
						card.Height = expanded ? 128 : 84;
						card.CornerRadius = expanded ? 22 : 20;
						artwork.Translation = expanded ? new(-60, 0) : Point.Zero;
						artwork.Scale = expanded ? 1 : 0.82;
						details.Translation = expanded ? new(44, 0) : new(24, 0);
						details.Opacity = expanded ? 1 : 0;
					});
			}

			static Grid Artwork() =>
				new()
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
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
			0 => "Animation.Spring(0.5, damping: 0.72)",
			1 => "Animation.Ease(0.3, Easing.EaseInOut)",
			_ => "Animation.Ease(0.3, Easing.EaseOut)"
		};
}
