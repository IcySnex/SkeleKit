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
- ◆ **Pop interception** — "unsaved changes" confirm-before-back: wrap
  `NavigationItem.BackAction` + `IsModalInPresentation` for sheets; surface as
  `ContentView.ConfirmLeave(Func<Task<bool>>)`.
- ★ **Toolbar item pull-down menus** — `UIBarButtonItem(menu:)`; a `ToolbarItem.Menu` list next to
  `Command`. Also gets the "..." overflow pattern.
- ★ **Nav bar tint & title attributes** — per-page accent, title color/font via
  `UINavigationBarAppearance` (standard vs scroll-edge).
- ~~★ **`NavigationItem.Prompt`**~~ — **done** (`ContentView.Prompt`).
- ★ **Bottom toolbar** — `SetToolbarItems`; pages with persistent bottom actions.
- ★ **Search**: ~~`HidesSearchBarWhenScrolling` is hardcoded false — expose~~ (**done**); scope
  buttons, cancel event, `ObscuresBackgroundDuringPresentation` remain.
- ◆ **Search suggestions** — `UISearchSuggestionItem` (iOS 16); typed suggestion list + pick command.
- ~~★ **Status bar style per page**~~ — **done** (`ContentView.StatusBar`, `BareStack` forwards).
- ★ **Tab badges** — `UITabBarItem.BadgeValue`/`BadgeColor`; bindable per tab.
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
- ★ **Typed gestures beyond pan/tap** — `OnLongPress`, `OnDoubleTap`, `OnPinch`, `OnRotate`
  (pinch/rotate feed `Scale`/`Rotation` naturally, pairs with the Animator).
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

- ★ **Template rendering + tint** — SF-symbol-style recoloring of raster images.
- ◆ **Symbol configuration** — point size/weight/scale, hierarchical & palette colors,
  variable value (0–1 progress symbols like speaker/wifi).
- ◆ **Symbol effects** — iOS 17 `UIImageView.AddSymbolEffect`: bounce, pulse, variable-color;
  pairs beautifully with `Animator`.
- ★ **URL loading UX** — placeholder image, error image, fade-in on load.

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

- ◆ **DatePicker** — `UIDatePicker`: date/time/date-and-time/countdown × compact/inline/wheels.
  Most-requested control that isn't there.
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
- ★ **Section footers** — header templates exist; footers are the same plumbing.
- ★ **Separator control** — hide/inset separators per list (config supports it).
- ◆ **Expandable sections** — list-config header disclosure + snapshot section collapse.
- ◆ **Mixed sections** — per-section layout (a carousel row inside a list) — compositional
  layout does this natively; the `CollectionLayout` model would grow a per-section variant.
- ★ **Selection styling** — highlight color / disable highlight.
- ★ **Near-end hook** — `LoadMore` threshold callback (infinite scroll), trivial via
  `WillDisplayCell`.
- ◆ **Prefetching** — `UICollectionViewDataSourcePrefetching` driving the image loader.
- ★ **ScrollTo position** — top/centre/bottom parameter (top-only today).
- ◆ **Context-menu previews** — custom preview view + `PreviewProvider` (peek content).
- ★ **Deselect on appear** — the standard "tapped row un-highlights when you come back" nicety.
- ★ (skip) **Section index** — A–Z fast-scroll strip (`IndexTitles`).

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
