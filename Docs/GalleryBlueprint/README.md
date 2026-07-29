# SkeleKit gallery blueprint

Clean-slate, visualization-first source of truth for a future control gallery. It inventories the library and its consumer tooling; it does not depend on or modify the existing Gallery app.

## Baseline and conventions

- Compiled XML baseline: **155 documented types** and **944 documented declared members** (**1099 symbols total**).
- Reconciled compiled inventory: **156 accessible types** and **1202 declared fields/properties/events/constructors/methods** after metadata-only symbols are added.
- Platform floor: **iOS 18.0**. Availability notes in an API row narrow that floor.
- A canonical page owns each type. `coverage.md` is the machine-checkable assignment ledger.
- Declared API tables use exact XML documentation IDs so overloads, generic arity, operators, conversions, and parameter types remain unambiguous.
- `Bindable<T>` means literal-or-binding input. Actual defaults come from property/backing-field initializers; `C# default` means zero, false, null, or the zero-valued struct/enum.
- `Visual/interaction only` means the implementation explicitly passes `affectsMeasure: false`; `Invalidates measure` means it uses the normal `Set`/`Register` path.
- UIKit is intentionally absent from normal consumer APIs except documented escape hatches.

## Inheritance map

```text
View
├── Panel ── Border / Grid / Overlay / StackPanel
├── ScrollView
├── Control ── native controls
├── ContentView ── ContentView<TViewModel>
└── ItemView<TItem>
```

## Navigate

### Shared

- [View](shared/view.md) — Visual showcase + interactive lab
- [Panels, pages, and item views](shared/panels-pages-items.md) — Visual showcase + advanced escape hatch
- [Binding](shared/binding.md) — Interactive lab + code-only reference
- [Styling and animation](shared/styling-animation.md) — Visual showcase + interactive lab

### Layout

- [Border](layout/border.md) — Visual showcase
- [Grid and grid extensions](layout/grid.md) — Visual showcase
- [Overlay](layout/overlay.md) — Visual showcase
- [ScrollView](layout/scroll-view.md) — Visual showcase + interactive lab
- [StackPanel](layout/stack-panel.md) — Visual showcase

### Controls

- [Label](controls/text-input/label.md) — Visual showcase
- [TextView](controls/text-input/text-view.md) — Shared visual reference
- [TextField](controls/text-input/text-field.md) — Visual showcase + interactive lab
- [SecureField](controls/text-input/secure-field.md) — Visual showcase + interactive lab
- [TextEditor](controls/text-input/text-editor.md) — Visual showcase + interactive lab
- [Actions and selection](controls/actions-selection/actions-selection.md) — Visual showcase + interactive lab
- [Values and status](controls/values-status/values-status.md) — Visual showcase + interactive lab
- [Media and native content](controls/media-content/media-content.md) — Visual showcase + interactive lab + escape hatch
- [Collections](controls/collections/collections.md) — Visual showcase + interactive lab

### Application

- [Application, shell, and services](application/application.md) — Interactive lab + non-gallery reference

### Primitives

- [Colors and brushes](primitives/colors-brushes.md) — Visual showcase
- [Geometry and layout values](primitives/geometry-layout.md) — Visual showcase + code-only reference
- [Typography and text input](primitives/typography-text.md) — Visual showcase + interactive lab
- [Gestures, menus, and actions](primitives/gestures-menus-actions.md) — Interactive lab
- [Images, maps, and collection configuration](primitives/images-maps-collections.md) — Visual showcase + interactive lab
- [Page, toolbar, and tab primitives](primitives/page-tabs.md) — Interactive lab + non-gallery reference

### Tooling

- [Generator, build, and Rider tooling](tooling/tooling.md) — Code-only/non-gallery

### Appendix

- [Implementation-shaped exports](appendix/implementation-surface.md) — Non-gallery

- [Coverage ledger](coverage.md)
- [Escape hatches](appendix/escape-hatches.md)
- [API findings](appendix/api-findings.md)
- [Verification report](appendix/verification.md)
