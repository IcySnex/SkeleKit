using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Layout;

[Page]
internal sealed class OverlayView : ShowcaseView<OverlayViewModel>
{
	public OverlayView(
		OverlayViewModel viewModel) : base(viewModel, "Overlay", Colors.Blue)
	{
		AddLayersShowcase(viewModel);
		AddAlignmentShowcase(viewModel);
	}


	void AddLayersShowcase(
		OverlayViewModel viewModel)
	{
		Overlay artwork = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
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

		AddShowcase(
			"Layer order",
			"Every child shares the same space. Children added later are drawn above earlier ones.",
			ShowcaseBox.Canvas(artwork, 250),
			ShowcaseBox.Code(Bind(model => model.LayersCode)));
	}

	void AddAlignmentShowcase(
		OverlayViewModel viewModel)
	{
		Border child = new()
		{
			Width = 88,
			Height = 44,
			HorizontalAlignment = viewModel.ChildHorizontalAlignment,
			VerticalAlignment = viewModel.ChildVerticalAlignment,
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
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
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

		SegmentedControl horizontal = new()
		{
			SelectedIndex = Bind(
				model => model.HorizontalIndex,
				static (model, value) => model.HorizontalIndex = value),
			SelectionChanged = index => child.HorizontalAlignment = index switch
			{
				0 => HorizontalAlignment.Start,
				2 => HorizontalAlignment.End,
				_ => HorizontalAlignment.Center
			}
		};
		horizontal.Items.Add("Start");
		horizontal.Items.Add("Center");
		horizontal.Items.Add("End");

		SegmentedControl vertical = new()
		{
			SelectedIndex = Bind(
				model => model.VerticalIndex,
				static (model, value) => model.VerticalIndex = value),
			SelectionChanged = index => child.VerticalAlignment = index switch
			{
				0 => VerticalAlignment.Start,
				2 => VerticalAlignment.End,
				_ => VerticalAlignment.Center
			}
		};
		vertical.Items.Add("Top");
		vertical.Items.Add("Center");
		vertical.Items.Add("Bottom");

		AddShowcase(
			"Child alignment",
			"Place a smaller child anywhere inside the overlay's shared bounds.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(overlay, 240),
				LabeledControl("Horizontal", horizontal),
				LabeledControl("Vertical", vertical)),
			ShowcaseBox.Code(Bind(model => model.AlignmentCode)));
	}
}
