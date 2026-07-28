# Gestures, menus, and actions

Classification: **Interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Command

Creates commands from plain delegates, for handlers that live in the view rather than a ViewModel.

- Source: `SkeleKit.iOS/Primitives/Command.cs`
- Inheritance/shape: `class Command`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Command.From(System.Action)` | public static | n/a | n/a | n/a | A command that runs `action`, always executable. |
| Method | `SkeleKit.Command.From``1(System.Action{``0})` | public static | n/a | n/a | n/a | A command that runs `action` with the command parameter, always executable. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## GestureState

Where a continuous gesture is in its lifetime.

- Source: `SkeleKit.iOS/Primitives/Gesture.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.GestureState.Began` | public | n/a | n/a | n/a | The finger went down and the gesture was recognized. |
| Field/value | `SkeleKit.GestureState.Changed` | public | n/a | n/a | n/a | The finger moved. |
| Field/value | `SkeleKit.GestureState.Ended` | public | n/a | n/a | n/a | The finger lifted. |
| Field/value | `SkeleKit.GestureState.Canceled` | public | n/a | n/a | n/a | The system took the gesture away. |
| Field/value | `SkeleKit.GestureState.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PanGesture

One update of a drag: how far it has moved from where it started, and how fast it is going.

- Source: `SkeleKit.iOS/Primitives/Gesture.cs`
- Inheritance/shape: `record PanGesture`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.PanGesture.#ctor(SkeleKit.GestureState,SkeleKit.Point,SkeleKit.Point)` | public (compiled) | n/a | n/a | n/a | One update of a drag: how far it has moved from where it started, and how fast it is going. |
| Property | `SkeleKit.PanGesture.State` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The current execution state of the gesture lifecycle. |
| Property | `SkeleKit.PanGesture.Translation` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The cumulative distance moved from the start position. |
| Property | `SkeleKit.PanGesture.Velocity` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The current speed and direction of the movement. |
| Method | `SkeleKit.PanGesture.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PanGesture.op_Inequality(SkeleKit.PanGesture,SkeleKit.PanGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PanGesture.op_Equality(SkeleKit.PanGesture,SkeleKit.PanGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PanGesture.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PanGesture.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PanGesture.Equals(SkeleKit.PanGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PanGesture.Deconstruct(SkeleKit.GestureState@,SkeleKit.Point@,SkeleKit.Point@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PinchGesture

One update of a pinch: the factor the touched distance has scaled by since the gesture began.

- Source: `SkeleKit.iOS/Primitives/Gesture.cs`
- Inheritance/shape: `record PinchGesture`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.PinchGesture.#ctor(SkeleKit.GestureState,System.Double,System.Double)` | public (compiled) | n/a | n/a | n/a | One update of a pinch: the factor the touched distance has scaled by since the gesture began. |
| Property | `SkeleKit.PinchGesture.State` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The current execution state of the gesture lifecycle. |
| Property | `SkeleKit.PinchGesture.Scale` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The cumulative scale factor, 1 at the start. |
| Property | `SkeleKit.PinchGesture.Velocity` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The scale change per second. |
| Method | `SkeleKit.PinchGesture.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PinchGesture.op_Inequality(SkeleKit.PinchGesture,SkeleKit.PinchGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PinchGesture.op_Equality(SkeleKit.PinchGesture,SkeleKit.PinchGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PinchGesture.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PinchGesture.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PinchGesture.Equals(SkeleKit.PinchGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PinchGesture.Deconstruct(SkeleKit.GestureState@,System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## RotateGesture

One update of a two-finger rotation, in degrees since the gesture began.

- Source: `SkeleKit.iOS/Primitives/Gesture.cs`
- Inheritance/shape: `record RotateGesture`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.RotateGesture.#ctor(SkeleKit.GestureState,System.Double,System.Double)` | public (compiled) | n/a | n/a | n/a | One update of a two-finger rotation, in degrees since the gesture began. |
| Property | `SkeleKit.RotateGesture.State` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The current execution state of the gesture lifecycle. |
| Property | `SkeleKit.RotateGesture.Degrees` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The cumulative rotation, clockwise positive. |
| Property | `SkeleKit.RotateGesture.Velocity` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The rotation change in degrees per second. |
| Method | `SkeleKit.RotateGesture.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.RotateGesture.op_Inequality(SkeleKit.RotateGesture,SkeleKit.RotateGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.RotateGesture.op_Equality(SkeleKit.RotateGesture,SkeleKit.RotateGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.RotateGesture.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.RotateGesture.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.RotateGesture.Equals(SkeleKit.RotateGesture)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.RotateGesture.Deconstruct(SkeleKit.GestureState@,System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## MenuAction

An entry in a row's long-press context menu.

- Source: `SkeleKit.iOS/Primitives/MenuAction.cs`
- Inheritance/shape: `class MenuAction`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.MenuAction.Text` | public get/set | "" | No | No automatic invalidation | The entry's title. |
| Property | `SkeleKit.MenuAction.Icon` | public get/set | null | No | No automatic invalidation | An SF Symbol name shown beside the title. |
| Property | `SkeleKit.MenuAction.IsDestructive` | public get/set | false | No | No automatic invalidation | Whether the entry is styled as destructive. |
| Property | `SkeleKit.MenuAction.Command` | public get/set | null | No | No automatic invalidation | Invoked with the row's item. |
| Method | `SkeleKit.MenuAction.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PointerEffect

How a view reacts to a hovering trackpad or mouse pointer on iPad.

- Source: `SkeleKit.iOS/Primitives/PointerEffect.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: No effect on iPhone, which has no pointer.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.PointerEffect.None` | public | n/a | n/a | n/a | No pointer effect (the default). |
| Field/value | `SkeleKit.PointerEffect.Automatic` | public | n/a | n/a | n/a | The system effect matched to the view's size and role, highlighting small controls and lifting larger tiles. The explicit variants are not exposed: Microsoft.iOS 26.0 binds only the automatic effect factory, so distinguishing highlight/lift/hover is not possible. |
| Field/value | `SkeleKit.PointerEffect.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PreviewShape

The shape of a row lifted as its own context-menu platter.

- Source: `SkeleKit.iOS/Primitives/PreviewShape.cs`
- Inheritance/shape: `record PreviewShape`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.PreviewShape.#ctor(System.Double,System.Double,System.Nullable{SkeleKit.Color})` | public (compiled) | n/a | n/a | n/a | The shape of a row lifted as its own context-menu platter. |
| Property | `SkeleKit.PreviewShape.Padding` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Uniform padding between the row's content and the platter's edge. |
| Property | `SkeleKit.PreviewShape.CornerRadius` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The platter's corner radius. |
| Property | `SkeleKit.PreviewShape.Background` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The platter's fill, or null for the system default. A transparent color draws none. |
| Method | `SkeleKit.PreviewShape.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.op_Inequality(SkeleKit.PreviewShape,SkeleKit.PreviewShape)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.op_Equality(SkeleKit.PreviewShape,SkeleKit.PreviewShape)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.Equals(SkeleKit.PreviewShape)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.<Clone>$` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.PreviewShape.Deconstruct(System.Double@,System.Double@,System.Nullable{SkeleKit.Color}@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SwipeSide

Which edge of a row a swipe action lives on.

- Source: `SkeleKit.iOS/Primitives/SwipeAction.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.SwipeSide.Trailing` | public | n/a | n/a | n/a | Revealed by swiping from the trailing edge. |
| Field/value | `SkeleKit.SwipeSide.Leading` | public | n/a | n/a | n/a | Revealed by swiping from the leading edge. |
| Field/value | `SkeleKit.SwipeSide.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SwipeAction

An action revealed by swiping a row.

- Source: `SkeleKit.iOS/Primitives/SwipeAction.cs`
- Inheritance/shape: `class SwipeAction`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SwipeAction.Text` | public get/set | null | No | No automatic invalidation | The action's title. |
| Property | `SkeleKit.SwipeAction.Icon` | public get/set | null | No | No automatic invalidation | An SF Symbol name shown on the action. |
| Property | `SkeleKit.SwipeAction.Side` | public get/set | SwipeSide.Trailing | No | No automatic invalidation | Which edge reveals the action. |
| Property | `SkeleKit.SwipeAction.IsDestructive` | public get/set | false | No | No automatic invalidation | Whether the action is styled as destructive, and runs on a full swipe. |
| Property | `SkeleKit.SwipeAction.Background` | public get/set | null | No | No automatic invalidation | The action's background color, or null for the system default. |
| Property | `SkeleKit.SwipeAction.Command` | public get/set | null | No | No automatic invalidation | Invoked with the row's item. |
| Method | `SkeleKit.SwipeAction.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

