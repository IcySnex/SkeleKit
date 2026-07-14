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
- ◆ **Search suggestions** — `UISearchSuggestionItem` (iOS 16); typed suggestion list + pick command.
- ~~★ **Status bar style per page**~~ — **done** (`ContentView.StatusBar`, `BareStack` forwards).
- ~~★ **Tab badges**~~ — **done** (`ContentView.TabBadge` bindable + `TabBadgeColor`; applies to
  never-opened tabs too).
- ~~★ **Hide tab bar on push**~~ — **done** (`ContentView.HidesTabBar`).
- ◆ **Sheet polish** — grabber (`PrefersGrabberVisible`), sheet corner radius,
  `LargestUndimmedDetent` (non-blocking sheets), `PrefersScrollingExpandsWhenScrolledToEdge`,
  dismiss prevention + `DidAttemptToDismiss` callback. Detents exist; these are the knobs around them.
- ◆ **Popover anchoring** — present as popover from a view/toolbar item with arrow direction
  (currently no source anchor, iPad needs it).
- ★ (skip) **Share sheet** — `INavigator.ShareAsync(items)` over `UIActivityViewController`.
- ★ (skip) **Open URL in-app** — `SFSafariViewController` wrapper on the navigator.
- ◆ **Alert with text input** — `AlertAsync` variant returning a string (UIAlertController text fields).

## View (every element)

- ★ (skip) **Border on any view** — `layer.borderWidth/borderColor`; today only the `Border` panel strokes.
- ★ (skip) **Per-corner rounding + continuous curve** — `MaskedCorners` and
  `CornerCurve = .continuous` (the Apple squircle — today's radius is the "cheap" circular look).
- ★ **TintColor** — propagating accent for buttons/images/controls under a subtree.
- ~~★ **Typed gestures beyond pan/tap**~~ — **done** (`OnLongPress`, `OnDoubleTap`, `OnPinch`,
  `OnRotate`).
- ◆ (skip) **Pointer/hover effects (iPad)** — `UIPointerInteraction` lift/highlight; one enum property.
- ◆ **Context menu on any view** — `UIContextMenuInteraction`; the list already has the
  `MenuAction` model, reuse it.
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
- ★ **Dynamic Type cap** — `MaximumContentSizeCategory` (stop a title exploding at AX5).
- ◆ **Selectable text / link detection** — readonly `UITextView` under the hood or iOS 17 text items.

## Button

- ~~★ **Subtitle**~~ — **done** (`Button.Subtitle`).
- ~~★ **Image placement & padding**~~ — **done** (`Button.IconPlacement`); `ContentInsets` remain.
- ~~★ **Size & shape**~~ — **done** (`Button.Size`; capsule already existed as `FilledCapsule`).
- ~~★ **Role**~~ — **done** (`Button.IsDestructive`).
- ◆ **Menu button** — ~~`Menu` + `ShowsMenuAsPrimaryAction`~~ **done** (`Button.Menu`); the
  popup-selection variant (`ChangesSelectionAsPrimaryAction`) remains.
- ~~★ **Loading state**~~ — **done** (`Button.IsLoading`, bindable).

## Image

- ~~★ **Template rendering + tint**~~ — **done** (`Image.Tint`, rasters render as templates).
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
- ★ **Keyboard appearance** — dark keyboard override remains;
  ~~`EnablesReturnKeyAutomatically`~~ **done** (`TextField.RequiresText`).

## Missing controls (all wrap one UIKit class)

- ~~◆ **DatePicker**~~ — **done** (modes × styles, min/max, two-way `Date`); countdown mode left
  out on purpose.
- ~~★ **SegmentedControl**~~ — **done** (new control, `Items` + two-way `SelectedIndex`).
- ★ **ColorWell** — `UIColorWell` (+ `UIColorPickerViewController` via navigator).
- ★ **PageControl** — dots, pairs with the carousel layout.
- ▲ **WebView** — `WKWebView`: url/html, navigation events, JS eval. Big but standard.
- ▲ **MapView** — `MKMapView`; probably out of scope, listed for completeness.
- ★ **MenuPicker** — the `UIButton` popup variant above may cover this; decide one spelling.

## Slider / Switch / Progress / Stepper

- ~~★ **Slider**: min/max SF symbols, track tints, step value (snap)~~ — **done**
  (`Min/MaxIcon`, `TrackColor`/`EmptyTrackColor`/`ThumbColor`, `Step`); `Continuous` toggle remains.
- ~~★ **Switch**: `OnTintColor`, `ThumbTintColor`~~ — **done** (`OnColor`, `ThumbColor`).
- ~~★ **ProgressBar**: `TrackTintColor`~~ — **done** (`TrackColor`); indeterminate question open.
- ~~★ **ActivityIndicator**: `Color`~~ — already existed; nothing to do.

## ScrollView

- ~~★ **Paging**~~ — **done** (`ScrollView.Paging`).
- ★ **Indicator control** — ~~show/hide~~ **done** (`ShowsIndicator`); style/insets remain.
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
- ◆ **Reorder** — diffable + `ReorderingHandlers`; drag-to-reorder with a moved-command.
- ◆ **Edit mode & multi-select** — `AllowsMultipleSelectionDuringEditing`, checkmarks,
  `SelectedItems` binding, Edit/Done toolbar pairing.
- ◆ **Prefetching** — `UICollectionViewDataSourcePrefetching` driving the image loader.
- ◆ **Context-menu previews** — custom preview view + `PreviewProvider` (peek content).
- ★ (skip) **Section index** — A–Z fast-scroll strip (`IndexTitles`).

### Bug — `ScrollTo` position lands wrong (2026-07-14)

`ScrollTo(item, ScrollPosition)` ships but **does not work**. Verified on the sim with a 20+ row
grid: `Top` lands ~3 rows below the target, `Bottom` ~2 rows below the top, `Center` is off as well.
The enum maps onto `UICollectionViewScrollPosition` correctly, so the fault is elsewhere — prime
suspect is that the cells are **self-sizing** (`PreferredLayoutAttributesFittingAttributes`), and
`ScrollToItem` resolves the target from *estimated* layout attributes for rows it has never
measured, so every unvisited row below the current viewport is mis-positioned. The usual fixes are
to give the compositional group an absolute item height when the layout allows it, or to scroll in
two passes (`ScrollToItem`, then re-measure on the next run-loop turn and correct the offset).
Nothing else in the batch depends on it.

## App level

- ~~★ **Global accent**~~ — **done** (`UseAccent`).
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
