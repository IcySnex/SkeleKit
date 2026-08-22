# ScrollView

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## ScrollView

A scrolling container for a single child.

- Source: `Source/Framework/SkeleKit.iOS/Layout/ScrollView.iOS.cs`
- Inheritance/shape: `class ScrollView`
- Inherited API: [`View`](../shared/view.md)
- Native counterpart: `UIScrollView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ScrollView.Orientation` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The scroll axis. |
| Property | `SkeleKit.ScrollView.AvoidsKeyboard` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the content is inset so the keyboard never covers the focused control. Only an overlapping keyboard is counted, an already-visible focused control keeps its scroll position, and bottom-anchored content follows the keyboard animation. |
| Property | `SkeleKit.ScrollView.RefreshCommand` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Command invoked when the user pulls to refresh. Setting it enables the refresh control. |
| Property | `SkeleKit.ScrollView.IsRefreshing` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the refresh spinner is showing. Two-way: the pull sets it true, the ViewModel sets it false when done. |
| Property | `SkeleKit.ScrollView.Scrolled` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked as the view scrolls, with the offset in points. |
| Property | `SkeleKit.ScrollView.KeyboardDismiss` | public/protected as emitted | implementation-defined; inspect source | No | n/a | How dragging the scroll view dismisses the keyboard. |
| Property | `SkeleKit.ScrollView.Paging` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether scrolling snaps to whole viewport pages. |
| Property | `SkeleKit.ScrollView.ShowsIndicator` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the scroll indicator is shown. |
| Property | `SkeleKit.ScrollView.IndicatorStyle` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The color of the scroll indicator. |
| Property | `SkeleKit.ScrollView.IndicatorInsets` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Insets the scroll indicator from the edges, or null to track the content insets. |
| Property | `SkeleKit.ScrollView.Content` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The single scrollable child. |
| Method | `SkeleKit.ScrollView.ScrollTo(System.Double,System.Boolean)` | public (compiled) | n/a | n/a | n/a | Scrolls to an offset along the scroll axis, in points. |
| Method | `SkeleKit.ScrollView.MeasureOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ScrollView.ArrangeOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ScrollView.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Orientation`, `AvoidsKeyboard`, `RefreshCommand`, `IsRefreshing`, `Scrolled`, `KeyboardDismiss`, `Paging`, `ShowsIndicator`, `IndicatorStyle`, `IndicatorInsets`, `Content` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ScrollView specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```
