using BareUI.Gallery.Models;
using BareUI.Gallery.Views.Demos;

namespace BareUI.Gallery.Services;

public sealed class DemoCatalog : IDemoCatalog
{
	public IReadOnlyList<DemoEntry> Demos { get; } =
	[
		new("Button", ButtonDemo.Build),
		new("TextField", TextFieldDemo.Build),
		new("TextEditor", TextEditorDemo.Build),
		new("Switch", SwitchDemo.Build),
		new("Slider", SliderDemo.Build),
		new("Stepper", StepperDemo.Build),
		new("ProgressBar", ProgressBarDemo.Build),
		new("ActivityIndicator", ActivityIndicatorDemo.Build),
		new("Divider", DividerDemo.Build),
		new("Picker", PickerDemo.Build),
		new("Image", ImageDemo.Build),
		new("NativeView", NativeViewDemo.Build)
	];
}
