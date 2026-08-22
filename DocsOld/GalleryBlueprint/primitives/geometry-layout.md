# Geometry and layout values

Classification: **Visual showcase + code-only reference**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## HorizontalAlignment

How a view is placed within the horizontal space its parent gives it.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Alignment.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.HorizontalAlignment.Stretch` | public | n/a | n/a | n/a | Fills the available width. |
| Field/value | `SkeleKit.HorizontalAlignment.Start` | public | n/a | n/a | n/a | Sized to content, pinned to the leading (left) edge. |
| Field/value | `SkeleKit.HorizontalAlignment.Center` | public | n/a | n/a | n/a | Sized to content, centered. |
| Field/value | `SkeleKit.HorizontalAlignment.End` | public | n/a | n/a | n/a | Sized to content, pinned to the trailing (right) edge. |
| Field/value | `SkeleKit.HorizontalAlignment.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(HorizontalAlignment specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## VerticalAlignment

How a view is placed within the vertical space its parent gives it.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Alignment.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.VerticalAlignment.Stretch` | public | n/a | n/a | n/a | Fills the available height. |
| Field/value | `SkeleKit.VerticalAlignment.Start` | public | n/a | n/a | n/a | Sized to content, pinned to the top edge. |
| Field/value | `SkeleKit.VerticalAlignment.Center` | public | n/a | n/a | n/a | Sized to content, centered. |
| Field/value | `SkeleKit.VerticalAlignment.End` | public | n/a | n/a | n/a | Sized to content, pinned to the bottom edge. |
| Field/value | `SkeleKit.VerticalAlignment.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(VerticalAlignment specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## GridUnitType

How a `GridLength` is interpreted by the grid layout.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/GridLength.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.GridUnitType.Auto` | public | n/a | n/a | n/a | Size to the content of the row or column (the largest child's desired size). |
| Field/value | `SkeleKit.GridUnitType.Pixel` | public | n/a | n/a | n/a | A fixed size in points. |
| Field/value | `SkeleKit.GridUnitType.Star` | public | n/a | n/a | n/a | A weighted share of the remaining space after Auto and Pixel tracks are placed. |
| Field/value | `SkeleKit.GridUnitType.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(GridUnitType specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## GridLength

The size of a grid row or column: absolute (points), auto-sized, or a weighted star share.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/GridLength.cs`
- Inheritance/shape: `record GridLength`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.GridLength.Auto` | public static readonly | n/a | n/a | n/a | An auto-sized track. |
| Field/value | `SkeleKit.GridLength.Star` | public static readonly | n/a | n/a | n/a | A single star track (weight 1). |
| Method | `SkeleKit.GridLength.Pixels(System.Double)` | public static | n/a | n/a | n/a | A fixed track of `points` points. |
| Method | `SkeleKit.GridLength.Stars(System.Double)` | public static | n/a | n/a | n/a | A star track with the given `weight` of the remaining space. |
| Method | `SkeleKit.GridLength.op_Implicit(System.Double)~SkeleKit.GridLength` | public static | n/a | n/a | n/a | A fixed track from a point value (so `Columns = { 200, GridLength.Star }` compiles). |
| Property | `SkeleKit.GridLength.Value` | public get | 0 | No | No automatic invalidation | Points for a pixel track, the weight for a star track, ignored for auto. |
| Property | `SkeleKit.GridLength.Type` | public get | C# default | No | No automatic invalidation | How `GridLength.Value` is interpreted. |
| Property | `SkeleKit.GridLength.IsAuto` | public get | false | No | No automatic invalidation | True for an `GridUnitType.Auto` track. |
| Property | `SkeleKit.GridLength.IsAbsolute` | public get | false | No | No automatic invalidation | True for a `GridUnitType.Pixel` track. |
| Property | `SkeleKit.GridLength.IsStar` | public get | false | No | No automatic invalidation | True for a `GridUnitType.Star` track. |
| Method | `SkeleKit.GridLength.op_Implicit(System.Double)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLength.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLength.op_Inequality(SkeleKit.GridLength,SkeleKit.GridLength)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLength.op_Equality(SkeleKit.GridLength,SkeleKit.GridLength)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLength.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLength.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLength.Equals(SkeleKit.GridLength)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Value`, `Type`, `IsAuto`, `IsAbsolute`, `IsStar` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(GridLength specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## GridLengthCollection

An observable list of `GridLength` values used by a `Grid`. Mutations automatically invalidate the owning grid's cached measurement.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/GridLengthCollection.cs`
- Inheritance/shape: `class GridLengthCollection : Collection<GridLength>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.GridLengthCollection.InsertItem(System.Int32,SkeleKit.GridLength)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLengthCollection.SetItem(System.Int32,SkeleKit.GridLength)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLengthCollection.RemoveItem(System.Int32)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GridLengthCollection.ClearItems` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(GridLengthCollection specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Orientation

The stacking axis of a `StackPanel`.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Orientation.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.Orientation.Vertical` | public | n/a | n/a | n/a | Children stacked top to bottom. |
| Field/value | `SkeleKit.Orientation.Horizontal` | public | n/a | n/a | n/a | Children laid out leading to trailing. |
| Field/value | `SkeleKit.Orientation.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Orientation specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Point

A point in the layout coordinate space (origin top-left, y grows downward).

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Point.cs`
- Inheritance/shape: `record Point`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Point.#ctor(System.Double,System.Double)` | public (compiled) | n/a | n/a | n/a | A point in the layout coordinate space (origin top-left, y grows downward). |
| Property | `SkeleKit.Point.X` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The horizontal coordinate. |
| Property | `SkeleKit.Point.Y` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The vertical coordinate. |
| Field/value | `SkeleKit.Point.Zero` | public static readonly | n/a | n/a | n/a | The origin (0, 0). |
| Method | `SkeleKit.Point.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Point.op_Inequality(SkeleKit.Point,SkeleKit.Point)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Point.op_Equality(SkeleKit.Point,SkeleKit.Point)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Point.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Point.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Point.Equals(SkeleKit.Point)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Point.Deconstruct(System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `X`, `Y` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Point specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Rect

An axis-aligned rectangle of a location and a size, produced by the arrangement pass.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Rect.cs`
- Inheritance/shape: `record Rect`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Rect.#ctor(System.Double,System.Double,System.Double,System.Double)` | public | n/a | n/a | n/a | An axis-aligned rectangle of a location and a size, produced by the arrangement pass. |
| Property | `SkeleKit.Rect.X` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The left edge. |
| Property | `SkeleKit.Rect.Y` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The top edge. |
| Property | `SkeleKit.Rect.Width` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The horizontal extent. |
| Property | `SkeleKit.Rect.Height` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The vertical extent. |
| Method | `SkeleKit.Rect.#ctor(SkeleKit.Point,SkeleKit.Size)` | public | n/a | n/a | n/a | Creates a rectangle from a location and a size. |
| Field/value | `SkeleKit.Rect.Zero` | public static readonly | n/a | n/a | n/a | A rectangle at the origin with zero size. |
| Property | `SkeleKit.Rect.Left` | public get | 0 | No | No automatic invalidation | The left edge (`Rect.X`). |
| Property | `SkeleKit.Rect.Top` | public get | 0 | No | No automatic invalidation | The top edge (`Rect.Y`). |
| Property | `SkeleKit.Rect.Right` | public get | 0 | No | No automatic invalidation | The right edge (`Rect.X` + `Rect.Width`). |
| Property | `SkeleKit.Rect.Bottom` | public get | 0 | No | No automatic invalidation | The bottom edge (`Rect.Y` + `Rect.Height`). |
| Property | `SkeleKit.Rect.Location` | public get | C# default | No | No automatic invalidation | The top-left corner. |
| Property | `SkeleKit.Rect.Size` | public get | C# default | No | No automatic invalidation | The width/height of the rectangle. |
| Method | `SkeleKit.Rect.Deflate(SkeleKit.Thickness)` | public | n/a | n/a | n/a | Returns this rectangle inset by `thickness`, clamped so size never goes negative. |
| Method | `SkeleKit.Rect.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Rect.op_Inequality(SkeleKit.Rect,SkeleKit.Rect)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Rect.op_Equality(SkeleKit.Rect,SkeleKit.Rect)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Rect.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Rect.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Rect.Equals(SkeleKit.Rect)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Rect.Deconstruct(System.Double@,System.Double@,System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `X`, `Y`, `Width`, `Height`, `Left`, `Top`, `Right`, `Bottom`, `Location`, `Size` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Rect specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## SafeAreaEdges

The edges of a view that should be inset by the safe area during arrange.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/SafeAreaEdges.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.SafeAreaEdges.None` | public | n/a | n/a | n/a | Ignore the safe area on all edges. |
| Field/value | `SkeleKit.SafeAreaEdges.Top` | public | n/a | n/a | n/a | Inset the top edge. |
| Field/value | `SkeleKit.SafeAreaEdges.Bottom` | public | n/a | n/a | n/a | Inset the bottom edge. |
| Field/value | `SkeleKit.SafeAreaEdges.Leading` | public | n/a | n/a | n/a | Inset the leading (left) edge. |
| Field/value | `SkeleKit.SafeAreaEdges.Trailing` | public | n/a | n/a | n/a | Inset the trailing (right) edge. |
| Field/value | `SkeleKit.SafeAreaEdges.All` | public | n/a | n/a | n/a | Inset all edges. |
| Field/value | `SkeleKit.SafeAreaEdges.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(SafeAreaEdges specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Size

A width and height pair used by the measure/arrange layout.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Size.cs`
- Inheritance/shape: `record Size`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Size.#ctor(System.Double,System.Double)` | public (compiled) | n/a | n/a | n/a | A width and height pair used by the measure/arrange layout. |
| Property | `SkeleKit.Size.Width` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The horizontal extent. |
| Property | `SkeleKit.Size.Height` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The vertical extent. |
| Field/value | `SkeleKit.Size.Zero` | public static readonly | n/a | n/a | n/a | A size of zero width and height. |
| Field/value | `SkeleKit.Size.Infinity` | public static readonly | n/a | n/a | n/a | A size unconstrained on both axes, used when measuring content that may grow without bound. |
| Property | `SkeleKit.Size.IsFinite` | public get | false | No | No automatic invalidation | True when both axes are finite (neither infinite nor NaN). |
| Method | `SkeleKit.Size.Deflate(SkeleKit.Thickness)` | public | n/a | n/a | n/a | Returns this size shrunk by `thickness` on both axes, clamped at zero. |
| Method | `SkeleKit.Size.Inflate(SkeleKit.Thickness)` | public | n/a | n/a | n/a | Returns this size grown by `thickness` on both axes. |
| Method | `SkeleKit.Size.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Size.op_Inequality(SkeleKit.Size,SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Size.op_Equality(SkeleKit.Size,SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Size.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Size.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Size.Equals(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Size.Deconstruct(System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Width`, `Height`, `IsFinite` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Size specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Stretch

How content is scaled to fill the space available to it.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Stretch.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.Stretch.None` | public | n/a | n/a | n/a | Content keeps its natural size and is centered. |
| Field/value | `SkeleKit.Stretch.Fill` | public | n/a | n/a | n/a | Content is stretched on both axes to fill, ignoring aspect ratio. |
| Field/value | `SkeleKit.Stretch.Uniform` | public | n/a | n/a | n/a | Content is scaled to fit while preserving aspect ratio (letterboxed). |
| Field/value | `SkeleKit.Stretch.UniformToFill` | public | n/a | n/a | n/a | Content is scaled to fill while preserving aspect ratio (cropped). |
| Field/value | `SkeleKit.Stretch.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Stretch specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Thickness

The thickness of a frame around a rectangle, as used for margins and padding.

- Source: `Source/Framework/SkeleKit.iOS/Primitives/Thickness.cs`
- Inheritance/shape: `record Thickness`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Thickness.#ctor(System.Double,System.Double,System.Double,System.Double)` | public | n/a | n/a | n/a | The thickness of a frame around a rectangle, as used for margins and padding. |
| Property | `SkeleKit.Thickness.Left` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The thickness on the left side. |
| Property | `SkeleKit.Thickness.Top` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The thickness on the top side. |
| Property | `SkeleKit.Thickness.Right` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The thickness on the right side. |
| Property | `SkeleKit.Thickness.Bottom` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The thickness on the bottom side. |
| Method | `SkeleKit.Thickness.#ctor(System.Double)` | public | n/a | n/a | n/a | Creates a uniform thickness: all four sides use the same value. |
| Method | `SkeleKit.Thickness.#ctor(System.Double,System.Double)` | public | n/a | n/a | n/a | Creates a symmetric thickness: `horizontal` for left/right, `vertical` for top/bottom. |
| Field/value | `SkeleKit.Thickness.Zero` | public static readonly | n/a | n/a | n/a | A thickness of zero on all sides. |
| Method | `SkeleKit.Thickness.op_Implicit(System.Double)~SkeleKit.Thickness` | public static | n/a | n/a | n/a | Creates a uniform thickness from a single numeric value. |
| Property | `SkeleKit.Thickness.Horizontal` | public get | 0 | No | No automatic invalidation | The total thickness on the horizontal axis (`Thickness.Left` + `Thickness.Right`). |
| Property | `SkeleKit.Thickness.Vertical` | public get | 0 | No | No automatic invalidation | The total thickness on the vertical axis (`Thickness.Top` + `Thickness.Bottom`). |
| Method | `SkeleKit.Thickness.op_Implicit(System.Double)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.op_Inequality(SkeleKit.Thickness,SkeleKit.Thickness)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.op_Equality(SkeleKit.Thickness,SkeleKit.Thickness)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.Equals(SkeleKit.Thickness)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Thickness.Deconstruct(System.Double@,System.Double@,System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Left`, `Top`, `Right`, `Bottom`, `Horizontal`, `Vertical` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Thickness specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

