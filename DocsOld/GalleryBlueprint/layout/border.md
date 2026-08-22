# Border

Classification: **Visual showcase**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Border

Wraps a single child with padding and an optional stroke; also the generic padding container.

- Source: `Source/Framework/SkeleKit.iOS/Layout/Border.iOS.cs`
- Inheritance/shape: `class Border`
- Inherited API: [`View`](../shared/view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Border.Stroke` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The stroke color, or null (default) for no stroke. |
| Property | `SkeleKit.Border.StrokeThickness` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The stroke width in points. Also insets the child so the stroke never overlaps content. |
| Property | `SkeleKit.Border.Child` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The single wrapped child. |
| Method | `SkeleKit.Border.MeasureOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Border.ArrangeOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Border.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Stroke`, `StrokeThickness`, `Child` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Border specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

