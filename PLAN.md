# BareUI.iOS — Implementation Plan

A declarative, WPF-inspired UI library for .NET for iOS (net10.0-ios). Wraps native UIKit
controls behind a clean C# object-initializer syntax with AOT-safe MVVM bindings, so app
code never touches `UIViewController`, `NSLayoutConstraint`, or manual view wiring.

**Reference app / acceptance target:** `Velura` (`../Velura`). The library is done when
Velura's screens can be rewritten in a fraction of the code with zero UIKit imports.

Related documents:

- [docs/architecture.md](docs/architecture.md) — technical design of every layer
- [docs/api-sketch.md](docs/api-sketch.md) — what app code looks like (Velura before/after)
- [docs/decisions.md](docs/decisions.md) — ADRs: naming, layout engine, binding mechanism, AOT

---

## Goals

1. **Zero visible UIKit** in app code. No `UIViewController`, no `UIView`, no constraints,
   no `AppDelegate` boilerplate. Escape hatches exist but are opt-in.
2. **WPF-like mental model, C#-only** (no XAML): element trees via object initializers,
   `Grid`/`StackPanel`/`Margin`/`Padding`/`Alignment`, MVVM with bindings and commands.
3. **100% native look & feel**: every BareUI control wraps the real UIKit control 1:1.
   BareUI only owns *composition and layout*, never rendering.
4. **AOT-safe by construction**: Velura ships with `PublishAot=true`. No reflection,
   no expression trees, no runtime code generation anywhere in the binding or navigation path.
5. **Performant lists**: virtualized `CollectionView` (grid, list, carousel) because the
   reference app is media-heavy.

## Non-Goals (v1)

- Styling/theming system (implicit styles, resource dictionaries) — deferred to v2.
- XAML or any markup language.
- Cross-platform backends (Android/Mac). Architecture keeps the door open (see ADR-001)
  but v1 is a single `BareUI.iOS` package.
- Animation framework (basic property animation helpers only if trivial).
- Right-to-left layout (tracked, not blocking v1).

## Constraints

| Constraint | Consequence |
|---|---|
| `PublishAot=true` + trimming in consumer app | Typed-delegate bindings, explicit view registration, no assembly scanning, no `Expression<>` |
| net10.0-ios, iOS 18 minimum (matches Velura) | Can use compositional layout, `UICollectionLayoutListConfiguration`, modern APIs freely |
| Native look & feel is non-negotiable | Controls are thin wrappers; BareUI never re-implements a control's drawing |
| CommunityToolkit.Mvvm already used for ViewModels | Bindings target plain `INotifyPropertyChanged` + `ICommand`; no BareUI-specific VM base class required |

---

## Deliverables

```
BareUI.sln
├── src/BareUI.iOS/            the library (net10.0-ios, NuGet-packable)
├── samples/BareUI.Gallery/    control/layout gallery app, doubles as manual test bed
└── tests/BareUI.Tests/        layout engine unit tests (measure/arrange is pure math → very testable)
```

---

## Milestones

### M0 — Scaffold (small)
- Solution, `src/BareUI.iOS` project (net10.0-ios, nullable, trimming/AOT analyzers on:
  `IsAotCompatible=true`), Gallery sample app, test project.
- CI-less for now; local `dotnet build` + simulator run.

### M1 — Core element model + layout engine (the foundation, biggest single chunk)
- `View` base class: wraps one `UIView`, exposes `Margin`, `Width`/`Height` (+ min/max),
  `HorizontalAlignment`/`VerticalAlignment`, `IsVisible`, `Opacity`, `Background`,
  `CornerRadius`.
- Two-pass measure/arrange layout engine hosted in a container `UIView` subclass
  (`LayoutSubviews` → arrange, `SizeThatFits` → measure). Native controls measured via
  their own `SizeThatFits`.
- Panels: `Grid` (star/auto/pixel + spans), `StackPanel` (V/H + `Spacing`), `ScrollView`,
  `Border` (padding + stroke + corner radius), `Overlay` (z-stack).
- Safe-area handling as a layout property (`SafeAreaEdges`), dynamic-type and rotation
  re-layout via trait/bounds observation in the root container.
- **Exit criteria:** Gallery page reproducing the poster+title+info layout of Velura's
  `MovieInfoViewController` top section, in pure BareUI, pixel-plausible on iPhone + iPad.

### M2 — Controls
- v1 set (each a thin native wrapper): `Label`, `Button`, `Image` (async source loading
  hook), `TextField`, `SecureField`, `TextEditor`, `Switch`, `Slider`, `Stepper`,
  `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker` (menu-style selection —
  replaces Velura's `UISelectionButton`).
- `NativeView` escape hatch: embed any `UIView` in the tree; `view.Native` property to
  reach the underlying UIKit object.
- **Exit criteria:** Gallery page per control; Velura's custom `UINumberField`,
  `UISelectionButton`, `UIPaddedView` all expressible without custom code.

### M3 — Binding system (AOT-safe)
- `Bindable<T>` value wrapper with implicit conversions so `Text = Bind(vm => vm.Title)`
  and `Text = "literal"` both compile (see ADR-003).
- Typed-delegate bindings: getter delegate + property-name extraction via
  `[CallerArgumentExpression]` (string parse, zero reflection). Two-way takes an explicit
  setter delegate. Nested paths via chained one-level segments.
- Modes: OneTime, OneWay, TwoWay, OneWayToSource. Converters as plain `Func<T,TResult>`.
- Control-to-source wiring built into each wrapper (e.g. `TextField` knows
  `EditingChanged`) — replaces Velura's `PropertyBindingMapper` registry.
- `ICommand` support on `Button`, tap gestures on any view; `CanExecuteChanged` → enabled state.
- Deterministic teardown: bindings owned by the view, disposed on unload (fixes the leak
  class Velura's `BindingSet` finalizer hints at).
- **Exit criteria:** binding unit tests incl. two-way round-trips; simulator run of the
  Gallery published with `PublishAot=true` (proves the AOT claim end to end).

### M4 — MVVM + navigation + app bootstrap
- `ContentView<TViewModel>`: the user's "code-behind" — sets `Content` tree in `Build()`,
  gets typed `ViewModel`, lifecycle hooks (`OnAppearing`/`OnDisappearing`/`OnLoaded`).
  Internally hosted by a hidden `UIViewController`.
- ViewModel-first navigation: explicit AOT-safe registry
  (`app.Map<MovieInfoViewModel, MovieInfoView>()`), `INavigator` with
  `PushAsync<TVm>()`, `PopAsync()`, modals + sheets (detents), alerts/action sheets/confirm
  dialogs (absorbs Velura's `IDialogHandler` + `INavigation`).
- Shell primitives: `NavigationHost` (nav stack, large titles, toolbar items),
  `TabsHost` (tab bar / iPadOS sidebar mode — covers Velura's `MainViewController`).
- `BareApp` bootstrap hiding `AppDelegate`/`UIWindow`/`Main.cs`; integrates
  `Microsoft.Extensions.DependencyInjection` (Velura already uses it).
- **Exit criteria:** Gallery restructured as tabs + pushable pages with zero UIKit code.

### M5 — CollectionView (virtualization)
- One `CollectionView` control over `UICollectionView` + diffable data source:
  - Layouts: `.List` (incl. native inset-grouped — covers Settings), `.Grid(columns)`
    (covers Home poster grids), `.Carousel` (horizontal sections).
  - `ItemsSource` (any `IReadOnlyList<T>`, live updates when `INotifyCollectionChanged` —
    works with Velura's `ObservableRangeCollection`).
  - `ItemTemplate`: `Func<Element>` built once per recycled cell + per-item rebind
    (cell binding context), so scrolling never rebuilds trees.
  - Sections with header templates, selection command, empty view.
- **Exit criteria:** Velura Home grid and Settings inset-grouped list reproduced in Gallery
  with smooth 120 Hz scrolling on device.

### M6 — Validation, docs, packaging
- Port two real Velura screens on a branch (`SettingsGroupViewController` and a simplified
  `MovieInfoViewController`) as the acceptance test; fix API friction found.
- README with quick-start, doc comments on all public API, NuGet packaging
  (`BareUI.iOS` package id).

Suggested order is strict M1→M3→M4→M5; M2 controls can trickle in parallel from M2 onward.

---

## Risks & mitigations

| Risk | Mitigation |
|---|---|
| Measure/arrange edge cases (dynamic type, rotation, keyboard) | Layout engine is pure math → heavy unit tests; keyboard avoidance handled once in `ScrollView`/root container |
| `[CallerArgumentExpression]` path parsing feels magical | Parsing is trivial (`vm => vm.A.B` → split on `.`); fallback overload with explicit string path; analyzer-friendly |
| Cell recycling + bindings interact badly | Cell = stable element tree + swappable binding context; pattern proven by Forms/Maui handlers |
| Scope creep toward MAUI | Non-goals list above; every feature must be justified by a Velura screen |
| AOT regression sneaks in via dependency | `IsAotCompatible=true` analyzers in library; Gallery published AOT in M3 and re-checked each milestone |
