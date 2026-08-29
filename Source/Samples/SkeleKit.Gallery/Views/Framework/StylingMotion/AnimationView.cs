using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.StylingMotion;

[Page]
internal sealed class AnimationView : ShowcaseView<AnimationViewModel>
{
	public AnimationView(
		AnimationViewModel viewModel) : base(viewModel, "Animation", Colors.Cyan)
	{
		AddTransitionShowcase(viewModel);
		AddAnimatorShowcase(viewModel);
	}


	void AddTransitionShowcase(
		AnimationViewModel viewModel)
	{
		Grid artwork = Artwork();

		StackPanel details = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 110,
			Spacing = 3,
			Translation = viewModel.IsExpanded ? new(44, 0) : new(24, 0),
			Opacity = viewModel.IsExpanded ? 1 : 0,

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
		artwork.HorizontalAlignment = HorizontalAlignment.Start;
		artwork.Margin = viewModel.IsExpanded ? new(48, 0, 0, 0) : new(10, 0, 0, 0);
		artwork.Scale = viewModel.IsExpanded ? 1 : 0.82;

		Border card = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = viewModel.IsExpanded ? 280 : 84,
			Height = viewModel.IsExpanded ? 128 : 84,
			Background = Colors.SecondaryGroupedBackground,
			CornerRadius = viewModel.IsExpanded ? 22 : 20,
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

		Overlay preview = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 300,
			Height = 156,
			Children =
			{
				card
			}
		};

		SegmentedControl timing = new()
		{
			SelectedIndex = Bind(vm => vm.TimingIndex)
				.TwoWay((vm, val) => vm.TimingIndex = val)
		};
		timing.Items.Add("Spring");
		timing.Items.Add("Ease in/out");
		timing.Items.Add("Ease out");

		Button toggle = new()
		{
			Text = Bind(vm => vm.ActionTitle),
			Kind = ButtonStyle.Tinted
		};
		toggle.Command = Command.From(() =>
		{
			viewModel.IsExpanded = !viewModel.IsExpanded;

			View.Animate(
				viewModel.SelectedTiming,
				() =>
				{
					card.Width = viewModel.IsExpanded ? 280 : 84;
					card.Height = viewModel.IsExpanded ? 128 : 84;
					card.CornerRadius = viewModel.IsExpanded ? 22 : 20;
					artwork.Margin = viewModel.IsExpanded ? new(48, 0, 0, 0) : new(10, 0, 0, 0);
					artwork.Scale = viewModel.IsExpanded ? 1 : 0.82;
					details.Translation = viewModel.IsExpanded ? new(44, 0) : new(24, 0);
					details.Opacity = viewModel.IsExpanded ? 1 : 0;
				});
		});

		AddShowcase(
			"Card expansion",
			"Expand centered artwork into a details card with an easing curve or spring.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(preview, 200),
				LabeledControl("Timing", timing),
				SettingRow("Card", toggle)),
			Code(vm => vm.AnimationCode));
	}

	void AddAnimatorShowcase(
		AnimationViewModel viewModel)
	{
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

		AddShowcase(
			"Interactive animator",
			"Drag the framed work, release it to settle, and grab it again while it is moving.",
			ShowcaseBox.Canvas(stage, 200),
			Code(vm => vm.AnimatorCode));
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

	static Border Tile(
		Color color) =>
		new()
		{
			Background = color,
			CornerRadius = 4
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

	static Border PositionMarker(
		double x) =>
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
}
