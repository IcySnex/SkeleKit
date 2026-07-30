# SecureField

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## SecureField

Secure-entry preset of `TextField`, masking input as it's typed.

- Source: `SkeleKit.iOS/Controls/SecureField.cs`
- Inheritance/shape: `class SecureField : TextField`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UITextField`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SecureField.RevealButton` | public get/set | false | No | Invalidates measure | Whether a trailing eye button toggles the masking of the entered text. Owns the trailing slot, so it wins over `TextField.TrailingIcon`. |
| Method | `SkeleKit.SecureField.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `RevealButton` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(SecureField specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```
