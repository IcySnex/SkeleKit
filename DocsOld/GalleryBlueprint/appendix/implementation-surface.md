# Implementation-shaped exports

Classification: **Non-gallery**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## SkeleApplicationDelegate

The UIKit application delegate SkeleKit registers for you.

- Source: `Source/Framework/SkeleKit.iOS/App/SkeleApplicationDelegate.cs`
- Inheritance/shape: `class SkeleApplicationDelegate : UIApplicationDelegate`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SkeleApplicationDelegate.Window` | public override get/set | null | No | No automatic invalidation | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.SkeleApplicationDelegate.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SkeleWindowSceneDelegate

The UIKit scene delegate SkeleKit registers; it builds the app's window and shell.

- Source: `Source/Framework/SkeleKit.iOS/App/SkeleWindowSceneDelegate.cs`
- Inheritance/shape: `class SkeleWindowSceneDelegate : UIWindowSceneDelegate`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SkeleWindowSceneDelegate.Window` | public override get/set | null | No | No automatic invalidation | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.SkeleWindowSceneDelegate.WillConnect(UIKit.UIScene,UIKit.UISceneSession,UIKit.UISceneConnectionOptions)` | public override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.SkeleWindowSceneDelegate.DidEnterBackground(UIKit.UIScene)` | public override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.SkeleWindowSceneDelegate.WillEnterForeground(UIKit.UIScene)` | public override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.SkeleWindowSceneDelegate.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB<T>.<M>$9B047F02C64B08A172A5A5DD2C88066F

_No XML summary is emitted for this exported type._

- Source: `compiled/generated metadata`
- Inheritance/shape: `compiler-generated extension container`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB`1.Row(System.Int32)` | public (compiled) | n/a | n/a | n/a | Places the view in grid row `row` (zero-based). |
| Method | `SkeleKit.GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB`1.Column(System.Int32)` | public (compiled) | n/a | n/a | n/a | Places the view in grid column `column` (zero-based). |
| Method | `SkeleKit.GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB`1.RowSpan(System.Int32)` | public (compiled) | n/a | n/a | n/a | Makes the view span `span` rows. |
| Method | `SkeleKit.GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB`1.ColumnSpan(System.Int32)` | public (compiled) | n/a | n/a | n/a | Makes the view span `span` columns. |
| Method | `SkeleKit.GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB`1.<M>$9B047F02C64B08A172A5A5DD2C88066F.<Extension>$(`0)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## GridExtensions.<G>$8682DAC7B51EDBAE0D7FA41B91C3E7AB<T>

_Exported in compiled metadata without a type-level XML documentation entry._

- Source: `compiled/generated metadata`
- Inheritance/shape: `compiler-generated extension container`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| — | — | — | — | — | — | No declared documented members. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

