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
		artwork.Translation = viewModel.IsExpanded ? new(-60, 0) : Point.Zero;

		Border card = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = viewModel.IsExpanded ? 280 : 112,
			Height = viewModel.IsExpanded ? 128 : 112,
			Background = Colors.SecondaryGroupedBackground,
			CornerRadius = viewModel.IsExpanded ? 22 : 26,
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
			SelectedIndex = Bind(
				model => model.TimingIndex,
				static (model, value) => model.TimingIndex = value)
		};
		timing.Items.Add("Spring");
		timing.Items.Add("Ease in/out");
		timing.Items.Add("Ease out");

		Button toggle = new()
		{
			Text = Bind(model => model.ActionTitle),
			Kind = ButtonStyle.Tinted
		};
		toggle.Command = Command.From(() =>
		{
			viewModel.IsExpanded = !viewModel.IsExpanded;

			View.Animate(
				viewModel.SelectedTiming,
				() =>
				{
					card.Width = viewModel.IsExpanded ? 280 : 112;
					card.Height = viewModel.IsExpanded ? 128 : 112;
					card.CornerRadius = viewModel.IsExpanded ? 22 : 26;
					artwork.Translation = viewModel.IsExpanded ? new(-60, 0) : Point.Zero;
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
			ShowcaseBox.Code(Bind(model => model.AnimationCode)));
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

	static Border Tile(
		Color color) =>
		new()
		{
			Background = color,
			CornerRadius = 4
		};
}
