using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.Models;
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
			CornerRadius = viewModel.EffectiveCornerRadius,
			CornerCurve = viewModel.SelectedCornerCurve.Value,

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
			Value = Bind(vm => vm.CornerRadius)
				.TwoWay((vm, val) => vm.CornerRadius = val),
			ValueChanged = value => frame.CornerRadius = value
		};
		View cornerRadiusSetting = LabeledSlider(
			"Corner radius",
			Bind(vm => vm.CornerRadiusLabel),
			cornerRadius);
		cornerRadiusSetting.IsVisible = Bind(vm => vm.UsesCustomCornerRadius);

		Picker<ShowcaseOption<double?>> systemCornerRadius = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.SystemCornerRadii,
			SelectedItem = Bind(vm => vm.SelectedSystemCornerRadius)
				.TwoWay((vm, val) => vm.SelectedSystemCornerRadius = val!),
			SelectionChanged = option => frame.CornerRadius = option.Value ?? viewModel.CornerRadius
		};

		Picker<ShowcaseOption<CornerCurve>> cornerCurve = new()
		{
			MinWidth = 150,
			ItemsSource = viewModel.CornerCurves,
			SelectedItem = Bind(vm => vm.SelectedCornerCurve)
				.TwoWay((vm, val) => vm.SelectedCornerCurve = val!),
			SelectionChanged = option => frame.CornerCurve = option.Value
		};

		Slider stroke = new()
		{
			Minimum = 0,
			Maximum = 6,
			Step = 0.5,
			Value = Bind(vm => vm.StrokeThickness)
				.TwoWay((vm, val) => vm.StrokeThickness = val),
			ValueChanged = value => frame.StrokeThickness = value
		};

		AddShowcase(
			"Corner radius & stroke",
			"Compare custom and system corner geometry, then adjust the outline around one text child.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(frame, 200),
				SettingRow("System corner radius", systemCornerRadius),
				cornerRadiusSetting,
				SettingRow("Corner curve", cornerCurve),
				LabeledSlider("Stroke width", Bind(vm => vm.StrokeLabel), stroke)),
			Code(vm => vm.FrameCode));
	}
}
