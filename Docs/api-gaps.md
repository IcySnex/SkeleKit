# API gaps — UIKit capability not yet surfaced

An audit of the wrapped controls against what their native counterparts actually offer
(2026-07-12). Not a to-do list: strike what you don't want, the rest becomes milestones.

Legend: ★ quick win (hours, additive) · ◆ medium (a day-ish, some design) · ▲ large (own milestone).

## Page & navigation chrome

- ~~★ **Leading toolbar item next to the back button**~~ — **done** (`LeftItemsSupplementBackButton`
  always on).
- ~~◆ **Large-title collapse driven by a BareUI scroll**~~ — **done** (`SetContentScrollView`
  wires the page's root scroll to the bar).
- ~~★ **Back button title / display mode**~~ — **done** (`ContentView.BackButtonTitle` +
  `BackButtonStyle`).
- ~~◆ **Pop interception**~~ — **done** (`ContentView.ConfirmLeave`: back button, both pop
  gestures (iOS 26 adds a content-wide one), and sheet swipe-down all funnel through it; a modal
  root's synthesized back button dismisses).
- ~~★ **Toolbar item pull-down menus**~~ — **done** (`ToolbarItem.Menu`, `MenuAction` list).
- ~~★ **Nav bar tint & title attributes**~~ — **done** (`BarAccent`, `TitleColor`,
  `LargeTitleColor`; per-item appearances copied from the live bar, item-level tints for iOS 26
  glass buttons).
- ~~★ **`NavigationItem.Prompt`**~~ — **done** (`ContentView.Prompt`).
- ~~★ **Bottom toolbar**~~ — **done** (`ContentView.BottomToolbarItems`: `UIToolbar` when the
  bottom edge is free, floats as the iOS 26 tab-bar accessory when the tab bar is visible).
  **Revisit:** the accessory path repurposes `UITabBarController.BottomAccessory` (Apple's
  app-global mini-player slot) per page, ignores `IsPrimary`/`Side`, and hand-wires `UIButton`s —
  a true shell-level `Tabs.Accessory(...)` would collide with it. Kevin unhappy with the
  conflation; redesign candidate.
- ~~★ **Search**~~ — **done** (`HidesSearchBarWhenScrolling`, `SearchScopes` +
  `SearchScopeChanged`, `SearchCancelled`, `SearchObscuresBackground`).
- ~~◆ **Search suggestions**~~ — **done** (`ContentView.SearchSuggestions`, replaced live from
  `SearchChanged`; tapping one invokes `SearchSuggestionCommand` with the `SearchSuggestion`.
  The items and the `UISearchResultsUpdating` are rooted on the host — both are weak natively).
- ~~★ **Status bar style per page**~~ — **done** (`ContentView.StatusBar`, `BareStack` forwards).
- ~~★ **Tab badges**~~ — **done** (`ContentView.TabBadge` bindable + `TabBadgeColor`; applies to
  never-opened tabs too).
- ~~★ **Hide tab bar on push**~~ — **done** (`ContentView.HidesTabBar`).
- ~~◆ **Sheet polish**~~ — **done**, but not as written. The real gap was that a sheet only ever got
  *one* detent, so it could never be dragged: `ModalStyle.Sheet(Detent.Medium, Detent.Large)` now
  opens half and grabs to full. The grabber follows from that (shown iff the sheet resizes) rather
  than being a knob. Corner radius, `LargestUndimmedDetent` and
  `PrefersScrollingExpandsWhenScrolledToEdge` are **declined** as API for a screenshot. Dismiss
  prevention already exists — `ContentView.ConfirmLeave` funnels the sheet's swipe-down through the
  same guard as the back button.
- ★ (skip) **Custom detent heights** — `UISheetPresentationControllerDetent.Create(id, resolver)`
  takes any height (a mini-player peek, a Maps-style three-stop sheet). Declined: `Detent` would have
  to stop being an enum, and the resolver is another NSObject peer to root — too niche for the cost.
- ~~◆ **Popover anchoring**~~ — **done** (`ModalStyle.Popover(anchor, arrows)` — the static
  property became a method; the anchor is a `View`, so a popover from a toolbar *item* still has
  no spelling. The style carries the anchor through the ViewModel's `PresentAsync` untouched).
  A delegate blocks UIKit's compact-width adaptation, so it stays a bubble on iPhone; that wins
  over `ConfirmLeave` on the same page — a popover has no dismiss swipe for the guard to catch.
- ★ (skip) **Share sheet** — `INavigator.ShareAsync(items)` over `UIActivityViewController`.
- ★ (skip) **Open URL in-app** — `SFSafariViewController` wrapper on the navigator.
- ~~◆ **Alert with text input**~~ — **done** (`INavigator.PromptAsync`, returns the typed string or
  null when cancelled).

## View (every element)

- ★ (skip) **Border on any view** — `layer.borderWidth/borderColor`; today only the `Border` panel strokes.
- ★ (skip) **Per-corner rounding + continuous curve** — `MaskedCorners` and
  `CornerCurve = .continuous` (the Apple squircle — today's radius is the "cheap" circular look).
- ~~★ **TintColor**~~ — **done** (`View.Tint`, an *inherited* property in the WPF sense: the getter
  walks up to the parent unless set locally, the way SwiftUI's `.tint()` environment value does,
  ending at the `UseAccent` color). UIKit's own `tintColor` inheritance only reaches the controls
  that read it — a `UISwitch` fill, a `UIActivityIndicatorView` and a `UIButton` configuration each
  paint from their own color and never see it, so they fall back to `Tint` when the app sets none
  of their own. A tint change walks the subtree (a plain virtual-method walk, no events; `Panel`
  recurses skipping locally-tinted branches, `CollectionView` forwards into live cells, whose trees
  inherit across the cell boundary via `View.TintHost`). An `Image` templates a raster only from a
  *locally* set tint (`View.LocalTint`) — an inherited one would flatten a photo.
- ~~★ **Typed gestures beyond pan/tap**~~ — **done** (`OnLongPress`, `OnDoubleTap`, `OnPinch`,
  `OnRotate`).
- ◆ (skip) **Pointer/hover effects (iPad)** — `UIPointerInteraction` lift/highlight; one enum property.
- ~~◆ **Context menu on any view**~~ — **done** (`View.ContextMenu`, same `MenuAction` model). The
  list's row menu is now `CollectionView.ItemContextMenu` — a `CollectionView` is a `View`, so the
  inherited name collided, and the row menu was always the more specific thing anyway.
- ▲ (skip) **Drag & drop** — `UIDrag/DropInteraction`, typed item providers.
- ★ (skip) **Anchor point** — transforms currently pivot the centre; corner-pivot rotations need it.
- ◆ (skip) **Accessibility custom actions** — the known debt; `UIAccessibilityCustomAction` list.
- ★ (skip) **Accessibility announce** — `UIAccessibility.PostNotification(announcement)` static helper.

## Label

- ▲ **Attributed spans** — ranges of bold/color/links with tap callbacks; the big one, maybe a
  `Span`-based `FormattedText` model. (Or markdown-lite via `NSAttributedString(markdown:)` — cheap!)
- ~~★ **Letter spacing & line spacing**~~ — **done** (`LetterSpacing`, `LineSpacing`).
- ~~★ **Underline / strikethrough**~~ — **done**.
- ~~★ **Auto-shrink**~~ — **done** (`Label.AutoShrink`).
- ~~★ **Dynamic Type cap**~~ — **done** (`Label.MaxFontSize`, NaN = uncapped). Not
  `MaximumContentSizeCategory`: that caps scaling through the view's trait collection, and our fonts
  are built in managed code by `Fonts.Preferred`/`Scaled`, which never read it. The cap goes where the
  font is made — `UIFontMetrics.GetScaledFont(font, maximumPointSize)`.
- ◆ **Selectable text / link detection** — readonly `UITextView` under the hood or iOS 17 text items.

## Button

- ~~★ **Subtitle**~~ — **done** (`Button.Subtitle`).
- ~~★ **Image placement & padding**~~ — **done** (`Button.IconPlacement`); `ContentInsets` remain.
- ~~★ **Size & shape**~~ — **done** (`Button.Size`; capsule already existed as `FilledCapsule`).
- ~~★ **Role**~~ — **done** (`Button.IsDestructive`).
- ~~◆ **Menu button**~~ — **done** (`Button.Menu`; the popup-selection variant is
  `Button.SelectsFromMenu` — the chosen entry becomes the title and fires its command).
- ~~★ **Loading state**~~ — **done** (`Button.IsLoading`, bindable).

## Image

- ~~★ **Template rendering + tint**~~ — **done** (`View.Tint`, rasters render as templates when the
  tint is set on the image itself).
- ~~◆ **Symbol configuration**~~ — **done** (`SymbolSize`/`SymbolWeight`/`SymbolScale`,
  `SymbolColors` — one is hierarchical, several are the palette — `PrefersMulticolor`,
  bindable `SymbolValue`).
- ~~◆ **Symbol effects**~~ — **done** (ambient `SymbolEffect` + one-shot `PlaySymbolEffect`).
- ~~★ **URL loading UX**~~ — **done** (`Placeholder`, `Fallback`, opt-in `FadesIn`).

## Text input

- ~~★ **`TextContentType`**~~ — **done** (`ContentKind` on `TextField`/`TextEditor`).
- ~~★ **Autocapitalization / autocorrection / spell-checking**~~ — **done** (`Capitalization`,
  `Autocorrection` drives both).
- ~~★ **Clear button mode**~~ — **done** (`TextField.ClearButton`).
- ◆ **Accessory icons in the field** — leading/trailing views (search icon, reveal-password eye).
- ◆ **Keyboard toolbar** — `InputAccessoryView` with Done/arrows; one shared bar, big win with
  `KeyboardDismiss`.
- ★ (skip) **Max length / input filter** — `ShouldChangeCharacters` hook as `Func<string, bool>`.
- ★ (skip) **TextEditor placeholder** — UIKit has none natively; overlay label, everyone needs it.
- ~~★ **Font weight/design on inputs**~~ — **done** (`FontWeight`/`FontDesign` on both).
- ~~★ **Keyboard appearance**~~ — **done** (`KeyboardLook` on `TextField`/`TextEditor`;
  `EnablesReturnKeyAutomatically` is `TextField.RequiresText`).

## Missing controls (all wrap one UIKit class)

- ~~◆ **DatePicker**~~ — **done** (modes × styles, min/max, two-way `Date`); countdown mode left
  out on purpose.
- ~~★ **SegmentedControl**~~ — **done** (new control, `Items` + two-way `SelectedIndex`).
- ~~★ **ColorWell**~~ — **done** (two-way `Selected`, `Title`, `SupportsAlpha`).
- ~~★ **PageControl**~~ — **done** (`Count`, two-way `Current`, dot colors, `AllowsScrubbing`). The
  unfilled dots keep UIKit's own default, which is near-invisible on a plain light background — set
  `DotColor` when the control doesn't sit over a photo.
- ▲ **WebView** — `WKWebView`: url/html, navigation events, JS eval. Big but standard.
- ▲ **MapView** — `MKMapView`; probably out of scope, listed for completeness.
- ~~★ **MenuPicker**~~ — covered by `Button.SelectsFromMenu`; no separate control.

## Slider / Switch / Progress / Stepper

- ~~★ **Slider**: min/max SF symbols, track tints, step value (snap)~~ — **done**
  (`Min/MaxIcon`, `TrackColor`/`EmptyTrackColor`/`ThumbColor`, `Step`, `Continuous`).
- ~~★ **Switch**: `OnTintColor`, `ThumbTintColor`~~ — **done** (`OnColor`, `ThumbColor`).
- ~~★ **ProgressBar**: `TrackTintColor`~~ — **done** (`TrackColor`, and the filled part is
  `FillColor` — renamed off `Tint`, which now means the inherited accent on every `View`);
  indeterminate question open.
- ~~★ **ActivityIndicator**: `Color`~~ — already existed; nothing to do.

## ScrollView

- ~~★ **Paging**~~ — **done** (`ScrollView.Paging`).
- ~~★ **Indicator control**~~ — **done** (`ShowsIndicator`, `IndicatorStyle`, `IndicatorInsets`).
- ★ (skip) **Bounce toggles** — `Bounces`, `AlwaysBounceVertical/Horizontal`.
- ~~★ **Programmatic scroll**~~ — **done** (`ScrollView.ScrollTo(offset)`).
- ★ (skip) **Deceleration rate** — normal/fast (fast = the "snappy" feel).
- ◆ (skip) **Zoom** — min/max zoom + zoomable child; photo viewers.
- ★ (skip) **Scrolls-to-top tap** — `ScrollsToTop` toggle.
- ★ (skip) **Refresh control tint/title** — applies to `CollectionView` too.

## CollectionView

- ◆ **Reorder** — diffable + `ReorderingHandlers`; drag-to-reorder with a moved-command.
- ◆ **Edit mode & multi-select** — `AllowsMultipleSelectionDuringEditing`, checkmarks,
  `SelectedItems` binding, Edit/Done toolbar pairing.
- ◆ **Expandable sections** — list-config header disclosure + snapshot section collapse.
- ◆ **Mixed sections** — per-section layout (a carousel row inside a list) — compositional
  layout does this natively; the `CollectionLayout` model would grow a per-section variant.
- ~~★ **Section footers**~~ — **done** (`FooterTemplate`, same plumbing as headers). Sections are
  now the app's own model (`ISection<TItem>` = `Items` and nothing else), so a header/footer binds
  whatever the app puts on it; `CollectionView<TItem, TSection>` carries the type, and
  `CollectionView<TItem>` stays the flat spelling. A section's own `Items` is a live source too —
  an `ObservableCollection` inside a section now diffs, which it never did.
- ~~★ **Separator control**~~ — **done** (`ShowsSeparators`, `SeparatorInsets`).
- ~~★ **Selection styling**~~ — **done** (`HighlightsSelection`, `HighlightColor`).
- ~~★ **Near-end hook**~~ — **done** (`LoadMoreCommand` + `LoadMoreThreshold`, via `WillDisplayCell`).
- ~~★ **Deselect on appear**~~ — **done** (`View.PageAppeared` walk off `ViewWillAppear`, so the row
  fades out with the pop rather than after it).
- ◆ **Prefetching** — `UICollectionViewDataSourcePrefetching` driving the image loader.
- ◆ **Context-menu previews** — custom preview view + `PreviewProvider` (peek content).
- ★ (skip) **Section index** — A–Z fast-scroll strip (`IndexTitles`).

### Not a bug — `ScrollTo` was always correct (closed 2026-07-15)

The 2026-07-14 report ("Top lands ~3 rows below") was a misread of the demo: `ScrollTo(item,
position)` brings *one item* into view aligned at a viewport edge (WPF `ScrollIntoView`), and the
GridDemo target "Moon" sits in the 4th row — so "3 rows below the top" *is* the item at the top.
Sim-verified with offset logs: all three positions land exact, even with `LoadMoreCommand` firing
mid-jump. The interim self-sizing correction machinery was removed again; `ScrollTo` is one native
`ScrollToItem` call. If real estimate drift ever shows on long unmeasured lists, the settle-loop
approach lives in this file's history.

## App level

- ~~★ **Global accent**~~ — **done** (`UseAccent`: window `TintColor` for the controls UIKit
  reaches, plus the `View.Tint` root fallback for the self-painting ones — switch fills, spinners,
  button configurations — which UIKit's inheritance never touches).
- ~~★ **Scene lifecycle**~~ — **done** (`UseLifecycle(background, foreground)`).
- ◆ (skip) **System pickers via navigator** — photo (`PHPickerViewController`), document
  (`UIDocumentPickerViewController`); both are present-and-await wrappers, AOT-safe.
- ★ (skip) **Haptic patterns** — `Haptics` covers impact/notify/selection; `CHHapticEngine` patterns
  only if Velura needs them (probably not).

## Honest capability note

Audit written from model knowledge of UIKit (current through the iOS 18/26 SDK era), not from
IDE metadata — but the .NET binding assemblies and their XML docs are on disk, so any single
signature can be verified exactly before implementing (`Microsoft.iOS.Ref` packs). ModalStyle-type
omissions happen when a wrapper is written from the common cases; this list is the systematic pass.
