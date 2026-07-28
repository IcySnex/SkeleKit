# Colors and brushes

Classification: **Visual showcase**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Brush

How a view's background is filled: a solid color, a gradient, or a blurred material.

- Source: `SkeleKit.iOS/Primitives/Brush.cs`
- Inheritance/shape: `class Brush`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Brush.op_Implicit(SkeleKit.Color)~SkeleKit.Brush` | public static | n/a | n/a | n/a | Fills with a solid color. |
| Method | `SkeleKit.Brush.op_Implicit(SkeleKit.Color)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Brush specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## SolidBrush

A single flat color.

- Source: `SkeleKit.iOS/Primitives/Brush.cs`
- Inheritance/shape: `class SolidBrush : Brush`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.SolidBrush.#ctor(SkeleKit.Color)` | public (compiled) | n/a | n/a | n/a | A single flat color. |
| Property | `SkeleKit.SolidBrush.Color` | public get | color | No | No automatic invalidation | The color painted. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Color` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(SolidBrush specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## GradientStop

One color of a gradient, at a position along it.

- Source: `SkeleKit.iOS/Primitives/Brush.cs`
- Inheritance/shape: `record GradientStop`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.GradientStop.#ctor(SkeleKit.Color,System.Double)` | public (compiled) | n/a | n/a | n/a | One color of a gradient, at a position along it. |
| Property | `SkeleKit.GradientStop.Color` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The color at this step. |
| Property | `SkeleKit.GradientStop.Offset` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The relative position from 0.0 to 1.0 along the axis. |
| Method | `SkeleKit.GradientStop.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GradientStop.op_Inequality(SkeleKit.GradientStop,SkeleKit.GradientStop)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GradientStop.op_Equality(SkeleKit.GradientStop,SkeleKit.GradientStop)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GradientStop.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GradientStop.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GradientStop.Equals(SkeleKit.GradientStop)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.GradientStop.Deconstruct(SkeleKit.Color@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Color`, `Offset` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(GradientStop specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## LinearGradient

A linear gradient between two points, given in unit space: (0,0) is the top-left corner, (1,1) the bottom-right.

- Source: `SkeleKit.iOS/Primitives/Brush.cs`
- Inheritance/shape: `class LinearGradient : Brush`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.LinearGradient.Stops` | public get/init | [] | No | No automatic invalidation | The colors and where they sit along the gradient. |
| Property | `SkeleKit.LinearGradient.Start` | public get/init | new(0.5, 0) | No | No automatic invalidation | Where the gradient starts, in unit space. Top-center by default. |
| Property | `SkeleKit.LinearGradient.End` | public get/init | new(0.5, 1) | No | No automatic invalidation | Where the gradient ends, in unit space. Bottom-center by default. |
| Method | `SkeleKit.LinearGradient.Vertical(SkeleKit.Color[])` | public static | n/a | n/a | n/a | A top-to-bottom gradient through evenly spaced colors. |
| Method | `SkeleKit.LinearGradient.Horizontal(SkeleKit.Color[])` | public static | n/a | n/a | n/a | A leading-to-trailing gradient through evenly spaced colors. |
| Method | `SkeleKit.LinearGradient.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Stops`, `Start`, `End` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(LinearGradient specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## MaterialKind

The thickness of a `Material`, mapping to the system blur styles.

- Source: `SkeleKit.iOS/Primitives/Brush.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.MaterialKind.UltraThin` | public | n/a | n/a | n/a | The thinnest material; most of the content behind shows through. |
| Field/value | `SkeleKit.MaterialKind.Thin` | public | n/a | n/a | n/a | A thin material. |
| Field/value | `SkeleKit.MaterialKind.Regular` | public | n/a | n/a | n/a | The default material, as used behind sheets. |
| Field/value | `SkeleKit.MaterialKind.Thick` | public | n/a | n/a | n/a | A thick, mostly opaque material. |
| Field/value | `SkeleKit.MaterialKind.Chrome` | public | n/a | n/a | n/a | The material used behind bars and toolbars. |
| Field/value | `SkeleKit.MaterialKind.Glass` | public | n/a | n/a | n/a | The Liquid Glass surface; touches light it up. Renders as Chrome before iOS 26. |
| Field/value | `SkeleKit.MaterialKind.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(MaterialKind specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Material

A blurred material, as used behind bars and sheets. Thinner materials let more of the content behind them through.

- Source: `SkeleKit.iOS/Primitives/Brush.cs`
- Inheritance/shape: `class Material : Brush`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Material.#ctor(SkeleKit.MaterialKind)` | public (compiled) | n/a | n/a | n/a | A blurred material, as used behind bars and sheets. Thinner materials let more of the content behind them through. |
| Property | `SkeleKit.Material.Kind` | public get | kind | No | No automatic invalidation | How much the material blurs what sits behind it. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Kind` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Material specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Color

A straight (non-premultiplied) RGBA color with each channel in the range 0..1.

- Source: `SkeleKit.iOS/Primitives/Color.cs`
- Inheritance/shape: `record Color`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Color.#ctor(System.Double,System.Double,System.Double,System.Double)` | public | n/a | n/a | n/a | A straight (non-premultiplied) RGBA color with each channel in the range 0..1. |
| Field/value | `SkeleKit.Color.Transparent` | public static readonly | n/a | n/a | n/a | Fully transparent. |
| Method | `SkeleKit.Color.Dynamic(SkeleKit.Color,SkeleKit.Color)` | public static | n/a | n/a | n/a | A color that resolves per appearance: `light` normally, `dark` in dark mode. |
| Method | `SkeleKit.Color.FromBytes(System.Byte,System.Byte,System.Byte,System.Byte)` | public static | n/a | n/a | n/a | Creates a color from 8-bit channel values (0..255). |
| Method | `SkeleKit.Color.FromHex(System.UInt32)` | public static | n/a | n/a | n/a | Creates a color from a packed `0xRRGGBB` or `0xAARRGGBB` hex value. |
| Method | `SkeleKit.Color.#ctor(System.Double,System.Double,System.Double)` | public | n/a | n/a | n/a | Creates an opaque color (alpha 1). |
| Method | `SkeleKit.Color.WithAlpha(System.Double)` | public | n/a | n/a | n/a | Returns this color with a different `alpha` (0..1). A system color flattens to its light-mode value. |
| Property | `SkeleKit.Color.Red` | public/protected as emitted | implementation-defined; inspect source | No | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Property | `SkeleKit.Color.Green` | public/protected as emitted | implementation-defined; inspect source | No | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Property | `SkeleKit.Color.Blue` | public/protected as emitted | implementation-defined; inspect source | No | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Property | `SkeleKit.Color.Alpha` | public/protected as emitted | implementation-defined; inspect source | No | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.op_Inequality(SkeleKit.Color,SkeleKit.Color)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.op_Equality(SkeleKit.Color,SkeleKit.Color)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.Equals(SkeleKit.Color)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Color.Deconstruct(System.Double@,System.Double@,System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Red`, `Green`, `Blue`, `Alpha` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Color specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Colors

The standard colors, which also adapt to dark mode.

- Source: `SkeleKit.iOS/Primitives/Colors.cs`
- Inheritance/shape: `class Colors`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Colors.Transparent(SkeleKit.Color)` | public static | n/a | n/a | n/a | The given color made fully transparent. |
| Property | `SkeleKit.Colors.TransparentBlack` | public static get | C# default | No | No automatic invalidation | A transparent color based on black. |
| Property | `SkeleKit.Colors.TransparentWhite` | public static get | C# default | No | No automatic invalidation | A transparent color based on white. |
| Property | `SkeleKit.Colors.Black` | public static get | C# default | No | No automatic invalidation | The color black. |
| Property | `SkeleKit.Colors.White` | public static get | C# default | No | No automatic invalidation | The color white. |
| Property | `SkeleKit.Colors.Red` | public static get | C# default | No | No automatic invalidation | A system-defined red color. |
| Property | `SkeleKit.Colors.Orange` | public static get | C# default | No | No automatic invalidation | A system-defined orange color. |
| Property | `SkeleKit.Colors.Yellow` | public static get | C# default | No | No automatic invalidation | A system-defined yellow color. |
| Property | `SkeleKit.Colors.Green` | public static get | C# default | No | No automatic invalidation | A system-defined green color. |
| Property | `SkeleKit.Colors.Mint` | public static get | C# default | No | No automatic invalidation | A system-defined mint color. |
| Property | `SkeleKit.Colors.Teal` | public static get | C# default | No | No automatic invalidation | A system-defined teal color. |
| Property | `SkeleKit.Colors.Cyan` | public static get | C# default | No | No automatic invalidation | A system-defined cyan color. |
| Property | `SkeleKit.Colors.Blue` | public static get | C# default | No | No automatic invalidation | A system-defined blue color. |
| Property | `SkeleKit.Colors.Indigo` | public static get | C# default | No | No automatic invalidation | A system-defined indigo color. |
| Property | `SkeleKit.Colors.Purple` | public static get | C# default | No | No automatic invalidation | A system-defined purple color. |
| Property | `SkeleKit.Colors.Pink` | public static get | C# default | No | No automatic invalidation | A system-defined pink color. |
| Property | `SkeleKit.Colors.Brown` | public static get | C# default | No | No automatic invalidation | A system-defined brown color. |
| Property | `SkeleKit.Colors.Gray` | public static get | C# default | No | No automatic invalidation | A system-defined gray color. |
| Property | `SkeleKit.Colors.Gray2` | public static get | C# default | No | No automatic invalidation | A system-defined level 2 gray color. |
| Property | `SkeleKit.Colors.Gray3` | public static get | C# default | No | No automatic invalidation | A system-defined level 3 gray color. |
| Property | `SkeleKit.Colors.Gray4` | public static get | C# default | No | No automatic invalidation | A system-defined level 4 gray color. |
| Property | `SkeleKit.Colors.Gray5` | public static get | C# default | No | No automatic invalidation | A system-defined level 5 gray color. |
| Property | `SkeleKit.Colors.Gray6` | public static get | C# default | No | No automatic invalidation | A system-defined level 6 gray color. |
| Property | `SkeleKit.Colors.Label` | public static get | C# default | No | No automatic invalidation | Primary text. |
| Property | `SkeleKit.Colors.SecondaryLabel` | public static get | C# default | No | No automatic invalidation | Secondary text: subtitles, footnotes. |
| Property | `SkeleKit.Colors.TertiaryLabel` | public static get | C# default | No | No automatic invalidation | Tertiary text: disabled or placeholder-adjacent. |
| Property | `SkeleKit.Colors.PlaceholderText` | public static get | C# default | No | No automatic invalidation | Placeholder text in empty fields. |
| Property | `SkeleKit.Colors.Separator` | public static get | C# default | No | No automatic invalidation | Thin dividing lines. |
| Property | `SkeleKit.Colors.Link` | public static get | C# default | No | No automatic invalidation | Tappable link text. |
| Property | `SkeleKit.Colors.Background` | public static get | C# default | No | No automatic invalidation | The main page background. |
| Property | `SkeleKit.Colors.SecondaryBackground` | public static get | C# default | No | No automatic invalidation | Content layered on the main background (a card). |
| Property | `SkeleKit.Colors.TertiaryBackground` | public static get | C# default | No | No automatic invalidation | Content layered on a secondary background. |
| Property | `SkeleKit.Colors.GroupedBackground` | public static get | C# default | No | No automatic invalidation | The page background behind grouped lists (Settings). |
| Property | `SkeleKit.Colors.SecondaryGroupedBackground` | public static get | C# default | No | No automatic invalidation | A cell on a grouped background. |
| Property | `SkeleKit.Colors.TertiaryGroupedBackground` | public static get | C# default | No | No automatic invalidation | Content layered on a grouped cell. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `TransparentBlack`, `TransparentWhite`, `Black`, `White`, `Red`, `Orange`, `Yellow`, `Green`, `Mint`, `Teal`, `Cyan`, `Blue`, `Indigo`, `Purple`, `Pink`, `Brown`, `Gray`, `Gray2`, `Gray3`, `Gray4`, `Gray5`, `Gray6`, `Label`, `SecondaryLabel`, `TertiaryLabel`, `PlaceholderText`, `Separator`, `Link`, `Background`, `SecondaryBackground`, `TertiaryBackground`, `GroupedBackground`, `SecondaryGroupedBackground`, `TertiaryGroupedBackground` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void ShowcaseColors()
{
	Color specimen = Colors.Blue;
	_ = specimen;
}
```

## Shadow

A drop shadow behind a view.

- Source: `SkeleKit.iOS/Primitives/Shadow.cs`
- Inheritance/shape: `record Shadow`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Shadow.#ctor(System.Double,System.Double,System.Double,System.Double,System.Nullable{SkeleKit.Color})` | public | n/a | n/a | n/a | A drop shadow behind a view. |
| Property | `SkeleKit.Shadow.Opacity` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The shadow intensity from 0.0 (invisible) to 1.0 (fully opaque). |
| Property | `SkeleKit.Shadow.Radius` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The blur radius of the shadow edges. |
| Property | `SkeleKit.Shadow.OffsetX` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The horizontal displacement of the shadow. |
| Property | `SkeleKit.Shadow.OffsetY` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The vertical displacement of the shadow. |
| Property | `SkeleKit.Shadow.Color` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The color of the shadow, or null to use the system default. |
| Method | `SkeleKit.Shadow.#ctor(System.Double,System.Double,System.Double)` | public | n/a | n/a | n/a | A shadow offset straight down, in the default shadow color. |
| Method | `SkeleKit.Shadow.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Shadow.op_Inequality(SkeleKit.Shadow,SkeleKit.Shadow)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Shadow.op_Equality(SkeleKit.Shadow,SkeleKit.Shadow)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Shadow.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Shadow.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Shadow.Equals(SkeleKit.Shadow)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Shadow.Deconstruct(System.Double@,System.Double@,System.Double@,System.Double@,System.Nullable{SkeleKit.Color}@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Opacity`, `Radius`, `OffsetX`, `OffsetY`, `Color` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Shadow specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

