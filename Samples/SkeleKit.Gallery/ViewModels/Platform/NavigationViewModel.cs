using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class NavigationViewModel(
	INavigator navigator) : ShowcaseViewModel
{
	static readonly List<NavigationModalOption> ModalStyleOptions =
	[
		new("Automatic", NavigationModalKind.Automatic, "ModalStyle.Automatic"),
		new("Sheet", NavigationModalKind.Sheet, null),
		new("Full screen", NavigationModalKind.FullScreen, "ModalStyle.FullScreen"),
		new("Form sheet", NavigationModalKind.FormSheet, "ModalStyle.FormSheet"),
		new("Current context", NavigationModalKind.CurrentContext, "ModalStyle.CurrentContext"),
		new("Over full screen", NavigationModalKind.OverFullScreen, "ModalStyle.OverFullScreen"),
		new("Over current context", NavigationModalKind.OverCurrentContext, "ModalStyle.OverCurrentContext"),
		new("Popover", NavigationModalKind.Popover, "ModalStyle.Popover(present)")
	];

	static readonly List<NavigationDetentOption> DetentOptions =
	[
		new(
			"Medium",
			"Detent.Medium",
			[Detent.Medium]),
		new(
			"Medium → Large",
			"Detent.Medium, Detent.Large",
			[Detent.Medium, Detent.Large]),
		new(
			"Large",
			"Detent.Large",
			[Detent.Large]),
		new(
			"Content",
			"Detent.Content",
			[Detent.Content]),
		new(
			"Content → Large",
			"Detent.Content, Detent.Large",
			[Detent.Content, Detent.Large]),
		new(
			"180 pt",
			"Detent.Height(180)",
			[Detent.Height(180)]),
		new(
			"420 pt",
			"Detent.Height(420)",
			[Detent.Height(420)]),
		new(
			"25%",
			"Detent.Fraction(0.25)",
			[Detent.Fraction(0.25)]),
		new(
			"75%",
			"Detent.Fraction(0.75)",
			[Detent.Fraction(0.75)])
	];

	static readonly List<NavigationSafariDismissButtonOption> SafariDismissButtonOptions =
	[
		new(
			"Close",
			SafariDismissButtonStyle.Close,
			"SafariDismissButtonStyle.Close"),
		new(
			"Done",
			SafariDismissButtonStyle.Done,
			"SafariDismissButtonStyle.Done"),
		new(
			"Cancel",
			SafariDismissButtonStyle.Cancel,
			"SafariDismissButtonStyle.Cancel")
	];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ModalCode))]
	NavigationModalOption selectedModalStyle = ModalStyleOptions[1];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ModalCode))]
	NavigationDetentOption selectedDetents = DetentOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(UrlCode))]
	NavigationModalOption selectedUrlModalStyle = ModalStyleOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(UrlCode))]
	NavigationDetentOption selectedUrlDetents = DetentOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(UrlCode))]
	NavigationSafariDismissButtonOption selectedSafariDismissButton = SafariDismissButtonOptions[0];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(UrlCode))]
	bool entersReaderIfAvailable;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(UrlCode))]
	bool barCollapsingEnabled = true;


	public List<string> Tabs { get; } =
	[
		"Framework",
		"Controls",
		"Platform"
	];

	public List<NavigationModalOption> ModalStyles =>
		ModalStyleOptions;

	public List<NavigationDetentOption> Detents =>
		DetentOptions;

	public List<NavigationModalOption> UrlModalStyles =>
		ModalStyleOptions;

	public List<NavigationDetentOption> UrlDetents =>
		DetentOptions;

	public List<NavigationSafariDismissButtonOption> SafariDismissButtons =>
		SafariDismissButtonOptions;

	public string ModalStyleCode =>
		SelectedModalStyle.Kind is NavigationModalKind.Sheet
			? $"ModalStyle.Sheet({SelectedDetents.Code})"
			: SelectedModalStyle.Code!;

	public IReadOnlyList<Span> StackCode { get; } =
	[
		new(
			"""
			await navigator.PushAsync(
				new NavigationDetailViewModel(navigator, depth: 1));

			await navigator.PopAsync();
			await navigator.PopToRootAsync();
			""")
	];

	public IReadOnlyList<Span> ModalCode =>
	[
		new(
			$$"""
			await navigator.PresentAsync(
				new NavigationSheetViewModel(navigator),
				{{ModalStyleCode}});
			""")
	];

	public IReadOnlyList<Span> TabsCode { get; } =
	[
		new(
			"""
			[RelayCommand]
			Task SelectTabAsync(string title) =>
				navigator.SelectTabAsync(title);
			""")
	];

	public string UrlModalStyleCode =>
		SelectedUrlModalStyle.Kind switch
		{
			NavigationModalKind.Sheet => $"ModalStyle.Sheet({SelectedUrlDetents.Code})",
			NavigationModalKind.Popover => "ModalStyle.Popover(anchor)",
			_ => SelectedUrlModalStyle.Code!
		};

	public IReadOnlyList<Span> UrlCode =>
	[
		new(
			$$"""
			await navigator.OpenUrlAsync(
				"https://github.com/IcySnex/SkeleKit",
				{{UrlModalStyleCode}},
				entersReaderIfAvailable: {{BooleanCode(EntersReaderIfAvailable)}},
				barCollapsingEnabled: {{BooleanCode(BarCollapsingEnabled)}},
				dismissButtonStyle: {{SelectedSafariDismissButton.Code}});
			""")
	];


	[RelayCommand]
	Task PushDetailAsync() =>
		navigator.PushAsync(
			new NavigationDetailViewModel(navigator, 1));

	internal Task PresentModalAsync(
		View anchor) =>
		navigator.PresentAsync(
			new NavigationSheetViewModel(navigator),
			CreateModalStyle(anchor));

	[RelayCommand]
	internal Task OpenUrlAsync(
		View anchor) =>
		navigator.OpenUrlAsync(
			"https://github.com/IcySnex/SkeleKit",
			CreateUrlModalStyle(anchor),
			EntersReaderIfAvailable,
			BarCollapsingEnabled,
			SelectedSafariDismissButton.Style);

	[RelayCommand]
	async Task SelectTabAsync(
		string? title)
	{
		if (string.IsNullOrWhiteSpace(title))
			return;

		await navigator.SelectTabAsync(title);
	}

	ModalStyle CreateModalStyle(
		View anchor) =>
		BuildModalStyle(
			SelectedModalStyle,
			SelectedDetents,
			anchor);

	ModalStyle CreateUrlModalStyle(
		View anchor) =>
		BuildModalStyle(
			SelectedUrlModalStyle,
			SelectedUrlDetents,
			anchor);

	static ModalStyle BuildModalStyle(
		NavigationModalOption option,
		NavigationDetentOption detents,
		View anchor) =>
		option.Kind switch
		{
			NavigationModalKind.Automatic => ModalStyle.Automatic,
			NavigationModalKind.Sheet => ModalStyle.Sheet(detents.Detents),
			NavigationModalKind.FullScreen => ModalStyle.FullScreen,
			NavigationModalKind.FormSheet => ModalStyle.FormSheet,
			NavigationModalKind.CurrentContext => ModalStyle.CurrentContext,
			NavigationModalKind.OverFullScreen => ModalStyle.OverFullScreen,
			NavigationModalKind.OverCurrentContext => ModalStyle.OverCurrentContext,
			NavigationModalKind.Popover => ModalStyle.Popover(anchor),
			_ => ModalStyle.Automatic
		};

	static string BooleanCode(
		bool value) =>
		value ? "true" : "false";
}

internal sealed record NavigationModalOption(
	string Title,
	NavigationModalKind Kind,
	string? Code);

internal sealed record NavigationDetentOption(
	string Title,
	string Code,
	Detent[] Detents);

internal sealed record NavigationSafariDismissButtonOption(
	string Title,
	SafariDismissButtonStyle Style,
	string Code);

internal enum NavigationModalKind
{
	Automatic,
	Sheet,
	FullScreen,
	FormSheet,
	CurrentContext,
	OverFullScreen,
	OverCurrentContext,
	Popover
}
