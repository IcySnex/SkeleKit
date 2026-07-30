# Actions and selection

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Button

A tappable button.

- Source: `SkeleKit.iOS/Controls/Button.cs`
- Inheritance/shape: `class Button : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIButton`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Button.Text` | public get/set | C# default | Yes | Invalidates measure | The button's single-line title text. |
| Property | `SkeleKit.Button.Icon` | public get/set | C# default | Yes | Invalidates measure | An SF Symbol name shown alongside the text, or null for none. |
| Property | `SkeleKit.Button.Subtitle` | public get/set | C# default | Yes | Invalidates measure | Smaller text shown under the title, or null for none. |
| Property | `SkeleKit.Button.Kind` | public get/set | ButtonStyle.Plain | No | Invalidates measure | The button's native style: plain, gray, tinted or filled. |
| Property | `SkeleKit.Button.Size` | public get/set | ButtonSize.Medium | No | Invalidates measure | The built-in size class. |
| Property | `SkeleKit.Button.IconPlacement` | public get/set | IconPlacement.Leading | No | Invalidates measure | Where the icon sits relative to the text. |
| Property | `SkeleKit.Button.IconSize` | public get/set | double.NaN | No | Invalidates measure | The icon's point size, or NaN to match the size class. |
| Property | `SkeleKit.Button.IconSpacing` | public get/set | 8 | No | Invalidates measure | Points between the icon (or spinner) and the text. |
| Property | `SkeleKit.Button.Padding` | public get/set | C# default | No | Invalidates measure | Padding around the content, or null for the size class default. |
| Property | `SkeleKit.Button.IsDestructive` | public get/set | C# default | No | Invalidates measure | Styles the button red, for destructive actions. |
| Property | `SkeleKit.Button.IsLoading` | public get/set | C# default | Yes | Invalidates measure | Shows a spinner in place of the icon while true. Bind it to a command's running state. |
| Property | `SkeleKit.Button.Menu` | public get | [] | No | No automatic invalidation | Menu entries shown on tap instead of invoking `Button.Command`. Empty for a plain button. |
| Property | `SkeleKit.Button.SelectsFromMenu` | public get/set | false | No | Visual/interaction only | When true the `Button.Menu` acts as a popup picker: choosing an entry shows it as the button's title, fires its command, and remeasures for the selected title. |
| Property | `SkeleKit.Button.Command` | public get/set | C# default | No | No automatic invalidation | Command invoked on tap; its CanExecute drives the enabled state. |
| Property | `SkeleKit.Button.CommandParameter` | public get/set | C# default | No | Visual/interaction only | The parameter passed to `Button.Command`. |
| Method | `SkeleKit.Button.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Text`, `Icon`, `Subtitle`, `Kind`, `Size`, `IconPlacement`, `IconSize`, `IconSpacing`, `Padding`, `IsDestructive`, `IsLoading`, `Menu`, `SelectsFromMenu`, `Command`, `CommandParameter` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Button specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ColorWell

A swatch that opens the system color picker.

- Source: `SkeleKit.iOS/Controls/ColorWell.cs`
- Inheritance/shape: `class ColorWell : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIColorWell`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ColorWell.Selected` | public get/set | Colors.Blue | Yes | Visual/interaction only | The picked color. Two-way: the picker writes it back as the user drags. |
| Property | `SkeleKit.ColorWell.Title` | public get/set | C# default | No | Visual/interaction only | The title shown above the picker. |
| Property | `SkeleKit.ColorWell.SupportsAlpha` | public get/set | true | No | Visual/interaction only | Whether the picker offers an opacity slider. |
| Property | `SkeleKit.ColorWell.SelectionChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new color whenever the user picks one. |
| Method | `SkeleKit.ColorWell.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Selected`, `Title`, `SupportsAlpha`, `SelectionChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ColorWell specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## DatePicker

A date and time picker.

- Source: `SkeleKit.iOS/Controls/DatePicker.cs`
- Inheritance/shape: `class DatePicker : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIDatePicker`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.DatePicker.Date` | public get/set | DateTime.Now | Yes | Visual/interaction only | The picked date, in local time. Two-way by default. |
| Property | `SkeleKit.DatePicker.Mode` | public get/set | DatePickerMode.Date | No | Invalidates measure | What the picker lets the user pick. |
| Property | `SkeleKit.DatePicker.Kind` | public get/set | DatePickerStyle.Compact | No | Invalidates measure | How the picker presents itself. |
| Property | `SkeleKit.DatePicker.Minimum` | public get/set | C# default | No | Visual/interaction only | The earliest pickable date, or null for no bound. |
| Property | `SkeleKit.DatePicker.Maximum` | public get/set | C# default | No | Visual/interaction only | The latest pickable date, or null for no bound. |
| Property | `SkeleKit.DatePicker.DateChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the user picks a date. |
| Method | `SkeleKit.DatePicker.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Date`, `Mode`, `Kind`, `Minimum`, `Maximum`, `DateChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(DatePicker specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Picker<T>

A menu-style selection button wrapping `UIButton` + `UIMenu`.

- Source: `SkeleKit.iOS/Controls/Picker.cs`
- Inheritance/shape: `class Picker<T> : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIButton` with `UIMenu`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Picker`1.ItemsSource` | public get/set | [] | Yes | Invalidates measure | The selectable items. Live when the list is an `ObservableCollection`. |
| Property | `SkeleKit.Picker`1.SelectedItem` | public get/set | C# default | Yes | Invalidates measure | The selected item, or null for none. |
| Property | `SkeleKit.Picker`1.ItemTitle` | public get/set | item => item.ToString() ?? "" | No | No automatic invalidation | How an item is labeled in the menu. Defaults to `ToString()`. |
| Property | `SkeleKit.Picker`1.Placeholder` | public get/set | C# default | Yes | Invalidates measure | Text shown when nothing is selected. |
| Property | `SkeleKit.Picker`1.SelectionChanged` | public get/set | null | No | No automatic invalidation | Invoked with the newly selected item. |
| Method | `SkeleKit.Picker`1.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `ItemsSource`, `SelectedItem`, `ItemTitle`, `Placeholder`, `SelectionChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Picker<string> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## SegmentedControl

A segmented control choosing one of a few options.

- Source: `SkeleKit.iOS/Controls/SegmentedControl.cs`
- Inheritance/shape: `class SegmentedControl : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UISegmentedControl`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SegmentedControl.Items` | public get | [] | No | No automatic invalidation | The segment titles, in order. |
| Property | `SkeleKit.SegmentedControl.SelectedIndex` | public get/set | C# default | Yes | Visual/interaction only | The selected segment's index. Two-way by default. |
| Property | `SkeleKit.SegmentedControl.SelectionChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new index whenever the user picks a segment. |
| Method | `SkeleKit.SegmentedControl.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Items`, `SelectedIndex`, `SelectionChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(SegmentedControl specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```
