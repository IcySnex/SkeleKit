# Media and native content

Classification: **Visual showcase + interactive lab + escape hatch**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Image

Displays an image from a symbol, bundle asset, or URL.

- Source: `SkeleKit.iOS/Controls/Image.cs`
- Inheritance/shape: `class Image : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIImageView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Image.Source` | public get/set | C# default | Yes | Invalidates measure | Where the image is loaded from. URL sources load asynchronously, so give them an explicit Width/Height. |
| Property | `SkeleKit.Image.Placeholder` | public get/set | null | No | Visual/interaction only | A symbol or bundle image shown while a URL source is still loading, or null for none. |
| Property | `SkeleKit.Image.Fallback` | public get/set | null | No | Visual/interaction only | A symbol or bundle image shown when a URL source fails to load, or null to keep the placeholder. |
| Property | `SkeleKit.Image.FadesIn` | public get/set | C# default | No | Visual/interaction only | Whether a URL image cross-dissolves in once it arrives, instead of popping. |
| Property | `SkeleKit.Image.Stretch` | public get/set | Stretch.Uniform | No | Visual/interaction only | How the image is scaled to fill its bounds. |
| Property | `SkeleKit.Image.SymbolSize` | public get/set | double.NaN | No | Invalidates measure | The symbol's point size, or NaN for its natural size. |
| Property | `SkeleKit.Image.SymbolWeight` | public get/set | null | No | Invalidates measure | The symbol's stroke weight, or null for its default. |
| Property | `SkeleKit.Image.SymbolScale` | public get/set | C# default | No | Invalidates measure | The symbol's relative scale within its font metrics. |
| Property | `SkeleKit.Image.SymbolColors` | public get | [] | No | No automatic invalidation | Colors for the symbol's layers: one gives the hierarchical look, several assign the palette explicitly. |
| Property | `SkeleKit.Image.PrefersMulticolor` | public get/set | false | No | Visual/interaction only | Whether a symbol with a built-in multicolor rendition uses it. |
| Property | `SkeleKit.Image.SymbolValue` | public get/set | double.NaN | Yes | Visual/interaction only | The value 0–1 driving a variable symbol's layers (a wifi or speaker level), or NaN for none. |
| Property | `SkeleKit.Image.SymbolEffect` | public get/set | C# default | No | Visual/interaction only | An ambient effect the symbol performs continuously while set. |
| Method | `SkeleKit.Image.PlaySymbolEffect(SkeleKit.SymbolEffect)` | public | n/a | n/a | n/a | Plays a symbol effect once, on top of any ambient `Image.SymbolEffect`. |
| Method | `SkeleKit.Image.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Source and layout | `Source`, `Placeholder`, `Fallback`, `FadesIn`, `Stretch` | Load a fixed stock photograph with an explicit frame, placeholder, fallback, and cross-dissolve. Switch to an invalid address to expose failure behavior and compare every stretch mode. |
| Symbol rendering | `Source`, `SymbolSize`, `SymbolWeight`, `SymbolScale`, `SymbolColors`, `PrefersMulticolor` | Render a local SF Symbol with a three-color palette. Adjust point size, stroke weight, relative scale, and the preferred multicolor rendition. |
| Variable symbols and effects | `Source`, `SymbolValue`, `SymbolEffect`, `PlaySymbolEffect` | Drive a speaker symbol from 0 through 1, choose a continuously repeating ambient effect, and trigger a one-shot bounce independently. |

```csharp
new Image
{
	Width = 280,
	Height = 180,
	Source = ImageSource.Url("https://example.com/image.png"),
	Placeholder = ImageSource.Symbol("photo"),
	Fallback = ImageSource.Symbol("exclamationmark.triangle.fill"),
	FadesIn = true,
	Stretch = Stretch.UniformToFill
};

new Image
{
	Source = ImageSource.Symbol("cloud.sun.rain.fill"),
	SymbolSize = 72,
	SymbolWeight = FontWeight.Semibold,
	SymbolScale = SymbolScale.Large,
	SymbolColors = { Colors.Orange, Colors.Blue, Colors.Cyan },
	PrefersMulticolor = true
};

Image image = new()
{
	Source = ImageSource.Symbol("speaker.wave.3.fill"),
	SymbolSize = 72,
	SymbolValue = Bind(model => model.SymbolValue),
	SymbolEffect = SymbolEffect.Pulse
};

image.PlaySymbolEffect(SymbolEffect.Bounce);
```

## MapView

Embeds an interactive map in the tree, backed by a UIKit map view.

- Source: `SkeleKit.iOS/Controls/MapView.cs`
- Inheritance/shape: `class MapView : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `MKMapView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.
- Behavior note: `ShowsUserLocation` needs `NSLocationWhenInUseUsageDescription` in the app plist.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.MapView.Region` | public get/set | C# default | Yes | Visual/interaction only | The visible extent, updated two-way as the user pans and zooms. |
| Property | `SkeleKit.MapView.Kind` | public get/set | C# default | No | Visual/interaction only | The base imagery the map draws. |
| Property | `SkeleKit.MapView.ShowsUserLocation` | public get/set | C# default | No | Visual/interaction only | Whether the blue dot marking the user's location is shown. |
| Property | `SkeleKit.MapView.ScrollEnabled` | public get/set | true | No | Visual/interaction only | Whether the user can pan the map. |
| Property | `SkeleKit.MapView.ZoomEnabled` | public get/set | true | No | Visual/interaction only | Whether the user can zoom the map. |
| Property | `SkeleKit.MapView.RotateEnabled` | public get/set | true | No | Visual/interaction only | Whether the user can rotate the map. |
| Property | `SkeleKit.MapView.PitchEnabled` | public get/set | true | No | Visual/interaction only | Whether the user can tilt the map into a 3D pitch. |
| Property | `SkeleKit.MapView.ShowsCompass` | public get/set | true | No | Visual/interaction only | Whether the compass appears when the map is rotated. |
| Property | `SkeleKit.MapView.ShowsScale` | public get/set | C# default | No | Visual/interaction only | Whether a distance scale appears while zooming. |
| Property | `SkeleKit.MapView.ShowsTraffic` | public get/set | C# default | No | Visual/interaction only | Whether live traffic is drawn. |
| Property | `SkeleKit.MapView.Pins` | public get/set | C# default | No | Invalidates measure | The markers dropped on the map. |
| Property | `SkeleKit.MapView.Overlays` | public get/set | C# default | No | Invalidates measure | The shapes drawn on the map beneath its pins. |
| Property | `SkeleKit.MapView.ClustersPins` | public get/set | false | No | Visual/interaction only | Whether nearby pins collapse into a single counted marker that splits apart on zoom. |
| Property | `SkeleKit.MapView.ClusterMarker` | public get/set | null | No | No automatic invalidation | Builds a custom view for a cluster from its pin count, or null for the native counted marker. Only used while `MapView.ClustersPins` is on. |
| Property | `SkeleKit.MapView.SelectionCommand` | public get/set | null | No | No automatic invalidation | Invoked with the tapped pin. |
| Property | `SkeleKit.MapView.PinSelected` | public get/set | null | No | No automatic invalidation | Called with the pin the user tapped. |
| Method | `SkeleKit.MapView.SetRegion(SkeleKit.MapRegion,System.Boolean)` | public | n/a | n/a | n/a | Moves the map to a region. |
| Method | `SkeleKit.MapView.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapView.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Region`, `Kind`, `ShowsUserLocation`, `ScrollEnabled`, `ZoomEnabled`, `RotateEnabled`, `PitchEnabled`, `ShowsCompass`, `ShowsScale`, `ShowsTraffic`, `Pins`, `Overlays`, `ClustersPins`, `ClusterMarker`, `SelectionCommand`, `PinSelected` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapView specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## NativeView

Escape hatch to embed any UIKit view in a SkeleKit tree.

- Source: `SkeleKit.iOS/Controls/NativeView.cs`
- Inheritance/shape: `class NativeView : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: consumer-supplied `UIView`
- Gallery role: Advanced escape hatch

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.NativeView.#ctor(UIKit.UIView)` | public | n/a | n/a | n/a | Creates a wrapper for the given UIKit view. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(NativeView specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## WebView

Embeds live web content in the tree, backed by a UIKit web view.

- Source: `SkeleKit.iOS/Controls/WebView.cs`
- Inheritance/shape: `class WebView : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `WKWebView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.
- Behavior note: Loads a `WebView.Url` or raw `WebView.Html`, reports navigation through `WebView.Navigated` and `WebView.NavigationFailed`, and runs JavaScript through `WebView.EvaluateAsync`. Give it a bounded slot (a fill row, an explicit height), since web content has no intrinsic size to measure against.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.WebView.Url` | public get/set | C# default | Yes | Visual/interaction only | The web address to load. Takes effect when `WebView.Html` is not set. |
| Property | `SkeleKit.WebView.Html` | public get/set | C# default | Yes | Visual/interaction only | Raw HTML to load, overriding `WebView.Url` when set. |
| Property | `SkeleKit.WebView.AllowsBackGestures` | public get/set | C# default | No | Visual/interaction only | Whether swiping from the screen edge navigates back and forward through history. |
| Property | `SkeleKit.WebView.Navigated` | public get/set | null | No | No automatic invalidation | Called with the final address each time a page finishes loading. |
| Property | `SkeleKit.WebView.NavigationFailed` | public get/set | null | No | No automatic invalidation | Called with the failure description when a load fails. |
| Method | `SkeleKit.WebView.GoBack` | public | n/a | n/a | n/a | Navigates back to the previous page in history, if any. |
| Method | `SkeleKit.WebView.GoForward` | public | n/a | n/a | n/a | Navigates forward to the next page in history, if any. |
| Method | `SkeleKit.WebView.Reload` | public | n/a | n/a | n/a | Reloads the current page. |
| Method | `SkeleKit.WebView.EvaluateAsync(System.String)` | public async | n/a | n/a | n/a | Runs JavaScript in the current page and returns its result as a string. |
| Method | `SkeleKit.WebView.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.WebView.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Content and navigation | `Url`, `Html`, `AllowsBackGestures`, `Navigated`, `NavigationFailed`, `GoBack`, `GoForward`, `Reload`, `EvaluateAsync` | Switch among deterministic local HTML, a live website, and an invalid address. Create a local history entry, navigate backward and forward, reload, toggle native gestures, run JavaScript, and inspect success or failure callbacks. |

```csharp
WebView web = new()
{
	Height = 280,
	Html = """
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<h1>SkeleKit</h1>
		""",
	AllowsBackGestures = true,
	Navigated = viewModel.RecordNavigation,
	NavigationFailed = viewModel.RecordFailure
};

web.GoBack();
web.GoForward();
web.Reload();
string? title = await web.EvaluateAsync("document.title");
```
