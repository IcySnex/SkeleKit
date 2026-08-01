
# SkeleKit.iOS

A C# UI framework for .NET for iOS (no MAUI, no XAML). Native UIKit
controls behind clean C# syntax with AOT-safe MVVM bindings. App code never
touches `UIViewController`/`NSLayoutConstraint`.

**Read first:** `Docs/PLAN.md` (roadmap), `Docs/architecture.md` (5-layer design),
`Docs/api-sketch.md` (target syntax), `Docs/decisions.md` (ADRs — binding/layout rationale).

**Reference app:** `../Velura` — the messy UIKit app this library exists to clean up.
Acceptance target: rewrite its screens with zero UIKit imports.

## Structure

- `SkeleKit.iOS/` — the library. Multi-targets `net10.0;net10.0-ios`, but **iOS is the only
  platform**: `net10.0` is a test shim so the layout engine unit-tests without a simulator.
  Root namespace is `SkeleKit` (not `SkeleKit.iOS`). Folders: `Primitives/` (structs+enums),
  `Elements/` (`View`, `Panel`, `ViewCollection`, `LayoutHost`), `Layout/` (panels),
  `Controls/` (native wrappers).
- `SkeleKit.Tests/` — xunit, plain `net10.0`, references the neutral TFM. Layout
  engine must stay testable here without a simulator.
- `Samples/SkeleKit.Gallery/` — iOS sample app for on-the-fly testing/debugging. Bootstraps through
  `SkeleApplication.CreateBuilder()` like any consumer would.

## Commands

- Test: `dotnet test SkeleKit.Tests`
- Build app: `dotnet build Samples/SkeleKit.Gallery -p:RuntimeIdentifier=iossimulator-arm64`
- Run app: add `-t:Run "-p:_DeviceName=:v2:udid=<UDID>"` (UDIDs: `xcrun simctl list devices available`)
- Screenshot to verify layout: `xcrun simctl io <UDID> screenshot out.png` then Read it.
- Release build on device (Mono full AOT + full trim; settings live in the Gallery csproj under
  `Release`+`ios-arm64`): `dotnet publish Samples/SkeleKit.Gallery -p:PublishProfile=iOS-Device`, then
  `xcrun devicectl device install app --device <UDID> <path>.app`. Watch for `IL2xxx` warnings.
- **Iterating on the sim:** rebuild the whole app after editing library code (a bare `-t:Run` can
  relaunch a stale binary). `simctl terminate <UDID> com.skelekit.gallery`, then `install` the fresh
  `.app` + `launch`.
- **A plist edit needs `rm -rf Samples/SkeleKit.Gallery/bin obj`** — incremental builds don't recopy
  `Info.plist`.

## Environment gotchas

- dotnet is **brew-managed** (`/opt/homebrew/bin/dotnet`, SDK 10.0.3xx). The old install
  at `/usr/local/share/dotnet` is stale — don't use it.
- `ValidateXcodeVersion=false` in `Directory.Build.props`: installed Xcode (26.6) is newer
  than the newest .NET for iOS release supports (26.5). Remove once Microsoft ships
  Xcode 26.6 support.

## Conventions

- Tabs, file-scoped namespaces, each ctor/method parameter on its own line.
- Doc comments (**public and plain-`protected` API only**): always the full block form
  (`/// <summary>` on its own line, never the compact `/// <summary>X</summary>` one-liner). Summary
  is **one sentence on one `///` line** — never wrap for width; a second sentence or any side-note /
  usage detail goes in a `<remarks>` block (each sentence its own line, `<br/>` for a deliberate
  break). `<inheritdoc/>` stays bare, never wrapped in `<summary>`. Plus `<param>`/`<returns>`/
  `<typeparam>` tags on methods. Write like a human for a UI framework: no em-dashes (rephrase), no
  redundant parentheticals, American spelling (color / canceled / center / gray), and don't restate a
  default the code already shows.
- **Non-public members get no XML docs at all** — `internal`, `private`, `private protected`, and
  implicitly-internal top-level types. Add a short `//` there only when the code genuinely can't say
  it, never by default.
- Inline `//` comments: cull aggressively — keep one **only** when the code genuinely can't say it
  and a competent iOS dev couldn't infer it (a routine UIKit rooting/retain note doesn't qualify).
  When kept: a short lowercase fragment, a few words. No full-sentence prose, no multi-line blocks,
  no explaining a bugfix inline — that goes in the commit body. Same American-spelling / no-em-dash
  rules as docs.
- Type-check with an explicit type pattern (`is Type x` / `is not Type x`), **never `is { }`**; a
  nullable scalar or enum takes `is double x` etc., only a nullable *tuple* falls back to
  `.HasValue`/`.Value`. Prefer `is not null` over `!= null`. Explicit types everywhere (no `var`).
- Omit redundant modifiers/types: no `private` where it's already the default; target-typed
  `new(...)`; collection expressions `[]`. Exception: top-level `internal` is written out explicitly.
- Prefer primary constructors, `field`-keyword semi-auto properties
  (`get; set => Set(ref field, ...)`), and expression bodies joined onto one line when short
  (`internal override bool Scrolls => true;`).
- **Member order** (template: `Controls/Button.cs`): nested types first, then all statics grouped
  together (static fields/properties/methods sit above every instance member), then a private
  cast-helper property like `Ui` and any rooted private fields, then the public properties, then the
  methods — helpers and lifecycle first, the public API last (`private` → `private protected` →
  `internal` → `public`). A property keeps its backing fields glued directly beneath it. Two blank
  lines between groups. It's a "reads best" judgment, not a rigid sort — order what reads cleanly. In
  a multi-type file the primary (filename) type leads and its supporting enums follow; split
  unrelated public types into their own files but keep tight families together, and don't nest a
  *public* helper enum (it renames the API).
- **No preprocessor directives. Ever** — no `#if IOS`, no `#pragma`, no `#region`. A wholly-iOS file
  goes in `Controls/` (or is named in the csproj's `net10.0` `Compile Remove` glob) and uses UIKit
  directly. A file mixing layout math with UIKit splits: neutral half in `Foo.cs`, native half in
  `Foo.iOS.cs` (`partial`, excluded from `net10.0`). Neutral code calls native via `partial void`
  hooks (see `View.ApplyFrame`, `Panel.OnChildrenChanged`).
- **Native peers must be rooted.** Every `NSObject` subclass we define needs (a) a
  `(NativeHandle handle)` ctor and (b) something *managed* holding it for as long as UIKit holds the
  native object. UIKit's own retain does **not** keep the managed peer alive — the GC takes it and
  the app aborts in the marshaller or silently stops laying out (black screen). Beware weak native
  refs (`UINavigationController.Delegate`); avoid `NSTimer` + `Action` and the default
  `NSUrlSessionHandler` (their peers die the same way).
- Commits: Conventional Commits, subject ≤50 chars, body only when the why isn't obvious.
- Everything must be AOT/trim-safe: no reflection, no `Expression<>`, no assembly scanning. iOS
  device builds are **Mono full AOT** + trimmed (no JIT). `PublishAot`/NativeAOT does not exist for
  iOS (ILCompiler ships no `ios-*` RID).
- Public API stays UIKit-free except explicit escape hatches (`NativeView`, `.Native`).

## Status

Feature-complete for v1: element model, panels, controls, bindings, styling, app/nav shell, and
`CollectionView`. Commits land on `main` (linear history). `Docs/api-gaps.md` tracks the
UIKit-capability audit (~~struck~~ = done). Remaining work is under "Known debt".

Shape:
- **Elements**: `View` (lazy realize, WPF measure/arrange, `Parent`, `InvalidateMeasure`), `Panel`/
  `ViewCollection`/`LayoutHost`. Property pipeline is `Set(ref field, value, apply)`; `CreateNative`
  only constructs, every property pushes through one `ApplyProperties()` hook.
- **Panels**: `StackPanel`, `Grid` (auto/star/px + spans + spacing), `Overlay`, `Border`,
  `ScrollView`. Layout math unit-tested in the neutral TFM.
- **Controls**: `Label`, `Button`, `Image`, `TextField`/`SecureField`/`TextEditor`/`TextView`,
  `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker`,
  `SegmentedControl`, `DatePicker`, `ColorWell`, `PageControl`, `WebView`, `NativeView`. Each exposes
  `Bindable<T>` properties and pushes target→source from its native events.
- **Bindings** (neutral, unit-tested): `Bindable<T>`/`BindingExpression<T>`/`Binding<T>`,
  `BindingContext` inherited down the tree, `BindingFactory.Bind/BindPath`. Compiled getters +
  `[CallerArgumentExpression]` paths, per-segment INPC, zero reflection. Commands are never bindable
  (plain `ICommand?` from the ctor-injected ViewModel, ADR-012); control values are two-way
  `Bindable<T>` state with optional past-tense `Action<T>` observers, while operations use commands
  and continuous signals remain Actions (`Panned`, `Scrolled`; `On…` = lifecycle overrides only).
  List sources are `BindableList<T>`.
- **App**: `SkeleApplication.CreateBuilder().UseServices().UsePages().Tabs().Build().Run(args)`.
  `ContentView<TVm>` takes its ViewModel by constructor; `PageHost` is the hidden
  `UIViewController`. `INavigator` is ViewModel-first push/pop/present + alert/confirm/sheet.
  `UsePages` uses factory lambdas — reflection-free page construction.
- **Hot reload** (simulator, Rider plugin): the Rider backend reconstructs each deployed assembly's
  Roslyn compilation from its captured csc arguments, emits EnC deltas on save, applies them through
  its Mono soft-debugger bridge, and synchronizes Rider's PDB/line mappings. The tiny
  `SkeleKit.iOS/App/HotReload.cs` listener receives only a UI-refresh signal and calls
  `PageHost.ReloadLive`, rebuilding live pages from their registry factories while retaining view
  models. It never transports or applies deltas. `SkeleKit.iOS/build/SkeleKit.iOS.targets` gates the
  simulator prerequisites behind `EnableHotReload=true`: `UseInterpreter=true`, a
  `MetadataUpdater.IsSupported=true` runtime option with **`Trim="true"`**, and
  `DOTNET_MODIFIABLE_ASSEMBLIES=debug`. Physical-device builds and Release builds stay untouched.
- **Styling** (neutral, unit-tested): `Style<T>` (+`BasedOn`) applies in `View.Style`'s setter;
  `Theme` (`UseTheme`) applies in the `View` base ctor. Precedence is C# construction order (field
  inits → theme → `Style` → local). Plain statics, no `ResourceDictionary` (ADR-008).
- **CollectionView<TItem>** over `UICollectionView` + `UICollectionViewDiffableDataSource` (UIKit
  diffs). `ItemView<TItem>` built once per recycled cell, rebound on reuse. `CollectionLayout.List/
  Grid/Carousel`, sections, `ItemCommand`, `EmptyView`, swipe/context/reorder/prefetch. One
  cached `ItemKey` per item (roots the peers); snapshots coalesce onto the next run-loop turn.

Hard-won rules (load-bearing — don't relearn):
- **Native peers must be rooted** (see Conventions) — caused the black-screen/SIGABRT bug.
- **Measure is cached per available-size** (`measureValid` + `lastAvailable`). `InvalidateMeasure()`
  clears the flag up the ancestor chain (stopping at an already-stale parent) and asks the root host
  for one pass.
- **A `Panel` diffs its native subviews** on a `Children` change (add/remove/move only). Re-inserting
  an unchanged subview makes a focused `UITextField` resign first responder.
- **A `Panel` lays out its own content in `ArrangeOverride`**, not waiting for UIKit — UIKit only
  calls `LayoutSubviews` when bounds change, so `ScrollView` otherwise kept stale content.
- **We own every scroll-view inset** (`ContentInsetAdjustmentBehavior = Never`): `Always` insets
  across the scroll axis, `ScrollableAxes` drops the cross-axis inset — both wrong. A view escapes
  the safe area with `IgnoresSafeArea`; a scrolling view turns that bleed into a content inset along
  its scroll axis. `ContentView.ScrollsUnderBars` bleeds a scrolling root vertically.
- **A list configuration paints its own opaque background** over `backgroundView` — set it clear or
  the `EmptyView` is invisible.
- **`UIViewPropertyAnimator` cannot host an interactive animation** (scrubbed fraction doesn't
  survive `continueAnimation`, timing params reset `isReversed`, `fractionComplete` is time not
  position). `Animator` owns the loop instead (ADR-010): `AnimationCapture` snapshots both ends,
  `Motion` (neutral, unit-tested) integrates a damped spring/curve, a `CADisplayLink` writes the
  lerped `ViewState` into the *model* each frame via `View.Apply`. `View.Animate` stays
  `UIView.AnimateNotify` (fire-and-forget only).
- **`ApplyFrame` positions by bounds+center, never `Frame`** (undefined under a transform). The
  bounds *origin* is preserved — it is a `UIScrollView`'s content offset.
- **A safe-area-only change lays nothing out** (manual frames don't follow safe-area guides).
  `PageHost` overrides `ViewSafeAreaInsetsDidChange` → `SetNeedsLayout` + `LayoutIfNeeded`;
  `ScrollView.ApplyContentInsets` re-anchors a scroll resting at the top when its owned insets change.
- **iOS 26 chrome has extra paths.** Pop is two gestures (disable both to intercept). Glass bar
  buttons need item-level tints; a `UIButtonConfiguration` paints from its own `BaseForegroundColor`,
  never the view tint. With a visible tab bar, `BottomToolbarItems` render as the tab-bar accessory.
- **A fill sublayer needs `ZPosition = -1`** — sublayer order isn't stable against subview layers, so
  a gradient can land above (and swallow) a label.
- **Clipping and a shadow are mutually exclusive on one layer.** A `Shadow` turns off the implicit
  `CornerRadius` clip; an explicit `ClipsToBounds` still wins. To round *and* cast: shadow on an
  outer view, radius on the inner one.
- **`Bindable<T>` can't take an interface `T`** (C# forbids user-defined conversions from
  interfaces) → list sources are `BindableList<T>` (implicit conversions from the concrete list
  types + binding expressions). Mutations animate only for `INotifyCollectionChanged` sources (WPF
  semantics). User-defined conversions don't chain → `Image.Source` needs `ImageSource.Symbol(...)`.

Known debt:
- DI (`Microsoft.Extensions.DependencyInjection`) is the only reflection surface; the Release device
  publish is clean (0 IL2xxx), but runtime resolve on device is still the real proof.
- v1 non-goals: animation *framework*, RTL, XAML; styling stops at ADR-008 (no state-based styles, no
  runtime theme switching past light/dark, no per-subtree themes). Accessibility custom actions
  unwrapped.
- Remaining validation: the Velura two-screen port (acceptance test), the on-device 120 Hz + runtime
  DI checks. No LICENSE file yet.
