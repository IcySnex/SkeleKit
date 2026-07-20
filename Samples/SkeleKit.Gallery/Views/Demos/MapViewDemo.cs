using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// Demonstrates the embeddable <see cref="MapView"/>, its pins, selection, and map kinds.
/// </summary>
[Page]
public class MapViewDemo : ContentView<MapViewDemoViewModel>
{
	static readonly MapRegion BayArea = MapRegion.FromRadius(new Coordinate(37.7749, -122.4194), 6_000);


	public MapViewDemo(
		MapViewDemoViewModel viewModel) : base(viewModel)
	{
		Title = "MapView";

		MapView map = new()
		{
			Region = BayArea,
			ShowsUserLocation = true,
			Pins =
			[
				new(new Coordinate(37.7749, -122.4194)) { Title = "San Francisco", Subtitle = "City Hall", Symbol = "building.columns" },
				new(new Coordinate(37.8199, -122.4783)) { Title = "Golden Gate", Subtitle = "Bridge", Symbol = "car.fill", Tint = Colors.Orange },
				new(new Coordinate(37.8080, -122.4177)) { Title = "Pier 39", Symbol = "ferry.fill", Tint = Colors.Blue }
			],
			PinSelected = pin => Prompt = pin.Title
		};

		Content = map;

		ToolbarItems.Add(new() { Icon = "map", Command = Command.From(() => map.Kind = MapKind.Standard) });
		ToolbarItems.Add(new() { Icon = "globe.americas.fill", Command = Command.From(() => map.Kind = MapKind.Hybrid) });
		ToolbarItems.Add(new() { Icon = "location.fill", Command = Command.From(() => map.SetRegion(BayArea, animated: true)) });
	}
}
