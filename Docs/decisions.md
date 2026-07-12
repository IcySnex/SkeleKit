# BareUI.iOS — Decision Records

## ADR-001: Name & package structure

**Decision:** Library named **BareUI.iOS** (note casing: `.iOS`, matching `Xamarin.iOS` /
.NET convention, not `.IOS`). Single project/package in v1.

**Context:** "Bare" reads as *bare-metal/minimal* — slightly at odds with a library whose
point is hiding bareness, but it also reads as "bare essentials syntax", is short, unique
on NuGet, and the owner likes it. Accepted.

**Cross-platform door:** kept open, not exercised. Discipline for v1: everything in
layers 4–5 (bindings, navigation abstractions, app model contracts) avoids UIKit types in
public signatures, so a later split into `BareUI` (core) + `BareUI.iOS` (backend) is a
mechanical refactor, not a redesign. No second head is built or tested in v1.

## ADR-002: Layout — custom measure/arrange, not Auto Layout translation

**Decision:** BareUI implements its own two-pass measure/arrange layout engine (WPF
semantics) that sets `UIView.Frame` directly. Panels host children in a `LayoutHost : UIView`
overriding `LayoutSubviews`/`SizeThatFits`.

**Requirement driving it:** 1:1 native look & feel. Note this is about *controls*, not
layout — a `UILabel` looks native regardless of who computes its frame. Auto Layout is
itself just a frame calculator; bypassing it loses nothing visual.

**Why not translate to NSLayoutConstraints:**
- WPF `Grid` star-sizing + spans map onto constraints only via generated spacer views and
  priority tricks — fragile and unreadable when it breaks.
- Constraint conflicts ("Unable to simultaneously satisfy…") would leak through the
  abstraction to users who never wrote a constraint.
- A solver is slower and less deterministic than direct computation for tree-shaped layouts.

**Why not UIStackView-based:** covers stacks only; Grid still needs custom work; two
layout systems to reason about instead of one.

**Native sizing preserved:** leaf measurement delegates to the control's own
`SizeThatFits` — text wrapping, dynamic type, intrinsic sizes behave exactly native.
Prior art: Xamarin.Forms and Flutter both computed frames themselves on iOS.

**Cost accepted:** we own correctness for safe areas, keyboard avoidance, RTL (deferred),
and re-layout triggers. Mitigated by the engine being pure math → unit-testable without
a simulator.

## ADR-003: Bindings — typed delegates + CallerArgumentExpression, no reflection

**Decision:** `Bind(vm => vm.X)` captures a compiled `Func<TVm,T>` getter; the property
path string for INPC matching comes from `[CallerArgumentExpression]` parsed once at
binding creation. TwoWay requires an explicit setter delegate. No `Expression<>`, no
reflection, no source generator in v1.

**Context:** iOS device builds are Mono full AOT with trimming (the platform forbids JIT), so the
same constraints apply as under NativeAOT. Expression trees fall back to
an interpreter and are trim-hostile; reflection-based path walking (the existing
`BindingSet` + `PropertyBindingMapper` design) is trim-fragile and stringly-typed.

**Consequences:**
- Zero-reflection hot path: value reads are direct delegate calls.
- Compile-time typing: renames refactor cleanly; converter type mismatches are build errors.
- TwoWay is more verbose (`(vm, v) => vm.X = v`) — accepted; it is also explicit and
  AOT-provable.
- `[CallerArgumentExpression]` parsing assumes simple member-access lambdas
  (`vm => vm.A.B.C`). Anything else (method calls, indexers, ternaries) is rejected at
  binding creation with a clear exception; an explicit-path overload
  (`Bind(getter, path: "A.B.C")`) exists as fallback.
- **Nested-path re-subscription:** to re-subscribe INPC on intermediate objects when they
  are replaced, the binding needs intermediate *getters*, and a leaf getter alone can't
  provide them. v1 scheme: multi-segment `Bind` overloads take chained segment getters —
  `Bind(vm => vm.Config, c => c.Appearance, a => a.AnimateTabBar)` — each segment
  subscribed independently; the common 1-segment case stays `Bind(vm => vm.X)`. Sugar for
  deep paths can come later via source generator (ADR-004) without breaking this API.
- Change *sources* must implement `INotifyPropertyChanged` (CommunityToolkit.Mvvm already
  guarantees this in Velura).

**Rejected alternatives:** expression trees (AOT), string paths + mapper registry
(status quo pain), source generator (highest ceiling but weeks of build-tooling work —
deferred, see ADR-004).

### Heritage: relationship to Velura's existing binding system

Velura already contains a hand-rolled WPF-style binding layer
(`Velura.iOS/Binding/*`: `BindingSet<TVm>`, `PropertyBinding`, `EventBinding`,
`PropertyBindingMapper` registry). Its *semantics* are correct and are carried over
wholesale — BareUI bindings are that design with the mechanism swapped:

| Velura (reflection-based) | BareUI (delegate-based) | Why changed |
|---|---|---|
| String paths + `PropertyInfo.GetValue/SetValue` | Compiled getter/setter delegates | AOT/trim safety (Velura already needs `PleaseLinkerPleaseDont.xml` to keep reflection alive), compile-time typing, no boxing |
| `BindingSet<TVm>` owns bindings, VM-level INPC dispatch by string match | `ContentView<TVm>` owns bindings, same INPC dispatch by parsed path segment | Same idea; set is now implicit in the view lifetime, disposal automatic on unrealize |
| `PropertyBindingMapper` registry (global, per target type+property) for target→source | Each control wrapper wires its own native change event | Registry was necessary when binding to raw `UIView`s; BareUI has a wrapper layer anyway, so mappers collapse into it |
| `CreateSubSet<TSubVm>` for nested VMs | Chained segment getters (`Bind(vm => vm.Sub, s => s.Leaf)`) + item binding contexts in `CollectionView` | `Type.GetProperty("A.B")` never actually walked paths; sub-set disposal wasn't tied to parent |
| `EventBinding` via `EventInfo.AddEventHandler` (string event name) | `Command`/`TapCommand` properties on wrappers | Reflection-free, typed, supports `CommandParameter` (old path always executed with `null`) |
| Finalizer-backed `Dispose` | Deterministic teardown on unrealize, no finalizers | GC pressure, nondeterministic cleanup |

Kept unchanged: `BindingMode` set (OneTime/OneWay/TwoWay/OneWayToSource),
`UpdateSourceTrigger` concept (property-changed / focus-lost / explicit), converter
support, binding ownership scoping.

## ADR-004: Source generator — deferred, planned as v2 sugar

A Roslyn generator could later provide: deep-path bindings from a single lambda,
auto-generated TwoWay setters, compile-time diagnostics for invalid paths. The delegate
API of ADR-003 is designed so generated code can *target it* (generator emits the chained
segment getters), meaning v2 sugar adds no new runtime and breaks nothing.

## ADR-005: Retained-mode MVVM, not MVU

**Decision:** element trees are long-lived objects updated by bindings (WPF model), not
rebuilt per state change (SwiftUI/MVU model).

**Why:** the owner asked for WPF-without-XAML explicitly; ViewModels already exist
(CommunityToolkit.Mvvm); MVU would demand a diffing engine (large scope) and fights
UIKit's stateful controls; retained mode maps 1:1 onto the wrapper design.

## ADR-006: Lists — one CollectionView over UICollectionView, no UITableView wrapper

**Decision:** a single `CollectionView` control backed by `UICollectionView` with
compositional layout + diffable data source. List (incl. inset-grouped via
`UICollectionLayoutListConfiguration`), grid, and carousel are layout *modes*, not
separate controls.

**Why:** iOS 14+ list configuration renders visually identical to `UITableView`
(inset-grouped Settings look included) while sharing one virtualization/recycling/diffing
implementation. iOS 18 minimum makes this safe. Cell recycling contract: template tree
built once per recycled cell, only the item binding context swaps on reuse.

## ADR-007: Explicit registration everywhere, no discovery

View↔ViewModel mapping (`UsePages(pages => pages.AddTransient<TView>())` — a view reports
its own ViewModel type, so registration takes one type parameter), tabs, services — all
explicit calls at startup. No assembly scanning, no attribute discovery. Trim/AOT-safe by
construction and keeps startup deterministic. Slightly more boilerplate per screen (one
line) — accepted.

## ADR-008: Styling — typed `Style<T>` actions, no setter/resource system

**Decision:** a style is `Style<T> : IStyle` wrapping an `Action<T>` (plus optional
`BasedOn`). Applied three ways: explicitly via a new `View.Style` property (applies
immediately in the setter), implicitly via an app-global `Theme` registered with
`BareApp.UseTheme(...)` (applied in the `View` base constructor, inheritance chain
base-most first), or manually (`style.Apply(view)`). Shared values ("resources") are plain
C# statics — **no `ResourceDictionary`**.

**Context:** WPF's styling stack is reflection-shaped end to end: `Setter` targets a
`DependencyProperty` with an `object` value (boxing + runtime type checks),
`ResourceDictionary` is a string-keyed runtime lookup, `BasedOn` resolution and implicit
style application walk type metadata. None of it survives the no-reflection rule, and none
of it is needed when the "markup" is already C#: a typed closure over the control *is* a
setter collection, with IntelliSense, compile-time checking and refactoring for free.

**Precedence without a property system:** WPF tracks value sources per dependency property.
BareUI gets the same observable order purely from execution order:

1. control defaults — C# field initializers, which run *before* base ctor bodies;
2. implicit theme styles — applied in the `View` base ctor (`GetType()` is already the
   final type there);
3. explicit `View.Style` — runs at its position in the object initializer (convention:
   first line);
4. local values — initializer assignments after it.

The cost: no "unset back to default", and a style assigned *after* local values overwrites
them. Accepted — statement order is visible in the source, unlike WPF's invisible
value-source table.

**Consequences:**
- `Button.Style` (the `ButtonStyle` enum) renames to `Button.Kind` to free the `Style`
  name on `View`. Pre-1.0 breaking change, single-line fix per call site.
- Controls must configure themselves via field initializers, never ctor-body property
  sets, or they would silently beat theme styles. Review rule.
- The theme registry is a static internal frozen before `Run` — same write-once-at-startup
  lifecycle as `UsePages`; mutation after freeze throws.
- Bindings inside styles are safe to share: applying a style registers a fresh
  `Binding<T>` per view.
- Styles are neutral code — precedence, chain order and BasedOn are unit-tested without
  a simulator.

**Rejected alternatives:** setter objects keyed by property (reflection or a hand-rolled
dependency-property system — the thing this library exists to avoid); string-keyed resource
dictionaries (stringly, trim-hostile for no gain over statics); per-subtree cascading
themes (complexity without a driving screen); source-generated styles (ADR-004 territory,
v2 sugar at most).

## ADR-009: Visual fills — one `Brush` property, not one control per effect

**Decision:** `View.Background` is a `Brush?`. `SolidBrush` (implicitly converted from `Color`, so
every existing call site is unchanged), `LinearGradient`, and `Material` (a blur) are the three
fills. No `BlurView` / `GradientView` controls.

**Context:** modern iOS is materials and gradients — blurred bars, glass cards, poster scrims. The
alternative was a control per effect, which every user then has to *nest*: a card wanting a blurred
background gains a wrapper view, and the layout tree grows a level for something that is not a
layout concern. A brush keeps the fill where the fill belongs, and it composes with styling for
free: `new Style<Border>(b => b.Background = new Material(MaterialKind.Thin))` needs no new
machinery.

**Consequences:**
- `Color` → `Brush` stays implicit, so `Background = Colors.Red` and every `Style<T>` that sets a
  background keep compiling. Conversions still do not chain, so a raw literal remains illegal —
  unchanged from today.
- A gradient is a `CAGradientLayer`, which does not autoresize: its frame is synced from
  `View.ApplyFrame`. Its CGColors are snapshots, so they re-resolve through the existing
  `ReapplyVisuals` walk on a dark-mode change — the same rule the border stroke and shadow follow.
- A material is a `UIVisualEffectView` inserted as subview 0, which *does* autoresize. `Panel`'s
  subview diff skips it and offsets the children by one slot, or the next `Children` change would
  tear the material out.
- **A gradient or a material needs a `Panel`** (`Border`, `Overlay`, `VStack`, …) and throws on a
  leaf control. Both fills sit under the view's *subviews* but above the layer's own drawing, and a
  `UILabel` renders its text into that layer — the fill would cover the text. A solid brush works
  anywhere; wrap the control in a `Border` to fill behind it.
- `SwipeAction.Background` stays a `Color`: `UIContextualAction.BackgroundColor` takes a color, and
  a gradient behind a swipe action is not a thing UIKit offers.

**Rejected:** `BlurView`/`GradientView` panels (nesting, and they cannot be styled onto an existing
control); a `Background` union of `Color?` + `Brush?` (two ways to say one thing).

## ADR-010: Animation — `UIViewPropertyAnimator`, still not an animation framework

**Decision:** `Animation` (a neutral struct: duration, delay, easing, optional spring damping) plus
`Animator`, a wrapper over `UIViewPropertyAnimator` exposing `Fraction`, `Pause`, `Continue`,
`Reverse` and `Stop`. `View.Animate(seconds, changes)` survives as sugar.

**Context:** the old `View.Animate` was `UIView.Animate` — fire and forget. It cannot spring, cannot
report completion, and above all cannot be *interrupted*: a gesture-driven card that the user grabs
mid-flight has to jump. Interruptible, scrubbable animation is the specific thing UIKit does better
than SwiftUI, and it is one class away.

This does **not** reverse the "no animation framework" non-goal (PLAN §Non-Goals). There is no
timeline, no declarative transition system, no implicit layout animation — only a first-class handle
on the animation UIKit already runs.

**Consequences:**
- An `Animator` owns a native peer, so it must be held in a field for as long as it runs (the
  rooting rule). A fire-and-forget animator is a crash, not a leak.
- **A layout property does not animate on its own.** Setting `Width` invalidates measure and the
  frame only moves on the *next* layout pass — after the animation block has closed. So
  `View.Animate` forces that pass (`View.LayoutNow`) from inside the block. Without it, half of what
  a user would think to animate simply snaps.
- **`Animator.Create` must *not* force that pass**, and this is the sharp edge of the design. A
  forced layout inside a scrubbed animator's block makes the animator capture the view's bounds and
  centre as animatable properties, so the drag then interpolates the card's *height* and *position*
  alongside its transform: it visibly shrinks, jumps, and springs from a stale origin instead of
  from the finger. An `Animator` therefore animates only what its block touches; a caller who really
  wants an interactively scrubbed layout property opts in with `View.LayoutNow()` and accepts that
  the bounds ride along. This is why `Translation`/`Scale`/`Rotation` exist: an interactive gesture
  should move a transform, not the layout.
- **Transform properties exist because layout cannot do this.** `Translation`, `Scale` and `Rotation`
  are draw-only: they never invalidate measure, so a drag moves a card without re-running the layout
  engine sixty times a second. A transformed view is positioned by bounds+centre — setting `Frame`
  under a transform is undefined — which `View.ApplyFrame` now handles.
- `View.OnPan` wraps `UIPanGestureRecognizer` in a typed `PanGesture`, so an app can scrub an
  animator without importing UIKit. That is the whole point of the interactive story: the gesture
  drives `Animator.Fraction`, and the release hands the rest back with `Continue()`.
- **`Animator` does not use `UIViewPropertyAnimator` at all.** Three generations of wrappers over it
  each produced a distinct glitch class: a scrubbed fraction does not survive `continueAnimation`
  through a spring's non-monotonic curve; new timing parameters silently reset `isReversed`;
  `durationFactor` squeezes a spring into the proportional remainder; a running spring's
  `fractionComplete` measures time, not position; and replacing animators walls the scrub at the
  segment edge. The replacement owns the loop outright: `AnimationCapture` snapshots the model state
  of every view the changes block touches (both ends become plain values), `Motion` — a neutral,
  unit-testable integrator — advances a position with a damped unit-mass spring (semi-implicit
  Euler, sub-stepped) or an eased curve, and a `CADisplayLink` writes `ViewState.Lerp(start, end,
  position)` into the *model* each frame via `View.Apply` (which bypasses `Set`'s equality check),
  inside a `CATransaction` with actions disabled. There is no presentation/model split anywhere:
  the screen *is* the shadow model, every frame, so the model can never end up a value ahead (the
  old reversed-animation bug is unrepresentable). Scrubbing assigns the position; `Continue` seeds
  the integrator with the gesture velocity; `Reverse` retargets and momentum carries through. Only
  draw-only properties interpolate — brushes and layout snap on completion. Corollary: `ApplyFrame`
  always positions by bounds+centre (origin preserved — it is a scroll offset), since setting
  `Frame` under a transform is undefined.

## ADR-011: Cells adopt UIKit's *state*, never its content configuration

**Decision:** `BareCell` derives from `UICollectionViewListCell` and overrides
`UpdateConfiguration(UICellConfigurationState)` to push `IsSelected` / `IsHighlighted` into the
hosted `ItemView`. Accessories map to `UICellAccessory`. The cell's **content** stays a BareUI tree;
we never build a `UIContentConfiguration`.

**Context:** `UIContentConfiguration` is UIKit's declarative "describe the cell, hand it over"
model — and its content is `UIListContentConfiguration`, a fixed text/secondaryText/image layout.
Adopting it would mean handing rendering of the cell's *contents* back to UIKit, which is exactly
the composition BareUI exists to own; anything past a stock row would fall off it. But cell
**state** (selected, highlighted, disabled) and **accessories** (disclosure, checkmark, reorder) are
not content — they are the cell chrome, they are what makes a row feel native, and hand-rolling them
is how the Gallery ended up drawing a `"›"` in a `Label`.

So: state and accessories come from UIKit, content stays ours. `UICollectionViewListCell` works in
grid and carousel sections too (it is a `UICollectionViewCell`), where the background configuration
is simply cleared.

**Consequences:** a cell can restyle itself on selection through ordinary bindings, because
`IsSelected` is just another bindable property on `ItemView`. List sections get the native grey tap
highlight for free.
