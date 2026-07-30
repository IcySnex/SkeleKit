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
| Property | `SkeleKit.TextEditor.FontWeight` | public get/set | FontWeight.Regular | No | Invalidates measure | The weight the text is drawn at. |
| Property | `SkeleKit.TextEditor.FontDesign` | public get/set | C# default | No | Invalidates measure | The system font design the text uses. |
| Property | `SkeleKit.TextEditor.TextChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the text changes. |
| Method | `SkeleKit.TextEditor.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.TextEditor.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Binding and live growth | `Text`, `TextChanged` | Edit a two-way value, observe every native change, and set or clear it from the ViewModel. Add and remove lines to verify that the editor remeasures with its content. |
| Keyboard behavior | `ContentKind`, `Capitalization`, `Autocorrection`, `KeyboardLook` | Change native text traits while focused and inspect autofill intent, capitalization, correction, spelling, and system/light/dark keyboard appearance. |
| Typography | `FontSize`, `FontWeight`, `FontDesign` | Adjust explicit size, every native weight, and all four system font designs while the editor remains editable and remeasures. |
| Keyboard accessories | `KeyboardToolbar`, `KeyboardAccessory` | Focus either of two editors and switch live among no accessory, Done, navigation, and a custom SkeleKit accessory. The custom view wins over the toolbar. |

```csharp
new TextEditor
{
	Text = Bind(
		model => model.Text,
		(model, value) => model.Text = value),
	ContentKind = ContentKind.None,
	Capitalization = Capitalization.Sentences,
	Autocorrection = true,
	KeyboardLook = KeyboardLook.Default,
	KeyboardToolbar = KeyboardToolbar.Done,
	FontSize = 17,
	FontWeight = FontWeight.Regular,
	FontDesign = FontDesign.Default,
	TextChanged = viewModel.RecordTextChanged
};
```
