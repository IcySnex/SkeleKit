# Images, maps, and collection configuration

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## CollectionLayoutKind

How a `CollectionView` arranges its items.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/CollectionLayout.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.CollectionLayoutKind.List` | public | n/a | n/a | n/a | A vertical list of full-width rows. |
| Field/value | `SkeleKit.CollectionLayoutKind.Grid` | public | n/a | n/a | n/a | A vertical grid of equal columns. |
| Field/value | `SkeleKit.CollectionLayoutKind.Carousel` | public | n/a | n/a | n/a | A horizontally scrolling row. |
| Field/value | `SkeleKit.CollectionLayoutKind.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(CollectionLayoutKind specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## CarouselSnap

How a carousel settles when the drag ends.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/CollectionLayout.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.
- Behavior note: Mirrors SwiftUI's scroll target behavior.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.CarouselSnap.None` | public | n/a | n/a | n/a | Free scrolling; stops wherever the drag ends. |
| Field/value | `SkeleKit.CarouselSnap.LeadingBoundary` | public | n/a | n/a | n/a | Free scrolling, but the resting offset lands on an item's leading edge. |
| Field/value | `SkeleKit.CarouselSnap.LeadingBoundaryPeek` | public | n/a | n/a | n/a | Like `CarouselSnap.LeadingBoundary`, but leaves a small slice of the previous item visible. |
| Field/value | `SkeleKit.CarouselSnap.Item` | public | n/a | n/a | n/a | Settles on an item, leading edge aligned. |
| Field/value | `SkeleKit.CarouselSnap.ItemPeek` | public | n/a | n/a | n/a | Like `CarouselSnap.Item`, but leaves a small slice of the previous item visible. |
| Field/value | `SkeleKit.CarouselSnap.ItemCentered` | public | n/a | n/a | n/a | Settles on an item, centered. |
| Field/value | `SkeleKit.CarouselSnap.Page` | public | n/a | n/a | n/a | Settles a full page at a time. |
| Field/value | `SkeleKit.CarouselSnap.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(CarouselSnap specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## CollectionLayout

The layout of a `CollectionView`: a list, a grid, or a carousel.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/CollectionLayout.cs`
- Inheritance/shape: `struct CollectionLayout`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.CollectionLayout.List(System.Boolean)` | public static | n/a | n/a | n/a | A list of full-width rows; `grouped` uses the native inset-grouped style. |
| Method | `SkeleKit.CollectionLayout.Grid(System.Int32,System.Double)` | public static | n/a | n/a | n/a | A grid of equal columns. |
| Method | `SkeleKit.CollectionLayout.Carousel(System.Double,System.Double,SkeleKit.CarouselSnap)` | public static | n/a | n/a | n/a | A horizontally scrolling row of fixed-width items, optionally snapping as it settles. |
| Property | `SkeleKit.CollectionLayout.Kind` | public get | C# default | No | No automatic invalidation | Which arrangement this is. |
| Property | `SkeleKit.CollectionLayout.Columns` | public get | 0 | No | No automatic invalidation | Columns per row, for a grid. |
| Property | `SkeleKit.CollectionLayout.Spacing` | public get | 0 | No | No automatic invalidation | Gap between items, in points. |
| Property | `SkeleKit.CollectionLayout.ItemWidth` | public get | 0 | No | No automatic invalidation | Item width for a carousel, in points. |
| Property | `SkeleKit.CollectionLayout.Grouped` | public get | false | No | No automatic invalidation | Whether a list uses the native inset-grouped style. |
| Property | `SkeleKit.CollectionLayout.Snap` | public get | C# default | No | No automatic invalidation | How a carousel settles when the drag ends. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Kind`, `Columns`, `Spacing`, `ItemWidth`, `Grouped`, `Snap` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(CollectionLayout specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## IExpandableSection<T>

A `ISection`1` whose items collapse behind its header.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/IExpandableSection.cs`
- Inheritance/shape: `interface IExpandableSection<T> : ISection<TItem>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.
- Behavior note: The header shows a chevron and tapping it toggles `IExpandableSection`1.IsExpanded`.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.IExpandableSection`1.IsExpanded` | get/set | false | No | No automatic invalidation | Whether the group's items are shown. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `IsExpanded` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(IExpandableSection<object> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ImageSource

Describes where an image comes from, without touching UIKit.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/ImageSource.cs`
- Inheritance/shape: `struct ImageSource`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.ImageSource.Symbol(System.String)` | public static | n/a | n/a | n/a | An image from an SF Symbol name. |
| Method | `SkeleKit.ImageSource.Bundle(System.String)` | public static | n/a | n/a | n/a | An image from a bundle asset name. |
| Method | `SkeleKit.ImageSource.Url(System.String)` | public static | n/a | n/a | n/a | An image from a remote URL, loaded asynchronously. |
| Method | `SkeleKit.ImageSource.Data(System.Byte[])` | public static | n/a | n/a | n/a | An image from raw encoded bytes. |
| Method | `SkeleKit.ImageSource.op_Implicit(System.String)~SkeleKit.ImageSource` | public static | n/a | n/a | n/a | Treats a string as a URL when it looks like one, otherwise resolves it automatically. |
| Property | `SkeleKit.ImageSource.Kind` | public get | C# default | No | No automatic invalidation | How `ImageSource.Value` should be resolved. |
| Property | `SkeleKit.ImageSource.Value` | public get | C# default | No | No automatic invalidation | The symbol name, bundle asset name, or URL. |
| Method | `SkeleKit.ImageSource.op_Implicit(System.String)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Kind`, `Value` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ImageSource specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ImageSourceKind

Where an `Image` loads its content from.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/ImageSource.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ImageSourceKind.Auto` | public | n/a | n/a | n/a | Resolve from a bundle asset first, then an SF Symbol. |
| Field/value | `SkeleKit.ImageSourceKind.Symbol` | public | n/a | n/a | n/a | An SF Symbol name. |
| Field/value | `SkeleKit.ImageSourceKind.Bundle` | public | n/a | n/a | n/a | A bundle asset name. |
| Field/value | `SkeleKit.ImageSourceKind.Url` | public | n/a | n/a | n/a | A remote URL loaded asynchronously. |
| Field/value | `SkeleKit.ImageSourceKind.Data` | public | n/a | n/a | n/a | Raw encoded image bytes held in memory. |
| Field/value | `SkeleKit.ImageSourceKind.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ImageSourceKind specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## IndicatorStyle

The color of a scroll view's indicator bar.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/IndicatorStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.IndicatorStyle.Default` | public | n/a | n/a | n/a | Follows the system appearance. |
| Field/value | `SkeleKit.IndicatorStyle.Dark` | public | n/a | n/a | n/a | A dark bar, for light content. |
| Field/value | `SkeleKit.IndicatorStyle.Light` | public | n/a | n/a | n/a | A light bar, for dark content. |
| Field/value | `SkeleKit.IndicatorStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(IndicatorStyle specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ISection<T>

A group of items in a `CollectionView`.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/ISection.cs`
- Inheritance/shape: `interface ISection<T>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.
- Behavior note: Implement it on your own section model, which the header and footer templates bind to.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ISection`1.Items` | get | C# default | No | No automatic invalidation | The items in this group. Mutations animate when the list is an `ObservableCollection`. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Items` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ISection<object> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ItemMove<T>

Describes a completed drag-to-reorder: which item moved and where it went.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/ItemMove.cs`
- Inheritance/shape: `record ItemMove<T>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.ItemMove`1.#ctor(`0,System.Int32,System.Int32,System.Int32,System.Int32)` | public (compiled) | n/a | n/a | n/a | Describes a completed drag-to-reorder: which item moved and where it went. |
| Property | `SkeleKit.ItemMove`1.Item` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The item the user moved. |
| Property | `SkeleKit.ItemMove`1.FromSection` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The section the item left. 0 for a flat list. |
| Property | `SkeleKit.ItemMove`1.FromIndex` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The item's index within its old section. |
| Property | `SkeleKit.ItemMove`1.ToSection` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The section the item landed in. 0 for a flat list. |
| Property | `SkeleKit.ItemMove`1.ToIndex` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The item's index within its new section. |
| Method | `SkeleKit.ItemMove`1.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.op_Inequality(SkeleKit.ItemMove{`0},SkeleKit.ItemMove{`0})` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.op_Equality(SkeleKit.ItemMove{`0},SkeleKit.ItemMove{`0})` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.Equals(SkeleKit.ItemMove{`0})` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.<Clone>$` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemMove`1.Deconstruct(`0@,System.Int32@,System.Int32@,System.Int32@,System.Int32@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Item`, `FromSection`, `FromIndex`, `ToSection`, `ToIndex` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ItemMove<object> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapKind

The base imagery a map draws.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapKind.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.MapKind.Standard` | public | n/a | n/a | n/a | The default road map. |
| Field/value | `SkeleKit.MapKind.Muted` | public | n/a | n/a | n/a | The road map with muted colors, so overlaid content stands out. |
| Field/value | `SkeleKit.MapKind.Satellite` | public | n/a | n/a | n/a | Satellite imagery with no labels. |
| Field/value | `SkeleKit.MapKind.Hybrid` | public | n/a | n/a | n/a | Satellite imagery with road and place labels. |
| Field/value | `SkeleKit.MapKind.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapKind specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapOverlay

A shape drawn onto a `MapView` beneath its pins.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapOverlay.cs`
- Inheritance/shape: `class MapOverlay`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.MapOverlay.StrokeColor` | public get/set | null | No | No automatic invalidation | The outline color, or null for none. |
| Property | `SkeleKit.MapOverlay.StrokeWidth` | public get/set | 2 | No | No automatic invalidation | The outline width in points. |
| Property | `SkeleKit.MapOverlay.FillColor` | public get/set | null | No | No automatic invalidation | The fill color, or null for none. A `MapPolyline` has no interior, so its fill is ignored. |
| Property | `SkeleKit.MapOverlay.LineDash` | public get/set | null | No | No automatic invalidation | The dash lengths of the outline, alternating on and off, or null for a solid line. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `StrokeColor`, `StrokeWidth`, `FillColor`, `LineDash` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapOverlay specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapPolyline

An open path connecting coordinates in order.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapOverlay.cs`
- Inheritance/shape: `class MapPolyline : MapOverlay`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.MapPolyline.#ctor(SkeleKit.Coordinate[])` | public (compiled) | n/a | n/a | n/a | An open path connecting coordinates in order. |
| Property | `SkeleKit.MapPolyline.Points` | public get/set | points | No | No automatic invalidation | The coordinates the line passes through, in order. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Points` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapPolyline specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapPolygon

A closed area bounded by coordinates.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapOverlay.cs`
- Inheritance/shape: `class MapPolygon : MapOverlay`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.MapPolygon.#ctor(SkeleKit.Coordinate[])` | public (compiled) | n/a | n/a | n/a | A closed area bounded by coordinates. |
| Property | `SkeleKit.MapPolygon.Points` | public get/set | points | No | No automatic invalidation | The boundary coordinates, in order. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Points` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapPolygon specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapCircle

A circular area of a fixed radius around a center.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapOverlay.cs`
- Inheritance/shape: `class MapCircle : MapOverlay`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.MapCircle.#ctor(SkeleKit.Coordinate,System.Double)` | public (compiled) | n/a | n/a | n/a | A circular area of a fixed radius around a center. |
| Property | `SkeleKit.MapCircle.Center` | public get/set | center | No | No automatic invalidation | The coordinate at the middle of the circle. |
| Property | `SkeleKit.MapCircle.RadiusMeters` | public get/set | radiusMeters | No | No automatic invalidation | The radius in meters. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Center`, `RadiusMeters` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapCircle specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapPin

A marker placed on a `MapView` at a coordinate.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapPin.cs`
- Inheritance/shape: `class MapPin`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.MapPin.#ctor(SkeleKit.Coordinate)` | public (compiled) | n/a | n/a | n/a | A marker placed on a `MapView` at a coordinate. |
| Property | `SkeleKit.MapPin.Coordinate` | public get/set | coordinate | No | No automatic invalidation | Where the pin sits. |
| Property | `SkeleKit.MapPin.Title` | public get/set | null | No | No automatic invalidation | The title shown in the pin's callout, or null for none. |
| Property | `SkeleKit.MapPin.Subtitle` | public get/set | null | No | No automatic invalidation | The subtitle shown under the title in the pin's callout, or null for none. |
| Property | `SkeleKit.MapPin.Symbol` | public get/set | null | No | No automatic invalidation | The SF Symbol drawn inside the marker, or null for the default dot. |
| Property | `SkeleKit.MapPin.Tint` | public get/set | null | No | No automatic invalidation | The marker's fill color, or null to follow the map tint. |
| Property | `SkeleKit.MapPin.Marker` | public get/set | null | No | No automatic invalidation | Builds a custom marker view, or null for the native marker styled by the properties above. Called when the marker comes on screen; return a fresh tree each time. |
| Property | `SkeleKit.MapPin.Callout` | public get/set | null | No | No automatic invalidation | Builds a custom callout view shown when the pin is tapped, or null for the native title and subtitle bubble. Called when the marker comes on screen; return a fresh tree each time. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Coordinate`, `Title`, `Subtitle`, `Symbol`, `Tint`, `Marker`, `Callout` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapPin specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MapRegion

A rectangular map extent, a center coordinate plus its span in degrees.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/MapRegion.cs`
- Inheritance/shape: `record MapRegion`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.MapRegion.#ctor(SkeleKit.Coordinate,System.Double,System.Double)` | public (compiled) | n/a | n/a | n/a | A rectangular map extent, a center coordinate plus its span in degrees. |
| Property | `SkeleKit.MapRegion.Center` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The coordinate at the middle of the extent. |
| Property | `SkeleKit.MapRegion.LatitudeSpan` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The north-south height in degrees. |
| Property | `SkeleKit.MapRegion.LongitudeSpan` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The east-west width in degrees. |
| Method | `SkeleKit.MapRegion.FromRadius(SkeleKit.Coordinate,System.Double)` | public static | n/a | n/a | n/a | Builds a region spanning roughly the given radius in meters around a center. |
| Method | `SkeleKit.MapRegion.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapRegion.op_Inequality(SkeleKit.MapRegion,SkeleKit.MapRegion)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapRegion.op_Equality(SkeleKit.MapRegion,SkeleKit.MapRegion)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapRegion.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapRegion.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapRegion.Equals(SkeleKit.MapRegion)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.MapRegion.Deconstruct(SkeleKit.Coordinate@,System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Center`, `LatitudeSpan`, `LongitudeSpan` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MapRegion specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ScrollPosition

Where a scrolled-to item lands in the viewport.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/ScrollPosition.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ScrollPosition.Top` | public | n/a | n/a | n/a | At the top (or leading edge of a carousel). |
| Field/value | `SkeleKit.ScrollPosition.Center` | public | n/a | n/a | n/a | Centered. |
| Field/value | `SkeleKit.ScrollPosition.Bottom` | public | n/a | n/a | n/a | At the bottom (or trailing edge of a carousel). |
| Field/value | `SkeleKit.ScrollPosition.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ScrollPosition specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

