# BareUI.iOS — Implementation Plan

A declarative, WPF-inspired UI library for .NET for iOS (net10.0-ios). Wraps native UIKit
controls behind a clean C# object-initializer syntax with AOT-safe MVVM bindings, so app
code never touches `UIViewController`, `NSLayoutConstraint`, or manual view wiring.

**Reference app / acceptance target:** `Velura` (`../Velura`). The library is done when
Velura's screens can be rewritten in a fraction of the code with zero UIKit imports.

Related documents:

- [Docs/architecture.md](Docs/architecture.md) — technical design of every layer
- [Docs/api-sketch.md](Docs/api-sketch.md) — what app code looks like (Velura before/after)
- [Docs/decisions.md](Docs/decisions.md) — ADRs: naming, layout engine, binding mechanism, AOT

---

## Goals

1. **Zero visible UIKit** in app code. No `UIViewController`, no `UIView`, no constraints,
   no `AppDelegate` boilerplate. Escape hatches exist but are opt-in.
2. **WPF-like mental model, C#-only** (no XAML): element trees via object initializers,
   `Grid`/`StackPanel`/`Margin`/`Padding`/`Alignment`, MVVM with bindings and commands.
3. **100% native look & feel**: every BareUI control wraps the real UIKit control 1:1.
   BareUI only owns *composition and layout*, never rendering.
4. **AOT-safe by construction**: iOS device builds are **Mono full AOT** — the platform forbids
   JIT, so everything is AOT-compiled and trimmed. No reflection, no expression trees, no runtime
   code generation anywhere in the binding or navigation path.
   (NativeAOT/`PublishAot` does *not* exist for iOS: ILCompiler ships no `ios-*` RID, and the iOS
   workload has no NativeAOT runtime pack. Velura sets `PublishAot=true` but it is inert — the iOS
   SDK only honours it on `publish`, which would fail with NETSDK1203.)
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
| Mono full AOT + trimming on device (no JIT allowed) | Typed-delegate bindings, explicit view registration, no assembly scanning, no `Expression<>` |
| net10.0-ios, iOS 18 minimum (matches Velura) | Can use compositional layout, `UICollectionLayoutListConfiguration`, modern APIs freely |
| Native look & feel is non-negotiable | Controls are thin wrappers; BareUI never re-implements a control's drawing |
| CommunityToolkit.Mvvm already used for ViewModels | Bindings target plain `INotifyPropertyChanged` + `ICommand`; no BareUI-specific VM base class required |

---

## Deliverables

```
BareUI.sln
├── BareUI.iOS/            the library (net10.0-ios, NuGet-packable)
├── Samples/BareUI.Gallery/    control/layout gallery app, doubles as manual test bed
└── BareUI.Tests/        layout engine unit tests (measure/arrange is pure math → very testable)
```

---

## Milestones

### M0 — Scaffold (small)
- Solution, `BareUI.iOS` project (net10.0-ios, nullable, trimming/AOT analyzers on:
  `IsAotCompatible=true`), Gallery sample app, test project.
- CI-less for now; local `dotnet build` + simulator run.

### M1 — Core element model + layout engine (the foundation, biggest single chunk) — ✅ DONE (2026-07-11)
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
- **Deferred out of M1** (not blocking exit, revisit in M2/later): `SafeAreaEdges` is defined
  as an enum but not yet consumed in arrange (Gallery uses a host-frame shortcut); no
  measure invalidation / dirty flags yet; trait/bounds re-layout relies on UIKit calling
  `LayoutSubviews`. `Control` base + `Label` were pulled forward (the exit page needed text).

### M2 — Controls — ✅ DONE (2026-07-11)
- v1 set (each a thin native wrapper): `Label` ✅ (done in M1), `Button`, `Image` (async source loading
  hook), `TextField`, `SecureField`, `TextEditor`, `Switch`, `Slider`, `Stepper`,
  `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker` (menu-style selection —
  replaces Velura's `UISelectionButton`).
- `NativeView` escape hatch: embed any `UIView` in the tree; `view.Native` property to
  reach the underlying UIKit object.
- **Exit criteria:** Gallery page per control; Velura's custom `UINumberField`,
  `UISelectionButton`, `UIPaddedView` all expressible without custom code.

### M3 — Binding system (AOT-safe) — ✅ DONE (2026-07-11)
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
- **Exit criteria:** binding unit tests incl. two-way round-trips; Gallery published for a device
  (`-c Release -r ios-arm64`, `MtouchLink=Full` → Mono full AOT + trims *our* assemblies) with zero
  `IL2xxx` warnings, and the binding page working on-device (trim failures surface at runtime, when
  a binding first fires — a clean build alone proves nothing).

### M4 — MVVM + navigation + app bootstrap — ✅ DONE (2026-07-11)
- `ContentView<TViewModel>`: composes its tree into `Content` **in its constructor** (no `Build()`
  — that keeps it XAML-compatible, where `InitializeComponent()` runs in the ctor and the VM is
  attached afterwards). Typed `ViewModel`, `OnAppearing`/`OnDisappearing`, `OnViewModelAttached`.
  Hosted by a hidden `PageHost : UIViewController`.
- ViewModel-first navigation, one path only. Explicit AOT-safe registry via
  `UsePages(pages => pages.AddTransient<TView>() / .AddSingleton<TView>())`; a view reports its own
  ViewModel type, so registration takes one type param. `INavigator`: `PushAsync<TVm>()`,
  `PopAsync()`, modals + sheets (detents), alert/confirm/action sheet.
- Shell: `Tabs(...)`, `Stack<TView>()`, `SinglePage<TView>()`. Tabs/stacks only *reference*
  registered pages.
- `BareApp` bootstrap hides `AppDelegate`/`UIWindow`/`Main.cs`; ships `BareAppDelegate` +
  `BareSceneDelegate` (the app's `Info.plist` names the latter). DI via
  `Microsoft.Extensions.DependencyInjection`.
- **Exit criteria met:** Gallery is one `Program.cs` + Views/ViewModels/Models/Services, zero UIKit
  outside `NativeViewDemo` (which demos the escape hatch). ViewModels use CommunityToolkit.Mvvm,
  proving no BareUI VM base class is needed.
- **Not done, deferred:** `ToolbarItems`, `LargeTitles`-on-scroll (`TitleRevealOnScroll`),
  iPadOS sidebar (`SidebarOnIPad`).

### M5 — CollectionView (virtualization) — ✅ DONE (2026-07-11, pending on-device scroll check)
- One `CollectionView` control over `UICollectionView` + diffable data source:
  - Layouts: `.List` (incl. native inset-grouped — covers Settings), `.Grid(columns)`
    (covers Home poster grids), `.Carousel` (horizontal sections).
  - `ItemsSource` (any `IReadOnlyList<T>`, live updates when `INotifyCollectionChanged` —
    works with Velura's `ObservableRangeCollection`).
  - `ItemTemplate`: `Func<Element>` built once per recycled cell + per-item rebind
    (cell binding context), so scrolling never rebuilds trees.
  - Sections with header templates, selection command, empty view.
- **Exit criteria:** Velura Home grid and Settings inset-grouped list reproduced in Gallery
  with smooth 120 Hz scrolling on device. *(Gallery has both, plus carousel + live-list demos.
  The 120 Hz check needs a device run.)*
- Also landed: `CarouselSnap` (all five native orthogonal behaviours), `View.IgnoresSafeArea`
  (per-view safe-area bleed), and a rewrite of safe-area handling — see ADR note below.

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
