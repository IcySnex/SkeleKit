
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
- Release build on device (Mono full AOT + full trim; settings live in the Gallery csproj under
  `Release`+`ios-arm64`, so Rider picks them up by switching the config dropdown to Release with a
  device selected): `dotnet publish Samples/BareUI.Gallery -p:PublishProfile=iOS-Device`, then
  `xcrun devicectl device install app --device <UDID> <path>.app`. Watch for `IL2xxx` warnings.
- **Iterating on the sim:** after editing library code, rebuild the whole app (a bare
  `-t:Run` can relaunch a *stale* binary). Sanity-check the bundle's `BareUI.iOS.dll`
  mtime vs your edits. `simctl launch` no-ops if the app is already running — `simctl
  terminate <UDID> com.bareui.gallery` first, then `install` the fresh `.app` + `launch`.
- **Incremental builds do not recopy `Info.plist`.** A plist edit silently does nothing until you
  `rm -rf Samples/BareUI.Gallery/bin obj`. This cost an hour: the bundle still named the deleted
  `SceneDelegate`, so no window was made and the app launched to a black screen with no crash.

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
- Everything must be AOT/trim-safe: no reflection, no `Expression<>`, no assembly scanning.
  iOS device builds are **Mono full AOT** + trimmed (the platform forbids JIT).
  `IsAotCompatible=true` keeps analyzers on. **`PublishAot`/NativeAOT does not exist for iOS** —
  ILCompiler ships no `ios-*` RID (`NETSDK1203`); Velura sets it, but it is inert.
- Public API stays UIKit-free except explicit escape hatches (`NativeView`, `.Native`).

## Status (2026-07-11)

**M0–M4 complete.** Commits land on `main` (linear history). Next: **M5 — `CollectionView`**.

Shape of the thing now:
- **Element model**: `View` (lazy realize, WPF measure/arrange, `Parent`, `InvalidateMeasure`),
  `Panel`/`ViewCollection`, `LayoutHost`. Property pipeline is `Set(ref field, value, apply)`;
  `CreateNative` only *constructs*, every property is pushed through one `ApplyProperties()` hook.
- **Panels**: `StackPanel` (+`VStack`/`HStack`), `Grid` (auto/star/px + spans + spacing), `Overlay`,
  `Border`, `ScrollView` (+`KeyboardDismiss`). Layout math unit-tested in the neutral TFM.
- **Controls**: `Label`, `Button`, `Image`, `TextField`/`SecureField`/`TextEditor`, `Switch`,
  `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker`, `NativeView`.
  All expose `Bindable<T>` properties and push target→source from their own native events.
- **Bindings** (neutral, unit-tested): `Bindable<T>`, `BindingExpression<T>`, `Binding<T>` runtime,
  `BindingContext` inherited down the tree, `BindingFactory.Bind/BindPath`. Compiled getter
  delegates + `[CallerArgumentExpression]` paths, per-segment INPC subscription, zero reflection.
  Commands are bindable (`Bindable<ICommand?>`).
- **App model**: `BareApp.Create().UseServices(...).UsePages(...).Tabs(...).Run(args)`.
  `ContentView<TVm>` composes its tree in the **constructor** (XAML-compatible); `PageHost` is the
  hidden `UIViewController`. `INavigator` = push/pop/present + alert/confirm/action sheet,
  **ViewModel-first only**. Registration is one path: `UsePages` (`AddTransient`/`AddSingleton`).
- **Gallery**: `Program.cs` + `Views/ViewModels/Models/Services`, CommunityToolkit.Mvvm VMs, zero
  UIKit outside `NativeViewDemo`.

Hard-won rules (don't relearn these):
- **Native peers must be rooted** — see the convention above. It caused the black-screen/SIGABRT bug.
- **Measure is cached per available-size** (`View.measureValid` + `lastAvailable`).
  `InvalidateMeasure()` clears the flag up the ancestor chain (stopping at an already-stale parent)
  and asks the **root** host for one layout pass.
- **A `Panel` must lay out its own content, not wait for UIKit.** UIKit only calls `LayoutSubviews`
  on a view whose bounds changed, so `ScrollView` kept stale content after a binding update — it now
  arranges its content in `ArrangeOverride`.
- **`Bindable<T>` can't take an interface `T`** (C# forbids user-defined conversions from
  interfaces) → `Picker.Items` stays plain; literals need `Bindable.From<ICommand?>(cmd)`.
- User-defined conversions don't chain → `Image.Source` needs `ImageSource.Symbol(...)`/`Url(...)`.
- `[RelayCommand]` generates `IRelayCommand`, and `Bindable<T>` isn't covariant → bind with an
  explicit type arg: `Bind<ICommand?>(vm => vm.SaveCommand)`.

Known debt, roughly in priority order:
- `Panel.RealizeChildren` rebuilds all native subviews on any `Children` change (no diffing).
- `MenuView`/`PickerDemo` still need `OnViewModelAttached` (lists + `Picker.Items`). `CollectionView`
  + `ItemsSource` should kill the first.
- DI (`Microsoft.Extensions.DependencyInjection`) is the only reflection surface in the stack —
  re-check it under `MtouchLink=Full` before shipping.
- `Image.Loader` is static mutable global state (make it `BareApp` config?).
- M4 leftovers: `ToolbarItems`, large-title-on-scroll, iPadOS sidebar.
- Packaging: the `net10.0` TFM is a test shim but would still ship as a hollow lib — exclude from
  `pack` before publishing.
