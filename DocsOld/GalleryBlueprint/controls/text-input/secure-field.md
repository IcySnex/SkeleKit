# SecureField

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## SecureField

Secure-entry preset of `TextField`, masking input as it's typed.

- Source: `Source/Framework/SkeleKit.iOS/Controls/SecureField.cs`
- Inheritance/shape: `class SecureField : TextField`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UITextField`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SecureField.RevealButton` | public get/set | false | No | Invalidates measure | Whether a trailing eye button toggles the masking of the entered text. Owns the trailing slot, so it wins over `TextField.TrailingIcon`. Toggling preserves focus; turning it off restores masking. |
| Method | `SkeleKit.SecureField.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Secure entry | `RevealButton`; inherited `Text`, `LeadingIcon`, `ContentKind`, `ReturnKey`, `RequiresText`, `SubmitCommand` | Edit a two-way new-password value and inspect masked, revealed, empty, populated, focused, and submitted states. Show deterministic strength feedback without displaying the password. |
| Password intent and trailing slot | `RevealButton`; inherited `ContentKind`, `TrailingIcon`, `ClearButton` | Switch among current-password, new-password, and no autofill intent. Toggle all three trailing controls and verify that reveal wins over a decorative icon, which wins over the clear button. |

```csharp
new SecureField
{
	Text = Bind(
		model => model.Text,
		(model, value) => model.Text = value),
	Placeholder = "Create a password",
	LeadingIcon = ImageSource.Symbol("lock.fill"),
	RevealButton = true,
	ContentKind = ContentKind.NewPassword,
	ReturnKey = ReturnKeyType.Done,
	RequiresText = true,
	SubmitCommand = viewModel.SubmitCommand
};
```
