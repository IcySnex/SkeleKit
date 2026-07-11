
# BareUI.iOS

Declarative, WPF-inspired UI library for .NET for iOS (no MAUI, no XAML). Native UIKit
controls behind C# object-initializer syntax with AOT-safe MVVM bindings. App code never
touches `UIViewController`/`NSLayoutConstraint`.

**Read first:** `PLAN.md` (milestones), `Docs/architecture.md` (5-layer design),
`Docs/api-sketch.md` (target syntax), `Docs/decisions.md` (ADRs — binding/layout rationale).

**Reference app:** `../Velura` — the messy UIKit app this library exists to clean up.
Acceptance target: rewrite its screens with zero UIKit imports.

## Structure

- `BareUI.iOS/` — the library. Multi-targets `net10.0;net10.0-ios`, but **iOS is the only
  platform**: `net10.0` is a test shim so the layout engine unit-tests without a simulator.
  Root namespace is `BareUI` (not `BareUI.iOS`). Folders: `Primitives/` (structs+enums),
  `Elements/` (`View`, `Panel`, `ViewCollection`, `LayoutHost`), `Layout/` (panels),
  `Controls/` (native wrappers).
- `BareUI.Tests/` — xunit, plain `net10.0`, references the neutral TFM. Layout
  engine must stay testable here without a simulator.
- `Samples/BareUI.Gallery/` — iOS sample app for on-the-fly testing/debugging. Currently
  a manual scene-based UIKit bootstrap; gets replaced by `BareApp` in M4.

## Commands

- Test: `dotnet test BareUI.Tests`
- Build app: `dotnet build Samples/BareUI.Gallery -p:RuntimeIdentifier=iossimulator-arm64`
- Run app: add `-t:Run "-p:_DeviceName=:v2:udid=<UDID>"` (UDIDs: `xcrun simctl list devices available`)
- Screenshot to verify layout: `xcrun simctl io <UDID> screenshot out.png` then Read it.
- **Iterating on the sim:** after editing library code, rebuild the whole app (a bare
  `-t:Run` can relaunch a *stale* binary). Sanity-check the bundle's `BareUI.iOS.dll`
  mtime vs your edits. `simctl launch` no-ops if the app is already running — `simctl
  terminate <UDID> com.bareui.gallery` first, then `install` the fresh `.app` + `launch`.

## Environment gotchas

- dotnet is **brew-managed** (`/opt/homebrew/bin/dotnet`, SDK 10.0.3xx). The old install
  at `/usr/local/share/dotnet` is stale — don't use it.
- `ValidateXcodeVersion=false` in `Directory.Build.props`: installed Xcode (26.6) is newer
  than the newest .NET for iOS release supports (26.5). Remove once Microsoft ships
  Xcode 26.6 support.

## Conventions

- Tabs, file-scoped namespaces, each ctor/method parameter on its own line (matches Velura style).
- Doc comments: standard `/// <summary>` block, summary text on **one line** (never wrap into
  multiple `///` lines). Keep them terse.
- Inline `//` comments: short fragments, lowercase, only when the code can't say it itself. No
  full-sentence prose, no multi-line blocks, no explaining a bugfix inline — that goes in the
  commit body.
- Omit redundant modifiers/types: no `private` where it's already the default; target-typed
  `new(...)` (no redundant type name); collection expressions `[]`. Matches Velura.
- **No `#if IOS`. Ever.** The library has zero preprocessor directives and stays that way. A
  wholly-iOS file goes in `Controls/` (or is named in the csproj's `net10.0` `Compile Remove`
  glob) and just uses UIKit directly. A file that mixes layout math with UIKit splits: neutral
  half in `Foo.cs`, native half in `Foo.iOS.cs` (`partial`, excluded from `net10.0` by glob).
  Neutral code calls into native via `partial void` hooks (see `View.ApplyFrame`,
  `Panel.OnChildrenChanged`).
- **Native peers must be rooted.** Every `NSObject` subclass we define needs (a) a
  `(NativeHandle handle)` ctor and (b) something *managed* holding it for as long as UIKit holds
  the native object. UIKit's own retain does **not** keep the managed peer alive — the GC will
  take it and the app either aborts in the marshaller or silently stops laying out (black screen).
  Beware weak native refs (`UINavigationController.Delegate`). Don't use `NSTimer` +
  `Action` or the default `NSUrlSessionHandler`: both have peers that die the same way.
- Commits: Conventional Commits, subject ≤50 chars, body only when the why isn't obvious.
- Everything must be AOT/trim-safe: no reflection, no `Expression<>`, no assembly scanning
  (consumer ships `PublishAot=true`). `IsAotCompatible=true` keeps analyzers on.
- Public API stays UIKit-free except explicit escape hatches (`NativeView`, `.Native`).

## Status (2026-07-11)

**M0 + M1 + M2 complete. M3 (bindings) code-complete, exit criteria not yet verified.**
Commits land on `main` (repo history is linear there).

M3 so far:
- Property pipeline: `View.Parent`, `InvalidateMeasure()` (bubbles to root → `SetNeedsLayout`),
  and the `Set(ref field, value, apply)` helper. `CreateNative` now only *constructs*; every
  property is pushed through the single `ApplyProperties()` hook (kills the old two-writer split).
- Bindings (all neutral, unit-tested): `Bindable<T>`, `BindingExpression<T>`, `Binding<T>` runtime,
  `BindingContext` on `View` (inherited down the tree), `BindingFactory.Bind/BindPath`. Compiled
  getter delegates + `[CallerArgumentExpression]` path parsing, per-segment INPC subscription,
  zero reflection. Modes + `UpdateTrigger` (TextField/TextEditor honour `FocusLost`).
- Every control exposes `Bindable<T>` properties and pushes target→source from its native event.
  `Button` does `ICommand` + `CanExecuteChanged`→enabled; `TapCommand`/`IsEnabled` on any `View`.
- Minimal `ContentView<TViewModel>` (typed `ViewModel` + protected `Bind`); M4 adds lifecycle,
  chrome and controller hosting. Gallery `BindingPage` demos one-way/two-way/converter/command.

M3 gotchas found:
- `Bindable<T>` can't take an interface `T` (C# forbids user-defined conversions from interfaces)
  → `Picker.Items` stays a plain property.
- User-defined conversions don't chain → `Image.Source` needs `ImageSource.Symbol(...)`/`Url(...)`,
  not a bare string literal.

**M3 exit criteria still open:** simulator check of `BindingPage`, and a Gallery publish with
`PublishAot=true` (proves the AOT claim end to end).

Done:
- Primitives: `Thickness`, `Size`, `Point`, `Rect`, `GridLength`, `Color`, alignment enums.
- `View` base (lazy realize, WPF measure/arrange), `Panel`/`ViewCollection`, `LayoutHost`.
- Panels: `StackPanel` (+`VStack`/`HStack`), `Grid` (auto/star/px + spans + spacing), `Overlay`,
  `Border`, `ScrollView`. Layout math unit-tested in neutral TFM (41 tests green).
- First controls: `Control` base (measures via native `SizeThatFits`) + `Label`.
- Gallery `MovieInfoPage` reproduces Velura's MovieInfo top section in pure BareUI = **M1 exit,
  verified on simulator**. Hosted by a temporary `BareHostController` (replaced by `BareApp` in M4).

- M2 controls (all thin native wrappers, verified on simulator via Gallery pages): `Button`
  (UIButtonConfiguration styles, SF-symbol `Icon`, plain `ICommand` + CanExecuteChanged→enabled),
  `Image` (`ImageSource` struct — Url/Bundle/Symbol/Auto, implicit from string; pluggable static
  `Image.Loader : IImageLoader`, cancellable async URL loads), `TextField`/`SecureField`/`TextEditor`,
  `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker`
  (UIButton+UIMenu), `NativeView` escape hatch (`OwnsNative=false` — caller-owned view not disposed).
  Pre-M3 pattern: properties are create-only (applied in `CreateNative`); interactive controls sync
  native→managed and expose settable `Action` callback props (`Toggled`, `TextChanged`, `Clicked`, …).
- All primitives now live in `namespace BareUI` (`BareUI.Primitives` namespace removed; folder kept).
- Gallery: `UINavigationController` shell, `MenuPage` + 13 demo pages (in `Pages/`, shared caption
  scaffold on `Demo`), `GALLERY_PAGE` env var (via `SIMCTL_CHILD_GALLERY_PAGE`) auto-pushes a page
  on launch — use it for screenshots.
- **Native-peer lifetime bug fixed** (was: random black screen / SIGABRT after navigating). `LayoutHost`,
  `ScrollHost`, `BareHostController` had no `(NativeHandle)` ctor and no managed root, so the GC
  collected their peers while UIKit still held the native objects. Fixed by rooting + ctors; the image
  loader moved to `SocketsHttpHandler` (the default `NSUrlSessionHandler`'s delegate peer dies
  mid-redirect). See the peer-rooting convention above — M4's `BareApp` must keep doing this.
- Library is now **`#if`-free**: iOS-only files excluded from `net10.0` by csproj glob, mixed files
  split into `Foo.cs` + `Foo.iOS.cs` partials.

Known shortcuts to revisit:
- Safe-area is handled ad-hoc by `BareHostController` (sets host frame to the safe-area guide);
  the planned `SafeAreaEdges`-in-arrange support (enum exists) is not wired into the engine yet.
- Layout has no dirty-flag/invalidation yet — `LayoutHost.LayoutSubviews` re-measures the whole
  subtree each pass. Fine for now; optimize when a screen gets heavy.
- `Panel.RealizeChildren` rebuilds all native subviews on any `Children` change (no diffing).

- Reviewer follow-up still open: `Image.Loader` is static mutable global state (decide in M3/M4 if
  it becomes `BareApp` config).
- Packaging: the `net10.0` TFM is a test shim but would still ship in a NuGet package as a hollow
  lib. Exclude it from `pack` before publishing.
- M3 structural note from review: `CreateNative` and `View.ApplyVisualState` are two uncoordinated
  writers of native state (an `Image` clipping bug came from this). M3 should introduce one
  property-application pipeline (base visual state, then control state).

Next — M3 bindings (see PLAN.md). M2 is committed on `main`.
