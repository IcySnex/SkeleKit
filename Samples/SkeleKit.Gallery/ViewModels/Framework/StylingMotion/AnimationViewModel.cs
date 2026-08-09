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
				Width = 220,
				Height = 100,
				Padding = 12,
				Background = Color.Dynamic(
					Color.FromHex(0xF0F6F7),
					Color.FromHex(0x263438)),
				CornerRadius = 20,
				Stroke = Colors.Cyan.WithAlpha(0.45),
				StrokeThickness = 0.75,

				Child = new Grid
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					ColumnSpacing = 12,
					Columns =
					{
						64,
						GridLength.Auto
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
									TextStyle = TextStyle.Headline
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
						card.Width = expanded ? 280 : 220;
						card.Height = expanded ? 128 : 100;
						card.CornerRadius = expanded ? 24 : 20;
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
			0 => "Animation.Spring(0.5, damping: 0.72)",
			1 => "Animation.Ease(0.3, Easing.EaseInOut)",
			_ => "Animation.Ease(0.3, Easing.EaseOut)"
		};
}
