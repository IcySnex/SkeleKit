using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.StylingMotion;

[Page]
internal sealed class ColorsBrushesView : ShowcaseView<ColorsBrushesViewModel>
{
	static readonly LinearGradient TealGradient = new()
	{
		Stops =
		[
			new(Color.FromHex(0x1D5D67), 0),
			new(Color.FromHex(0x497889), 0.55),
			new(Color.FromHex(0x7C8B91), 1)
		],
		Start = new(0, 0),
		End = new(1, 1)
	};

	static readonly LinearGradient SlateGradient = new()
	{
		Stops =
		[
			new(Color.FromHex(0x43566B), 0),
			new(Color.FromHex(0x687989), 0.55),
			new(Color.FromHex(0x8A8580), 1)
		],
		Start = new(0, 0),
		End = new(1, 1)
	};


	public ColorsBrushesView(
		ColorsBrushesViewModel viewModel) : base(viewModel, "Colors & Brushes", Colors.Cyan)
	{
		AddSemanticShowcase(viewModel);
		AddGradientShowcase(viewModel);
	}


	void AddSemanticShowcase(
		ColorsBrushesViewModel viewModel)
	{
		Grid palette = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 308,
			ColumnSpacing = 8,
			RowSpacing = 8,
			Columns =
			{
				GridLength.Star,
				GridLength.Star
			},
			Rows =
			{
				GridLength.Star,
				GridLength.Star,
				GridLength.Star
			},
			Children =
			{
				Swatch("Label", Colors.Label).Row(0).Column(0),
				Swatch("Secondary label", Colors.SecondaryLabel).Row(0).Column(1),
				Swatch("Background", Colors.Background).Row(1).Column(0),
				Swatch("Secondary bg", Colors.SecondaryBackground).Row(1).Column(1),
				Swatch("Separator", Colors.Separator).Row(2).Column(0),
				Swatch(
					"Dynamic",
					Color.Dynamic(
						Color.FromHex(0xDCECEF),
						Color.FromHex(0x24464D)))
					.Row(2)
					.Column(1)
			}
		};

		AddShowcase(
			"Semantic colors",
			"Use system colors for native appearance changes, or provide explicit light and dark values.",
			ShowcaseBox.Canvas(palette, 232),
			Code(model => model.SemanticCode));
	}

	void AddGradientShowcase(
		ColorsBrushesViewModel viewModel)
	{
		bool alternate = false;

		Border surface = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 240,
			Height = 132,
			Background = TealGradient,
			CornerRadius = 20,

			Child = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Spacing = 3,

				Children =
				{
					new Label
					{
						Text = "Linear gradient",
						TextStyle = TextStyle.Title3,
						FontWeight = FontWeight.Bold,
						TextColor = Colors.White
					},

					new Label
					{
						Text = "Three matching stops",
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.White.WithAlpha(0.78)
					}
				}
			}
		};

		Button transition = new()
		{
			Text = "Transition",
			Kind = ButtonStyle.Tinted,
			Command = Command.From(() =>
			{
				alternate = !alternate;

				View.Animate(
					Animation.Ease(0.8),
					() => surface.Background = alternate
						? SlateGradient
						: TealGradient);
			})
		};

		AddShowcase(
			"Gradient interpolation",
			"Animate between two linear gradients that use the same number of custom-color stops.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(surface, 188),
				SettingRow("Palette", transition)),
			Code(model => model.GradientCode));
	}


	static Border Swatch(
		string title,
		Color color) =>
		new()
		{
			Padding = 10,
			Background = Colors.SecondaryGroupedBackground,
			CornerRadius = 12,
			Stroke = Colors.Separator,
			StrokeThickness = 0.5,

			Child = new Grid
			{
				ColumnSpacing = 8,
				Columns =
				{
					GridLength.Auto,
					GridLength.Star
				},

				Children =
				{
					new Border
					{
						Width = 28,
						Height = 28,
						VerticalAlignment = VerticalAlignment.Center,
						Background = color,
						CornerRadius = 8,
						Stroke = Colors.Separator,
						StrokeThickness = 0.5
					},

					new Label
					{
						VerticalAlignment = VerticalAlignment.Center,
						Text = title,
						TextStyle = TextStyle.Footnote,
						MaxLines = 2
					}.Column(1)
				}
			}
		};
}
