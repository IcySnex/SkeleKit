using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Layout;

[Page]
internal sealed class BorderView : ShowcaseView<BorderViewModel>
{
	public BorderView(
		BorderViewModel viewModel) : base(viewModel, "Border", Colors.Blue)
	{
		AddFrameShowcase(viewModel);
	}


	void AddFrameShowcase(
		BorderViewModel viewModel)
	{
		Border frame = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 280,
			Height = 130,
			Stroke = Colors.Blue,
			StrokeThickness = viewModel.StrokeThickness,
			Background = Colors.Blue.WithAlpha(0.16),
			CornerRadius = viewModel.CornerRadius,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = "Border",
				TextStyle = TextStyle.Title2,
				FontWeight = FontWeight.Bold,
				TextColor = Colors.Blue
			}
		};

		Slider cornerRadius = new()
		{
			Minimum = 0,
			Maximum = 36,
			Step = 1,
			Value = Bind(
				model => model.CornerRadius,
				static (model, value) => model.CornerRadius = value),
			ValueChanged = value => frame.CornerRadius = value
		};

		Slider stroke = new()
		{
			Minimum = 0,
			Maximum = 6,
			Step = 0.5,
			Value = Bind(
				model => model.StrokeThickness,
				static (model, value) => model.StrokeThickness = value),
			ValueChanged = value => frame.StrokeThickness = value
		};

		AddShowcase(
			"Corner radius & stroke",
			"Adjust the outline around one text child.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(frame, 200),
				LabeledSlider("Corner radius", Bind(model => model.CornerRadiusLabel), cornerRadius),
				LabeledSlider("Stroke width", Bind(model => model.StrokeLabel), stroke)),
			Code(model => model.FrameCode));
	}
}
