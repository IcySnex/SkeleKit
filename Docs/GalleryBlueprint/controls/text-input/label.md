# Label

Classification: **Visual showcase**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Label

A text label.

- Source: `SkeleKit.iOS/Controls/Label.cs`
- Inheritance/shape: `class Label : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UILabel`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Label.Text` | public get/set | C# default | Yes | Invalidates measure | The text to display. |
| Property | `SkeleKit.Label.Spans` | public get/set | C# default | No | Invalidates measure | Styled runs composing the text, overriding `Label.Text` when set. Each run styles itself over the label's own font and color. |
| Property | `SkeleKit.Label.TextStyle` | public get/set | C# default | No | Invalidates measure | The step of the native type hierarchy the text follows, or null to size it by `Label.FontSize`. |
| Property | `SkeleKit.Label.FontSize` | public get/set | double.NaN | Yes | Invalidates measure | Explicit font size in points, overriding `Label.TextStyle`. NaN falls back to the text style, or 17 points without one. |
| Property | `SkeleKit.Label.Bold` | public get/set | SkeleKit.FontWeight.Regular | Yes | Invalidates measure | Shorthand for a bold `Label.FontWeight`. |
| Property | `SkeleKit.Label.FontWeight` | public get/set | SkeleKit.FontWeight.Regular | Yes | Invalidates measure | The font's weight. |
| Property | `SkeleKit.Label.FontDesign` | public get/set | FontDesign.Default | No | Invalidates measure | The font's design: system, rounded, serif or monospaced. |
| Property | `SkeleKit.Label.Truncation` | public get/set | Truncation.Tail | No | Invalidates measure | How the text is shortened when it does not fit. |
| Property | `SkeleKit.Label.TextColor` | public get/set | C# default | Yes | Visual/interaction only | Text color, or null for the system label color. |
| Property | `SkeleKit.Label.MaxLines` | public get/set | C# default | Yes | Invalidates measure | Maximum number of lines, or 0 for unlimited (wraps freely). |
| Property | `SkeleKit.Label.TextAlignment` | public get/set | SkeleKit.TextAlignment.Leading | Yes | Visual/interaction only | Horizontal alignment of the text. |
| Property | `SkeleKit.Label.LineSpacing` | public get/set | C# default | No | Invalidates measure | Extra points between lines. |
| Property | `SkeleKit.Label.LetterSpacing` | public get/set | C# default | No | Invalidates measure | Extra points between characters (negative tightens). |
| Property | `SkeleKit.Label.Underline` | public get/set | C# default | No | Visual/interaction only | Underlines the text. |
| Property | `SkeleKit.Label.Strikethrough` | public get/set | C# default | No | Visual/interaction only | Strikes the text through. |
| Property | `SkeleKit.Label.AutoShrink` | public get/set | C# default | No | Visual/interaction only | How far the text may shrink to fit its width, 0.5 meaning half size, or 0 to truncate instead. |
| Property | `SkeleKit.Label.MaxFontSize` | public get/set | double.NaN | No | Invalidates measure | The largest point size Dynamic Type may scale the text to, or NaN to follow the accessibility sizes all the way up. |
| Method | `SkeleKit.Label.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Text`, `Spans`, `TextStyle`, `FontSize`, `Bold`, `FontWeight`, `FontDesign`, `Truncation`, `TextColor`, `MaxLines`, `TextAlignment`, `LineSpacing`, `LetterSpacing`, `Underline`, `Strikethrough`, `AutoShrink`, `MaxFontSize` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Label specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

