namespace SkeleKit.Gallery;

internal static class GalleryCatalog
{
	public static List<GalleryCategory> Categories { get; } =
	[
		new(
			"Foundations",
			"The shared view model, panels and data binding.",
			"square.stack.3d.up.fill",
			Colors.Indigo,
			["View", "Control", "ContentView", "Panels", "Binding"]),

		new(
			"Layout",
			"Compose responsive interfaces with native-backed layout.",
			"rectangle.3.group.fill",
			Colors.Blue,
			["Border", "Grid", "Overlay", "ScrollView", "StackPanel"]),

		new(
			"Text & Input",
			"Typography, rich text and keyboard-aware editing.",
			"textformat",
			Colors.Purple,
			["Label", "TextView", "TextField", "SecureField", "TextEditor"]),

		new(
			"Actions & Selection",
			"Commands, choices, dates and color selection.",
			"hand.tap.fill",
			Colors.Orange,
			["Button", "Picker", "SegmentedControl", "DatePicker", "ColorWell"]),

		new(
			"Values & Status",
			"Continuous values, progress and compact status controls.",
			"slider.horizontal.3",
			Colors.Green,
			["Switch", "Slider", "Stepper", "ProgressBar", "ActivityIndicator", "PageControl", "Divider"]),

		new(
			"Media & Content",
			"Images, web content, maps and native escape hatches.",
			"photo.on.rectangle.angled",
			Colors.Pink,
			["Image", "WebView", "MapView", "NativeView"]),

		new(
			"Collections",
			"Data-driven lists, grids, carousels and sections.",
			"list.bullet.rectangle.fill",
			Colors.Teal,
			["Lists", "Grids", "Carousels", "Sections", "Collection interactions"]),

		new(
			"Styling & Motion",
			"Themes, materials, brushes and interactive animation.",
			"wand.and.stars",
			Colors.Cyan,
			["Colors & Brushes", "Materials & Shadows", "Styles & Themes", "Animation", "Animator"]),

		new(
			"Application & System",
			"Navigation, presentation and device integrations.",
			"apps.iphone",
			Colors.Red,
			["Navigation", "Modals & Dialogs", "Sharing & Picking", "Haptics", "Page Chrome"])
	];
}
