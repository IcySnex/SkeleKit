# SkeleKit.iOS — Decision Records

Terse ADRs: the decision and the core reason. Deeper mechanics live in `architecture.md` and the
code; UIKit gotchas live in `CLAUDE.md`.

## ADR-001: Name & package structure

**Decision:** library **SkeleKit.iOS** (`.iOS` casing, .NET convention), single package in v1.
**Why:** short, unique on NuGet, owner's pick. A later `SkeleKit` (core) + `SkeleKit.iOS` (backend) split
stays mechanical because layers 4–5 avoid UIKit types in public signatures; not exercised in v1.

## ADR-002: Layout — custom measure/arrange, not Auto Layout translation

**Decision:** own two-pass measure/arrange (WPF semantics) setting `UIView.Frame` directly, panels
hosting children in a `LayoutHost : UIView` (`LayoutSubviews`/`SizeThatFits`).
**Why:** Grid star-sizing + spans map poorly onto constraints; conflicts would leak to users; direct
computation is faster and deterministic. Native sizing preserved (leaf measurement → the control's
`SizeThatFits`). Cost: we own safe-area / keyboard / RTL, mitigated by the engine being unit-testable
pure math.

## ADR-003: Bindings — typed delegates + CallerArgumentExpression, no reflection

**Decision:** `Bind(vm => vm.X)` captures a compiled `Func<TVm,T>`; the INPC path comes from
`[CallerArgumentExpression]`, parsed once. TwoWay needs an explicit setter. No `Expression<>`,
reflection, or generator in v1.
**Why:** Mono full AOT + trim forbids reflection and interprets expression trees. Zero-reflection
reads, compile-time typing. Nested paths use chained segment getters. (Velura's binding semantics
carried over, mechanism swapped to delegates.)

## ADR-004: Source generator — deferred

**Decision:** no Roslyn generator in v1; the delegate API is designed so a v2 generator can target it
(deep-path sugar, auto TwoWay setters, path diagnostics) with no new runtime.

## ADR-005: Retained-mode MVVM, not MVU

**Decision:** long-lived element trees updated by bindings (WPF), not rebuilt per state change.
**Why:** owner asked for WPF-without-XAML; MVU needs a diffing engine and fights UIKit's stateful
controls.

## ADR-006: One CollectionView over UICollectionView, no UITableView wrapper

**Decision:** single `CollectionView` (compositional layout + diffable data source); list (incl.
inset-grouped), grid, carousel are layout modes, not separate controls.
**Why:** iOS list configuration renders identical to `UITableView` while sharing one
virtualization/diffing path. Template tree built once per recycled cell; only the item context swaps
on reuse.

## ADR-007: Explicit registration everywhere, no discovery

**Decision:** view↔ViewModel mapping (`UsePages(pages => pages.AddTransient(...))`, a view reports its
own VM type), tabs, and services are all explicit startup calls — no scanning, no attribute discovery.
**Why:** trim/AOT-safe and deterministic startup, for one extra line per screen.

## ADR-008: Styling — typed `Style<T>` actions, no setter/resource system

**Decision:** a style is `Style<T> : IStyle` wrapping an `Action<T>` (+ optional `BasedOn`), applied
explicitly (`View.Style`, in its setter), implicitly (`Theme` via `UseTheme`, in the `View` base
ctor), or manually. Shared values are plain C# statics — no `ResourceDictionary`.
**Why:** WPF's styling stack is reflection-shaped (boxed setters, string-keyed dictionaries) — none
survives the no-reflection rule, none is needed when the "markup" is C#. Precedence is execution
order (defaults → theme → `Style` → local); cost is no "unset to default" and a style after locals
overwrites them. (`Button.Style` renamed to `Button.Kind` to free the name.)

## ADR-009: Visual fills — one `Brush` property, not one control per effect

**Decision:** `View.Background` is `Brush?` — `SolidBrush` (implicit from `Color`), `LinearGradient`,
`Material`. No `BlurView`/`GradientView`.
**Why:** a control per effect forces nesting for a non-layout concern; a brush composes with styling
for free. A gradient/material needs a `Panel` (both sit under subviews but over a leaf control's own
drawing, which they'd cover); `SwipeAction.Background` stays a `Color`.

## ADR-010: Animation — `Animator` owns a display-link loop (not `UIViewPropertyAnimator`)

**Decision:** `Animation` (neutral struct: duration, delay, easing, optional spring) + `Animator`
(`Fraction`, `Pause`, `Continue`, `Reverse`, `Stop`, `OnCompleted`) — interruptible and scrubbable.
`View.Animate` stays fire-and-forget sugar.
**Why:** `UIViewPropertyAnimator` cannot host an interactive animation (a scrubbed fraction doesn't
survive `continueAnimation` through a spring's non-monotonic curve, timing params reset `isReversed`,
`fractionComplete` is time not position). So `Animator` owns the loop: `AnimationCapture` snapshots
both ends, `Motion` (neutral, unit-tested) integrates a damped spring/curve, and a `CADisplayLink`
writes the lerped `ViewState` into the *model* each frame (`View.Apply`) — the screen *is* the model,
so the reversed-animation bug is unrepresentable. Only draw-only props interpolate; brushes and
layout snap on completion. This is a handle on the animation UIKit already runs, not an animation
framework (still a non-goal). Corollary: `Translation`/`Scale`/`Rotation` are draw-only so a gesture
moves a transform, not the layout engine 60×/s.

## ADR-011: Cells adopt UIKit's *state*, never its content configuration

**Decision:** `SkeleCell : UICollectionViewListCell` overrides `UpdateConfiguration` to push
`IsSelected`/`IsHighlighted` into the hosted `ItemView`; accessories map to `UICellAccessory`. The
cell's content stays a SkeleKit tree — no `UIContentConfiguration`.
**Why:** adopting content configuration hands cell rendering back to UIKit (the composition SkeleKit
owns), but state and accessories are chrome that make a row feel native. `IsSelected` is a bindable
prop, so cells restyle on selection through ordinary bindings; list sections get the native tap
highlight free.

## ADR-012: Commands for intents, Actions for streams, ViewModels by constructor

**Decision:** discrete intents (tap, long-press, submit, selection, toolbar, swipe, menu) are plain
`ICommand?` properties, never bindable; continuous signals (pan, pinch, scroll, text-as-you-type,
value-during-drag) are past-tense `Action<T>` (no `On` prefix — `On…` is lifecycle only). A page's
ViewModel arrives by constructor (`ContentView<TVm>` stores it, `UsePages` registers factory lambdas),
so commands are assigned directly (`Command = ViewModel.SaveCommand`); view-local handlers use
`Command.From`.
**Why:** commands never change after construction (nothing to bind), and `ICommand.Execute` boxes
every tick at 60–120 Hz (streams stay Actions). Factory lambdas keep page construction
reflection-free. Pull-to-refresh is `RefreshCommand` + a two-way `IsRefreshing`.

## ADR-013: Slim pages — instance navigation beside VM-first

**Decision:** a page is either MVVM (`ContentView<TVm>`, `[Page]`, navigated by ViewModel) or slim (a
plain `ContentView` with no ViewModel/attribute/registration/DI, navigated as a living instance
`Navigator.PushAsync(new MovieView(movie))`), mixed on one `INavigator` (which gains the instance
overloads). Slim pages reach the navigator via protected `ContentView.Navigator` and update UI
directly (`label.Text = …`).
**Why:** the VM type is the navigation key, so a stateless page would otherwise need a marker VM +
registration. Pushing instances keeps the compiler owning the payload (string routes / view-type keys
rejected). An instance is pushed at most once (per-navigation `new`); ViewModels still navigate
VM-first only.

## ADR-014: Shell vocabulary — universal tabs, one iPad scope, one bubble

**Decision:** universal `Tab<TView>(title, icon)`, `Group(...)`, `Search<TView>()`,
`Accessory<TView>()`, `Minimizes()`; iPad-only concerns in one deletable `OnIPad(pad => …)` block
(`Sidebar()`, `PlaceTab<TView>`, iPad-only destinations, `SidebarFooter<TView>()`). The trailing
bubble is single and dual-mode: `Search<TView>()` **or** `Action(icon, …)` (a FAB via a repurposed
`UISearchTab`) — declaring both throws.
**Why:** placement flags on the universal `Tab` polluted iPhone code with iPad concepts, and Apple
ships only one separated tab slot (`UISearchTab`), so it's the FAB or search, never both. iPadOS
persists customization keyed by tab identifier (the ViewModel-type name) — renaming a ViewModel or a
group resets the user's arrangement.

## ADR-015: MapView — pins, overlays, and View-tree markers over neutral geo primitives

**Decision:** `MapView` wraps `MKMapView` with a bindable two-way `Region`, a `Kind`, interaction and
chrome toggles, a `Pins` `BindableList<MapPin>` (native `MKMarkerAnnotationView` markers with
title/subtitle callouts and a `SelectionCommand`), and an `Overlays` `BindableList<MapOverlay>` drawn
as `MKPolyline`/`MKPolygon`/`MKCircle` renderers. A pin may instead supply its own `Marker` and
`Callout` builders (`Func<View>`) hosted in the annotation view and detail-callout slot. The geography
types are neutral SkeleKit primitives (`Coordinate`, `MapRegion`, `MapKind`, `MapPin`, `MapOverlay`
and its sealed shapes) so the public API stays UIKit-free and unit-tests in the shim; the
`MKMapView`/`CoreLocation` conversions and view hosting live inside the iOS-only control. Nearby pins
collapse into counted clusters through `ClustersPins` (with an optional `ClusterMarker` view builder).
Anything the typed API leaves out is reached through the base `View.Native` handle (the `MKMapView`).
**Why:** pins-plus-overlays plus per-pin `View` builders is "draw or show anything" without a second
generic control: `Func<View>` on `MapPin` keeps the flat list API and gives arbitrary marker and
callout content (chosen over a data-driven `MapView<TItem>` with recycled templates, which is a much
heavier parallel surface and only pays off past thousands of pins). Custom raster tile overlays
(`MKTileOverlay`) stay behind `.Native` rather than growing a URL-template sub-API. Pins and overlays
fully refresh on a list change or `INotifyCollectionChanged` mutation rather than diffing: a map has
no focus to preserve and low churn. `MapOverlay`'s ctor is `private protected`, closing the hierarchy
to the three shapes we can render (custom overlays go through `.Native`). User location
(`ShowsUserLocation`) needs `NSLocationWhenInUseUsageDescription` in the app plist.
