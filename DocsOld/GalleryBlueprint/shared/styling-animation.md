# Styling and animation

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Styling and animation labs

Show theme, `Style<T>`, `BasedOn`, local override ordering, light/dark factories, solid/gradient/material fills, shadow/clipping composition, fire-and-forget animation, delayed curves, springs, pause/scrub/reverse/continue/stop, completion results, and a layout animation that calls `LayoutNow`. Keep the `Animator` rooted for the run.
## Animation

How to animate a change: a duration with an easing curve, or a spring.

- Source: `Source/Framework/SkeleKit.iOS/Animation/Animation.cs`
- Inheritance/shape: `record Animation`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Describes the timing only, never what changes.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Animation.Default` | public static get | C# default | No | No automatic invalidation | The default: 0.3 seconds, eased in and out. |
| Method | `SkeleKit.Animation.Ease(System.Double,SkeleKit.Easing)` | public static | n/a | n/a | n/a | An animation of `duration` seconds following a curve. |
| Method | `SkeleKit.Animation.Spring(System.Double,System.Double)` | public static | n/a | n/a | n/a | A spring that settles over `duration` seconds; lower `damping` bounces more. |
| Method | `SkeleKit.Animation.#ctor` | public | n/a | n/a | n/a | Creates the default animation: 0.3 seconds, eased in and out. |
| Property | `SkeleKit.Animation.Duration` | public get/init | 0.3 | No | No automatic invalidation | How long the animation runs, in seconds. |
| Property | `SkeleKit.Animation.Delay` | public get/init | 0 | No | No automatic invalidation | How long to wait before it starts, in seconds. |
| Property | `SkeleKit.Animation.Easing` | public get/init | Easing.EaseInOut | No | No automatic invalidation | The curve the animation follows. Ignored when `Animation.SpringDamping` is set. |
| Property | `SkeleKit.Animation.SpringDamping` | public get/init | null | No | No automatic invalidation | The damping of a spring, from 0 (bounces forever) to 1 (settles without overshoot), or null for a curve instead. |
| Method | `SkeleKit.Animation.After(System.Double)` | public | n/a | n/a | n/a | The same animation, started `seconds` later. |
| Method | `SkeleKit.Animation.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Animation.op_Inequality(SkeleKit.Animation,SkeleKit.Animation)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Animation.op_Equality(SkeleKit.Animation,SkeleKit.Animation)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Animation.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Animation.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Animation.Equals(SkeleKit.Animation)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Animator

A running animation that can be paused, scrubbed by a gesture, reversed, or interrupted mid-flight.

- Source: `Source/Framework/SkeleKit.iOS/Animation/Animator.cs`
- Inheritance/shape: `class Animator : IDisposable`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Hold it in a field for as long as it runs: it owns a native peer, and a collected animator stops ticking.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Animator.Create(SkeleKit.Animation,System.Action)` | public static | n/a | n/a | n/a | Prepares an animation of the changes made in `changes`. It does not run until `Animator.Start`. Only what `changes` touches is animated. Transforms, Opacity, CornerRadius, colors, gradients and layout lengths all interpolate; what has no in-between (a Material, a system color, an auto-sized Width) snaps when the animation settles. |
| Property | `SkeleKit.Animator.Fraction` | public get/set | C# default | No | No automatic invalidation | How far the animation has run, from 0 to 1. Assign it to scrub, e.g. from a drag. |
| Property | `SkeleKit.Animator.IsRunning` | public get | false | No | No automatic invalidation | Whether the animation is currently running on its own. |
| Property | `SkeleKit.Animator.IsReversed` | public get/set | 1 | No | No automatic invalidation | Whether the animation is headed backwards, towards where it started. Takes effect on the next `Animator.Continue`. |
| Method | `SkeleKit.Animator.Start(System.Double)` | public | n/a | n/a | n/a | Runs the animation, after `delay` seconds if given. |
| Method | `SkeleKit.Animator.Pause` | public | n/a | n/a | n/a | Freezes the animation where it is, so `Animator.Fraction` can drive it instead. |
| Method | `SkeleKit.Animator.Continue(System.Double)` | public | n/a | n/a | n/a | Runs the animation from wherever it is towards its current heading. For a spring, `velocity` carries the gesture's speed in, as full travels per second, positive towards the end. |
| Method | `SkeleKit.Animator.Reverse` | public | n/a | n/a | n/a | Turns the animation around, keeping its momentum. |
| Method | `SkeleKit.Animator.Stop(System.Boolean)` | public | n/a | n/a | n/a | Ends the animation. It settles where it is, unless `finish` jumps it to the end. |
| Method | `SkeleKit.Animator.OnCompleted(System.Action{System.Boolean})` | public | n/a | n/a | n/a | Calls `handler` when the animation ends, with true if it reached the end rather than being interrupted. |
| Method | `SkeleKit.Animator.Dispose` | public | n/a | n/a | n/a | _Undocumented in the XML baseline; see finding._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Easing

How an animation's speed is distributed over its duration.

- Source: `Source/Framework/SkeleKit.iOS/Animation/Easing.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.Easing.Linear` | public | n/a | n/a | n/a | Constant speed. |
| Field/value | `SkeleKit.Easing.EaseIn` | public | n/a | n/a | n/a | Starts slow. |
| Field/value | `SkeleKit.Easing.EaseOut` | public | n/a | n/a | n/a | Ends slow. |
| Field/value | `SkeleKit.Easing.EaseInOut` | public | n/a | n/a | n/a | Starts and ends slow. |
| Field/value | `SkeleKit.Easing.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## IStyle

A reusable block of property setters for one view type.

- Source: `Source/Framework/SkeleKit.iOS/Styling/IStyle.cs`
- Inheritance/shape: `interface IStyle`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.IStyle.TargetType` | get | C# default | No | No automatic invalidation | The view type the style configures. |
| Method | `SkeleKit.IStyle.Apply(SkeleKit.View)` | public interface member | n/a | n/a | n/a | Runs the style's setters against `view`. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Style<T>

A named, reusable set of property setters for views of type `T`.

- Source: `Source/Framework/SkeleKit.iOS/Styling/Style.cs`
- Inheritance/shape: `class Style<T> : IStyle`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Style`1.#ctor(System.Action{`0})` | public | n/a | n/a | n/a | Creates a style from a block of setters over the typed view. |
| Method | `SkeleKit.Style`1.#ctor(SkeleKit.IStyle,System.Action{`0})` | public | n/a | n/a | n/a | Creates a style that runs `basedOn` first, then its own setters over the top. |
| Property | `SkeleKit.Style`1.TargetType` | public get | C# default | No | No automatic invalidation | _Undocumented in the XML baseline; see finding._ |
| Method | `SkeleKit.Style`1.Apply(SkeleKit.View)` | public | n/a | n/a | n/a | _Undocumented in the XML baseline; see finding._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Theme

The app's implicit styles.

- Source: `Source/Framework/SkeleKit.iOS/Styling/Theme.cs`
- Inheritance/shape: `class Theme`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Theme.Style(SkeleKit.IStyle)` | public | n/a | n/a | n/a | Registers a style applied to every view of its target type, including subtypes. |
| Method | `SkeleKit.Theme.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

