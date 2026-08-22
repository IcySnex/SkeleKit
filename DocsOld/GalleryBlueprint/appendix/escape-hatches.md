# Escape hatches

These APIs deliberately cross the framework boundary. Keep their guidance in the Native View page's advanced code-only section and show the lifetime/platform consequences next to the sample.

| Escape hatch | Use | Contract and pitfall |
| --- | --- | --- |
| `NativeView` | Host a consumer-supplied UIKit view | The factory and native peer are iOS-only. The managed peer must stay rooted for as long as UIKit retains it. |
| `View.Native` | Reach the realized native view | Access realizes the view. Mutations made directly in UIKit can bypass SkeleKit measurement, binding, styling, and animation state. |
| `ContentView.Controller` | Configure the owning view controller | Available only once hosted; page chrome should use normal `ContentView` APIs when one exists. |
| Native gestures | Attach UIKit recognizers for an unwrapped gesture | Root delegate/target peers and coordinate state with `IsEnabled`, commands, and SkeleKit gestures. |
| `Color` UIKit conversion | Interoperate with `UIColor` | Dynamic/system colors cannot always be reduced to stable RGBA values; trait changes matter. |
| Custom controls | Derive from `View`/`Control` and use protected realization/layout hooks | Implement native creation and property replay together; preserve lazy realization and invalidation. |
| `IImageLoader` | Replace network image loading | Implement cancellation, deterministic fallback, caching policy, and main-thread native updates. |

## Lab requirements

Each escape-hatch specimen must show normal operation, unrealized/realized access, disposal or reuse, light/dark appearance, and an explicit fallback. The inline sample belongs beside the specimen, not in the primary beginner path.
