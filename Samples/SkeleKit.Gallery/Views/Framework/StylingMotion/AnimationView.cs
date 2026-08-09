using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.StylingMotion;

[Page]
internal sealed class AnimationView : ShowcaseView<AnimationViewModel>
{
	static readonly Color CardBackground = Color.Dynamic(
		Color.FromHex(0xF0F6F7),
		Color.FromHex(0x263438));


	public AnimationView(
		AnimationViewModel viewModel) : base(viewModel, "Animation", Colors.Cyan)
	{
		AddTransitionShowcase(viewModel);
	}


	void AddTransitionShowcase(
		AnimationViewModel viewModel)
	{
		Label detail = new()
		{
			Text = "12 works",
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel,
			Opacity = viewModel.IsExpanded ? 1 : 0
		};

		Border card = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = viewModel.IsExpanded ? 280 : 220,
			Height = viewModel.IsExpanded ? 128 : 100,
			Padding = 12,
			Background = CardBackground,
			CornerRadius = viewModel.IsExpanded ? 24 : 20,
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
					card.Width = viewModel.IsExpanded ? 280 : 220;
					card.Height = viewModel.IsExpanded ? 128 : 100;
					card.CornerRadius = viewModel.IsExpanded ? 24 : 20;
					detail.Opacity = viewModel.IsExpanded ? 1 : 0;
				});
		});

		AddShowcase(
			"Layout transition",
			"Animate size, corner radius and content opacity with an easing curve or spring.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(preview, 200),
				LabeledControl("Timing", timing),
				SettingRow("Card", toggle)),
			ShowcaseBox.Code(Bind(model => model.AnimationCode)));
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

	static Border Tile(
		Color color) =>
		new()
		{
			Background = color,
			CornerRadius = 4
		};
}
