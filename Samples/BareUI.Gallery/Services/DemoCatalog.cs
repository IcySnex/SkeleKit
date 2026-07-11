using BareUI.Gallery.Models;
using BareUI.Gallery.Views.Demos;

namespace BareUI.Gallery.Services;

public sealed class DemoCatalog : IDemoCatalog
{
	public IReadOnlyList<DemoEntry> Demos { get; } =
	[
		new("Button", () => new ButtonDemo()),
		new("TextField", () => new TextFieldDemo()),
		new("TextEditor", () => new TextEditorDemo()),
		new("Switch", () => new SwitchDemo()),
		new("Slider", () => new SliderDemo()),
		new("Stepper", () => new StepperDemo()),
		new("ProgressBar", () => new ProgressBarDemo()),
		new("ActivityIndicator", () => new ActivityIndicatorDemo()),
		new("Divider", () => new DividerDemo()),
		new("Picker", () => new PickerDemo()),
		new("Image", () => new ImageDemo()),
		new("NativeView", () => new NativeViewDemo())
	];
}
