using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Services;

internal sealed class GalleryCatalog : IGalleryCatalog
{
	readonly List<GalleryTopic> topics;


	public GalleryCatalog()
	{
		Controls =
		[
			Section(
				"Text & Input",
				Topic("Label", "Plain and attributed text with Dynamic Type.", "textformat", Colors.Purple, GalleryArea.Controls),
				Topic("TextView", "Selectable rich text with links and menus.", "doc.richtext", Colors.Purple, GalleryArea.Controls),
				Topic("TextField", "Single-line text entry and keyboard behavior.", "character.cursor.ibeam", Colors.Purple, GalleryArea.Controls),
				Topic("SecureField", "Password entry with reveal and validation states.", "lock.fill", Colors.Purple, GalleryArea.Controls),
				Topic("TextEditor", "Multi-line editing, selection and formatting.", "square.and.pencil", Colors.Purple, GalleryArea.Controls)),

			Section(
				"Actions & Selection",
				Topic("Button", "Native button configurations, menus and commands.", "button.programmable", Colors.Pink, GalleryArea.Controls),
				Topic("Picker", "Wheel-based selection from deterministic values.", "dial.medium", Colors.Pink, GalleryArea.Controls),
				Topic("SegmentedControl", "Compact mutually exclusive choices.", "rectangle.split.3x1", Colors.Pink, GalleryArea.Controls),
				Topic("DatePicker", "Dates, times, ranges and presentation styles.", "calendar", Colors.Pink, GalleryArea.Controls),
				Topic("ColorWell", "System color selection with alpha support.", "paintpalette.fill", Colors.Pink, GalleryArea.Controls)),

			Section(
				"Values & Status",
				Topic("Switch", "A native binary setting with two-way state.", "switch.2", Colors.Red, GalleryArea.Controls),
				Topic("Slider", "Continuous values, ranges and custom tinting.", "slider.horizontal.3", Colors.Red, GalleryArea.Controls),
				Topic("Stepper", "Increment and decrement bounded values.", "plusminus", Colors.Red, GalleryArea.Controls),
				Topic("ProgressBar", "Determinate task progress and tinting.", "chart.bar.fill", Colors.Red, GalleryArea.Controls),
				Topic("ActivityIndicator", "Indeterminate work and visibility states.", "progress.indicator", Colors.Red, GalleryArea.Controls),
				Topic("PageControl", "Page position, direction and interaction.", "ellipsis", Colors.Red, GalleryArea.Controls),
				Topic("Divider", "Native-scale separators and semantic colors.", "minus", Colors.Red, GalleryArea.Controls)),

			Section(
				"Media & Content",
				Topic("Image", "Symbols, bundle assets, remote images and effects.", "photo.fill", Colors.Pink, GalleryArea.Controls),
				Topic("WebView", "Web navigation, loading and failure states.", "globe", Colors.Pink, GalleryArea.Controls),
				Topic("MapView", "Regions, pins, overlays and user location.", "map.fill", Colors.Pink, GalleryArea.Controls),
				Topic("NativeView", "Host a custom UIKit view when needed.", "shippingbox.fill", Colors.Pink, GalleryArea.Controls))
		];

		Framework =
		[
			Section(
				"Foundations",
				Topic("View", "Layout, visibility, styling and interaction shared by every element.", "square.dashed", Colors.Indigo, GalleryArea.Framework),
				Topic("ContentView", "Page composition, chrome, search and lifecycle.", "rectangle.portrait", Colors.Indigo, GalleryArea.Framework),
				Topic("Panels", "Child collections, padding and binding inheritance.", "square.stack.3d.up.fill", Colors.Indigo, GalleryArea.Framework),
				Topic("Binding", "Compiled one-way, two-way and list bindings.", "arrow.trianglehead.2.clockwise.rotate.90", Colors.Indigo, GalleryArea.Framework)),

			Section(
				"Layout",
				Topic("Border", "Padding, strokes and single-child composition.", "square", Colors.Blue, GalleryArea.Framework),
				Topic("Grid", "Auto, pixel and star tracks with spans.", "grid", Colors.Blue, GalleryArea.Framework),
				Topic("Overlay", "Layered children aligned in one shared space.", "square.3.layers.3d", Colors.Blue, GalleryArea.Framework),
				Topic("ScrollView", "Scrolling, keyboard avoidance and paging.", "scroll", Colors.Blue, GalleryArea.Framework),
				Topic("StackPanel", "Horizontal and vertical linear layout.", "rectangle.stack", Colors.Blue, GalleryArea.Framework)),

			Section(
				"Collections",
				Topic("Lists", "Native lists with diffable updates and selection.", "list.bullet", Colors.Teal, GalleryArea.Framework),
				Topic("Grids", "Adaptive multi-column collection layouts.", "square.grid.2x2", Colors.Teal, GalleryArea.Framework),
				Topic("Carousels", "Horizontal snapping and peeking content.", "rectangle.on.rectangle.angled", Colors.Teal, GalleryArea.Framework),
				Topic("Sections", "Headers, footers and mixed section layouts.", "list.bullet.rectangle", Colors.Teal, GalleryArea.Framework),
				Topic("Collection Interactions", "Refresh, menus, swipe, reorder and prefetch.", "hand.draw.fill", Colors.Teal, GalleryArea.Framework)),

			Section(
				"Styling & Motion",
				Topic("Colors & Brushes", "Semantic colors, gradients and interpolation.", "paintbrush.fill", Colors.Cyan, GalleryArea.Framework),
				Topic("Materials & Shadows", "Native blur materials, clipping and depth.", "circle.lefthalf.filled", Colors.Cyan, GalleryArea.Framework),
				Topic("Styles & Themes", "Reusable setters and implicit application themes.", "swatchpalette.fill", Colors.Cyan, GalleryArea.Framework),
				Topic("Animation", "Curves, springs and layout transitions.", "waveform.path", Colors.Cyan, GalleryArea.Framework),
				Topic("Animator", "Pause, scrub, reverse and continue interactive motion.", "timeline.selection", Colors.Cyan, GalleryArea.Framework))
		];

		Platform =
		[
			Section(
				"Application",
				Topic("Navigation", "ViewModel-first push, pop and tab selection.", "arrow.left.arrow.right", Colors.Red, GalleryArea.Platform),
				Topic("Page Chrome", "Titles, search, toolbars, badges and status bars.", "platter.filled.top.iphone", Colors.Red, GalleryArea.Platform),
				Topic("Tabs & iPad", "Bottom tabs, search bubbles and sidebar arrangements.", "sidebar.left", Colors.Red, GalleryArea.Platform),
				Topic("Lifecycle & DI", "Application services and foreground transitions.", "app.badge.checkmark", Colors.Red, GalleryArea.Platform)),

			Section(
				"Presentation",
				Topic("Modals", "Sheets, detents, popovers and guarded dismissal.", "rectangle.portrait.bottomhalf.filled", Colors.Orange, GalleryArea.Platform),
				Topic("Dialogs", "Alerts, confirmations, prompts and selections.", "exclamationmark.bubble.fill", Colors.Orange, GalleryArea.Platform),
				Topic("Sharing", "Text, links and images through the share sheet.", "square.and.arrow.up", Colors.Orange, GalleryArea.Platform),
				Topic("System Picking", "Photos and files with cancel and permission states.", "photo.badge.plus", Colors.Orange, GalleryArea.Platform)),

			Section(
				"Device",
				Topic("Haptics", "Impact, selection, notification and custom patterns.", "waveform", Colors.Pink, GalleryArea.Platform),
				Topic("Image Loading", "Remote loading, cancellation and custom loaders.", "arrow.down.circle.fill", Colors.Pink, GalleryArea.Platform),
				Topic("Accessibility", "VoiceOver labels, traits, values and focus.", "accessibility", Colors.Pink, GalleryArea.Platform),
				Topic("Native Access", "UIKit views, controllers and gesture escape hatches.", "apple.terminal.fill", Colors.Pink, GalleryArea.Platform))
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

		return topics
			.Where(topic => area is null || topic.Area == area)
			.Where(topic => Rank(topic, query) < int.MaxValue)
			.OrderBy(topic => Rank(topic, query))
			.ThenBy(topic => topic.Title)
			.ToList();
	}


	static GallerySection Section(
		string title,
		params GalleryTopic[] topics) =>
		new(title, topics);

	static GalleryTopic Topic(
		string title,
		string summary,
		string symbol,
		Color accent,
		GalleryArea area) =>
		new(title, summary, symbol, accent, area);

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
}
