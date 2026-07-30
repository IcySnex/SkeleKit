# TextView

Classification: **Visual showcase**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## TextView

Read-only rich text that can be selected, with tappable `Link` runs.

- Source: `SkeleKit.iOS/Controls/TextView.cs`
- Inheritance/shape: `class TextView : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UITextView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.TextView.Spans` | public get/set | C# default | Yes | Invalidates measure | The styled runs to display; a plain string becomes an unstyled run, a `Link` a tappable one. Changes re-render and animate nothing, since they replace the text. Live when the list is an `ObservableCollection`. |
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
| Selection and links | `Spans`, `IsSelectable`, `LinkColor` | Switch one live `ObservableCollection<Span>` between plain text and links. Toggle selection and select or copy the plain text. Tap each link and hold the documentation link for its native menu. The selection setting is hidden for link content because UIKit necessarily makes linked text selectable. Compare app tint with an explicit blue link color. |

```csharp
ObservableCollection<Span> spans =
[
	"Read the ",
	new Link("documentation")
	{
		Command = viewModel.OpenLinkCommand,
		CommandParameter = "Documentation"
	}
];

new TextView
{
	Spans = spans,
	IsSelectable = true,
	LinkColor = null
};
```

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Typography | `Spans`, `TextStyle`, `FontSize`, `FontWeight`, `FontDesign`, `TextColor` | Choose Dynamic Type to select every native text style, or choose Fixed to replace the style picker with a 12–40 point size slider. Select every weight, compare all four system font designs, and switch between the semantic system text color and blue. Base typography flows through every run except a deliberately overridden span. |

```csharp
new TextView
{
	Spans =
	[
		"Base typography flows through every run, while ",
		new("individual spans")
		{
			FontWeight = FontWeight.Bold,
			Underline = true
		},
		" can override it."
	],
	TextStyle = TextStyle.Body,
	FontSize = double.NaN,
	FontWeight = FontWeight.Regular,
	FontDesign = FontDesign.Rounded,
	TextColor = Colors.Blue
};
```

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Text container | `Spans`, `MaxLines`, `TextAlignment`, `LineSpacing`, `LetterSpacing` | Constrain rich text to 250 points, select one, two, or unlimited lines, compare every alignment, and adjust line and letter spacing. The native text container wraps freely at zero lines and uses tail truncation at a finite limit. |

```csharp
new TextView
{
	Width = 250,
	Spans =
	[
		"Text views use their native text container to wrap ",
		new("styled content")
		{
			TextColor = Colors.Pink,
			FontWeight = FontWeight.Semibold
		},
		" across a constrained width and truncate at the selected line limit."
	],
	MaxLines = 2,
	TextAlignment = TextAlignment.Leading,
	LineSpacing = 5,
	LetterSpacing = 0
};
```
