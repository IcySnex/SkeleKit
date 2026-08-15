using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class TabsIpadViewModel : ShowcaseViewModel
{
	static readonly List<ShowcaseOption<TabBarMinimize>> MinimizeOptions =
	[
		new("Never", TabBarMinimize.Never),
		new("On scroll down", TabBarMinimize.OnScrollDown),
		new("On scroll up", TabBarMinimize.OnScrollUp)
	];

	bool isActive;
	TabBarMinimize previousMinimizeBehavior;


	internal event Action<string?>? BadgeChanged;


	[ObservableProperty]
	bool showsAccessory = true;

	[ObservableProperty]
	bool accessoryVisible;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PlayerIcon))]
	bool isPlaying;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(BadgeText))]
	[NotifyPropertyChangedFor(nameof(BadgeLabel))]
	double badgeCount = 3;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(MinimizeCode))]
	ShowcaseOption<TabBarMinimize> selectedMinimizeBehavior = MinimizeOptions[0];

	public string PlayerIcon =>
		IsPlaying ? "pause.fill" : "play.fill";

	public string? BadgeText =>
		BadgeCount <= 0
			? null
			: ((int)BadgeCount).ToString(CultureInfo.InvariantCulture);

	public string BadgeLabel =>
		BadgeText is string badge
			? $"Platform badge: {badge}"
			: "Platform badge cleared";

	public List<ShowcaseOption<TabBarMinimize>> MinimizeBehaviors =>
		MinimizeOptions;

	public IReadOnlyList<Span> AccessoryCode { get; } =
	[
		new(
			"""
			.Tabs(tabs => tabs
				.Accessory<NowPlayingAccessory>()
				.Tab<HomeView>("Home", "house")
				.Tab<LibraryView>("Library", "books.vertical"));

			public sealed class NowPlayingAccessory : StackPanel
			{
				public NowPlayingAccessory()
				{
					PlayerViewModel viewModel = SkeleApplication.Current!.Services
						.GetRequiredService<PlayerViewModel>();
					BindingContext = viewModel;
					IsVisible = BindingFactory.Bind(
						(PlayerViewModel model) => model.IsVisible);
				}
			}
			""")
	];

	public IReadOnlyList<Span> BadgeCode { get; } =
	[
		new(
			"""
			public sealed class PlatformView : ContentView<PlatformViewModel>
			{
				public PlatformView(PlatformViewModel viewModel) : base(viewModel)
				{
					TabBadge = Bind(model => model.Badge);
				}
			}
			""")
	];

	public IReadOnlyList<Span> TabsCode { get; } =
	[
		new(
			"""
			.Tabs(tabs => tabs
				.LargeTitles()
				.Tab<HomeView>("Home", "house")
				.Tab<LibraryView>("Library", "books.vertical")
				.Search<SearchView>());
			""")
	];

	public IReadOnlyList<Span> MinimizeCode =>
	[
		new(
			$$"""
			SkeleApplication.Current!.TabBarMinimizeBehavior =
				TabBarMinimize.{{SelectedMinimizeBehavior.Value}};
			""")
	];

	public IReadOnlyList<Span> PadCode { get; } =
	[
		new(
			"""
			.OnPad(pad => pad
				.Sidebar()
				.PlaceTab<HomeView>(TabPlacement.Locked)
				.Tab<InsightsView>(
					"Insights",
					"chart.bar",
					TabPlacement.Optional)
				.Group("Collections", "square.grid.2x2", group => group
					.Tab<AlbumsView>("Albums", "rectangle.stack")
					.Tab<ArtistsView>("Artists", "music.mic"))
				.SidebarFooter<AccountFooter>());
			""")
	];


	partial void OnShowsAccessoryChanged(
		bool value)
	{
		if (isActive)
			AccessoryVisible = value;
	}

	partial void OnBadgeCountChanged(
		double value)
	{
		if (isActive)
			BadgeChanged?.Invoke(BadgeText);
	}

	partial void OnSelectedMinimizeBehaviorChanged(
		ShowcaseOption<TabBarMinimize> value)
	{
		if (isActive && SkeleApplication.Current is SkeleApplication app)
			app.TabBarMinimizeBehavior = value.Value;
	}

	[RelayCommand]
	void ToggleAccessory() =>
		ShowsAccessory = !ShowsAccessory;

	[RelayCommand]
	void TogglePlayback() =>
		IsPlaying = !IsPlaying;

	[RelayCommand]
	void ClearBadge() =>
		BadgeCount = 0;

	internal void Enter()
	{
		if (SkeleApplication.Current is SkeleApplication app)
		{
			previousMinimizeBehavior = app.TabBarMinimizeBehavior;
			SelectedMinimizeBehavior = MinimizeOptions.First(
				option => option.Value == previousMinimizeBehavior);
		}

		isActive = true;
		AccessoryVisible = ShowsAccessory;
		BadgeChanged?.Invoke(BadgeText);
	}

	internal void Leave()
	{
		isActive = false;
		AccessoryVisible = false;
		BadgeChanged?.Invoke(null);

		if (SkeleApplication.Current is SkeleApplication app)
			app.TabBarMinimizeBehavior = previousMinimizeBehavior;
	}
}
