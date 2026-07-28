# View

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Shared lab organization

Keep inherited behavior in one gallery area with tabs for layout/visibility, fills and transforms, gestures/commands, accessibility/focus, realization/native access, and animation. Use nested colored frames so margin, explicit/automatic size, min/max, alignment, safe-area bleed, clipping, shadow, transform anchor, and desired/arranged output are visible. An event log records tap, double-tap, long-press, press, pan, pinch, rotate, focus, and animation completion.
## View

Base of every SkeleKit element: owns one lazily-created native view and takes part in the measure/arrange engine.

- Source: `SkeleKit.iOS/Elements/View.cs`
- Inheritance/shape: `class View`
- Native counterpart: `UIView` (lazy native peer)
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.View.#ctor` | protected | n/a | n/a | n/a | Applies the app's implicit theme styles for this view's type. |
| Property | `SkeleKit.View.Style` | public get/set | null | No | No automatic invalidation | A style applied the moment it is assigned. Put it first in an object initializer; later assignments override it. |
| Property | `SkeleKit.View.Parent` | public get/private set | null | No | No automatic invalidation | The panel this view sits in, or null when it is a root or unparented. |
| Property | `SkeleKit.View.BindingContext` | public get/set | null | No | No automatic invalidation | The object bindings resolve against. Inherited from the parent unless set explicitly. |
| Method | `SkeleKit.View.InvalidateMeasure` | public | n/a | n/a | n/a | Drops the cached measurement here and up the tree, and asks the root host for a layout pass. |
| Method | `SkeleKit.View.InvalidateSubtree` | public | n/a | n/a | n/a | Drops every cached measurement in this subtree. For changes that hit leaves directly, like a dynamic-type resize. |
| Property | `SkeleKit.View.IsEnabled` | public get/set | true | No | Visual/interaction only | Whether the view responds to touches. |
| Property | `SkeleKit.View.PointerEffect` | public get/set | C# default | No | Visual/interaction only | The iPad pointer effect shown when a trackpad or mouse hovers this view, or None (the default). |
| Property | `SkeleKit.View.ContextMenu` | public get | [] | No | No automatic invalidation | Entries in the view's long-press context menu. Empty for none. |
| Property | `SkeleKit.View.TapCommand` | public get/set | C# default | No | Visual/interaction only | Command invoked when the view is tapped. |
| Property | `SkeleKit.View.TapCommandParameter` | public get/set | null | No | No automatic invalidation | The parameter passed to `View.TapCommand`. |
| Property | `SkeleKit.View.DoubleTapCommand` | public get/set | C# default | No | Visual/interaction only | Command invoked when the view is double-tapped. |
| Property | `SkeleKit.View.LongPressCommand` | public get/set | C# default | No | Visual/interaction only | Command invoked when the view is held down for `View.LongPressDuration`. |
| Property | `SkeleKit.View.LongPressDuration` | public get/set | 0.5 | No | No automatic invalidation | How long a press must be held to count as a long press, in seconds. |
| Property | `SkeleKit.View.Pressed` | public get/set | C# default | No | Visual/interaction only | Invoked with true on touch-down anywhere in the view, false on release or cancel. Child controls still receive their touches. |
| Property | `SkeleKit.View.Panned` | public get/set | C# default | No | Visual/interaction only | Invoked as the view is dragged. Drive an `Animator` from it to make an animation interactive. |
| Property | `SkeleKit.View.Pinched` | public get/set | C# default | No | Visual/interaction only | Invoked as the view is pinched. Feed the scale into `View.Scale` to zoom it. |
| Property | `SkeleKit.View.Rotated` | public get/set | C# default | No | Visual/interaction only | Invoked as the view is rotated with two fingers. Feed the degrees into `View.Rotation` to turn it. |
| Property | `SkeleKit.View.AccessibilityLabel` | public get/set | C# default | Yes | Visual/interaction only | Text VoiceOver reads for the view, or null for the control's own default. |
| Property | `SkeleKit.View.AccessibilityHint` | public get/set | C# default | No | Visual/interaction only | Extra VoiceOver context describing what activating the view does, or null for none. |
| Property | `SkeleKit.View.AccessibilityValue` | public get/set | C# default | Yes | Visual/interaction only | The current value VoiceOver reads after the label (a slider's percentage), or null for the control's own default. |
| Property | `SkeleKit.View.AccessibilityTraits` | public get/set | C# default | No | Visual/interaction only | Extra traits VoiceOver applies on top of the control's own (Header, Selected, ...). |
| Property | `SkeleKit.View.AccessibilityIdentifier` | public get/set | C# default | No | Visual/interaction only | Identifier for UI tests. Never read to the user. |
| Property | `SkeleKit.View.IsAccessibilityElement` | public get/set | C# default | No | Visual/interaction only | Overrides whether VoiceOver treats the view as one element, or null for the control's own default. |
| Method | `SkeleKit.View.Focus` | public | n/a | n/a | n/a | Gives the view keyboard focus, raising the keyboard for a text control. No-op until realized. |
| Method | `SkeleKit.View.Unfocus` | public | n/a | n/a | n/a | Takes keyboard focus away, dismissing the keyboard. |
| Property | `SkeleKit.View.IgnoresSafeArea` | public get/set | SafeAreaEdges.None | No | Invalidates measure | Edges this view is allowed to extend past the safe area. A scrolling view still keeps its content inside it, so only the scroll passes under the bar. |
| Property | `SkeleKit.View.Margin` | public get/set | Thickness.Zero | No | Invalidates measure | Empty space around the view, outside its bounds. |
| Property | `SkeleKit.View.Width` | public get/set | double.NaN | No | Invalidates measure | Explicit width in points, or NaN to size to content. |
| Property | `SkeleKit.View.Height` | public get/set | double.NaN | No | Invalidates measure | Explicit height in points, or NaN to size to content. This constrains layout bounds but does not imply clipping. Set `View.ClipsToBounds` when children should not draw beyond an explicitly constrained container. |
| Property | `SkeleKit.View.MinWidth` | public get/set | 0 | No | Invalidates measure | Minimum width in points. |
| Property | `SkeleKit.View.MaxWidth` | public get/set | double.PositiveInfinity | No | Invalidates measure | Maximum width in points. |
| Property | `SkeleKit.View.MinHeight` | public get/set | 0 | No | Invalidates measure | Minimum height in points. |
| Property | `SkeleKit.View.MaxHeight` | public get/set | double.PositiveInfinity | No | Invalidates measure | Maximum height in points. |
| Property | `SkeleKit.View.HorizontalAlignment` | public get/set | HorizontalAlignment.Stretch | No | Invalidates measure | How the view is placed within the horizontal space its parent gives it. |
| Property | `SkeleKit.View.VerticalAlignment` | public get/set | VerticalAlignment.Stretch | No | Invalidates measure | How the view is placed within the vertical space its parent gives it. |
| Property | `SkeleKit.View.IsVisible` | public get/set | true | Yes | Invalidates measure | When false the view takes no space and is hidden natively. |
| Property | `SkeleKit.View.Background` | public get/set | C# default | No | Visual/interaction only | The background fill — a color, a gradient or a material — or null for transparent. |
| Property | `SkeleKit.View.Tint` | public get/set | C# default | No | Visual/interaction only | The accent color for this view and everything under it. Inherited from the parent unless set here, falling back to the app accent. |
| Property | `SkeleKit.View.Opacity` | public get/set | 1.0 | No | Visual/interaction only | Opacity from 0 (transparent) to 1 (opaque). |
| Property | `SkeleKit.View.CornerRadius` | public get/set | C# default | No | Visual/interaction only | Corner radius in points applied to the layer. |
| Property | `SkeleKit.View.Shadow` | public get/set | null | No | Visual/interaction only | A drop shadow behind the view, or null for none. A shadow needs unclipped bounds: it stops a corner radius from clipping the content, and an explicit `View.ClipsToBounds` hides it. |
| Property | `SkeleKit.View.ClipsToBounds` | public get/set | false | No | Visual/interaction only | When true, content is clipped to the bounds and corner radius. False by default so child shadows and effects can extend beyond layout bounds. Scrolling views clip by default because their bounds are a viewport. |
| Property | `SkeleKit.View.Translation` | public get/set | Point.Zero | No | Visual/interaction only | Shifts the view from where layout put it, in points. Does not affect layout. |
| Property | `SkeleKit.View.Scale` | public get/set | 1 | No | Visual/interaction only | Scales the view about its center, 1 being its laid-out size. Does not affect layout. |
| Property | `SkeleKit.View.Rotation` | public get/set | C# default | No | Visual/interaction only | Rotates the view about its `View.AnchorPoint`, in degrees. Does not affect layout. |
| Property | `SkeleKit.View.AnchorPoint` | public get/set | new(0.5, 0.5) | No | Visual/interaction only | The pivot for `View.Rotation` and `View.Scale`, in unit coordinates. (0.5, 0.5) is the center (the default), (0, 0) the top-left corner, (1, 1) the bottom-right. |
| Property | `SkeleKit.View.DesiredSize` | public get/private set | C# default | No | No automatic invalidation | Size requested by the last measure pass, including `View.Margin`. |
| Property | `SkeleKit.View.ArrangedBounds` | public get/private set | C# default | No | No automatic invalidation | Frame from the last arrange pass, in the parent's coordinates (margin excluded). |
| Method | `SkeleKit.View.Measure(SkeleKit.Size)` | public | n/a | n/a | n/a | First pass: computes `View.DesiredSize` for the space the parent offers. |
| Method | `SkeleKit.View.Arrange(SkeleKit.Rect)` | public | n/a | n/a | n/a | Second pass: positions the view within its slot, honoring margin and alignment. |
| Method | `SkeleKit.View.MeasureOverride(SkeleKit.Size)` | protected virtual | n/a | n/a | n/a | Content measurement. Panels recurse; controls delegate to the native SizeThatFits. |
| Method | `SkeleKit.View.ArrangeOverride(SkeleKit.Size)` | protected virtual | n/a | n/a | n/a | Content arrangement. Panels override to place their children. |
| Property | `SkeleKit.View.Native` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The underlying UIKit view, created on first access. Primary escape hatch to UIKit. |
| Property | `SkeleKit.View.IsRealized` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the native view has been created yet. |
| Property | `SkeleKit.View.IsFocused` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether this view currently holds keyboard focus. |
| Method | `SkeleKit.View.Realize` | public (compiled) | n/a | n/a | n/a | Builds the native view (if needed) and realizes children and bindings. |
| Method | `SkeleKit.View.Unrealize` | public (compiled) | n/a | n/a | n/a | Tears down the native view and children deterministically (no finalizers). |
| Method | `SkeleKit.View.LayoutNow` | public (compiled) | n/a | n/a | n/a | Runs any pending layout right now. Call it inside an `Animator`'s changes to animate a layout property. A layout property (Width, Margin, ...) only reaches the native frame on the next layout pass, which lands after an animation block has closed — so it would snap instead of animating. `View.Animate` does this for you. |
| Method | `SkeleKit.View.AddGesture(UIKit.UIGestureRecognizer)` | public (compiled) | n/a | n/a | n/a | Adds a native gesture recognizer. An escape hatch for gestures the library does not wrap. |
| Method | `SkeleKit.View.Animate(System.Double,System.Action)` | public (compiled) | n/a | n/a | n/a | Animates the property changes made inside `changes`. |
| Method | `SkeleKit.View.Animate(SkeleKit.Animation,System.Action,System.Action{System.Boolean})` | public (compiled) | n/a | n/a | n/a | Animates the property changes made inside `changes`, following `animation`. For an animation the user can grab mid-flight, create an `Animator` instead. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Style`, `Parent`, `BindingContext`, `IsEnabled`, `PointerEffect`, `ContextMenu`, `TapCommand`, `TapCommandParameter`, `DoubleTapCommand`, `LongPressCommand`, `LongPressDuration`, `Pressed`, `Panned`, `Pinched`, `Rotated`, `AccessibilityLabel`, `AccessibilityHint`, `AccessibilityValue`, `AccessibilityTraits`, `AccessibilityIdentifier`, `IsAccessibilityElement`, `IgnoresSafeArea`, `Margin`, `Width`, `Height`, `MinWidth`, `MaxWidth`, `MinHeight`, `MaxHeight`, `HorizontalAlignment`, `VerticalAlignment`, `IsVisible`, `Background`, `Tint`, `Opacity`, `CornerRadius`, `Shadow`, `ClipsToBounds`, `Translation`, `Scale`, `Rotation`, `AnchorPoint`, `DesiredSize`, `ArrangedBounds`, `Native`, `IsRealized`, `IsFocused` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(View specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

