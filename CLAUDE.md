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
- Commits: Conventional Commits, subject ≤50 chars, body only when the why isn't obvious.
- Everything must be AOT/trim-safe: no reflection, no `Expression<>`, no assembly scanning
  (consumer ships `PublishAot=true`). `IsAotCompatible=true` keeps analyzers on.
- Public API stays UIKit-free except explicit escape hatches (`NativeView`, `.Native`).

## Status (2026-07-10)

Done — M0 scaffold:
- Solution, three projects, first primitive (`Thickness`) + tests, Gallery runs on simulator.
- Toolchain verified: brew dotnet + ios workload 26.5 + Xcode 26.6.

Next — M1 core element model + layout engine (see PLAN.md for exit criteria):
1. Neutral primitives: `Size`, `Point`, `Rect`, `GridLength`, alignment enums.
2. `View` base (wraps one `UIView`, lazy realize) + measure/arrange contract.
3. `LayoutHost : UIView` bridging `SizeThatFits`/`LayoutSubviews` to measure/arrange.
4. Panels: `StackPanel`, `Grid`, `Overlay`, `Border`, `ScrollView` — unit-test layout math in neutral TFM.
5. Gallery page reproducing Velura MovieInfo top section = M1 exit criteria.
