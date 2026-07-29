# TextView

Classification: **Shared visual reference**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## TextView

Read-only rich text that can be selected, with tappable `Link` runs.

- Source: `SkeleKit.iOS/Controls/TextView.cs`
- Inheritance/shape: `class TextView : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.TextView.Spans` | public get/set | C# default | No | Invalidates measure | The styled runs to display; a plain string becomes an unstyled run, a `Link` a tappable one. Changes re-render and animate nothing, since they replace the text. Live when the list is an `ObservableCollection`. |
| Property | `SkeleKit.TextView.IsSelectable` | public get/set | C# default | Yes | Visual/interaction only | Whether the text can be selected and copied. A `Link` run forces selection on, since UIKit only makes text items tappable while the view is selectable. |
| Property | `SkeleKit.TextView.TextStyle` | public get/set | C# default | No | Invalidates measure | The step of the native type hierarchy the text follows, or null to size it by `TextView.FontSize`. |
| Property | `SkeleKit.TextView.FontSize` | public get/set | double.NaN | No | Invalidates measure | Explicit font size in points, overriding `TextView.TextStyle`. NaN falls back to the text style, or 17 points without one. |
| Property | `SkeleKit.TextView.FontWeight` | public get/set | FontWeight.Regular | No | Invalidates measure | The base font weight the runs build on. |
| Property | `SkeleKit.TextView.FontDesign` | public get/set | FontDesign.Default | No | Invalidates measure | The base font design: system, rounded, serif or monospaced. |
| Property | `SkeleKit.TextView.TextColor` | public get/set | C# default | Yes | Visual/interaction only | Base text color, or null for the system label color. |
| Property | `SkeleKit.TextView.LinkColor` | public get/set | C# default | No | Visual/interaction only | Color the links paint in, or null for the app tint. |
| Property | `SkeleKit.TextView.MaxLines` | public get/set | C# default | No | Invalidates measure | Maximum number of lines, or 0 for unlimited (wraps freely). |
| Property | `SkeleKit.TextView.TextAlignment` | public get/set | Leading | No | Invalidates measure | Horizontal alignment of the text. |
| Property | `SkeleKit.TextView.LineSpacing` | public get/set | C# default | No | Invalidates measure | Extra points between lines. |
| Property | `SkeleKit.TextView.LetterSpacing` | public get/set | C# default | No | Invalidates measure | Extra points between characters (negative tightens). |
| Method | `SkeleKit.TextView.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Spans`, `IsSelectable`, `TextStyle`, `FontSize`, `FontWeight`, `FontDesign`, `TextColor`, `LinkColor`, `MaxLines`, `TextAlignment`, `LineSpacing`, `LetterSpacing` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(TextView specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```
