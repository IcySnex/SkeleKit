using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// Demonstrates the embeddable <see cref="MapView"/>, its pins, selection, and map kinds.
/// </summary>
[Page]
public class MapViewDemo : ContentView<MapViewDemoViewModel>
{
	static readonly MapRegion BayArea = MapRegion.FromRadius(new Coordinate(37.7749, -122.4194), 6_000);

	static readonly Coordinate UnionSquare = new(37.7880, -122.4074);
	static readonly Coordinate Mission = new(37.7599, -122.4148);
	static readonly Coordinate Park = new(37.7649, -122.4550);


	static View Badge() =>
		new Border
		{
			Background = Colors.Blue,
			CornerRadius = 15,
			Padding = new Thickness(12, 7),
			Child = new Label { Text = "Fortnite", TextColor = Colors.White, FontSize = 14 }
		};

	static View Card() =>
		new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 10,
			Children =
			{
				new Image { Source = ImageSource.Symbol("bag.fill"), Tint = Colors.Red },
				new StackPanel
				{
					Spacing = 2,
					Children =
					{
						new Label { Text = "Fortnite", TextColor = Colors.Label, FontSize = 15 },
						new Label { Text = "Get the Fortnite Battle Pass!", TextColor = Colors.SecondaryLabel, FontSize = 13 }
					}
				}
			}
		};

	static IEnumerable<MapPin> Crowd()
	{
		Coordinate center = new(37.7760, -122.3950);
		Random random = new(42);

		for (int index = 0; index < 16; index++)
			yield return new MapPin(new Coordinate(center.Latitude + (random.NextDouble() - 0.5) * 0.012, center.Longitude + (random.NextDouble() - 0.5) * 0.012)) { Symbol = "mappin", Tint = Colors.Orange };
	}


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
				new(UnionSquare) { Title = "Union Square", Symbol = "star.fill", Tint = Colors.Red },
				new(Mission) { Title = "Mission", Subtitle = "Burritos", Symbol = "heart.fill", Tint = Colors.Green },
				new(Park) { Title = "Golden Gate Park", Subtitle = "Green", Symbol = "leaf.fill", Tint = Colors.Purple },
				new(new Coordinate(37.8010, -122.4180)) { Marker = Badge, Callout = Card },
				.. Crowd()
			],
			Overlays =
			[
				new MapPolyline([UnionSquare, Mission, Park]) { StrokeColor = Colors.Blue, StrokeWidth = 4, LineDash = [6, 4] },
				new MapCircle(Mission, 800) { StrokeColor = Colors.Green, FillColor = Colors.Green.WithAlpha(0.2) }
			],
			ClustersPins = true,
			PinCommand = viewModel.SelectCommand
		};

		Content = map;

		ToolbarItems.Add(new() { Icon = "circle.grid.3x3.fill", Command = Command.From(() => map.ClustersPins = !map.ClustersPins) });
		ToolbarItems.Add(new() { Icon = "map", Command = Command.From(() => map.Kind = MapKind.Standard) });
		ToolbarItems.Add(new() { Icon = "globe.americas.fill", Command = Command.From(() => map.Kind = MapKind.Hybrid) });
		ToolbarItems.Add(new() { Icon = "location.fill", Command = Command.From(() => map.SetRegion(BayArea, animated: true)) });
	}
}
