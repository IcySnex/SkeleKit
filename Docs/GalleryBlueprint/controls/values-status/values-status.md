# Values and status

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## ActivityIndicator

An activity indicator spinner.

- Source: `Source/Framework/SkeleKit.iOS/Controls/ActivityIndicator.cs`
- Inheritance/shape: `class ActivityIndicator : Control`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UIActivityIndicatorView`
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ActivityIndicator.IsAnimating` | public get/set | true | Yes | Visual/interaction only | Whether the spinner is animating. |
| Property | `SkeleKit.ActivityIndicator.IsLarge` | public get/set | false | No | Invalidates measure | Whether to use the large style instead of medium. Updates the realized native indicator immediately. |
| Property | `SkeleKit.ActivityIndicator.Color` | public get/set | C# default | Yes | Visual/interaction only | The spinner color, or null for the system default. |
| Method | `SkeleKit.ActivityIndicator.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Loading state | `IsAnimating`, `IsLarge`, `Color` | Switch one custom-colored native indicator between medium and large. Stop and restart its bound animation state; the stopped indicator hides while its state label remains visible. |

```csharp
new ActivityIndicator
{
	IsAnimating = Bind(model => model.IsAnimating),
	IsLarge = true,
	Color = Colors.Red
};
```

## Divider

A hairline separator view.

- Source: `Source/Framework/SkeleKit.iOS/Controls/Divider.cs`
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
| Separator | `Color` | Show a native-scale hairline between adjacent content. Toggle its bound color between the adaptive system separator and the page accent in both appearances. |

```csharp
new Divider
{
	HorizontalAlignment = HorizontalAlignment.Stretch,
	Color = Bind(model => model.DividerColor)
};
```

## PageControl

A row of dots marking the current page of a paging scroll or a carousel.

- Source: `Source/Framework/SkeleKit.iOS/Controls/PageControl.cs`
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
| Pages and interaction | `Count`, `Current`, `DotColor`, `CurrentDotColor`, `HidesForSinglePage`, `AllowsScrubbing`, `PageChanged` | Switch among 1, 3, 5, and 10 pages. Tap or scrub the dots, advance the two-way current page programmatically, observe callback delivery, and compare hidden and visible single-page states with custom dot colors. |

```csharp
new PageControl
{
	Count = Bind(model => model.Count),
	Current = Bind(
		model => model.Current,
		(model, value) => model.Current = value),
	DotColor = Colors.Red.WithAlpha(0.25),
	CurrentDotColor = Colors.Red,
	HidesForSinglePage = true,
	AllowsScrubbing = true,
	PageChanged = viewModel.RecordPage
};
```

## ProgressBar

A progress bar.

- Source: `Source/Framework/SkeleKit.iOS/Controls/ProgressBar.cs`
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
| Progress | `Progress`, `FillColor`, `TrackColor` | Switch among empty, 25%, 65%, and completed determinate progress. Show the bound percentage with a custom fill and matching translucent track in both appearances. |

```csharp
new ProgressBar
{
	HorizontalAlignment = HorizontalAlignment.Stretch,
	Progress = Bind(model => model.Progress),
	FillColor = Colors.Red,
	TrackColor = Colors.Red.WithAlpha(0.16)
};
```

## Slider

A continuous value picker.

- Source: `Source/Framework/SkeleKit.iOS/Controls/Slider.cs`
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

- Source: `Source/Framework/SkeleKit.iOS/Controls/Stepper.cs`
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
| Value and range | `Value`, `Minimum`, `Maximum`, `Step`, `ValueChanged`; inherited `IsEnabled` | Increment and decrement a two-way value bounded from 0 through 20. Switch among 0.5, 1, 2, and 5-point steps, reset the bound value programmatically, observe callback delivery, and compare enabled and disabled interaction. |

```csharp
new Stepper
{
	Value = Bind(
		model => model.Value,
		(model, value) => model.Value = value),
	Minimum = 0,
	Maximum = 20,
	Step = 1,
	ValueChanged = viewModel.RecordChange
};
```

## Switch

A binary on/off toggle.

- Source: `Source/Framework/SkeleKit.iOS/Controls/Switch.cs`
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
