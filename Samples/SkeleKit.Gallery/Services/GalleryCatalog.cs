using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services.Abstract;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.ViewModels.Framework.Layout;

namespace SkeleKit.Gallery.Services;

internal sealed class GalleryCatalog : IGalleryCatalog
{
	static GallerySection Section(
		string title,
		params GalleryTopic[] topics) =>
		new(title, topics);

	static GalleryTopic Topic(
		string title,
		string summary,
		string symbol,
		Color accent,
		GalleryArea area,
		Type destination) =>
		new(title, summary, symbol, accent, area, destination);

	static int Rank(
		GalleryTopic topic,
		string query)
	{
		if (topic.Title.Equals(query, StringComparison.OrdinalIgnoreCase))
			return 0;
		if (topic.Title.StartsWith(query, StringComparison.OrdinalIgnoreCase))
			return 1;
		if (StartsWord(topic.Title, query))
			return 2;
		if (topic.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
			return 3;
		if (StartsWord(topic.Summary, query))
			return 4;
		if (topic.Summary.Contains(query, StringComparison.OrdinalIgnoreCase))
			return 5;

		return int.MaxValue;
	}

	static bool StartsWord(
		string value,
		string query)
	{
		int index = value.IndexOf(query, StringComparison.OrdinalIgnoreCase);

		while (index >= 0)
		{
			if (index == 0 || !char.IsLetterOrDigit(value[index - 1]))
				return true;

			index = value.IndexOf(query, index + query.Length, StringComparison.OrdinalIgnoreCase);
		}

		return false;
	}


	readonly List<GalleryTopic> topics;

	public GalleryCatalog()
	{
		Controls =
		[
			Section(
				"Actions & Selection",
				Topic("Button", "Native button configurations, menus and commands.", "button.programmable", Colors.Pink, GalleryArea.Controls, typeof(ButtonViewModel)),
				Topic("Picker", "Menu selection, custom labels and live items.", "dial.medium", Colors.Pink, GalleryArea.Controls, typeof(PickerViewModel)),
				Topic("SegmentedControl", "Compact selection, binding and content density.", "rectangle.split.3x1", Colors.Pink, GalleryArea.Controls, typeof(SegmentedControlViewModel)),
				Topic("DatePicker", "Dates, times, ranges and presentation styles.", "calendar", Colors.Pink, GalleryArea.Controls, typeof(DatePickerViewModel)),
				Topic("ColorWell", "System color selection with alpha support.", "paintpalette.fill", Colors.Pink, GalleryArea.Controls, typeof(ColorWellViewModel))),

			Section(
				"Text & Input",
				Topic("Label", "Plain and attributed text with Dynamic Type.", "textformat", Colors.Purple, GalleryArea.Controls, typeof(LabelViewModel)),
				Topic("TextView", "Selectable rich text with links and menus.", "doc.richtext", Colors.Purple, GalleryArea.Controls, typeof(TextViewViewModel)),
				Topic("TextField", "Single-line text entry and keyboard behavior.", "character.cursor.ibeam", Colors.Purple, GalleryArea.Controls, typeof(TextFieldViewModel)),
				Topic("SecureField", "Password entry with reveal and validation states.", "lock.fill", Colors.Purple, GalleryArea.Controls, typeof(SecureFieldViewModel)),
				Topic("TextEditor", "Multi-line editing, live growth and keyboard behavior.", "square.and.pencil", Colors.Purple, GalleryArea.Controls, typeof(TextEditorViewModel))),

			Section(
				"Values & Status",
				Topic("Switch", "A native binary setting with two-way state.", "switch.2", Colors.Red, GalleryArea.Controls, typeof(SwitchViewModel)),
				Topic("Slider", "Continuous values, snapping and update behavior.", "slider.horizontal.3", Colors.Red, GalleryArea.Controls, typeof(SliderViewModel)),
				Topic("Stepper", "Increment and decrement bounded values.", "plusminus", Colors.Red, GalleryArea.Controls, typeof(StepperViewModel)),
				Topic("ProgressBar", "Determinate task progress and tinting.", "chart.bar.fill", Colors.Red, GalleryArea.Controls, typeof(ProgressBarViewModel)),
				Topic("ActivityIndicator", "Indeterminate work and visibility states.", "progress.indicator", Colors.Red, GalleryArea.Controls, typeof(ActivityIndicatorViewModel)),
				Topic("PageControl", "Page position, direction and interaction.", "ellipsis", Colors.Red, GalleryArea.Controls, typeof(PageControlViewModel)),
				Topic("Divider", "Native-scale separators and semantic colors.", "minus", Colors.Red, GalleryArea.Controls, typeof(DividerViewModel))),

			Section(
				"Media & Content",
				Topic("Image", "Symbols, bundle assets, remote images and effects.", "photo.fill", Colors.Orange, GalleryArea.Controls, typeof(ImageViewModel)),
				Topic("WebView", "Web navigation, loading and failure states.", "globe", Colors.Orange, GalleryArea.Controls, typeof(WebViewModel)),
				Topic("MapView", "Regions, pins, overlays and clustering.", "map.fill", Colors.Orange, GalleryArea.Controls, typeof(MapViewModel)),
				Topic("NativeView", "Host a custom UIKit view when needed.", "shippingbox.fill", Colors.Orange, GalleryArea.Controls, typeof(NativeViewModel)))
		];

		Framework =
		[
			Section(
				"Foundations",
				Topic("View", "Layout, visibility, styling and interaction shared by every element.", "square.dashed", Colors.Indigo, GalleryArea.Framework, typeof(ViewViewModel)),
				Topic("ContentView", "Page composition, chrome, search and lifecycle.", "rectangle.portrait", Colors.Indigo, GalleryArea.Framework, typeof(ContentViewViewModel)),
				Topic("Panels", "Child collections, padding and binding inheritance.", "square.stack.3d.up.fill", Colors.Indigo, GalleryArea.Framework, typeof(PanelsViewModel)),
				Topic("Binding", "Compiled one-way, two-way and list bindings.", "arrow.trianglehead.2.clockwise.rotate.90", Colors.Indigo, GalleryArea.Framework, typeof(BindingViewModel))),

			Section(
				"Layout",
				Topic("Border", "Corner radius, stroke width and wrapped content.", "square", Colors.Blue, GalleryArea.Framework, typeof(BorderViewModel)),
				Topic("Grid", "Rows, columns, track sizing and spans.", "grid", Colors.Blue, GalleryArea.Framework, typeof(GridViewModel)),
				Topic("Overlay", "Layered children aligned in one shared space.", "square.3.layers.3d", Colors.Blue, GalleryArea.Framework, typeof(OverlayViewModel)),
				Topic("ScrollView", "Vertical content, live offsets and horizontal paging.", "scroll", Colors.Blue, GalleryArea.Framework, typeof(ScrollViewViewModel)),
				Topic("StackPanel", "Horizontal and vertical linear layout.", "rectangle.stack", Colors.Blue, GalleryArea.Framework, typeof(StackPanelViewModel))),

			Section(
				"Collections",
				Topic("Lists", "Native lists with diffable updates and selection.", "list.bullet", Colors.Teal, GalleryArea.Framework, typeof(ListsViewModel)),
				Topic("Grids", "Adaptive multi-column collection layouts.", "square.grid.2x2", Colors.Teal, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Carousels", "Horizontal snapping and peeking content.", "rectangle.on.rectangle.angled", Colors.Teal, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Sections", "Headers, footers and mixed section layouts.", "list.bullet.rectangle", Colors.Teal, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Collection Interactions", "Refresh, menus, swipe, reorder and prefetch.", "hand.draw.fill", Colors.Teal, GalleryArea.Framework, typeof(AboutViewModel))),

			Section(
				"Styling & Motion",
				Topic("Colors & Brushes", "Semantic colors, gradients and interpolation.", "paintbrush.fill", Colors.Cyan, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Materials & Shadows", "Native blur materials, clipping and depth.", "circle.lefthalf.filled", Colors.Cyan, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Styles & Themes", "Reusable setters and implicit application themes.", "swatchpalette.fill", Colors.Cyan, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Animation", "Curves, springs and layout transitions.", "waveform.path", Colors.Cyan, GalleryArea.Framework, typeof(AboutViewModel)),
				Topic("Animator", "Pause, scrub, reverse and continue interactive motion.", "timeline.selection", Colors.Cyan, GalleryArea.Framework, typeof(AboutViewModel)))
		];

		Platform =
		[
			Section(
				"Application",
				Topic("Navigation", "ViewModel-first push, pop and tab selection.", "arrow.left.arrow.right", Colors.Green, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Page Chrome", "Titles, search, toolbars, badges and status bars.", "platter.filled.top.iphone", Colors.Green, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Tabs & iPad", "Bottom tabs, search bubbles and sidebar arrangements.", "sidebar.left", Colors.Green, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Lifecycle & DI", "Application services and foreground transitions.", "app.badge.checkmark", Colors.Green, GalleryArea.Platform, typeof(AboutViewModel))),

			Section(
				"Presentation",
				Topic("Modals", "Sheets, detents, popovers and guarded dismissal.", "rectangle.portrait.bottomhalf.filled", Colors.Mint, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Dialogs", "Alerts, confirmations, prompts and selections.", "exclamationmark.bubble.fill", Colors.Mint, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Sharing", "Text, links and images through the share sheet.", "square.and.arrow.up", Colors.Mint, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("System Picking", "Photos and files with cancel and permission states.", "photo.badge.plus", Colors.Mint, GalleryArea.Platform, typeof(AboutViewModel))),

			Section(
				"Device",
				Topic("Haptics", "Impact, selection, notification and custom patterns.", "waveform", Colors.Teal, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Image Loading", "Remote loading, cancellation and custom loaders.", "arrow.down.circle.fill", Colors.Teal, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Accessibility", "VoiceOver labels, traits, values and focus.", "accessibility", Colors.Teal, GalleryArea.Platform, typeof(AboutViewModel)),
				Topic("Native Access", "UIKit views, controllers and gesture escape hatches.", "apple.terminal.fill", Colors.Teal, GalleryArea.Platform, typeof(AboutViewModel)))
		];

		topics =
		[
			.. Controls.SelectMany(section => section.Items),
			.. Framework.SelectMany(section => section.Items),
			.. Platform.SelectMany(section => section.Items)
		];
	}


	public List<GallerySection> Controls { get; }
	public List<GallerySection> Framework { get; }
	public List<GallerySection> Platform { get; }


	public List<GalleryTopic> Search(
		string query,
		GalleryArea? area)
	{
		if (string.IsNullOrWhiteSpace(query))
			return [];

		query = query.Trim();

		return
		[
			.. topics
				.Where(topic => area is null || topic.Area == area)
				.Where(topic => Rank(topic, query) < int.MaxValue)
				.OrderBy(topic => Rank(topic, query))
				.ThenBy(topic => topic.Title)
		];
	}
}
