# TextEditor

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## TextEditor

A multi-line text input.

- Source: `SkeleKit.iOS/Controls/TextEditor.cs`
- Inheritance/shape: `class TextEditor : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UITextView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.TextEditor.Text` | public get/set | C# default | Yes | Invalidates measure | The current text. Two-way by default. |
| Property | `SkeleKit.TextEditor.ContentKind` | public get/set | C# default | No | Visual/interaction only | What the editor holds, so the system can offer autofill. |
| Property | `SkeleKit.TextEditor.Capitalization` | public get/set | Capitalization.Sentences | No | Visual/interaction only | When typing is automatically capitalized. |
| Property | `SkeleKit.TextEditor.Autocorrection` | public get/set | true | No | Visual/interaction only | Whether the keyboard autocorrects and spell-checks the input. |
| Property | `SkeleKit.TextEditor.KeyboardLook` | public get/set | KeyboardLook.Default | No | Visual/interaction only | The color scheme of the raised keyboard. |
| Property | `SkeleKit.TextEditor.KeyboardToolbar` | public get/set | C# default | No | Visual/interaction only | A bar above the raised keyboard with Done and optional previous/next arrows. |
| Property | `SkeleKit.TextEditor.KeyboardAccessory` | public get/set | C# default | No | Visual/interaction only | A custom view above the raised keyboard. Wins over `TextEditor.KeyboardToolbar`; one view per field. |
| Property | `SkeleKit.TextEditor.FontSize` | public get/set | 17 | Yes | Invalidates measure | Font size in points. |
| Property | `SkeleKit.TextEditor.FontWeight` | public get/set | FontWeight.Regular | No | Visual/interaction only | The weight the text is drawn at. |
| Property | `SkeleKit.TextEditor.FontDesign` | public get/set | C# default | No | Visual/interaction only | The system font design the text uses. |
| Property | `SkeleKit.TextEditor.TextChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the text changes. |
| Method | `SkeleKit.TextEditor.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.TextEditor.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Text`, `ContentKind`, `Capitalization`, `Autocorrection`, `KeyboardLook`, `KeyboardToolbar`, `KeyboardAccessory`, `FontSize`, `FontWeight`, `FontDesign`, `TextChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(TextEditor specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

