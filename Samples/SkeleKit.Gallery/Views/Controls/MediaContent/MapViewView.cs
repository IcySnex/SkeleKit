using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Controls.MediaContent;

[Page]
internal sealed class MapView : ShowcaseView<MapViewModel>
{
	static readonly MapRegion SanFrancisco =
		MapRegion.FromRadius(new(37.7749, -122.4194), 5_000);

	static readonly MapRegion ClusterRegion =
		MapRegion.FromRadius(new(37.7749, -122.4194), 14_000);


	public MapView(
		MapViewModel viewModel) : base(viewModel, "Map View", Colors.Orange)
	{
		AddPresentationShowcase(viewModel);
		AddPinsShowcase(viewModel);
	}


	void AddPresentationShowcase(
		MapViewModel viewModel)
	{
		SkeleKit.MapView map = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Height = 300,
			Region = Bind(
				model => model.Region,
				static (model, value) => model.Region = value),
			Kind = viewModel.SelectedKind.Value,
			ShowsCompass = true,
			CornerRadius = 18
		};

		Picker<ShowcaseOption<MapKind>> kind = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Kinds,
			SelectedItem = Bind(
				model => model.SelectedKind,
				static (model, value) => model.SelectedKind = value!),
			SelectionChanged = option => map.Kind = option.Value
		};

		Picker<ShowcaseOption<MapRegion>> region = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Regions,
			SelectedItem = Bind(
				model => model.SelectedRegion,
				static (model, value) => model.SelectedRegion = value!),
			SelectionChanged = option => map.SetRegion(option.Value, animated: true)
		};

		Picker<ShowcaseOption<MapGestureMode>> gestures = new()
		{
			MinWidth = 130,
			ItemsSource = viewModel.Gestures,
			SelectedItem = Bind(
				model => model.SelectedGestures,
				static (model, value) => model.SelectedGestures = value!),
			SelectionChanged = option =>
			{
				map.ScrollEnabled = viewModel.ScrollEnabled;
				map.ZoomEnabled = viewModel.ZoomEnabled;
				map.RotateEnabled = viewModel.RotateEnabled;
				map.PitchEnabled = viewModel.PitchEnabled;
			}
		};

		Switch scale = new()
		{
			IsOn = Bind(
				model => model.ShowsScale,
				static (model, value) => model.ShowsScale = value),
			Toggled = value =>
			{
				map.ShowsScale = value;
			}
		};

		Switch compass = new()
		{
			IsOn = Bind(
				model => model.ShowsCompass,
				static (model, value) => model.ShowsCompass = value),
			Toggled = value =>
			{
				map.ShowsCompass = value;
			}
		};

		Switch traffic = new()
		{
			IsOn = Bind(
				model => model.ShowsTraffic,
				static (model, value) => model.ShowsTraffic = value),
			Toggled = value =>
			{
				map.ShowsTraffic = value;
			}
		};

		AddShowcase(
			"Region & presentation",
			"Move between regions, compare native map imagery, and choose how the map responds to gestures.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 8,

						Children =
						{
							map,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.RegionSummary),
								TextStyle = TextStyle.Caption1,
								TextColor = Colors.SecondaryLabel
							}
						}
					},
					350),
				SettingRow("Map style", kind),
				SettingRow("Region", region),
				SettingRow("Gestures", gestures),
				SettingRow("Compass when rotated", compass),
				SettingRow("Scale while zooming", scale),
				SettingRow("Traffic", traffic)),
			ShowcaseBox.Code(Bind(model => model.PresentationCode)));
	}

	void AddPinsShowcase(
		MapViewModel viewModel)
	{
		ObservableCollection<MapOverlay> overlays = [];
		MapOverlay[] overlaySource = CreateOverlays();

		foreach (MapOverlay overlay in overlaySource)
			overlays.Add(overlay);

		SkeleKit.MapView map = new()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Height = 320,
			Region = SanFrancisco,
			Kind = MapKind.Muted,
			Pins = CreatePins(),
			Overlays = overlays,
			PinCommand = viewModel.SelectPinCommand,
			CornerRadius = 18
		};

		SegmentedControl clustering = new()
		{
			SelectedIndex = Bind(
				model => model.ClusterModeIndex,
				static (model, value) => model.ClusterModeIndex = value),
			SelectionChanged = index =>
			{
				map.ClusterMarker = index is 2 ? BuildCluster : null;
				map.ClustersPins = index > 0;
				map.SetRegion(index > 0 ? ClusterRegion : SanFrancisco, animated: true);
			}
		};
		clustering.Items.Add("Off");
		clustering.Items.Add("Native");
		clustering.Items.Add("Custom");

		Switch showsOverlays = new()
		{
			IsOn = Bind(
				model => model.ShowsOverlays,
				static (model, value) => model.ShowsOverlays = value),
			Toggled = value =>
			{
				overlays.Clear();

				if (value)
					foreach (MapOverlay overlay in overlaySource)
						overlays.Add(overlay);
			}
		};

		AddShowcase(
			"Pins & overlays",
			"Select native and custom markers, inspect callouts, cluster nearby places, and layer map shapes.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(
					new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 8,

						Children =
						{
							map,

							new Label
							{
								HorizontalAlignment = HorizontalAlignment.Center,
								Text = Bind(model => model.SelectionStatus),
								TextStyle = TextStyle.Caption1,
								TextColor = Colors.SecondaryLabel,
								MaxLines = 2,
								TextAlignment = TextAlignment.Center
							}
						}
					},
					380),
				LabeledControl("Clustering", clustering),
				SettingRow("Overlays", showsOverlays)),
			ShowcaseBox.Code(Bind(model => model.PinsCode)));
	}


	static MapPin[] CreatePins() =>
	[
		new(new(37.7955, -122.3937))
		{
			Title = "Ferry Building",
			Subtitle = "Marketplace and waterfront",
			Symbol = "ferry.fill",
			Tint = Colors.Orange,
			Callout = static () => BuildCallout(
				"Ferry Building",
				"A native marker with a custom SkeleKit callout.")
		},
		new(new(37.7786, -122.3893))
		{
			Title = "Oracle Park",
			Subtitle = "Baseball by the bay",
			Symbol = "baseball.fill",
			Tint = Colors.Red
		},
		new(new(37.8199, -122.4783))
		{
			Title = "Golden Gate Bridge",
			Subtitle = "San Francisco landmark",
			Symbol = "bridge.2.fill",
			Tint = Colors.Indigo
		},
		new(new(37.7694, -122.4862))
		{
			Title = "Golden Gate Park",
			Subtitle = "Gardens and trails",
			Marker = BuildParkMarker
		},
		new(new(37.8024, -122.4058))
		{
			Title = "Coit Tower",
			Subtitle = "Telegraph Hill",
			Symbol = "binoculars.fill",
			Tint = Colors.Teal
		}
	];

	static MapOverlay[] CreateOverlays() =>
	[
		new MapPolyline(
		[
			new(37.7955, -122.3937),
			new(37.8024, -122.4058),
			new(37.8199, -122.4783)
		])
		{
			StrokeColor = Colors.Orange,
			StrokeWidth = 4,
			LineDash = [8, 5]
		},
		new MapPolygon(
		[
			new(37.7715, -122.5110),
			new(37.7715, -122.4540),
			new(37.7640, -122.4540),
			new(37.7640, -122.5110)
		])
		{
			StrokeColor = Colors.Green,
			StrokeWidth = 2,
			FillColor = Colors.Green.WithAlpha(0.16)
		},
		new MapCircle(new(37.7786, -122.3893), 450)
		{
			StrokeColor = Colors.Blue,
			StrokeWidth = 2,
			FillColor = Colors.Blue.WithAlpha(0.12)
		}
	];

	static View BuildParkMarker() =>
		new Border
		{
			Padding = 7,
			Background = Colors.Green,
			CornerRadius = 16,

			Child = new Image
			{
				Source = ImageSource.Symbol("leaf.fill"),
				SymbolSize = 18,
				Tint = Colors.White
			}
		};

	static View BuildCluster(
		int count) =>
		new Border
		{
			Padding = new(10, 7),
			Background = Colors.Indigo,
			CornerRadius = 18,

			Child = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,

				Children =
				{
					new Image
					{
						Source = ImageSource.Symbol("square.stack.3d.up.fill"),
						SymbolSize = 14,
						Tint = Colors.White
					},

					new Label
					{
						VerticalAlignment = VerticalAlignment.Center,
						Text = count.ToString(),
						TextStyle = TextStyle.Caption1,
						FontWeight = FontWeight.Bold,
						TextColor = Colors.White
					}
				}
			}
		};

	static View BuildCallout(
		string title,
		string summary) =>
		new StackPanel
		{
			MaxWidth = 220,
			Padding = 8,
			Spacing = 3,

			Children =
			{
				new Label
				{
					Text = title,
					TextStyle = TextStyle.Subheadline,
					FontWeight = FontWeight.Semibold
				},

				new Label
				{
					Text = summary,
					TextStyle = TextStyle.Footnote,
					TextColor = Colors.SecondaryLabel,
					MaxLines = 2
				}
			}
		};
}
