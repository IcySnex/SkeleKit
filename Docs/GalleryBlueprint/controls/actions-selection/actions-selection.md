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
| Selection and presentation | `Selected`, `Title`, `SupportsAlpha`, `SelectionChanged` | Open the system picker and drag through colors and opacity. The bound RGBA summary and callback status update live. Toggle the title and opacity slider, reopen the picker to verify each presentation, then reset the bound color from the ViewModel. |

```csharp
new ColorWell
{
	Selected = Bind(
		model => model.SelectedColor,
		(model, value) => model.SelectedColor = value),
	Title = "Gallery accent",
	SupportsAlpha = true,
	SelectionChanged = viewModel.RecordSelection
};
```

## DatePicker

A date and time picker.

- Source: `SkeleKit.iOS/Controls/DatePicker.cs`
- Inheritance/shape: `class DatePicker : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIDatePicker`
- Layout: Uses the native intrinsic size by default. Explicit `Width`, `MinWidth`, and `MaxWidth` constrain its bounds while UIKit keeps compact formatting adaptive.
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.DatePicker.Date` | public get/set | DateTime.Now | Yes | Invalidates measure | The picked date, in local time. Two-way by default. Compact styles remeasure as the localized value changes width. |
| Property | `SkeleKit.DatePicker.Mode` | public get/set | DatePickerMode.Date | No | Invalidates measure | What the picker lets the user pick. |
| Property | `SkeleKit.DatePicker.Kind` | public get/set | DatePickerStyle.Compact | No | Invalidates measure | How the picker presents itself. |
| Property | `SkeleKit.DatePicker.Minimum` | public get/set | C# default | No | Visual/interaction only | The earliest pickable date, or null for no bound. |
| Property | `SkeleKit.DatePicker.Maximum` | public get/set | C# default | No | Visual/interaction only | The latest pickable date, or null for no bound. |
| Property | `SkeleKit.DatePicker.DateChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the user picks a date. |
| Method | `SkeleKit.DatePicker.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Mode and presentation | `Date`, `Mode`, `Kind` | Switch between date, time, and combined input while comparing compact, inline, and wheel presentations. The native picker remeasures for every combination and retains the deterministic value. |

```csharp
new DatePicker
{
	Date = new DateTime(2026, 8, 12, 14, 30, 0),
	Mode = DatePickerMode.DateAndTime,
	Kind = DatePickerStyle.Inline
};
```

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Range and binding | `Date`, `Minimum`, `Maximum`, `DateChanged` | Choose start, middle, or end from the ViewModel, then change the native picker. The visible value, two-way source, and callback status remain synchronized, while dates outside August 10–14 are unavailable. |

```csharp
new DatePicker
{
	HorizontalAlignment = HorizontalAlignment.Center,
	Width = 215,
	Date = Bind(
		model => model.SelectedDate,
		(model, value) => model.SelectedDate = value),
	Mode = DatePickerMode.DateAndTime,
	Kind = DatePickerStyle.Compact,
	Minimum = new DateTime(2026, 8, 10, 9, 0, 0),
	Maximum = new DateTime(2026, 8, 14, 18, 0, 0),
	DateChanged = viewModel.RecordDateChanged
};
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
| Selection & items | `ItemsSource`, `SelectedItem`, `ItemTitle`, `Placeholder`, `SelectionChanged` | Pick and clear a destination while switching one `ObservableCollection` between base, empty, and extended contents. Verify the formatted title, checked menu action, two-way source, callback status, placeholder, retained selection, and intrinsic width update together. |

```csharp
ObservableCollection<PickerDestination> destinations =
[
	new("Berlin", "Germany", "BER"),
	new("Copenhagen", "Denmark", "CPH")
];

Picker<PickerDestination> picker = new()
{
	ItemsSource = destinations,
	SelectedItem = Bind(
		model => model.SelectedDestination,
		(model, value) => model.SelectedDestination = value),
	Placeholder = "Choose a destination",
	ItemTitle = item => $"{item.City}, {item.Country}",
	SelectionChanged = viewModel.RecordSelection
};

destinations.Clear();
destinations.Add(new("San Francisco", "United States", "SFO"));
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
| Selection & binding | `Items`, `SelectedIndex`, `SelectionChanged` | Select Overview, Details, and Reviews. Verify the two-way source index, visible segment, derived title, and callback status update together; reset from the ViewModel and verify the first segment becomes selected. |

```csharp
SegmentedControl sections = new()
{
	SelectedIndex = Bind(
		model => model.SelectedIndex,
		(model, value) => model.SelectedIndex = value),
	SelectionChanged = viewModel.RecordSelection
};
sections.Items.Add("Overview");
sections.Items.Add("Details");
sections.Items.Add("Reviews");
```
