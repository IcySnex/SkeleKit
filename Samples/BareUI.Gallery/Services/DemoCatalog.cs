using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Services;

public sealed class DemoCatalog : IDemoCatalog
{
	public IReadOnlyList<DemoEntry> Demos { get; } =
	[
		new("Button", typeof(ButtonDemoViewModel)),
		new("TextField", typeof(TextFieldDemoViewModel)),
		new("TextEditor", typeof(TextEditorDemoViewModel)),
		new("Switch", typeof(SwitchDemoViewModel)),
		new("Slider", typeof(SliderDemoViewModel)),
		new("Stepper", typeof(StepperDemoViewModel)),
		new("ProgressBar", typeof(ProgressBarDemoViewModel)),
		new("ActivityIndicator", typeof(ActivityIndicatorDemoViewModel)),
		new("Divider", typeof(DividerDemoViewModel)),
		new("Picker", typeof(PickerDemoViewModel)),
		new("Image", typeof(ImageDemoViewModel)),
		new("NativeView", typeof(NativeViewDemoViewModel))
	];
}
