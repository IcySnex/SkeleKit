using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class PageChromeViewModel : ShowcaseViewModel
{
	static readonly List<PageChromeTitleOption> TitleOptions =
	[
		new("Large", TitleStyle.Large, "TitleStyle.Large"),
		new("Inline", TitleStyle.Inline, "TitleStyle.Inline")
	];

	static readonly List<PageChromeBackgroundOption> BackgroundOptions =
	[
		new("Default", PageBackground.Default, "PageBackground.Default"),
		new("Grouped", PageBackground.Grouped, "PageBackground.Grouped"),
		new("None", PageBackground.None, "PageBackground.None")
	];

	static readonly List<PageChromeStatusBarOption> StatusBarOptions =
	[
		new("Default", StatusBarStyle.Default, "StatusBarStyle.Default"),
		new("Light", StatusBarStyle.Light, "StatusBarStyle.Light"),
		new("Dark", StatusBarStyle.Dark, "StatusBarStyle.Dark")
	];

	static readonly List<PageChromeSafeAreaOption> SafeAreaOptions =
	[
		new("All edges", SafeAreaEdges.All, "SafeAreaEdges.All"),
		new("None", SafeAreaEdges.None, "SafeAreaEdges.None"),
		new("Vertical", SafeAreaEdges.Top | SafeAreaEdges.Bottom, "SafeAreaEdges.Top | SafeAreaEdges.Bottom"),
		new("Top only", SafeAreaEdges.Top, "SafeAreaEdges.Top")
	];

	static readonly List<PageChromeColorOption> ColorOptions =
	[
		new("System", null, "null"),
		new("Green", Colors.Green, "Colors.Green"),
		new("Purple", Colors.Purple, "Colors.Purple"),
		new("Orange", Colors.Orange, "Colors.Orange")
	];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	PageChromeTitleOption selectedTitleStyle = TitleOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	PageChromeBackgroundOption selectedBackground = BackgroundOptions[1];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	PageChromeStatusBarOption selectedStatusBar = StatusBarOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	PageChromeSafeAreaOption selectedSafeArea = SafeAreaOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	PageChromeColorOption selectedAccentColors = ColorOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	bool showsPrompt;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	bool hidesNavigationBar;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	bool hidesTabBar;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	bool hasToolbar = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PageCode))]
	bool hasBottomToolbar;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SearchCode))]
	bool hidesSearchBarWhenScrolling;

	public List<PageChromeTitleOption> TitleStyles =>
		TitleOptions;

	public List<PageChromeBackgroundOption> Backgrounds =>
		BackgroundOptions;

	public List<PageChromeStatusBarOption> StatusBars =>
		StatusBarOptions;

	public List<PageChromeSafeAreaOption> SafeAreas =>
		SafeAreaOptions;

	public List<PageChromeColorOption> AccentColors =>
		ColorOptions;

	internal PageChromeConfiguration Configuration =>
		new(
			SelectedTitleStyle.Value,
			ShowsPrompt,
			SelectedSafeArea.Value,
			HidesNavigationBar,
			SelectedBackground.Value,
			SelectedStatusBar.Value,
			SelectedAccentColors.Value,
			HidesTabBar,
			HasToolbar,
			HasBottomToolbar);

	internal PageChromeSearchConfiguration SearchConfiguration =>
		new(
			HidesSearchBarWhenScrolling);

	public IReadOnlyList<Span> PageCode =>
	[
		new(
			$$"""
			ContentView page = new()
			{
				Title = "Page chrome",
				TitleStyle = {{SelectedTitleStyle.Code}},
				Prompt = {{(ShowsPrompt ? "\"ContentView\"" : "null")}},
				SafeAreaEdges = {{SelectedSafeArea.Code}},
				HidesNavigationBar = {{Bool(HidesNavigationBar)}},
				BackgroundStyle = {{SelectedBackground.Code}},
				StatusBar = {{SelectedStatusBar.Code}},
				BarTint = {{SelectedAccentColors.Code}},
				TitleColor = {{SelectedAccentColors.Code}},
				LargeTitleColor = {{SelectedAccentColors.Code}},
				HidesTabBar = {{Bool(HidesTabBar)}}
			};

			{{ToolbarCode}}
			""")
	];

	public IReadOnlyList<Span> SearchCode =>
	[
		new(
			$$"""
			ContentView page = new()
			{
				Title = "Search",
				SearchPlaceholder = "Search gallery",
				HidesSearchBarWhenScrolling = {{Bool(HidesSearchBarWhenScrolling)}},
				SearchChanged = query => status.Text = $"Typing: {query}",
				SearchCommand = Command.From<string>(query => status.Text = $"Submitted: {query}"),
				SearchCanceled = () => status.Text = "Search cancelled",
				Content = status
			};

			page.SearchScopes.Add("All");
			page.SearchScopes.Add("Recent");
			page.SearchScopes.Add("Saved");
			""")
	];

	string ToolbarCode =>
		$$"""
		if ({{Bool(HasToolbar)}})
			page.ToolbarItems.Add(new ToolbarItem { Icon = "plus", IsPrimary = true });

		if ({{Bool(HasBottomToolbar)}})
			page.BottomToolbarItems.Add(new ToolbarItem { Text = "Done", Icon = "checkmark", IsPrimary = true, Tint = Colors.Green });
		""";

	static string Bool(
		bool value) =>
		value ? "true" : "false";
}

internal sealed record PageChromeTitleOption(
	string Title,
	TitleStyle Value,
	string Code);

internal sealed record PageChromeBackgroundOption(
	string Title,
	PageBackground Value,
	string Code);

internal sealed record PageChromeStatusBarOption(
	string Title,
	StatusBarStyle Value,
	string Code);

internal sealed record PageChromeSafeAreaOption(
	string Title,
	SafeAreaEdges Value,
	string Code);

internal sealed record PageChromeColorOption(
	string Title,
	Color? Value,
	string Code);

internal sealed record PageChromeConfiguration(
	TitleStyle TitleStyle,
	bool ShowsPrompt,
	SafeAreaEdges SafeAreaEdges,
	bool HidesNavigationBar,
	PageBackground BackgroundStyle,
	StatusBarStyle StatusBar,
	Color? AccentColor,
	bool HidesTabBar,
	bool HasToolbar,
	bool HasBottomToolbar);

internal sealed record PageChromeSearchConfiguration(
	bool HidesSearchBarWhenScrolling);
