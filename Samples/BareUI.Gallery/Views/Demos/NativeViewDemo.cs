using BareUI;
using BareUI.Gallery.Views;
using BareUI.Gallery.ViewModels.Demos;
using UIKit;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Demonstrates <see cref="NativeView"/> wrapping native UIKit controls (escape hatch for controls not yet in BareUI).
/// </summary>
public class NativeViewDemo : ContentView<NativeViewDemoViewModel>
{
	public NativeViewDemo()
	{
		Title = "NativeView";

		Content =
			new ScrollView
			{
				Content = new VStack
				{
					Spacing = 20,
					Margin = new Thickness(16),
					Children =
					{
						new Label { Style = Styles.Caption, Text = "UISegmentedControl" },
						new NativeView(CreateSegmentedControl()),

						new Label { Style = Styles.Caption, Text = "UIDatePicker" },
						new NativeView(CreateDatePicker()),

						new Label { Style = Styles.Caption, Text = "UISlider (native)" },
						new NativeView(CreateNativeSlider())
					}
				}
			};
	}

	static UIView CreateSegmentedControl()
	{
		var segmented = new UISegmentedControl(["Option 1", "Option 2", "Option 3"])
		{
			SelectedSegment = 0
		};
		segmented.ValueChanged += (sender, e) =>
			Console.WriteLine($"NativeViewDemo: segmented control changed to {segmented.SelectedSegment}");
		return segmented;
	}

	static UIView CreateDatePicker()
	{
		var datePicker = new UIDatePicker
		{
			Mode = UIDatePickerMode.Date,
			Date = NSDate.Now
		};
		datePicker.ValueChanged += (sender, e) =>
			Console.WriteLine($"NativeViewDemo: date changed to {datePicker.Date}");
		return datePicker;
	}

	static UIView CreateNativeSlider()
	{
		var slider = new UISlider
		{
			MinValue = 0,
			MaxValue = 100,
			Value = 50
		};
		slider.ValueChanged += (sender, e) =>
			Console.WriteLine($"NativeViewDemo: native slider changed to {slider.Value}");
		return slider;
	}
}
