using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Layout;

[Page]
internal sealed class StackPanelView : ShowcaseView<StackPanelViewModel>
{
	public StackPanelView(
		StackPanelViewModel viewModel) : base(viewModel, "StackPanel", Colors.Blue)
	{
		AddConfigurationShowcase(viewModel);
	}


	void AddConfigurationShowcase(
		StackPanelViewModel viewModel)
	{
		StackPanel stack = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Orientation = viewModel.Orientation,
			Spacing = viewModel.Spacing,

			Children =
			{
				Item("One"),
				Item("Two"),
				Item("Three")
			}
		};

		SegmentedControl orientation = new()
		{
			SelectedIndex = Bind(
				model => model.OrientationIndex,
				static (model, value) => model.OrientationIndex = value),
			SelectionChanged = index => stack.Orientation = index == 1
				? Orientation.Horizontal
				: Orientation.Vertical
		};
		orientation.Items.Add("Vertical");
		orientation.Items.Add("Horizontal");

		Slider spacing = new()
		{
			Minimum = 0,
			Maximum = 24,
			Step = 2,
			Value = Bind(
				model => model.Spacing,
				static (model, value) => model.Spacing = value),
			ValueChanged = value => stack.Spacing = value
		};

		AddShowcase(
			"Orientation & spacing",
			"Switch the stacking direction and adjust the gap between adjacent children.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(stack, 260),
				LabeledControl("Orientation", orientation),
				LabeledSlider("Spacing", Bind(model => model.SpacingLabel), spacing)),
			Code(model => model.ConfigurationCode));
	}


	static Border Item(
		string text) =>
		new()
		{
			Width = 72,
			Height = 56,
			Background = Colors.Blue,
			CornerRadius = 12,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = text,
				TextStyle = TextStyle.Subheadline,
				FontWeight = FontWeight.Semibold,
				TextColor = Colors.White
			}
		};
}
