# BareUI.iOS — Plan

A declarative, WPF-inspired UI library for .NET for iOS (net10.0-ios). Wraps native UIKit
controls behind a clean C# object-initializer syntax with AOT-safe MVVM bindings, so app
code never touches `UIViewController`, `NSLayoutConstraint`, or manual view wiring.

**Reference app / acceptance target:** `Velura` (`../Velura`). The library is done when
Velura's screens can be rewritten in a fraction of the code with zero UIKit imports.

Related documents:

- [Docs/architecture.md](Docs/architecture.md) — technical design of every layer
- [Docs/api-sketch.md](Docs/api-sketch.md) — what app code looks like
- [Docs/decisions.md](Docs/decisions.md) — ADRs: naming, layout engine, binding mechanism, AOT, styling

---

## Goals

1. **Zero visible UIKit** in app code. Escape hatches exist but are opt-in.
2. **WPF-like mental model, C#-only** (no XAML): element trees via object initializers,
   `Grid`/`StackPanel`/`Margin`/`Padding`/`Alignment`, MVVM with bindings and commands.
3. **100% native look & feel**: every BareUI control wraps the real UIKit control 1:1.
   BareUI only owns *composition and layout*, never rendering.
4. **AOT-safe by construction**: iOS device builds are **Mono full AOT** + trimmed (the platform
   forbids JIT). No reflection, no expression trees, no runtime code generation anywhere.
   (NativeAOT/`PublishAot` does *not* exist for iOS — ILCompiler ships no `ios-*` RID.)
5. **Performant lists**: virtualized `CollectionView` (grid, list, carousel).

## Non-Goals

- XAML or any markup language.
- Cross-platform backends (Android/Mac). Architecture keeps the door open (ADR-001).
- Animation *framework* (`View.Animate` property-animation helper exists; that's it).
- Right-to-left layout (tracked, not blocking).
- MVU / rebuild-per-state-change rendering (ADR-005).
- Styling beyond ADR-008: no state-based styling (UIKit's own control states cover the native
  cases), no runtime theme switching past light/dark (`Color.Dynamic`), no per-subtree theme
  overrides, no style serialization.

## Constraints

| Constraint | Consequence |
|---|---|
| Mono full AOT + trimming on device (no JIT allowed) | Typed-delegate bindings, explicit view registration, no assembly scanning, no `Expression<>` |
| net10.0-ios, iOS 18 minimum | Compositional layout, `UICollectionLayoutListConfiguration`, modern APIs used freely |
| Native look & feel is non-negotiable | Controls are thin wrappers; BareUI never re-implements a control's drawing |
| CommunityToolkit.Mvvm used by consuming apps | Bindings target plain `INotifyPropertyChanged` + `ICommand`; no BareUI-specific VM base class |

---

## Current state (2026-07-12)

M0–M7 delivered (M6's Velura two-screen port outstanding, see below). The library is
feature-complete for building a real app:

- **Element model + layout engine**: `View` (lazy realize, cached measure/arrange, invalidation),
  `Panel`, `Grid`/`StackPanel`/`Overlay`/`Border`/`ScrollView`, safe-area regime,
  keyboard avoidance. Engine is pure math, unit-tested on the neutral `net10.0` TFM.
- **Controls**: `Label`, `Button`, `Image` (cached async loader), `TextField`/`SecureField`/
  `TextEditor`, `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`,
  `Picker<T>`, `SegmentedControl`, `DatePicker`, `PageControl`, `ColorWell`, `NativeView`.
- **Bindings**: AOT-safe, all four modes, converters, triggers, nested paths, main-thread
  marshalling of background INPC.
- **App model**: `BareApplication` bootstrap, DI, ViewModel-first `INavigator`, tabs/stack/single-page
  shells, iPad sidebar, page chrome (titles, toolbar, search), lifecycle hooks.
- **CollectionView**: diffable, virtualized; list/grid/carousel; sections, selection,
  refresh, swipe actions, context menus, empty view.
- **System integration**: dark mode (semantic + dynamic colors), Dynamic Type, VoiceOver
  (labels/hints/traits), haptics, gestures.
- **Styling**: typed `Style<T>` (+ `BasedOn`), explicit `View.Style`, an app-global `Theme` of
  implicit styles (`UseTheme`), `Label.TextStyle` for the native type hierarchy. Resources are
  plain statics — no `ResourceDictionary` (ADR-008). Precedence: control defaults → theme styles
  (base type first) → explicit `Style` → local values after it.
- **Packaging**: NuGet ships the iOS lib + XML docs + README; Release device publish is
  clean (zero trim/AOT warnings).

Outstanding validation (needs the reference app):

- Port two Velura screens (`SettingsGroupViewController`, simplified `MovieInfoViewController`)
  on a branch as the acceptance test; fix API friction found.
- Add a LICENSE before publishing the package.

The on-device run (120 Hz scroll, runtime DI resolve under full trim) is done and clean.

---

## M8 — Modern surface: brushes, animation, collections, controls

**Problem:** the library builds a real app, but not a *2026* one. An audit against how modern UIKit
apps are actually built found four gaps that no amount of app code can close:

1. `CollectionView` applies **one layout to the whole collection** — a screen with a hero carousel
   *and* a grid *and* a list is not expressible, though `UICollectionViewCompositionalLayout` exists
   to do exactly that. (`Layout` is also only read in `CreateNative`: reassigning it does nothing.)
2. **No cell state, no accessories.** `BareCell` is a plain `UICollectionViewCell` — no selection
   highlight, no pressed state, no disclosure chevron. The Gallery's `SettingsCell` hand-draws a
   `"›"`, which is the gap made visible.
3. **No materials, no gradients.** Glass bars and hero scrims — the defining look of modern iOS —
   are reachable only through `NativeView`.
4. **Animation is the legacy block API.** No spring, no completion, and no interruptible or
   scrubbable animator. Interruptible interaction is UIKit's whole advantage over SwiftUI.

Diffable data sources, by contrast, are already right (cached `ItemKey`, coalesced snapshots, no
animation off-screen) and need no work.

See **ADR-009** (brushes), **ADR-010** (animation), **ADR-011** (cell state).

### Feature 1 — `Brush`: solid, gradient, material

`View.Background` becomes `Brush?`. An implicit `Color` → `Brush` conversion keeps every existing
call site compiling.

- `SolidBrush` → `BackgroundColor` (today's path).
- `LinearGradient` (stops + start/end in unit space) → a `CAGradientLayer` at sublayer 0, frame
  synced from `ApplyFrame`, CGColors re-resolved by the existing `ReapplyVisuals` dark-mode walk.
- `Material` (`UltraThin`…`Chrome`) → a `UIVisualEffectView` at subview 0.

`SwipeAction.Background` stays a `Color`: `UIContextualAction` takes a color, not a brush.

### Feature 2 — animation: `Animation` + `Animator`

- `Animation` (neutral struct): duration, delay, `Easing`, optional spring damping.
  `Animation.Spring(...)` / `.Ease(...)`.
- `Animator` wraps `UIViewPropertyAnimator`: `Fraction` (scrub from a gesture), `Start`, `Pause`,
  `Continue`, `Reverse`, `Stop`, `OnCompleted`. It is the managed root of its native peer.
- `View.Animate` keeps its current signature as sugar over `Animation.Ease(seconds)`.

### Feature 3 — collection completeness

- **Per-section layouts**: a view-side `SectionStyle<TItem>` (layout + item template + optional
  header template) resolved per section by a compositional *section provider*. Cell reuse
  identifiers become per-style. Reassigning the layout after realize now calls
  `SetCollectionViewLayout`.
- **Cell state + accessories**: `BareCell` derives from `UICollectionViewListCell`;
  `UpdateConfiguration` pushes `IsSelected`/`IsHighlighted` into the hosted `ItemView`, list sections
  get the native background configuration (the grey tap highlight), and `ItemView.Accessory` maps to
  `UICellAccessory` (`Disclosure`, `Checkmark`, `Detail`, `Reorder`, `Delete`).
- **Selection modes + reorder**: `SelectionMode` (None/Single/Multiple), `SelectedItems`, and reorder
  through the data source's `ReorderingHandlers`.
- **Prefetch + pinned headers**: `IUICollectionViewDataSourcePrefetching` warms the image cache
  ahead of the scroll; `CollectionLayout.List(pinnedHeaders:)` sticks section headers.

### Feature 4 — the missing controls

`SegmentedControl`, `DatePicker`, and a pull-down `Menu` on `Button` and `ToolbarItem` (`Picker`
already wraps `UIMenu` — the pattern is proven).

### Out of scope for M8

Attributed/rich text and inline links; hero / shared-element page transitions; tab-bar badges;
cell *content* configurations (we adopt the cell's state and accessories, never its content —
ADR-011).

### Exit criteria

- One `CollectionView` renders a carousel, a grid and a list section on one screen (Gallery
  `ShowcaseDemo`).
- `SettingsCell`'s hand-drawn `"›"` is gone, replaced by a real accessory, and rows highlight on tap.
- A Gallery card can be dragged, released mid-flight, and reversed — one `Animator`, no jump.
- Materials and gradients are set through `Background`, with no extra nesting.
- Unit tests: brushes, `Animation`, section-style resolution. Release device publish stays at zero
  trim/AOT warnings.

---

## Conventions the design imposes

| Rule | Why |
|---|---|
| Controls configure themselves in field initializers, never in ctor *bodies* | Field initializers run before the `View` base ctor, ctor bodies after it — a ctor-body property set silently beats a theme style |
| `View.Style` goes first in an object initializer | It applies in its setter, so anything written after it wins (ADR-008) |
| Every addition to the styling surface needs a concrete screen that demands it | The value of ADR-008 is what it leaves out |
| `IsAotCompatible` analyzers stay on; re-publish the Release device build at milestone end | New surface is where trim/AOT regressions enter |
