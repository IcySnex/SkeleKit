using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.MediaContent;

internal enum MapGestureMode
{
	All,
	PanAndZoom,
	Locked
}

internal sealed partial class MapViewModel : ShowcaseViewModel
{
	public MapViewModel()
	{
		SelectedKind = Kinds[0];
		SelectedRegion = Regions[0];
		SelectedGestures = Gestures[0];
		Region = SelectedRegion.Value;
	}


	public List<ShowcaseOption<MapKind>> Kinds { get; } =
	[
		new("Standard", MapKind.Standard),
		new("Muted", MapKind.Muted),
		new("Satellite", MapKind.Satellite),
		new("Hybrid", MapKind.Hybrid)
	];

	public List<ShowcaseOption<MapRegion>> Regions { get; } =
	[
		new("San Francisco", MapRegion.FromRadius(new(37.7749, -122.4194), 5_000)),
		new("London", MapRegion.FromRadius(new(51.5072, -0.1276), 5_000)),
		new("Tokyo", MapRegion.FromRadius(new(35.6764, 139.6500), 5_000))
	];

	public List<ShowcaseOption<MapGestureMode>> Gestures { get; } =
	[
		new("All", MapGestureMode.All),
		new("Pan & zoom", MapGestureMode.PanAndZoom),
		new("Locked", MapGestureMode.Locked)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PresentationCode))]
	ShowcaseOption<MapKind> selectedKind = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PresentationCode))]
	ShowcaseOption<MapRegion> selectedRegion = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PresentationCode))]
	ShowcaseOption<MapGestureMode> selectedGestures = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(RegionSummary))]
	MapRegion region;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PresentationCode))]
	bool showsCompass = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PresentationCode))]
	bool showsScale;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PresentationCode))]
	bool showsTraffic;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PinsCode))]
	int clusterModeIndex;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PinsCode))]
	bool showsOverlays = true;

	[ObservableProperty]
	string selectionStatus = "Tap a marker to inspect its command and callback.";

	string? callbackSelection;
	string? commandSelection;

	public string RegionSummary =>
		$"{Number(Region.Center.Latitude)}, {Number(Region.Center.Longitude)}";

	public IReadOnlyList<Span> PresentationCode =>
	[
		new(
			$$"""
			new MapView
			{
				Height = 300,
				Region = Bind(
					model => model.Region,
					(model, value) => model.Region = value),
				Kind = MapKind.{{SelectedKind.Value}},
				ScrollEnabled = {{Boolean(ScrollEnabled)}},
				ZoomEnabled = {{Boolean(ZoomEnabled)}},
				RotateEnabled = {{Boolean(RotateEnabled)}},
				PitchEnabled = {{Boolean(PitchEnabled)}},
				ShowsCompass = {{Boolean(ShowsCompass)}},
				ShowsScale = {{Boolean(ShowsScale)}},
				ShowsTraffic = {{Boolean(ShowsTraffic)}}
			};

			map.SetRegion(
				MapRegion.FromRadius(
					new Coordinate({{Number(SelectedRegion.Value.Center.Latitude)}}, {{Number(SelectedRegion.Value.Center.Longitude)}}),
					5_000),
				animated: true);
			""")
	];

	public IReadOnlyList<Span> PinsCode =>
	[
		new(
			$$"""
			MapPin ferryBuilding = new(new(37.7955, -122.3937))
			{
				Title = "Ferry Building",
				Subtitle = "San Francisco",
				Symbol = "ferry.fill",
				Tint = Colors.Orange,
				Callout = BuildCallout
			};

			new MapView
			{
				Height = 320,
				Region = MapRegion.FromRadius(new(37.7749, -122.4194), 5_000),
				Pins = pins,
				Overlays = overlays,
				ClustersPins = {{Boolean(ClusterModeIndex > 0)}},
				ClusterMarker = {{(ClusterModeIndex is 2 ? "BuildCluster" : "null")}},
				SelectionCommand = viewModel.SelectPinCommand,
				PinSelected = viewModel.RecordPinSelection
			};
			""")
	];

	internal bool ScrollEnabled =>
		SelectedGestures.Value is not MapGestureMode.Locked;

	internal bool ZoomEnabled =>
		SelectedGestures.Value is not MapGestureMode.Locked;

	internal bool RotateEnabled =>
		SelectedGestures.Value is MapGestureMode.All;

	internal bool PitchEnabled =>
		SelectedGestures.Value is MapGestureMode.All;


	[RelayCommand]
	void SelectPin(
		MapPin pin)
	{
		commandSelection = pin.Title ?? "Untitled pin";
		UpdateSelectionStatus();
	}

	internal void RecordPinSelection(
		MapPin pin)
	{
		callbackSelection = pin.Title ?? "Untitled pin";
		UpdateSelectionStatus();
	}


	void UpdateSelectionStatus() =>
		SelectionStatus = $"Command · {commandSelection ?? "waiting"} | Callback · {callbackSelection ?? "waiting"}";

	static string Boolean(
		bool value) =>
		value ? "true" : "false";

	static string Number(
		double value) =>
		value.ToString("0.####", CultureInfo.InvariantCulture);
}
