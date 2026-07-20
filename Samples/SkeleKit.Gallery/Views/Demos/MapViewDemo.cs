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
				new(new Coordinate(37.7880, -122.4074)) { Title = "Union Square", Subtitle = "Shopping", Symbol = "star.fill", Tint = Colors.Red },
				new(new Coordinate(37.7599, -122.4148)) { Title = "Mission", Subtitle = "Burritos", Symbol = "heart.fill", Tint = Colors.Green },
				new(new Coordinate(37.7649, -122.4550)) { Title = "Golden Gate Park", Subtitle = "Green", Symbol = "leaf.fill", Tint = Colors.Purple }
			],
			PinSelected = pin => Prompt = pin.Title
		};

		Content = map;

		ToolbarItems.Add(new() { Icon = "map", Command = Command.From(() => map.Kind = MapKind.Standard) });
		ToolbarItems.Add(new() { Icon = "globe.americas.fill", Command = Command.From(() => map.Kind = MapKind.Hybrid) });
		ToolbarItems.Add(new() { Icon = "location.fill", Command = Command.From(() => map.SetRegion(BayArea, animated: true)) });
	}
}
