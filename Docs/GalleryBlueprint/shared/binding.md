# Binding

Classification: **Interactive lab + code-only reference**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Binding labs

Use a local ViewModel implementing `INotifyPropertyChanged`. Demonstrate one-way, two-way, one-time, explicit/property-changed/lost-focus triggers, nested-path replacement, null intermediate paths, converter factories, binding-context inheritance/override, list mutation, detach on unrealize, and page/item binding helpers. Display source and target values together so direction and timing are observable.
## Bindable<T>

A control property that takes either a literal or a `Bind(...)` expression.

- Source: `SkeleKit.iOS/Binding/Bindable.cs`
- Inheritance/shape: `struct Bindable<T>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Bindable`1.op_Implicit(`0)~SkeleKit.Bindable{`0}` | public static | n/a | n/a | n/a | Creates a bindable container from a constant value. |
| Method | `SkeleKit.Bindable`1.op_Implicit(SkeleKit.BindingExpression{`0})~SkeleKit.Bindable{`0}` | public static | n/a | n/a | n/a | Creates a bindable container from an active binding expression. |
| Method | `SkeleKit.Bindable`1.#ctor(`0)` | public | n/a | n/a | n/a | Wraps a literal value. Needed for interface-typed properties. |
| Property | `SkeleKit.Bindable`1.Value` | public get | null | No | No automatic invalidation | The literal value, or the last value a binding produced. |
| Method | `SkeleKit.Bindable`1.op_Implicit(`0)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Bindable`1.op_Implicit(SkeleKit.BindingExpression{`0})` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Bindable

Creates `Bindable`1` values from literals.

- Source: `SkeleKit.iOS/Binding/Bindable.cs`
- Inheritance/shape: `class Bindable`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Bindable.From``1(``0)` | public static | n/a | n/a | n/a | Wraps a literal, for property types C# will not implicitly convert (interfaces). |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## BindableList<T>

A list source: any list literal, or a `Bind(...)` expression.

- Source: `SkeleKit.iOS/Binding/BindableList.cs`
- Inheritance/shape: `struct BindableList<T>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Changes animate when the list is an `ObservableCollection`.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.BindableList`1.op_Implicit(`0[])~SkeleKit.BindableList{`0}` | public static | n/a | n/a | n/a | Wraps an array literal. |
| Method | `SkeleKit.BindableList`1.op_Implicit(System.Collections.Generic.List{`0})~SkeleKit.BindableList{`0}` | public static | n/a | n/a | n/a | Wraps a list literal. |
| Method | `SkeleKit.BindableList`1.op_Implicit(System.Collections.ObjectModel.ObservableCollection{`0})~SkeleKit.BindableList{`0}` | public static | n/a | n/a | n/a | Wraps an observable collection, whose changes animate into place. |
| Method | `SkeleKit.BindableList`1.op_Implicit(SkeleKit.BindingExpression{System.Collections.Generic.IReadOnlyList{`0}})~SkeleKit.BindableList{`0}` | public static | n/a | n/a | n/a | Wraps an active binding to a list-typed source property. |
| Method | `SkeleKit.BindableList`1.op_Implicit(SkeleKit.BindingExpression{System.Collections.Generic.List{`0}})~SkeleKit.BindableList{`0}` | public static | n/a | n/a | n/a | Wraps an active binding to a `List`-typed source property. |
| Method | `SkeleKit.BindableList`1.op_Implicit(SkeleKit.BindingExpression{System.Collections.ObjectModel.ObservableCollection{`0}})~SkeleKit.BindableList{`0}` | public static | n/a | n/a | n/a | Wraps an active binding to an `ObservableCollection`-typed source property. |
| Method | `SkeleKit.BindableList`1.op_Implicit(`0[])` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.BindableList`1.op_Implicit(System.Collections.Generic.List{`0})` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.BindableList`1.op_Implicit(System.Collections.ObjectModel.ObservableCollection{`0})` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.BindableList`1.op_Implicit(SkeleKit.BindingExpression{System.Collections.Generic.IReadOnlyList{`0}})` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.BindableList`1.op_Implicit(SkeleKit.BindingExpression{System.Collections.Generic.List{`0}})` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.BindableList`1.op_Implicit(SkeleKit.BindingExpression{System.Collections.ObjectModel.ObservableCollection{`0}})` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.BindableList`1.GetEnumerator` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## BindableList

Builds `BindableList`1` values from collection expressions (`[a, b, c]`).

- Source: `SkeleKit.iOS/Binding/BindableList.cs`
- Inheritance/shape: `class BindableList`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.BindableList.Create``1(System.ReadOnlySpan{``0})` | public static | n/a | n/a | n/a | Wraps the elements of a collection expression as a list source. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## BindingExpression<T>

A binding described by `Bind(...)`, not yet attached to a source.

- Source: `SkeleKit.iOS/Binding/BindingExpression.cs`
- Inheritance/shape: `class BindingExpression<T>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Assign it to a `Bindable`1` property.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.BindingExpression`1.On(SkeleKit.UpdateTrigger)` | public | n/a | n/a | n/a | Chooses when a two-way binding writes back to the source. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## BindingFactory

Builds `BindingExpression`1` values.

- Source: `SkeleKit.iOS/Binding/BindingExpression.cs`
- Inheritance/shape: `class BindingFactory`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Prefer the `Bind(...)` helper on `ContentView&lt;TViewModel&gt;`.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.BindingFactory.Bind``2(System.Func{``0,``1},System.String)` | public static | n/a | n/a | n/a | A one-way binding that reads `getter` from the source. |
| Method | `SkeleKit.BindingFactory.Bind``2(System.Func{``0,``1},System.Action{``0,``1},System.String)` | public static | n/a | n/a | n/a | A two-way binding: `setter` writes the control's value back to the source. |
| Method | `SkeleKit.BindingFactory.Bind``3(System.Func{``0,``1},System.Func{``1,``2},System.String)` | public (compiled) | n/a | n/a | n/a | A one-way binding that converts the source value with `format`. |
| Method | `SkeleKit.BindingFactory.Bind``3(System.Func{``0,``1},System.Action{``0,``1},System.Func{``1,``2},System.Func{``2,``1},System.String)` | public (compiled) | n/a | n/a | n/a | A two-way binding that converts both ways: `format` out, `parse` back in. |
| Method | `SkeleKit.BindingFactory.BindToSource``2(System.Func{``0,``1},System.Action{``0,``1},System.String)` | public static | n/a | n/a | n/a | A control-to-source binding: the control writes to the source and never reads from it. |
| Method | `SkeleKit.BindingFactory.BindOnce``2(System.Func{``0,``1},System.String)` | public static | n/a | n/a | n/a | A one-time binding: read once when the context attaches, then never again. |
| Method | `SkeleKit.BindingFactory.BindPath``3(System.Func{``0,``1},System.Func{``1,``2},System.String,System.String)` | public (compiled) | n/a | n/a | n/a | A nested one-way binding. Each segment is subscribed on its own, so replacing an intermediate re-resolves the rest. |
| Method | `SkeleKit.BindingFactory.BindPath``3(System.Func{``0,``1},System.Func{``1,``2},System.Action{``1,``2},System.String,System.String)` | public (compiled) | n/a | n/a | n/a | A nested two-way binding; `setter` runs against the resolved intermediate. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## BindingMode

Which way values flow between the binding source and the control.

- Source: `SkeleKit.iOS/Binding/BindingMode.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.BindingMode.OneTime` | public | n/a | n/a | n/a | Read once when the context is attached, then never again. |
| Field/value | `SkeleKit.BindingMode.OneWay` | public | n/a | n/a | n/a | Source to control (default). |
| Field/value | `SkeleKit.BindingMode.TwoWay` | public | n/a | n/a | n/a | Both ways; needs an explicit setter. |
| Field/value | `SkeleKit.BindingMode.OneWayToSource` | public | n/a | n/a | n/a | Control to source only; needs an explicit setter. |
| Field/value | `SkeleKit.BindingMode.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## UpdateTrigger

When a two-way binding pushes the control's value back to the source.

- Source: `SkeleKit.iOS/Binding/UpdateTrigger.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.UpdateTrigger.PropertyChanged` | public | n/a | n/a | n/a | On every change (default). |
| Field/value | `SkeleKit.UpdateTrigger.FocusLost` | public | n/a | n/a | n/a | When the control loses focus. |
| Field/value | `SkeleKit.UpdateTrigger.Explicit` | public | n/a | n/a | n/a | Only when the app asks for it. |
| Field/value | `SkeleKit.UpdateTrigger.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

