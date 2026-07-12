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
  `Panel`, `Grid`/`VStack`/`HStack`/`Overlay`/`Border`/`ScrollView`, safe-area regime,
  keyboard avoidance. Engine is pure math, unit-tested on the neutral `net10.0` TFM.
- **Controls**: `Label`, `Button`, `Image` (cached async loader), `TextField`/`SecureField`/
  `TextEditor`, `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`,
  `Picker<T>`, `NativeView`.
- **Bindings**: AOT-safe, all four modes, converters, triggers, nested paths, main-thread
  marshalling of background INPC.
- **App model**: `BareApp` bootstrap, DI, ViewModel-first `INavigator`, tabs/stack/single-page
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

Outstanding validation (needs hardware / the reference app):

- Port two Velura screens (`SettingsGroupViewController`, simplified `MovieInfoViewController`)
  on a branch as the acceptance test; fix API friction found.
- On-device: 120 Hz scroll check, runtime DI resolve under full trim.
- Add a LICENSE before publishing the package.

---

## Conventions the design imposes

| Rule | Why |
|---|---|
| Controls configure themselves in field initializers, never in ctor *bodies* | Field initializers run before the `View` base ctor, ctor bodies after it — a ctor-body property set silently beats a theme style |
| `View.Style` goes first in an object initializer | It applies in its setter, so anything written after it wins (ADR-008) |
| Every addition to the styling surface needs a concrete screen that demands it | The value of ADR-008 is what it leaves out |
| `IsAotCompatible` analyzers stay on; re-publish the Release device build at milestone end | New surface is where trim/AOT regressions enter |
