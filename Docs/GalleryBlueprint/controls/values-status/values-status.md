# Values and status

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## ActivityIndicator

An activity indicator spinner.

- Source: `SkeleKit.iOS/Controls/ActivityIndicator.cs`
- Inheritance/shape: `class ActivityIndicator : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIActivityIndicatorView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ActivityIndicator.IsAnimating` | public get/set | true | Yes | Visual/interaction only | Whether the spinner is animating. |
| Property | `SkeleKit.ActivityIndicator.IsLarge` | public get/set | false | No | No automatic invalidation | Whether to use the large style instead of medium. |
| Property | `SkeleKit.ActivityIndicator.Color` | public get/set | C# default | Yes | Visual/interaction only | The spinner color, or null for the system default. |
| Method | `SkeleKit.ActivityIndicator.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `IsAnimating`, `IsLarge`, `Color` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ActivityIndicator specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Divider

A hairline separator view.

- Source: `SkeleKit.iOS/Controls/Divider.cs`
- Inheritance/shape: `class Divider : View`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Divider.Color` | public get/set | C# default | Yes | Visual/interaction only | The divider color, or null for the system separator color. |
| Method | `SkeleKit.Divider.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Divider.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Color` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Divider specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## PageControl

A row of dots marking the current page of a paging scroll or a carousel.

- Source: `SkeleKit.iOS/Controls/PageControl.cs`
- Inheritance/shape: `class PageControl : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIPageControl`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.PageControl.Count` | public get/set | C# default | Yes | Invalidates measure | How many dots are shown. |
| Property | `SkeleKit.PageControl.Current` | public get/set | C# default | Yes | Visual/interaction only | The filled dot. Two-way: tapping or scrubbing the dots writes it back. |
| Property | `SkeleKit.PageControl.DotColor` | public get/set | C# default | No | Visual/interaction only | The color of the unfilled dots, or null for the system default. |
| Property | `SkeleKit.PageControl.CurrentDotColor` | public get/set | C# default | No | Visual/interaction only | The color of the filled dot, or null for the system default. |
| Property | `SkeleKit.PageControl.HidesForSinglePage` | public get/set | true | No | Invalidates measure | Whether the control hides itself while there is only one page. |
| Property | `SkeleKit.PageControl.AllowsScrubbing` | public get/set | true | No | Visual/interaction only | Whether dragging across the dots scrubs through the pages, rather than only tapping them. |
| Property | `SkeleKit.PageControl.PageChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new page whenever the user taps or scrubs the dots. |
| Method | `SkeleKit.PageControl.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Count`, `Current`, `DotColor`, `CurrentDotColor`, `HidesForSinglePage`, `AllowsScrubbing`, `PageChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(PageControl specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ProgressBar

A progress bar.

- Source: `SkeleKit.iOS/Controls/ProgressBar.cs`
- Inheritance/shape: `class ProgressBar : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIProgressView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ProgressBar.Progress` | public get/set | C# default | Yes | Visual/interaction only | The progress value from 0 (empty) to 1 (full). |
| Property | `SkeleKit.ProgressBar.FillColor` | public get/set | C# default | Yes | Visual/interaction only | The filled track color, or null for the system default. |
| Property | `SkeleKit.ProgressBar.TrackColor` | public get/set | C# default | No | Visual/interaction only | The unfilled track color, or null for the system default. |
| Method | `SkeleKit.ProgressBar.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Progress`, `FillColor`, `TrackColor` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ProgressBar specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Slider

A continuous value picker.

- Source: `SkeleKit.iOS/Controls/Slider.cs`
- Inheritance/shape: `class Slider : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UISlider`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Slider.Value` | public get/set | C# default | Yes | Visual/interaction only | The current value. |
| Property | `SkeleKit.Slider.Minimum` | public get/set | C# default | No | Visual/interaction only | The minimum selectable value. |
| Property | `SkeleKit.Slider.Maximum` | public get/set | 1 | No | Visual/interaction only | The maximum selectable value. |
| Property | `SkeleKit.Slider.Step` | public get/set | C# default | No | Visual/interaction only | The increment the value snaps to, or 0 for continuous. User changes are reported once per snapped value. Stepping requires iOS 26 or later. |
| Property | `SkeleKit.Slider.Continuous` | public get/set | true | No | Visual/interaction only | Whether the value updates all through the drag, rather than only when the thumb is released. |
| Property | `SkeleKit.Slider.TrackColor` | public get/set | C# default | No | Visual/interaction only | The color of the filled part of the track, or null for the system tint. |
| Property | `SkeleKit.Slider.EmptyTrackColor` | public get/set | C# default | No | Visual/interaction only | The color of the unfilled part of the track, or null for the system default. |
| Property | `SkeleKit.Slider.ThumbColor` | public get/set | C# default | No | Visual/interaction only | The thumb color, or null for the system default. |
| Property | `SkeleKit.Slider.MinIcon` | public get/set | C# default | No | Invalidates measure | The SF Symbol shown at the minimum end, or null for none. |
| Property | `SkeleKit.Slider.MaxIcon` | public get/set | C# default | No | Invalidates measure | The SF Symbol shown at the maximum end, or null for none. |
| Property | `SkeleKit.Slider.ValueChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the user moves the slider. |
| Method | `SkeleKit.Slider.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Value and behavior | `Value`, `Minimum`, `Maximum`, `Step`, `Continuous`, default `TrackColor`, default `EmptyTrackColor`, default `ThumbColor`, `MinIcon`, `MaxIcon`, `ValueChanged`; inherited `IsEnabled` | Drag a two-way 0–100 slider and observe its value and callback. Switch among continuous, 1, 5, and 10-point steps; compare continuous and release-only reporting; toggle endpoint symbols and enabled state. Default colors demonstrate inherited/native styling without a redundant color-customization lab. |

```csharp
new Slider
{
	Value = Bind(
		model => model.Value,
		(model, value) => model.Value = value),
	Minimum = 0,
	Maximum = 100,
	Step = 5,
	Continuous = true,
	MinIcon = "speaker.fill",
	MaxIcon = "speaker.wave.3.fill",
	ValueChanged = viewModel.RecordChange
};
```

## Stepper

An increment/decrement control.

- Source: `SkeleKit.iOS/Controls/Stepper.cs`
- Inheritance/shape: `class Stepper : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIStepper`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Stepper.Value` | public get/set | C# default | Yes | Visual/interaction only | The current value. |
| Property | `SkeleKit.Stepper.Minimum` | public get/set | C# default | No | Visual/interaction only | The minimum selectable value. |
| Property | `SkeleKit.Stepper.Maximum` | public get/set | 100 | No | Visual/interaction only | The maximum selectable value. |
| Property | `SkeleKit.Stepper.Step` | public get/set | 1 | No | Visual/interaction only | The amount added or subtracted per tap. |
| Property | `SkeleKit.Stepper.ValueChanged` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the user taps the stepper. |
| Method | `SkeleKit.Stepper.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Value`, `Minimum`, `Maximum`, `Step`, `ValueChanged` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Stepper specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Switch

A binary on/off toggle.

- Source: `SkeleKit.iOS/Controls/Switch.cs`
- Inheritance/shape: `class Switch : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UISwitch`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Switch.IsOn` | public get/set | C# default | Yes | Visual/interaction only | Whether the switch is on. |
| Property | `SkeleKit.Switch.OnColor` | public get/set | C# default | No | Visual/interaction only | The fill color while on, or null for the inherited tint. |
| Property | `SkeleKit.Switch.ThumbColor` | public get/set | C# default | No | Visual/interaction only | The thumb color, or null for the system default. |
| Property | `SkeleKit.Switch.Toggled` | public get/set | null | No | No automatic invalidation | Invoked with the new value whenever the user toggles the switch. |
| Method | `SkeleKit.Switch.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| State | `IsOn`, `Toggled`, default `OnColor`, default `ThumbColor`; inherited `IsEnabled` | Toggle the plain native control and observe its two-way ViewModel value and callback count. Change the value programmatically, then compare enabled and disabled interaction. The default colors demonstrate inherited tint and the system thumb without a redundant color-customization lab. |

```csharp
new Switch
{
	IsOn = Bind(
		model => model.IsOn,
		(model, value) => model.IsOn = value),
	OnColor = null,
	ThumbColor = null,
	Toggled = viewModel.RecordToggle
};
```
