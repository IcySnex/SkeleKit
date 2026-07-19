using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Services;

public sealed class DemoCatalog : IDemoCatalog
{
	public ObservableCollection<DemoEntry> Demos { get; } =
	[
		new("Styling", typeof(StylingDemoViewModel)),
		new("Page chrome", typeof(ChromeDemoViewModel)),
		new("Tab accessory", typeof(AccessoryDemoViewModel)),
		new("Animation", typeof(AnimationDemoViewModel)),
		new("Button", typeof(ButtonDemoViewModel)),
		new("TextField", typeof(TextFieldDemoViewModel)),
		new("TextEditor", typeof(TextEditorDemoViewModel)),
		new("Switch", typeof(SwitchDemoViewModel)),
		new("SegmentedControl", typeof(SegmentedDemoViewModel)),
		new("DatePicker", typeof(DatePickerDemoViewModel)),
		new("Tint & ColorWell", typeof(TintDemoViewModel)),
		new("PageControl", typeof(PageControlDemoViewModel)),
		new("Slider", typeof(SliderDemoViewModel)),
		new("Stepper", typeof(StepperDemoViewModel)),
		new("ProgressBar", typeof(ProgressBarDemoViewModel)),
		new("ActivityIndicator", typeof(ActivityIndicatorDemoViewModel)),
		new("Divider", typeof(DividerDemoViewModel)),
		new("Picker", typeof(PickerDemoViewModel)),
		new("System Picker", typeof(SystemPickerDemoViewModel)),
		new("Image", typeof(ImageDemoViewModel)),
		new("WebView & Safari", typeof(WebViewDemoViewModel)),
		new("NativeView", typeof(NativeViewDemoViewModel)),
		new("Keyboard (no scroll)", typeof(KeyboardDemoViewModel)),
		new("CollectionView grid", typeof(GridDemoViewModel)),
		new("CollectionView list", typeof(ListDemoViewModel)),
		new("CollectionView contacts", typeof(ContactsDemoViewModel)),
		new("CollectionView carousel", typeof(CarouselDemoViewModel)),
		new("CollectionView mixed", typeof(MixedDemoViewModel)),
		new("CollectionView live + empty", typeof(LiveListDemoViewModel))
	];
}
