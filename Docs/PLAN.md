# BareUI.iOS — Plan

A declarative, WPF-inspired UI library for .NET for iOS (net10.0-ios). Wraps native UIKit controls
behind a clean C# object-initializer syntax with AOT-safe MVVM bindings, so app code never touches
`UIViewController`, `NSLayoutConstraint`, or manual view wiring.

**Reference app / acceptance target:** `../Velura`. The library is done when Velura's screens can be
rewritten in a fraction of the code with zero UIKit imports.

Related: [architecture.md](architecture.md) (design) · [api-sketch.md](api-sketch.md) (app code) ·
[decisions.md](decisions.md) (ADRs).

## Goals

1. **Zero visible UIKit** in app code; escape hatches are opt-in.
2. **WPF-like mental model, C#-only** (no XAML): element trees via object initializers, MVVM with
   bindings and commands.
3. **100% native look & feel**: every control wraps the real UIKit control 1:1. BareUI owns
   composition and layout, never rendering.
4. **AOT-safe by construction**: device builds are Mono full AOT + trimmed (no JIT). No reflection,
   expression trees, or runtime codegen. (NativeAOT/`PublishAot` does not exist for iOS.)
5. **Performant lists**: virtualized `CollectionView` (grid, list, carousel).

## Non-Goals

- XAML or any markup language.
- Cross-platform backends (architecture keeps the door open — ADR-001).
- Animation *framework* (`View.Animate` + `Animator` exist; that's it).
- Right-to-left layout (tracked, not blocking).
- MVU / rebuild-per-state rendering (ADR-005).
- Styling beyond ADR-008: no state-based styling, no runtime theme switching past light/dark, no
  per-subtree overrides, no serialization.

## Constraints

| Constraint | Consequence |
|---|---|
| Mono full AOT + trimming on device (no JIT) | Typed-delegate bindings, explicit view registration, no assembly scanning, no `Expression<>` |
| net10.0-ios, iOS 18 minimum | Compositional layout, `UICollectionLayoutListConfiguration`, modern APIs used freely |
| Native look & feel non-negotiable | Controls are thin wrappers; never re-implement a control's drawing |
| Consumers use CommunityToolkit.Mvvm | Bindings target plain `INotifyPropertyChanged` + `ICommand`; no BareUI VM base class |

## Current state

Feature-complete for building a real app. Commits land on `main` (linear history).

- **Element model + layout**: `View`, `Panel`, `Grid`/`StackPanel`/`Overlay`/`Border`/`ScrollView`,
  safe-area regime, keyboard avoidance. Engine is pure math, unit-tested on the neutral TFM.
- **Controls**: `Label`, `Button`, `Image`, `TextField`/`SecureField`/`TextEditor`/`TextView`,
  `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker<T>`,
  `SegmentedControl`, `DatePicker`, `PageControl`, `ColorWell`, `WebView`, `NativeView`.
- **Bindings**: AOT-safe, all four modes, converters, triggers, nested paths, main-thread marshalling.
- **App model**: `BareApplication` bootstrap, DI, ViewModel-first `INavigator`, tabs/stack/single-page
  shells, iPad sidebar, page chrome, lifecycle hooks.
- **CollectionView**: diffable, virtualized; per-section list/grid/carousel layouts; sections, cell
  accessories + selection state, selection modes, refresh, swipe, context menus, reorder, prefetch,
  empty view.
- **Visual**: `Brush` (solid / `LinearGradient` / `Material`), `Shadow`, `CornerRadius`, `Opacity`.
- **Animation**: `Animation` (spring/curve) + `Animator` (scrubbable, interruptible; owns a
  `CADisplayLink` loop, not `UIViewPropertyAnimator` — ADR-010).
- **System integration**: dark mode, Dynamic Type, VoiceOver, haptics, gestures.
- **Styling**: typed `Style<T>` (+`BasedOn`), `View.Style`, app-global `Theme` (ADR-008).
- **Packaging**: NuGet ships the iOS lib + XML docs; Release device publish is clean (0 trim/AOT
  warnings).

## Remaining

- Port two Velura screens (`SettingsGroupViewController`, simplified `MovieInfoViewController`) on a
  branch as the acceptance test; fix API friction found.
- Add a LICENSE before publishing.
- (On-device 120 Hz scroll + runtime-DI-under-trim check: done and clean.)

## Design-imposed conventions

- Controls configure in **field initializers**, never ctor bodies — field inits run before the
  `View` base ctor (theme), ctor bodies after, so a ctor-body set silently beats a theme style.
- `View.Style` goes **first** in an object initializer — it applies in its setter, so later lines
  win (ADR-008).
- Every styling-surface addition needs a concrete screen that demands it — ADR-008's value is what
  it leaves out.
