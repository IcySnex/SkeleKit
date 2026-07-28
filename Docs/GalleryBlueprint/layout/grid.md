# Grid and grid extensions

Classification: **Visual showcase**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Grid

A grid placing children into cells of `Grid.Rows` and `Grid.Columns` (absolute, auto, or star).

- Source: `SkeleKit.iOS/Layout/Grid.cs`
- Inheritance/shape: `class Grid : Panel`
- Inherited API: [`View`](../shared/view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Grid.#ctor` | public | n/a | n/a | n/a | Creates an empty grid. |
| Property | `SkeleKit.Grid.Rows` | public get | empty `GridLengthCollection`; effective single star track | No | No automatic invalidation | The row definitions, top to bottom. Empty means a single star row. |
| Property | `SkeleKit.Grid.Columns` | public get | empty `GridLengthCollection`; effective single star track | No | No automatic invalidation | The column definitions, leading to trailing. Empty means a single star column. |
| Property | `SkeleKit.Grid.RowSpacing` | public get/set | 0 | No | Invalidates measure | The gap in points inserted between rows. |
| Property | `SkeleKit.Grid.ColumnSpacing` | public get/set | 0 | No | Invalidates measure | The gap in points inserted between columns. |
| Method | `SkeleKit.Grid.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Grid.ArrangeOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Rows`, `Columns`, `RowSpacing`, `ColumnSpacing` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Grid specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## GridExtensions

Fluent attached-property setters for placing a view inside a `Grid`.

- Source: `SkeleKit.iOS/Layout/GridExtensions.cs`
- Inheritance/shape: `class GridExtensions`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.GridExtensions.Row``1(``0,System.Int32)` | public (compiled) | n/a | n/a | n/a | _Undocumented in the XML baseline; see finding._ |
| Method | `SkeleKit.GridExtensions.Column``1(``0,System.Int32)` | public (compiled) | n/a | n/a | n/a | _Undocumented in the XML baseline; see finding._ |
| Method | `SkeleKit.GridExtensions.RowSpan``1(``0,System.Int32)` | public (compiled) | n/a | n/a | n/a | _Undocumented in the XML baseline; see finding._ |
| Method | `SkeleKit.GridExtensions.ColumnSpan``1(``0,System.Int32)` | public (compiled) | n/a | n/a | n/a | _Undocumented in the XML baseline; see finding._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static View ShowcaseGridPlacement(View specimen) =>
	specimen.Row(0).Column(0).RowSpan(1).ColumnSpan(1);
```

