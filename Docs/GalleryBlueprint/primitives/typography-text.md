# Typography and text input

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## KeyboardDismiss

How scrolling dismisses the on-screen keyboard.

- Source: `SkeleKit.iOS/Primitives/KeyboardDismiss.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UIScrollView.KeyboardDismissMode`
- Gallery role: Interactive lab in the ScrollView showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.KeyboardDismiss.None` | public | n/a | n/a | n/a | Scrolling never dismisses the keyboard. |
| Field/value | `SkeleKit.KeyboardDismiss.OnDrag` | public | n/a | n/a | n/a | The keyboard is dismissed as soon as a drag starts. |
| Field/value | `SkeleKit.KeyboardDismiss.Interactive` | public | n/a | n/a | n/a | The keyboard follows the drag and can be pulled away. |
| Field/value | `SkeleKit.KeyboardDismiss.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(KeyboardDismiss specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## KeyboardLook

The color scheme of the keyboard raised by a text input.

- Source: `SkeleKit.iOS/Primitives/KeyboardLook.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UIKeyboardAppearance`
- Gallery role: Interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.KeyboardLook.Default` | public | n/a | n/a | n/a | Follows the system appearance. |
| Field/value | `SkeleKit.KeyboardLook.Light` | public | n/a | n/a | n/a | Always light, whatever the system appearance. |
| Field/value | `SkeleKit.KeyboardLook.Dark` | public | n/a | n/a | n/a | Always dark, whatever the system appearance. |
| Field/value | `SkeleKit.KeyboardLook.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField keyboard behavior | `Default`, `Light`, `Dark` | Keep a text field focused while selecting each appearance. Default follows the effective system appearance; Light and Dark override the raised keyboard immediately. |

```csharp
new TextField
{
	KeyboardLook = KeyboardLook.Dark
};
```

## KeyboardToolbar

The bar shown above the raised keyboard.

- Source: `SkeleKit.iOS/Primitives/KeyboardToolbar.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UIToolbar` assigned as an input accessory
- Gallery role: Interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.KeyboardToolbar.None` | public | n/a | n/a | n/a | No bar. |
| Field/value | `SkeleKit.KeyboardToolbar.Done` | public | n/a | n/a | n/a | A Done button that dismisses the keyboard. |
| Field/value | `SkeleKit.KeyboardToolbar.Navigation` | public | n/a | n/a | n/a | Previous/next arrows that move focus between inputs, plus Done. |
| Field/value | `SkeleKit.KeyboardToolbar.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField keyboard accessories | `None`, `Done`, `Navigation` | Change modes while a field is focused. None removes the accessory, Done adds a dismissal button, and Navigation adds previous/next focus actions plus Done. Compare all three with a custom `KeyboardAccessory`, which takes precedence. |

```csharp
new TextField
{
	KeyboardToolbar = KeyboardToolbar.Navigation
};
```

## KeyboardType

The on-screen keyboard shown while editing a text input.

- Source: `SkeleKit.iOS/Primitives/KeyboardType.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UIKeyboardType`
- Gallery role: Interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.KeyboardType.Default` | public | n/a | n/a | n/a | The standard keyboard. |
| Field/value | `SkeleKit.KeyboardType.Numeric` | public | n/a | n/a | n/a | A numeric keypad (digits only). |
| Field/value | `SkeleKit.KeyboardType.Decimal` | public | n/a | n/a | n/a | A numeric keypad with a decimal point. |
| Field/value | `SkeleKit.KeyboardType.Phone` | public | n/a | n/a | n/a | A keypad for entering phone numbers. |
| Field/value | `SkeleKit.KeyboardType.Email` | public | n/a | n/a | n/a | A keyboard optimized for entering email addresses. |
| Field/value | `SkeleKit.KeyboardType.Url` | public | n/a | n/a | n/a | A keyboard optimized for entering URLs. |
| Field/value | `SkeleKit.KeyboardType.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField keyboard behavior | `Default`, `Numeric`, `Decimal`, `Phone`, `Email`, `Url` | Keep a field focused while selecting every keyboard. The raised keyboard immediately changes its native key layout; hardware keyboards may hide the visible distinction. |

```csharp
new TextField
{
	Keyboard = KeyboardType.Email
};
```

## Link

A tappable run of text inside a `TextView`'s `TextView.Spans`.

- Source: `SkeleKit.iOS/Primitives/Link.cs`
- Inheritance/shape: `class Link : Span`
- Native counterpart: link attributes on a `UITextView` text item
- Gallery role: Interactive lab in the TextView showcase.
- Behavior note: It renders like a `Span` but fires `Link.Command` when tapped and shows `Link.ContextMenu` as a native hold-to-peek menu. Inside a plain `Label` it is styled text only: the command and menu are ignored, since a `Label` is not interactive.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Link.#ctor(System.String)` | public | n/a | n/a | n/a | Creates a link. |
| Property | `SkeleKit.Link.Command` | public get/set | null | No | No automatic invalidation | The command run when the link is tapped. |
| Property | `SkeleKit.Link.CommandParameter` | public get/set | null | No | No automatic invalidation | The parameter passed to `Link.Command`. |
| Property | `SkeleKit.Link.ContextMenu` | public get | [] | No | No automatic invalidation | Entries shown in the link's long-press peek menu, or empty for a plain tappable link. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextView selection and links | `Command`, `CommandParameter`, `ContextMenu` | Tap links with distinct command parameters and verify the callback result. Hold the documentation link to open a native menu, then select its icon-bearing actions and verify their commands and parameters. The same `Link` inside a `Label` remains visual text and does not expose these interactions. |

```csharp
Link documentation = new("documentation")
{
	Command = viewModel.OpenLinkCommand,
	CommandParameter = "Documentation"
};
documentation.ContextMenu.Add(new()
{
	Text = "Open",
	Icon = "arrow.up.forward",
	Command = viewModel.RunMenuActionCommand,
	CommandParameter = "Open"
});

new TextView
{
	Spans =
	[
		"Read the ",
		documentation
	]
};
```

## ReturnKeyType

The label shown on the keyboard's return key.

- Source: `SkeleKit.iOS/Primitives/ReturnKeyType.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UIReturnKeyType`
- Gallery role: Interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ReturnKeyType.Default` | public | n/a | n/a | n/a | The standard "return" label. |
| Field/value | `SkeleKit.ReturnKeyType.Go` | public | n/a | n/a | n/a | "Go". |
| Field/value | `SkeleKit.ReturnKeyType.Next` | public | n/a | n/a | n/a | "Next". |
| Field/value | `SkeleKit.ReturnKeyType.Search` | public | n/a | n/a | n/a | "Search". |
| Field/value | `SkeleKit.ReturnKeyType.Send` | public | n/a | n/a | n/a | "Send". |
| Field/value | `SkeleKit.ReturnKeyType.Done` | public | n/a | n/a | n/a | "Done". |
| Field/value | `SkeleKit.ReturnKeyType.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField keyboard behavior | `Default`, `Go`, `Next`, `Search`, `Send`, `Done` | Keep a field focused while selecting every return-key type and verify its native label updates. The binding-and-submission lab uses Send to invoke `SubmitCommand`. |

```csharp
new TextField
{
	ReturnKey = ReturnKeyType.Send
};
```

## Span

A styled run of text inside a `Label`'s `Label.Spans`.

- Source: `SkeleKit.iOS/Primitives/Span.cs`
- Inheritance/shape: `class Span`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.
- Behavior note: Every unset visual property follows the label; a set one overrides it for this run alone.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Span.#ctor(System.String)` | public | n/a | n/a | n/a | Creates a span. |
| Method | `SkeleKit.Span.op_Implicit(System.String)~SkeleKit.Span` | public static | n/a | n/a | n/a | Wraps a plain string as an unstyled span, so string literals sit beside styled runs in a list. |
| Property | `SkeleKit.Span.Text` | public get/set | C# default | No | No automatic invalidation | The run's text. |
| Property | `SkeleKit.Span.Bold` | public get/set | false | No | No automatic invalidation | Shorthand for a bold `Span.FontWeight`. |
| Property | `SkeleKit.Span.FontWeight` | public get/set | null | No | No automatic invalidation | The run's font weight, or null to follow the label. |
| Property | `SkeleKit.Span.FontDesign` | public get/set | null | No | No automatic invalidation | The run's font design, or null to follow the label. |
| Property | `SkeleKit.Span.FontSize` | public get/set | double.NaN | No | No automatic invalidation | The run's font size in points, or NaN to follow the label. |
| Property | `SkeleKit.Span.TextColor` | public get/set | null | No | No automatic invalidation | The run's text color, or null to follow the label. |
| Property | `SkeleKit.Span.Underline` | public get/set | false | No | No automatic invalidation | Underlines the run. |
| Property | `SkeleKit.Span.Strikethrough` | public get/set | false | No | No automatic invalidation | Strikes the run through. |
| Method | `SkeleKit.Span.op_Implicit(System.String)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Label attributed text | `Text`, `Bold`, `FontWeight`, `FontDesign`, `FontSize`, `TextColor`, `Underline`, `Strikethrough` | Render all fields together inside the Label attributed-text lab. Unset fields inherit the owning label, string literals use the implicit conversion, and per-run settings remain distinct when whole-label spacing or decoration changes. |

```csharp
IReadOnlyList<Span> spans =
[
	"Mix ",
	new("weight") { Bold = true },
	new("color") { TextColor = Colors.Purple },
	new("design")
	{
		FontWeight = FontWeight.Light,
		FontDesign = FontDesign.Serif
	},
	new("Per-run styling")
	{
		FontSize = 22,
		Underline = true
	},
	new("uniform text") { Strikethrough = true }
];
```

## TextAlignment

Horizontal alignment of text within a control.

- Source: `SkeleKit.iOS/Primitives/TextAlignment.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.TextAlignment.Leading` | public | n/a | n/a | n/a | Aligned to the leading (left) edge. |
| Field/value | `SkeleKit.TextAlignment.Center` | public | n/a | n/a | n/a | Centered. |
| Field/value | `SkeleKit.TextAlignment.Trailing` | public | n/a | n/a | n/a | Aligned to the trailing (right) edge. |
| Field/value | `SkeleKit.TextAlignment.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(TextAlignment specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ContentKind

What the field holds, so the system can offer autofill (passwords, one-time codes, contacts).

- Source: `SkeleKit.iOS/Primitives/TextInput.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UITextContentType`
- Gallery role: Interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ContentKind.None` | public | n/a | n/a | n/a | No autofill hint. |
| Field/value | `SkeleKit.ContentKind.Username` | public | n/a | n/a | n/a | A login user name; the QuickType bar offers saved credentials. |
| Field/value | `SkeleKit.ContentKind.Password` | public | n/a | n/a | n/a | An existing password; offers the saved credential. |
| Field/value | `SkeleKit.ContentKind.NewPassword` | public | n/a | n/a | n/a | A password being created; the system suggests a strong one and saves it. |
| Field/value | `SkeleKit.ContentKind.OneTimeCode` | public | n/a | n/a | n/a | A one-time code; autofills from incoming messages. |
| Field/value | `SkeleKit.ContentKind.Email` | public | n/a | n/a | n/a | An email address. |
| Field/value | `SkeleKit.ContentKind.Name` | public | n/a | n/a | n/a | A person's full name. |
| Field/value | `SkeleKit.ContentKind.PhoneNumber` | public | n/a | n/a | n/a | A phone number. |
| Field/value | `SkeleKit.ContentKind.StreetAddress` | public | n/a | n/a | n/a | A street address. |
| Field/value | `SkeleKit.ContentKind.Url` | public | n/a | n/a | n/a | A web address. |
| Field/value | `SkeleKit.ContentKind.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField keyboard behavior | `None`, `Username`, `Password`, `NewPassword`, `OneTimeCode`, `Email`, `Name`, `PhoneNumber`, `StreetAddress`, `Url` | Select every autofill hint on a focused field. The property updates immediately; actual QuickType suggestions depend on matching device credentials, contacts, messages, and surrounding form context. |

```csharp
new TextField
{
	ContentKind = ContentKind.Email
};
```

## Capitalization

When typing is automatically capitalized.

- Source: `SkeleKit.iOS/Primitives/TextInput.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UITextAutocapitalizationType`
- Gallery role: Interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.Capitalization.Sentences` | public | n/a | n/a | n/a | The start of every sentence. The system default for plain text. |
| Field/value | `SkeleKit.Capitalization.None` | public | n/a | n/a | n/a | Never. |
| Field/value | `SkeleKit.Capitalization.Words` | public | n/a | n/a | n/a | The start of every word. |
| Field/value | `SkeleKit.Capitalization.Characters` | public | n/a | n/a | n/a | Every character. |
| Field/value | `SkeleKit.Capitalization.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField keyboard behavior | `Sentences`, `None`, `Words`, `Characters` | Keep a plain field focused and type after selecting every mode. The keyboard shifts according to the selected capitalization policy, subject to hardware-keyboard behavior. |

```csharp
new TextField
{
	Capitalization = Capitalization.Words
};
```

## ClearButton

When a text field shows its built-in clear button.

- Source: `SkeleKit.iOS/Primitives/TextInput.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: `UITextFieldViewMode`
- Gallery role: Visual and interactive lab in the TextField showcase.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ClearButton.Never` | public | n/a | n/a | n/a | Never. The default. |
| Field/value | `SkeleKit.ClearButton.WhileEditing` | public | n/a | n/a | n/a | While the field is being edited. |
| Field/value | `SkeleKit.ClearButton.UnlessEditing` | public | n/a | n/a | n/a | Only while the field is not being edited. |
| Field/value | `SkeleKit.ClearButton.Always` | public | n/a | n/a | n/a | Always. |
| Field/value | `SkeleKit.ClearButton.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| TextField chrome and typography | `Never`, `WhileEditing`, `UnlessEditing`, `Always` | Inspect every mode with populated and empty text while focused and unfocused. Switch the trailing slot to a decorative icon and verify it intentionally suppresses the clear button. |

```csharp
new TextField
{
	Text = "SkeleKit",
	ClearButton = ClearButton.WhileEditing
};
```

## Truncation

How text is shortened when it does not fit.

- Source: `SkeleKit.iOS/Primitives/Truncation.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.Truncation.None` | public | n/a | n/a | n/a | Wrap onto the next line, up to MaxLines. |
| Field/value | `SkeleKit.Truncation.Tail` | public | n/a | n/a | n/a | An ellipsis at the end. |
| Field/value | `SkeleKit.Truncation.Head` | public | n/a | n/a | n/a | An ellipsis at the start. |
| Field/value | `SkeleKit.Truncation.Middle` | public | n/a | n/a | n/a | An ellipsis in the middle. |
| Field/value | `SkeleKit.Truncation.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Truncation specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## FontWeight

The weight of a font.

- Source: `SkeleKit.iOS/Primitives/Typography.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.FontWeight.UltraLight` | public | n/a | n/a | n/a | Ultra light. |
| Field/value | `SkeleKit.FontWeight.Thin` | public | n/a | n/a | n/a | Thin. |
| Field/value | `SkeleKit.FontWeight.Light` | public | n/a | n/a | n/a | Light. |
| Field/value | `SkeleKit.FontWeight.Regular` | public | n/a | n/a | n/a | The default weight. |
| Field/value | `SkeleKit.FontWeight.Medium` | public | n/a | n/a | n/a | Medium. |
| Field/value | `SkeleKit.FontWeight.Semibold` | public | n/a | n/a | n/a | Semibold. |
| Field/value | `SkeleKit.FontWeight.Bold` | public | n/a | n/a | n/a | Bold. |
| Field/value | `SkeleKit.FontWeight.Heavy` | public | n/a | n/a | n/a | Heavy. |
| Field/value | `SkeleKit.FontWeight.Black` | public | n/a | n/a | n/a | Black. |
| Field/value | `SkeleKit.FontWeight.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(FontWeight specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## FontDesign

The design of a font: the system face, a rounded one, a serif, or monospaced.

- Source: `SkeleKit.iOS/Primitives/Typography.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.FontDesign.Default` | public | n/a | n/a | n/a | The system font. |
| Field/value | `SkeleKit.FontDesign.Rounded` | public | n/a | n/a | n/a | The rounded system font. |
| Field/value | `SkeleKit.FontDesign.Serif` | public | n/a | n/a | n/a | A serif face. |
| Field/value | `SkeleKit.FontDesign.Monospaced` | public | n/a | n/a | n/a | A monospaced face. |
| Field/value | `SkeleKit.FontDesign.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(FontDesign specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## TextStyle

A step in the native type hierarchy. Each one carries its own Dynamic Type curve.

- Source: `SkeleKit.iOS/Primitives/Typography.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.TextStyle.LargeTitle` | public | n/a | n/a | n/a | The largest title, used once per screen. |
| Field/value | `SkeleKit.TextStyle.Title1` | public | n/a | n/a | n/a | The first title level. |
| Field/value | `SkeleKit.TextStyle.Title2` | public | n/a | n/a | n/a | The second title level. |
| Field/value | `SkeleKit.TextStyle.Title3` | public | n/a | n/a | n/a | The third title level. |
| Field/value | `SkeleKit.TextStyle.Headline` | public | n/a | n/a | n/a | An emphasized heading above a block of body text. |
| Field/value | `SkeleKit.TextStyle.Subheadline` | public | n/a | n/a | n/a | A heading below `TextStyle.Headline`. |
| Field/value | `SkeleKit.TextStyle.Body` | public | n/a | n/a | n/a | Running text. |
| Field/value | `SkeleKit.TextStyle.Callout` | public | n/a | n/a | n/a | A remark set slightly smaller than body text. |
| Field/value | `SkeleKit.TextStyle.Footnote` | public | n/a | n/a | n/a | A footnote. |
| Field/value | `SkeleKit.TextStyle.Caption1` | public | n/a | n/a | n/a | The first caption level. |
| Field/value | `SkeleKit.TextStyle.Caption2` | public | n/a | n/a | n/a | The second, smallest caption level. |
| Field/value | `SkeleKit.TextStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(TextStyle specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```
