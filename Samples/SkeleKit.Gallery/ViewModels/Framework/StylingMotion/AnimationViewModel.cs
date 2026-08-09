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
			artwork.HorizontalAlignment = HorizontalAlignment.Start;
			artwork.Margin = new(10, 0, 0, 0);

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
				StrokeThickness = 0.5,

				Child = new Overlay
				{
					Children =
					{
						artwork,
						details
					}
				}
			};
			artwork.Scale = 0.82;

			Overlay preview = new()
			{
				Width = 300,
				Height = 156,
				Children =
				{
					card
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
						artwork.Margin = expanded ? new(48, 0, 0, 0) : new(10, 0, 0, 0);
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
					Padding = 8,
					ColumnSpacing = 4,
					RowSpacing = 4,
					Background = Colors.SecondaryBackground,
					CornerRadius = 8,
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

	public IReadOnlyList<Span> AnimatorCode =>
		[new(
			"""
			Border artwork = FramedArtwork();
			artwork.Translation = new(-88, 0);
			artwork.Scale = 0.86;
			artwork.Opacity = 0.7;

			Overlay stage = new()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 280,
				Height = 120,
				Children =
				{
					new Border
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Width = 232,
						Height = 2,
						Background = Colors.Separator,
						CornerRadius = 1
					},
					PositionMarker(-88),
					PositionMarker(88),
					artwork
				}
			};

			Animator animator = Animator.Create(
				Animation.Spring(0.5, damping: 0.72),
				() =>
				{
					artwork.Translation = new(88, 0);
					artwork.Scale = 1;
					artwork.Opacity = 1;
				});

			// Materialize both endpoints and return to the captured start.
			animator.Fraction = 0;

			const double distance = 176;
			const double maxReleaseVelocity = 4;
			double grabbedAt = 0;
			double panStart = 0;
			artwork.Panned = pan =>
			{
				switch (pan.State)
				{
					case GestureState.Began:
						animator.Pause();
						grabbedAt = animator.Fraction;
						panStart = pan.Translation.X;
						break;

					case GestureState.Changed:
						animator.Fraction = Math.Clamp(
							grabbedAt + (pan.Translation.X - panStart) / distance,
							0,
							1);
						break;

					default:
						double velocity = pan.Velocity.X;
						bool towardEnd = Math.Abs(velocity) > 600
							? velocity > 0
							: animator.Fraction >= 0.5;

						animator.IsReversed = !towardEnd;
						animator.Continue(Math.Clamp(
							velocity / distance,
							-maxReleaseVelocity,
							maxReleaseVelocity));
						break;
				}
			};

			static Border FramedArtwork() =>
				new()
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Width = 70,
					Height = 84,
					Padding = 7,
					Background = Colors.SecondaryGroupedBackground,
					CornerRadius = 12,
					Stroke = Colors.Separator,
					StrokeThickness = 0.5,
					Child = new Border
					{
						Background = Colors.Cyan.WithAlpha(0.18),
						CornerRadius = 7,
						Child = new Label
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Text = "03",
							TextStyle = TextStyle.Title2,
							FontWeight = FontWeight.Semibold,
							TextColor = Colors.Cyan
						}
					}
				};

			static Border PositionMarker(double x) =>
				new()
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Width = 8,
					Height = 8,
					Translation = new(x, 0),
					Background = Colors.Separator,
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
