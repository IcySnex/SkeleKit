# BareUI.iOS

Declarative, WPF-inspired UI library for .NET for iOS (no MAUI, no XAML). Native UIKit
controls behind C# object-initializer syntax with AOT-safe MVVM bindings. App code never
touches `UIViewController`/`NSLayoutConstraint`.

**Read first:** `PLAN.md` (milestones), `Docs/architecture.md` (5-layer design),
`Docs/api-sketch.md` (target syntax), `Docs/decisions.md` (ADRs — binding/layout rationale).

**Reference app:** `../Velura` — the messy UIKit app this library exists to clean up.
Acceptance target: rewrite its screens with zero UIKit imports.

## Structure

- `BareUI.iOS/` — the library. Multi-targets `net10.0;net10.0-ios`: layout math and
  primitives live in neutral code, UIKit-touching code behind `#if IOS`. Root namespace
  is `BareUI` (not `BareUI.iOS`).
- `BareUI.Tests/` — xunit, plain `net10.0`, references the neutral TFM. Layout
  engine must stay testable here without a simulator.
- `Samples/BareUI.Gallery/` — iOS sample app for on-the-fly testing/debugging. Currently
  a manual scene-based UIKit bootstrap; gets replaced by `BareApp` in M4.

## Commands

- Test: `dotnet test BareUI.Tests`
- Build app: `dotnet build Samples/BareUI.Gallery -p:RuntimeIdentifier=iossimulator-arm64`
- Run app: add `-t:Run "-p:_DeviceName=:v2:udid=<UDID>"` (UDIDs: `xcrun simctl list devices available`)

## Environment gotchas

- dotnet is **brew-managed** (`/opt/homebrew/bin/dotnet`, SDK 10.0.3xx). The old install
  at `/usr/local/share/dotnet` is stale — don't use it.
- `ValidateXcodeVersion=false` in `Directory.Build.props`: installed Xcode (26.6) is newer
  than the newest .NET for iOS release supports (26.5). Remove once Microsoft ships
  Xcode 26.6 support.

## Conventions

- Tabs, file-scoped namespaces, each ctor/method parameter on its own line (matches Velura style).
- Doc comments: standard `/// <summary>` block, summary text on **one line** (never wrap into
  multiple `///` lines). Keep them terse. Non-public helpers use a plain `//` line.
- Omit redundant modifiers/types: no `private` where it's already the default; target-typed
  `new(...)` (no redundant type name); collection expressions `[]`. Matches Velura.
- **Single platform.** The neutral `net10.0` TFM exists *only* so the layout engine unit-tests
  without a simulator — this is not a cross-platform library. Put UIKit code inline behind
  `#if IOS` in the same file; do **not** split into separate `.iOS.cs` partial files.
- Commits: Conventional Commits, subject ≤50 chars, body only when the why isn't obvious.
- Everything must be AOT/trim-safe: no reflection, no `Expression<>`, no assembly scanning
  (consumer ships `PublishAot=true`). `IsAotCompatible=true` keeps analyzers on.
- Public API stays UIKit-free except explicit escape hatches (`NativeView`, `.Native`).

## Status (2026-07-11)

Done — M0 scaffold + most of M1:
- Solution, three projects, Gallery runs on simulator. Toolchain verified.
- Primitives: `Thickness`, `Size`, `Point`, `Rect`, `GridLength`, `Color`, alignment enums.
- `View` base (lazy realize, WPF measure/arrange), `Panel`/`ViewCollection`, `LayoutHost`.
- Panels: `StackPanel` (+`VStack`/`HStack`), `Grid` (auto/star/px + spans), `Overlay`, `Border`,
  `ScrollView`. Layout math unit-tested in neutral TFM (41 tests green).

Next — finish M1:
- Gallery page reproducing Velura `MovieInfoViewController` top section (poster+title+info) in
  pure BareUI = M1 exit criteria. Then M2 controls (`Label`, `Image`, `Button`, …) so the page
  has real content to lay out.
