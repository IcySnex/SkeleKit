# TextField

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## TextField

A single-line text input.

- Source: `SkeleKit.iOS/Controls/TextField.cs`
- Inheritance/shape: `class TextField : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UITextField`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.TextField.Text` | public get/set | C# default | Yes | Invalidates measure | The current text. Two-way by default. |
| Property | `SkeleKit.TextField.Placeholder` | public get/set | C# default | Yes | Invalidates measure | Placeholder text shown when empty. |
| Property | `SkeleKit.TextField.LeadingIcon` | public get/set | null | No | Invalidates measure | A decorative symbol or bundle icon shown before the text, or null for none. |
| Property | `SkeleKit.TextField.TrailingIcon` | public get/set | null | No | Invalidates measure | A decorative symbol or bundle icon shown after the text, or null for none. Shares the trailing slot with `TextField.ClearButton`, so an icon hides the clear button. |
| Property | `SkeleKit.TextField.Keyboard` | public get/set | KeyboardType.Default | No | Visual/interaction only | Which on-screen keyboard to show while editing. |
| Property | `SkeleKit.TextField.ReturnKey` | public get/set | ReturnKeyType.Default | No | Visual/interaction only | The label shown on the keyboard's return key. |
| Property | `SkeleKit.TextField.ContentKind` | public get/set | C# default | No | Visual/interaction only | What the field holds, so the system can offer autofill (passwords, one-time codes, contacts). |
| Property | `SkeleKit.TextField.Capitalization` | public get/set | Capitalization.Sentences | No | Visual/interaction only | When typing is automatically capitalized. |
| Property | `SkeleKit.TextField.Autocorrection` | public get/set | true | No | Visual/interaction only | Whether the keyboard autocorrects and spell-checks the input. |
| Property | `SkeleKit.TextField.ClearButton` | public get/set | C# default | No | Invalidates measure | When the field shows its built-in clear button. Clearing preserves focus and updates `Text` and `TextChanged`. |
| Property | `SkeleKit.TextField.RequiresText` | public get/set | C# default | No | Visual/interaction only | Whether the return key is disabled while the field is empty. |
| Property | `SkeleKit.TextField.KeyboardLook` | public get/set | KeyboardLook.Default | No | Visual/interaction only | The color scheme of the raised keyboard. |
| Property | `SkeleKit.TextField.KeyboardToolbar` | public get/set | C# default | No | Visual/interaction only | A bar above the raised keyboard with Done and optional previous/next arrows. |
| Property | `SkeleKit.TextField.KeyboardAccessory` | public get/set | C# default | No | Visual/interaction only | A custom view above the raised keyboard. Wins over `TextField.KeyboardToolbar`; one view per field. |
| Property | `SkeleKit.TextField.FontSize` | public get/set | 17 | Yes | Invalidates measure | Font size in points. |
| Property | `SkeleKit.TextField.FontWeight` | public get/set | FontWeight.Regular | No | Invalidates measure | The weight the text is drawn at. |
| Property | `SkeleKit.TextField.FontDesign` | public get/set | C# default | No | Invalidates measure | The system font design the text uses. |
| Property | `SkeleKit.TextField.TextChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the text changes. |
| Property | `SkeleKit.TextField.SubmitCommand` | public get/set | null | No | No automatic invalidation | Command invoked when the user taps the keyboard's return key. |
| Method | `SkeleKit.TextField.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Binding and submission | `Text`, `Placeholder`, `LeadingIcon`, `ClearButton`, `ReturnKey`, `ContentKind`, `Capitalization`, `Autocorrection`, `RequiresText`, `TextChanged`, `SubmitCommand` | Edit an email field and verify its two-way ViewModel value and `TextChanged` status update on every native edit. Set and clear the value from the ViewModel. Toggle the required-text behavior and verify the Send key is disabled only while empty. Submit through the keyboard and verify the command result. |

```csharp
new TextField
{
	Text = Bind(
		model => model.Text,
		(model, value) => model.Text = value),
	Placeholder = "name@example.com",
	LeadingIcon = ImageSource.Symbol("envelope"),
	ClearButton = ClearButton.WhileEditing,
	Keyboard = KeyboardType.Email,
	ReturnKey = ReturnKeyType.Send,
	ContentKind = ContentKind.Email,
	Capitalization = Capitalization.None,
	Autocorrection = false,
	RequiresText = true,
	TextChanged = viewModel.RecordTextChanged,
	SubmitCommand = viewModel.SubmitCommand
};
```

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Keyboard behavior | `Keyboard`, `ReturnKey`, `ContentKind`, `Capitalization`, `Autocorrection`, `KeyboardLook` | Keep the field focused while selecting every keyboard type, return-key label, autofill kind, capitalization mode, correction state, and keyboard appearance. The raised keyboard refreshes immediately where UIKit exposes a visible distinction; autofill suggestions remain dependent on device data and context. |

```csharp
new TextField
{
	Placeholder = "Tap to inspect the keyboard",
	Keyboard = KeyboardType.Email,
	ReturnKey = ReturnKeyType.Send,
	ContentKind = ContentKind.Email,
	Capitalization = Capitalization.None,
	Autocorrection = false,
	KeyboardLook = KeyboardLook.Default
};
```

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Chrome and typography | `Text`, `LeadingIcon`, `TrailingIcon`, `ClearButton`, `FontSize`, `FontWeight`, `FontDesign` | Toggle the leading symbol and switch the shared trailing slot among a clear button, decorative icon, and empty state. In clear mode inspect Never, While Editing, Unless Editing, and Always. Adjust explicit size, every native weight, and all four font designs. A trailing icon intentionally suppresses the clear button. |

```csharp
new TextField
{
	Text = "SkeleKit",
	LeadingIcon = ImageSource.Symbol("character.cursor.ibeam"),
	TrailingIcon = ImageSource.Symbol("checkmark.circle.fill"),
	ClearButton = ClearButton.Never,
	FontSize = 20,
	FontWeight = FontWeight.Regular,
	FontDesign = FontDesign.Rounded
};
```

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Keyboard accessories | `KeyboardToolbar`, `KeyboardAccessory` | Focus each of three fields and switch live among no accessory, Done, navigation, and a custom SkeleKit accessory. Done dismisses the keyboard, navigation moves through page inputs in document order, and the custom accessory overrides the toolbar with a trailing Liquid Glass dismissal capsule on supported systems. |

```csharp
TextField field = new()
{
	KeyboardToolbar = KeyboardToolbar.Navigation
};

field.KeyboardAccessory = new Grid
{
	Padding = new(8, 6),
	Columns =
	{
		GridLength.Star,
		GridLength.Auto
	},
	Children =
	{
		new Button
		{
			Text = "Done",
			Icon = "keyboard.chevron.compact.down",
			Kind = ButtonStyle.Glass,
			Command = Command.From(field.Unfocus)
		}.Column(1)
	}
};
```
