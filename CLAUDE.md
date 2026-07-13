
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
- Doc comments (**public API only**): always the full block form (`/// <summary>` on its own line,
  never the compact `/// <summary>X</summary>` one-liner), summary text on **one** `///` line, plus
  `<param>`/`<returns>`/`<typeparam>` tags on methods. **Internal/private members get no XML docs
  at all** — an inline `//` where the code can't say it is enough.
- Inline `//` comments: short fragments, lowercase, only when the code can't say it itself. No
  full-sentence prose, no multi-line blocks, no explaining a bugfix inline — that goes in the
  commit body.
- Omit redundant modifiers/types: no `private` where it's already the default; target-typed
  `new(...)` (no redundant type name); collection expressions `[]`. Matches Velura. Exception:
  top-level `internal` is written out explicitly.
- Prefer primary constructors, `field`-keyword semi-auto properties
  (`get; set => Set(ref field, ...)`), and expression bodies joined onto one line when short
  (`internal override bool Scrolls => true;`). Two blank lines between member groups
  (fields / properties / methods).
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

## Status (2026-07-12)

**M0–M7 complete** apart from validation that needs hardware or the reference app: the Velura
two-screen port, the on-device 120 Hz scroll + runtime-DI checks, and a LICENSE file. Commits land
on `main` (linear history). PLAN.md and the docs describe current state only — historical milestone
trackers were removed 2026-07-12.

Shape of the thing now:
- **Element model**: `View` (lazy realize, WPF measure/arrange, `Parent`, `InvalidateMeasure`),
  `Panel`/`ViewCollection`, `LayoutHost`. Property pipeline is `Set(ref field, value, apply)`;
  `CreateNative` only *constructs*, every property is pushed through one `ApplyProperties()` hook.
- **Panels**: `StackPanel` (no `VStack`/`HStack` sugar — removed 2026-07-12), `Grid` (auto/star/px + spans + spacing), `Overlay`,
  `Border`, `ScrollView` (+`KeyboardDismiss`). Layout math unit-tested in the neutral TFM.
- **Controls**: `Label`, `Button`, `Image`, `TextField`/`SecureField`/`TextEditor`, `Switch`,
  `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker`, `NativeView`.
  All expose `Bindable<T>` properties and push target→source from their own native events.
- **Bindings** (neutral, unit-tested): `Bindable<T>`, `BindingExpression<T>`, `Binding<T>` runtime,
  `BindingContext` inherited down the tree, `BindingFactory.Bind/BindPath`. Compiled getter
  delegates + `[CallerArgumentExpression]` paths, per-segment INPC subscription, zero reflection.
  Commands are **never bindable**: every intent is a plain `ICommand?` property assigned directly
  from the ctor-injected ViewModel (ADR-012); `Command.From(action)` wraps view-local handlers.
  Continuous streams (`Panned`, `Pinched`, `Rotated`, `Scrolled`, `TextChanged`, ...) are `Action<T>`
  properties — past tense, no `On` prefix (`On` = lifecycle overrides only).
- **App model**: `BareApp.Create().UseServices(...).UsePages(...).Tabs(...).Run(args)`.
  `ContentView<TVm>` takes its ViewModel **by constructor** (`: base(viewModel)`) and composes its
  tree against it directly — no `OnViewModelAttached`; `PageHost` is the
  hidden `UIViewController`. `INavigator` = push/pop/present + alert/confirm/action sheet,
  **ViewModel-first only**. Registration is one path: `UsePages` with factory lambdas
  (`pages.AddTransient((FooViewModel vm) => new FooView(vm))`) — reflection-free page construction.
- **Styling** (neutral, unit-tested): `Style<T>` wraps an `Action<T>` (+ `BasedOn`); `View.Style`
  applies **in its setter** (so it goes first in an initializer); `Theme` (`BareApp.UseTheme`) holds
  the app-global implicit styles and applies them **in the `View` base ctor**, chain base-most first,
  per-type chain cached. Precedence is pure C# construction order: field initializers → theme →
  explicit `Style` → local values. Resources are plain statics — no `ResourceDictionary` (ADR-008).
- **Gallery**: `Program.cs` + `Views/ViewModels/Models/Services`, CommunityToolkit.Mvvm VMs, zero
  UIKit outside `NativeViewDemo`. `Views/Palette.cs` + `Views/Styles.cs` are the styling pattern.

M5:
- `CollectionView<TItem>` over `UICollectionView` + **`UICollectionViewDiffableDataSource`** (UIKit does
  the diffing; we do not hand-roll batch updates). `ItemView<TItem>` is the cell's tree, built once per
  recycled cell and rebound on reuse. `CollectionLayout.List(grouped:)/.Grid(columns)/.Carousel(...)`,
  `Section<T>` + `HeaderTemplate`, `SelectionCommand`, `EmptyView`, `CarouselSnap` (all five native
  orthogonal behaviours).
- Perf: one cached `ItemKey` per item (no identifier allocation per snapshot, and it roots the NSObject
  peers); snapshots coalesce onto the next run-loop turn, so an `ObservableCollection.Add` loop is one
  diff, not N; off-screen collections apply without animation.

Hard-won rules (don't relearn these):
- **Native peers must be rooted** — see the convention above. It caused the black-screen/SIGABRT bug.
- **Measure is cached per available-size** (`View.measureValid` + `lastAvailable`).
  `InvalidateMeasure()` clears the flag up the ancestor chain (stopping at an already-stale parent)
  and asks the **root** host for one layout pass.
- **A `Panel` diffs its native subviews** on a `Children` change (add/remove/move only). Never
  re-insert an unchanged subview: that makes a focused `UITextField` resign first responder.
- **A `Panel` must lay out its own content, not wait for UIKit.** UIKit only calls `LayoutSubviews`
  on a view whose bounds changed, so `ScrollView` kept stale content after a binding update — it now
  arranges its content in `ArrangeOverride`.
- **We own every scroll-view inset.** `ContentInsetAdjustmentBehavior = Never` everywhere: `Always`
  insets *across* the scroll axis (a vertical list gains a horizontal scrollable range) and
  `ScrollableAxes` drops the cross-axis inset (content under the notch). Both guesses are wrong.
  A page always sits inside the safe area; a view escapes it with `IgnoresSafeArea`, and a scrolling
  view turns that bleed into a content inset along its scroll axis (so the scroll passes under the bar
  but its content never does). `ContentView.ScrollsUnderBars` bleeds a scrolling root vertically.
- **A list configuration paints its own opaque background** over `backgroundView` — set it clear or the
  `EmptyView` is invisible.
- **`UIViewPropertyAnimator` cannot host an interactive animation** — three wrapper generations
  each hit a different wall: a scrubbed fraction doesn't survive `continueAnimation` (a spring's
  curve is non-monotonic), new timing parameters silently reset `isReversed`, a running spring's
  `fractionComplete` is time not position, and replacing animators walls the scrub at the segment
  edge. `Animator` owns the loop instead (ADR-010): `AnimationCapture` snapshots both ends,
  `Motion` (neutral, unit-tested) integrates a damped spring or curve, and a `CADisplayLink`
  writes the lerped `ViewState` into the *model* each frame via `View.Apply` (bypasses `Set`'s
  equality check). Screen == shadow model by construction, so no reconcile step exists and a
  native-side revert is unrepresentable. `View.Animate` stays `UIView.AnimateNotify` —
  fire-and-forget only.
- **`ApplyFrame` positions by bounds+centre, never `Frame`** (undefined under a transform, and an
  animation can leave the native transform non-identity while the model reads none). The bounds
  *origin* is preserved — it is a `UIScrollView`'s content offset.
- **A fill sublayer needs `ZPosition = -1`.** Sublayer array order is not stable against subview
  layers — the card gradient ended up *above* the label and silently swallowed it.
- **Clipping and a shadow are mutually exclusive on one layer** (the shadow is drawn outside the
  bounds). `CornerRadius > 0` used to force `ClipsToBounds` unconditionally, which silently ate the
  shadow of every rounded card. A `Shadow` now turns that implicit clip off; an explicit
  `ClipsToBounds` still wins. To round *and* cast: shadow on an outer view, radius on the inner one.
- **`Bindable<T>` can't take an interface `T`** (C# forbids user-defined conversions from
  interfaces) → list sources (`CollectionView.ItemsSource`/`GroupedItemsSource`, `Picker.ItemsSource`)
  are typed `ObservableCollection<T>`, so `ItemsSource = ViewModel.Items` assigns plainly and change
  notification is guaranteed by the type.
- User-defined conversions don't chain → `Image.Source` needs `ImageSource.Symbol(...)`/`Url(...)`.

Framework surface (completion pass — every previously deferred item is now implemented):
- **Text**: `Label.TextStyle` (the native hierarchy: LargeTitle…Caption2, each with its own Dynamic
  Type curve, resolved by `GetPreferredFont`), `FontWeight` (9 weights), `FontDesign`
  (system/rounded/serif/mono), `Truncation`, `MaxLines`. All `UIFontMetrics`-scaled, so Dynamic Type
  works. `FontSize` defaults to **NaN** = "follow `TextStyle`" (17 without one); an explicit size
  always beats a text style, weight and design compose on top of whichever wins.
- **Visual**: `View.Shadow`, `CornerRadius`, `Opacity`, `ClipsToBounds`. **Dark mode**: `Colors`
  palette + semantics (`Label`/`Separator`/backgrounds) resolve live UIKit colors;
  `Color.Dynamic(light, dark)`; `WithAlpha` flattens a system color to its light value. CGColor
  snapshots (border stroke, shadow) re-resolve via the `ReapplyVisuals` walk that `PageHost`
  triggers on a `UITraitUserInterfaceStyle` change (`CollectionView` forwards it into live cells).
- **Bindings**: all four modes (`BindToSource` = OneWayToSource, `BindOnce` = OneTime), converters
  both ways (`format:`/`parse:`), `UpdateTrigger.FocusLost`. `View.Focus()/Unfocus()/IsFocused`.
  Off-main-thread INPC is marshalled to the main thread (`MainThread.Post`; inline in the shim).
- **Accessibility**: `View.AccessibilityLabel`/`AccessibilityValue` (bindable),
  `AccessibilityHint`, `AccessibilityIdentifier`, `AccessibilityTraits` (flags, OR'd onto the
  control's own), `IsAccessibilityElement` (null = control default).
- **Images**: default loader has an `NSCache` (64 MB, cost = decoded bytes), one download per
  url no matter how many cells ask, and pre-decodes via `PrepareForDisplayAsync`.
- **Page chrome**: `ToolbarItems`, `TitleStyle.Large`, `HidesNavigationBar`, `BackgroundStyle`,
  `SearchPlaceholder`/`SearchChanged`; lifecycle `OnLoaded`/`OnUnloaded` alongside
  `OnAppearing`/`OnDisappearing`; `ContentView.Controller` escape hatch.
- **Lists**: pull-to-refresh, native swipe actions, context menus, `ScrollTo(item)`, `Scrolled`.
- **Shell**: `Tabs`/`Stack`/`SinglePage`, `SidebarOnIPad()`.
- **Misc**: `Haptics`, `View.Animate`, `View.AddGesture`, `BareApp.UseImageLoader` (no more static
  mutable loader).
- `Picker<TItem>` is typed: `ItemsSource`/`SelectedItem`, not `Items`/`SelectedIndex`.
- Package ships **iOS only**; the `net10.0` shim is excluded from `pack` (`IncludeBuildOutput=false`).

Known debt:
- DI (`Microsoft.Extensions.DependencyInjection`) is the only reflection surface in the stack —
  the Release device publish is clean (zero IL2xxx), but resolve-at-runtime on device is still
  the real proof.
- v1 non-goals (see PLAN): animation *framework*, RTL, XAML; styling stops at ADR-008 (no
  state-based styles, no runtime theme switching past light/dark, no per-subtree themes).
  Accessibility custom actions still unwrapped.
- M6 remaining: the Velura two-screen port (the acceptance test) and the on-device 120 Hz check.
- No LICENSE file yet — needed before the package is published.
