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

## Constraints

| Constraint | Consequence |
|---|---|
| Mono full AOT + trimming on device (no JIT allowed) | Typed-delegate bindings, explicit view registration, no assembly scanning, no `Expression<>` |
| net10.0-ios, iOS 18 minimum | Compositional layout, `UICollectionLayoutListConfiguration`, modern APIs used freely |
| Native look & feel is non-negotiable | Controls are thin wrappers; BareUI never re-implements a control's drawing |
| CommunityToolkit.Mvvm used by consuming apps | Bindings target plain `INotifyPropertyChanged` + `ICommand`; no BareUI-specific VM base class |

---

## Current state (2026-07-12)

M0–M6 delivered (M6's Velura two-screen port outstanding, see below). The library is
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
- **Packaging**: NuGet ships the iOS lib + XML docs + README; Release device publish is
  clean (zero trim/AOT warnings).

Outstanding validation (needs hardware / the reference app):

- Port two Velura screens (`SettingsGroupViewController`, simplified `MovieInfoViewController`)
  on a branch as the acceptance test; fix API friction found.
- On-device: 120 Hz scroll check, runtime DI resolve under full trim.
- Add a LICENSE before publishing the package.

---

## M7 — Styling & resources

**Problem:** a real app repeats itself — every caption is the same three property
assignments, every card the same background/corner/shadow blob. WPF solves this with
`Style`/`Setter`/`ResourceDictionary`; that machinery is reflection-shaped
(dependency properties, object-typed setters, string keys) and cannot come along. M7 is
the minimal typed equivalent: enough to build a beautiful, consistent app without
repetition, nothing more.

See **ADR-008** (styling mechanism) and the styling section of `api-sketch.md` (target syntax).

### Feature 1 — `Style<T>`: a named, reusable property block

```csharp
public sealed class Style<T> : IStyle where T : View
{
	public Style(Action<T> apply);
	public Style(IStyle basedOn, Action<T> apply);   // WPF BasedOn: base applies first
}
```

- A style is an **action over the typed control** — setters with full IntelliSense,
  compile-time checking, zero reflection. `Bindable` properties accept literals or
  `BindingFactory.Bind` expressions inside a style; each application registers a fresh
  binding, so styles are safely shared across views.
- Immutable once created. `IStyle` is the non-generic handle (`void Apply(View view)`;
  applying to a non-`T` view throws `InvalidOperationException` with both type names).
- App code convention (documented, not enforced): styles live in a static class, e.g.
  `static class Styles { public static readonly Style<Label> Caption = new(l => { ... }); }`.

### Feature 2 — explicit application: `View.Style`

```csharp
new Label { Style = Styles.Caption, Text = "Runtime" }
```

- New `IStyle? Style` property on `View`. **Applies immediately in the setter** — so
  assignments written after it in the object initializer override the style, matching WPF's
  local-beats-style precedence through nothing but statement order. Document loudly:
  *`Style` goes first in the initializer.*
- **Breaking rename**: `Button.Style` (the `ButtonStyle` enum: Plain/Tinted/Filled/…) becomes
  **`Button.Kind`**, freeing the `Style` name on `View`. Update Gallery + README + doc snippets.

### Feature 3 — implicit styles: `Theme`

```csharp
BareApp.Create()
	.UseTheme(theme => theme
		.Style(new Style<Label>(l => l.TextColor = Colors.Label))
		.Style(new Style<Button>(b => b.Kind = ButtonStyle.Tinted)))
	...
```

- One theme per app, registered before `Run` like services/pages. The registry is
  write-once at startup, read-only afterwards (same lifecycle discipline as `UsePages`;
  the registry itself is a static internal — acceptable because it is frozen before the
  first view exists).
- **Applied in the `View` base constructor**, keyed by `GetType()`, walking the inheritance
  chain **base-most first** (`Style<View>` → `Style<Control>` → `Style<Label>`). C# runs
  derived field initializers *before* base ctor bodies, so the precedence falls out of
  language semantics with no tracking:
  1. control defaults (field initializers) —
  2. implicit theme styles (base ctor) —
  3. explicit `Style` (initializer, position-dependent) —
  4. local values (initializer assignments after them).
- Per-type resolved style chains are cached (`Type` → flattened `IStyle[]`) after first
  construction; the per-view cost is one dictionary hit.
- Neutral code, fully unit-testable: precedence, chain order, BasedOn, write-once.

### Feature 4 — native text hierarchy: `Label.TextStyle`

```csharp
new Label { TextStyle = TextStyle.Headline, Text = Bind(vm => vm.Title) }
```

- Enum mapping 1:1 to `UIFontTextStyle`: `LargeTitle`, `Title1`–`Title3`, `Headline`, `Body`,
  `Callout`, `Subheadline`, `Footnote`, `Caption1`, `Caption2`. Resolves through
  `UIFont.GetPreferredFont` (correct per-style Dynamic Type curves, not one linear scale).
- `FontWeight`/`FontDesign` still compose on top; an explicit `FontSize` wins over `TextStyle`.
  This is what makes "beautiful native app" typography a one-property decision.

### Feature 5 — resources: a documented pattern, not machinery

**Decision: no `ResourceDictionary`.** In a C#-only tree, shared values are just static
members — `static class Palette { public static readonly Color Accent = ...; }` — already
typed, trim-safe, refactorable, and theme-aware via `Color.Dynamic`/semantic colors.
A string-keyed runtime lookup would re-add WPF's weakest part and solve nothing C# doesn't.
README gets a "Styling" section showing the statics + `Style<T>` + `UseTheme` pattern together.

### Out of scope for M7

- State-based styling (pressed/focused triggers) — UIKit's own control states already
  handle the native cases; revisit only with a concrete screen that needs more.
- Runtime theme *switching* beyond light/dark (`Color.Dynamic` covers dark mode; an
  accent-color swap mid-session is v2).
- Per-subtree theme overrides (a theme is app-global).
- Style serialization of any kind.

### Exit criteria

- Gallery restyled: shared `Styles`/`Palette` statics + `UseTheme`, repeated property blobs
  gone (`Theme.Caption(...)` helper replaced by a `Style<Label>`).
- Unit tests: precedence chain, inheritance-walk order, BasedOn, type-mismatch throw,
  write-once enforcement, `TextStyle`/`FontSize` interaction (measure math stays neutral-testable).
- Release device publish stays at zero trim/AOT warnings.
- README + `api-sketch.md` show the styling syntax; `Button.Kind` rename propagated everywhere.

---

## Risks

| Risk | Mitigation |
|---|---|
| Style-in-ctor timing surprises (control ctor *bodies* overwrite theme styles) | Convention: controls configure via field initializers, never ctor-body property sets; enforce in review |
| `View.Style` position-dependence confuses users | Doc + README show it first in every snippet; consider an analyzer later |
| Scope creep toward full WPF styling | Out-of-scope list above; every addition must be justified by a concrete screen |
| AOT regression via new surface | `IsAotCompatible` analyzers stay on; re-publish Release for device at milestone end |
