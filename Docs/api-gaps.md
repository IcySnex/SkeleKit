 # API gaps — UIKit capability not yet surfaced

An audit of the wrapped controls against what their native counterparts actually offer
(2026-07-12). Not a to-do list: strike what you don't want, the rest becomes milestones.

Legend: ★ quick win (hours, additive) · ◆ medium (a day-ish, some design) · ▲ large (own milestone).

## Page & navigation chrome

- ~~★ **Leading toolbar item next to the back button**~~ — **done** (`LeftItemsSupplementBackButton`
  always on).
- ~~◆ **Large-title collapse driven by a SkeleKit scroll**~~ — **done** (`SetContentScrollView`
  wires the page's root scroll to the bar).
- ~~★ **Back button title / display mode**~~ — **done** (`ContentView.BackButtonTitle` +
  `BackButtonStyle`).
- ~~◆ **Pop interception**~~ — **done** (`ContentView.ConfirmLeave`: back button, sheet
  swipe-down and popover tap-out funnel through it; both pop gestures (iOS 26 adds a
  content-wide one) are *disabled* while it is set — an in-flight interactive pop cannot await a
  confirm. The property is live: set it null while nothing needs guarding and the swipe returns;
  a modal root's synthesized back button dismisses).
- ~~★ **Toolbar item pull-down menus**~~ — **done** (`ToolbarItem.Menu`, `MenuAction` list).
- ~~★ **Nav bar tint & title attributes**~~ — **done** (`BarAccent`, `TitleColor`,
  `LargeTitleColor`; per-item appearances copied from the live bar, item-level tints for iOS 26
  glass buttons).
- ~~★ **`NavigationItem.Prompt`**~~ — **done** (`ContentView.Prompt`).
- ~~★ **Bottom toolbar**~~ — **done** (`ContentView.BottomToolbarItems` → the system `UIToolbar`,
  nothing else). UIKit has no toolbar-plus-floating-tab-bar combo — they share the edge — so the
  toolbar only shows when the tab bar is gone: a page that wants one sets `HidesTabBar`, the
  `HidesBottomBarWhenPushed` idiom Apple's own apps use (Mail edit mode, Files). The 2026-07-15
  accessory path (per-page hand-built `UITabAccessory`) was removed — it repurposed Apple's
  app-global mini-player slot and hand-wired chrome.
- ~~◆ **Tab accessory**~~ — **done** (`Tabs.Accessory<PlayerBar>()`; the view resolves its
  ViewModel from `SkeleApplication.Current.Services` in its ctor and sets its own
  BindingContext). The view's own `IsVisible` controls the slot — bind it or set it;
  showing and hiding animates, and a page that hides the tab bar takes the accessory with it.
  Content rides in an `AccessoryHost` answering `IntrinsicContentSize` from our measure pass.
  iOS 26 only; earlier systems have no slot. **Gotcha (sim-verified):** the slot's glass
  treatment repaints flat `Background` fills as tint- or vibrancy-colored shapes — use real
  content (symbols, images, text), not colored tiles. **Follow-up:** compact accessory *content*
  via the accessory environment trait — the bar minimizes now (`Tabs.Minimizes()`) and the
  accessory docks inline automatically, but there is no API yet for a condensed tree in that
  state.
- ~~★ **Search**~~ — **done** (`HidesSearchBarWhenScrolling`, `SearchScopes` +
  `SearchScopeChanged`, `SearchCancelled`, `SearchObscuresBackground`).
- ◆ (skip) **Search suggestions** — built on `UISearchSuggestionItem` 2026-07-15, then **removed
  the same day**: with a nil results controller, UIKit's built-in presentation is a flat gray
  panel below an immovable dead gap (verified on the iOS 26.5 sim: scopes, `ObscuresBackground`
  and title style all change nothing), and it never renders `localizedDescription`. Apple's own
  apps (Files, Weather) ship *custom results lists* instead of this UI. If suggestions are ever
  needed, the real path is a results overlay built from a `CollectionView` driven by
  `SearchChanged` — not this API.
- ~~★ **Status bar style per page**~~ — **done** (`ContentView.StatusBar`, `SkeleStack` forwards).
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
- ~~★ **Share sheet**~~ — **done**, but as its own service, not on the navigator: `ISharer.ShareAsync(
  ShareContent)` over `UIActivityViewController` (injected like `INavigator`, also reachable from page
  code via `ContentView.Sharer`). `ShareContent` is one coherent payload — `Text`, `Url`, `Image` — not
  a bag of items, because the sheet has exactly one header preview and a bag makes them race for it; a
  `string`/`Uri`/`ImageSource` converts to it implicitly so a one-part share stays terse. One
  `LPLinkMetadata` is built for the whole share and only when an `Image` is present (a bare image has no
  preview otherwise); text/link alone keep iOS's native previews (a link auto-fetches its card). A remote
  image is fetched through the shared image loader first. Consumer note: the sheet's *Save Image* needs
  `NSPhotoLibraryAddUsageDescription` in the app plist or it crashes on tap. Split off the navigator on
  purpose, to grow later (excluded activities, a result, file/data items).
- ~~★ **Open URL in-app**~~ — **done** (`INavigator.OpenUrlAsync(url)` presents an `SFSafariViewController`
  from the top controller; rejects a non-`http`/`https` address, completes once presented).
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
  `OnRotate`; plus the `Pressed` touch-down stream, which observes without stealing).
- ~~◆ **Liquid Glass**~~ — **done** (`MaterialKind.Glass` renders an interactive `UIGlassEffect`:
  the rim shapes through `CornerConfiguration` off the same `View.CornerRadius` — a layer clip
  would flatten it — and a glass panel hosts its children in the effect's content view, so the
  glow tracks touches on them too. `ButtonStyle.Glass`/`ProminentGlass`/`ClearGlass` wrap the
  button configurations. Not exposed: `UIGlassContainerEffect` droplet merging — add when a real
  screen needs it).
- ~~◆ **Pointer/hover effects (iPad)**~~ — **done** (`View.PointerEffect`: None/Automatic over a
  `UIPointerInteraction` with a rooted delegate returning a `UIPointerStyle` off a `UITargetedPreview` of
  the view). iPad-only (no pointer on iPhone). Only **Automatic** is exposed: sim-confirmed on hardware
  that Highlight/Lift/Hover all rendered identically, because Microsoft.iOS 26.0 binds only the base
  `UIPointerEffect.Create(preview)` factory — the subtype effect initializers (`init(preview:)`) are not
  bound (parameterless ctor + read-only `Preview`), so an explicit variant is unreachable AOT-safely.
  Automatic already picks highlight-for-small / lift-for-large, so nothing useful is lost. Re-expose the
  variants if a later binding adds the initializers.
- ~~◆ **Context menu on any view**~~ — **done** (`View.ContextMenu`, same `MenuAction` model). The
  list's row menu is now `CollectionView.ItemContextMenu` — a `CollectionView` is a `View`, so the
  inherited name collided, and the row menu was always the more specific thing anyway.
- ▲ (skip) **Drag & drop** — `UIDrag/DropInteraction`, typed item providers.
- ~~★ **Anchor point**~~ — **done** (`View.AnchorPoint`, unit coordinates, default centre `(0.5, 0.5)`).
  Baked into the transform matrix, **not** `layer.AnchorPoint` — UIView's frame system resets a directly
  set anchor (both pivots rendered identically). `ApplyTransform` keeps the anchor at centre and adds the
  translation `(I − L)·(P − C)` a pivot change introduces (`P` the anchor, `C` the centre, `L` the
  scale·rotate), so `Rotation`/`Scale` turn about the anchor. The offset scales with size, so a resize
  re-bakes it (`ApplyFrame` re-applies the transform on resize).
- ◆ (skip) **Accessibility custom actions** — the known debt; `UIAccessibilityCustomAction` list.
- ★ (skip) **Accessibility announce** — `UIAccessibility.PostNotification(announcement)` static helper.

## Label

- ~~▲ **Attributed spans**~~ — **done, styling only** (`Label.Spans` = a `Span` list; each run overrides
  the label's weight/design/size/color and adds underline/strikethrough, composed into one
  `NSAttributedString`; a `Span`'s unset props follow the label). **Read-only by decision:** tap
  callbacks, links and selection are *not* on `Label`. UILabel can't do native tap-highlight / hold-to-peek /
  selection — a manual `NSLayoutManager` hit-test was built and removed (fragile, no native feel), so
  interactive/selectable rich text belongs to the UITextView-backed control below (*Selectable text /
  link detection*). Markdown-lite (`NSAttributedString(markdown:)`) declined: URL-only links, no per-run
  color/weight, and a per-assign reparse in recycled cells. `Span` is not bindable (WPF-`Inlines`
  static structure); reassign the list to change it.
- ~~★ **Letter spacing & line spacing**~~ — **done** (`LetterSpacing`, `LineSpacing`).
- ~~★ **Underline / strikethrough**~~ — **done**.
- ~~★ **Auto-shrink**~~ — **done** (`Label.AutoShrink`).
- ~~★ **Dynamic Type cap**~~ — **done** (`Label.MaxFontSize`, NaN = uncapped). Not
  `MaximumContentSizeCategory`: that caps scaling through the view's trait collection, and our fonts
  are built in managed code by `Fonts.Preferred`/`Scaled`, which never read it. The cap goes where the
  font is made — `UIFontMetrics.GetScaledFont(font, maximumPointSize)`.
- ~~◆ **Selectable text / link detection**~~ — **done** (`TextView`, a `UITextView`-backed read-only
  control). `IsSelectable` turns on native selection; bindable `Spans` (a `BindableList<Span>`) reuses
  the `Label` run model, and a `Link : Span` run carries `Command`/`CommandParameter` + a `ContextMenu`
  (the same `MenuAction` list). Links become native `.link` text items (the `UITextItemTagAttributeName`
  custom-tag key is unbound in the 26.0 ref pack, so a link's index rides in the item's URL and the
  delegate dispatches by range); the `GetPrimaryAction` override fires the command and `GetMenuConfiguration`
  builds the hold-to-peek menu, both suppressing UIKit's default URL open. Two ergonomics fell out and
  went library-wide: an implicit `string`→`Span` and a `[CollectionBuilder]` on `BindableList<T>`, so
  `[...]` literals now work for every list source (`CollectionView`, `Picker`), not just `new T[]{}`.
  Automatic data-detector link *detection* (phone/date/address) is not wired — add if a screen needs it.

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
- ~~◆ **Accessory icons in the field**~~ — **done** (`TextField.LeadingIcon`/`TrailingIcon`, decorative
  symbol/bundle icons only, no tap). `SecureField.RevealButton` adds a caret-safe eye toggle owning the
  trailing slot. Icons are gray, ~15pt, padded by a `UITextField` subclass overriding the rect methods
  (`LeftViewRect`/`RightViewRect`/`TextRect`/`EditingRect`) since UITextField sizes overlay views from
  their intrinsic size and ignores a set frame. `TextEditor` has no native slot, so it stays out.
  Trailing icon shares the slot with `ClearButton` (icon wins).
- ~~◆ **Keyboard toolbar**~~ — **done** (`KeyboardToolbar` on `TextField`/`SecureField`/
  `TextEditor`: `Done` dismisses, `Navigation` adds previous/next arrows walking the top page's
  inputs in tree order. One shared bar per kind, rooted statically — per-field bars made the
  iOS 26 glass buttons flash on every focus change. `KeyboardAccessory` hosts a custom view
  instead (an `AccessoryHost` per view, shared across fields), the Safari-style single bar).
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
- ~~▲ **WebView**~~ — **done** (`WebView` over `WKWebView`: bindable `Url`/`Html`, `Navigated`/
  `NavigationFailed` streams, `AllowsBackGestures`, `GoBack`/`GoForward`/`Reload`, `EvaluateAsync` for
  JS. Fill-measured — it takes the offered slot, since web content has no intrinsic size — so give it a
  star row or an explicit height. Rooted `WKNavigationDelegate` peer).
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

- ~~◆ **Reorder**~~ — **done** (`ReorderCommand`: setting it enables a long-press drag via
  interactive movement; the framework applies the move to the writable source first
  (`IList<TItem>` / a section's `Items`, `ObservableCollection.Move` when it is one) and then
  fires the command with an `ItemMove<TItem>`. Not `ReorderingHandlers`: the binding class is an
  empty stub (no handler properties, verified against the 26.0 ref pack), so `CollectionSource`
  overrides `CanMoveItem`/`MoveItem` on the diffable data source instead, and the matching
  snapshot lands synchronously so the drop settles against moved data. Programmatic-drag
  sim-verified; the finger drag next to `ItemContextMenu` (both live on a long-press) still
  wants a hand check).
- ~~◆ **Edit mode & multi-select**~~ — **done** (`IsEditing` two-way + `SelectedItems`: hand the
  view an `ObservableCollection` and taps keep it in sync, mutations move the checkmarks; leaving
  edit mode clears both sides, and a diff re-syncs checkmarks to the shuffled index paths.
  `SkeleCell` is now a `UICollectionViewListCell`, which brings the native edit accessories:
  multiselect circles when `SelectedItems` is set, the reorder drag handle when `ReorderCommand`
  is. The selection highlight moved from `SelectedBackgroundView` (dead on a list cell) to a
  `UIBackgroundConfiguration` in `UpdateConfiguration`. Edit/Done stays app-side: a toolbar item
  toggling the bound `IsEditing`. Sim-verified in the list; the grid and carousel look after the
  cell rebase want a hand check).
- ~~◆ **Expandable sections**~~ — **done** (`IExpandableSection<TItem>`: a section implementing it (two-way
  `IsExpanded`, INPC for programmatic collapse) becomes collapsible). Flat-snapshot approach, not the native
  outline: a collapsed section emits only its header, so the existing diff animates rows in/out and the
  reorder/edit/selection/prefetch pipeline is untouched (works across list/grid/carousel too). Built-in
  chevron on the header (rotates 90 on expand); tapping the header toggles. Needs a `HeaderTemplate` (the
  chevron lives there); a collapsed section still shows its footer.
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
- ~~◆ **Prefetching**~~ — **done** (`Prefetch`: map an item to its image url and the delegate
  becomes the collection's `PrefetchDataSource`, warming the shared image loader (whose cache and
  per-url dedupe make the cell's later load a hit) with a `CancellationTokenSource` per index
  path honoured on cancel. Sim-verified: rows a screen ahead warm during a scroll).
- ~~◆ **Context-menu previews**~~ — **done** as `ItemPreview` only: a SkeleKit tree as the floating
  peek (hosted in a `PreviewHost` controller sized by our measure pass, rooted on the element
  while UIKit presents it), plus `PreviewCommand` fired with the item on a preview tap — deferred
  through `animator.AddCompletion`, or the dismissal tears the presented alert down with it. A
  preview with no `ItemContextMenu` entries is a plain peek. **Shaped row platters
  (`UITargetedPreview` + `VisiblePath`, the Velura pattern) were built and removed**: on the 26.5
  sim any custom targeted preview from our delegate leaves the pressed cell render-suppressed for
  ~1s after dismissal (portal teardown waits on something GC-timed). Bisected hard — clean-room
  lab page reproduced it with a minimal grid; survived deterministic disposal of the preview, the
  parameters, the animator/configuration wrappers, matched highlight+dismissal targets, and a
  forced `GC.Collect` at interaction end; identical code is clean in Velura. Root cause never
  found on our stack; only the system platter is reliable there. Also: exporting the iOS 16
  `GetContextMenuConfiguration*Preview` selectors kills the interaction outright — never override
  those.
- ~~★ **Section index**~~ — **done** (`SectionIndexTitle` maps each section to its letter, enabling the
  strip; optional `IndexTitles` gives an explicit full alphabet, a tapped gap-letter jumping to the
  nearest section at or after it). Grouped only. Overrides `GetIndexTitles`/`GetIndexPath` on the
  diffable source; titles are suppressed until the collection has live cells (UIKit validates each title
  against a cell during `reloadData`, which runs before the first async snapshot lands). Use a plain list
  so the letter headers pin under the bar, or an index jump tucks the header behind it.

## App level

- ~~◆ **Search tab**~~ — **done** (`Tabs.Search<TView>()`, ADR-014: the system trailing search tab that
  morphs the bar into the field; the shell now builds on `UITab`/`SetTabs`, and `TabBadge` routes
  through `UITab.BadgeValue` — UITab has no badge *color*, so `TabBadgeColor` only applies on
  item-based bars).
- ~~★ **Tab bar minimize**~~ — **done** (`Tabs.Minimizes()`, iOS 26; the accessory docks inline
  automatically).
- ~~★ **Tab customization control**~~ — **done**, reshaped by ADR-014: placements live in
  `OnIPad(pad => pad.PlaceTab<TView>(TabPlacement.Locked))`, iPad-only destinations via
  `pad.Tab`/`pad.Group`, the sidebar footer via `pad.SidebarFooter<TView>()`. iPadOS persists the
  user's layout keyed by tab identifier (the ViewModel type name; group identifiers derive from
  titles) — renames reset the user's arrangement.
- ~~▲ **Tab groups**~~ — **done** (`Group(title, icon, g => g.Tab...)`: a sidebar section on
  iPad, a drill-in tab on iPhone; one shared navigation controller per root group).
- ~~◆ **Action bubble**~~ — **done** (`Tabs.Action(icon, ...)`: the separated bubble as a FAB, a
  repurposed `UISearchTab` with vetoed selection. Search XOR Action, enforced at build).

- ~~★ **Global accent**~~ — **done** (`UseAccent`: window `TintColor` for the controls UIKit
  reaches, plus the `View.Tint` root fallback for the self-painting ones — switch fills, spinners,
  button configurations — which UIKit's inheritance never touches).
- ~~★ **Scene lifecycle**~~ — **done** (`UseLifecycle(background, foreground)`).
- ◆ (skip) **System pickers via navigator** — photo (`PHPickerViewController`), document
  (`UIDocumentPickerViewController`); both are present-and-await wrappers, AOT-safe.
- ★ **Haptic patterns** — `Haptics` covers impact/notify/selection; `CHHapticEngine` patterns
  only if Velura needs them (probably not).

## Honest capability note

Audit written from model knowledge of UIKit (current through the iOS 18/26 SDK era), not from
IDE metadata — but the .NET binding assemblies and their XML docs are on disk, so any single
signature can be verified exactly before implementing (`Microsoft.iOS.Ref` packs). ModalStyle-type
omissions happen when a wrapper is written from the common cases; this list is the systematic pass.
