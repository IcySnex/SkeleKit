# Collections

Classification: **Visual showcase + interactive lab**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Required collection labs

Use deterministic local models and an `ObservableCollection<T>` for mutation scenarios. Cover list, grid, carousel, mixed section layouts, empty state, headers/footers, single and multiple selection, highlight clearing on appearance, pull-to-refresh, near-end load, separators and index titles, swipe actions, context menus, previews, edit/reorder, prefetch/cancel, programmatic scrolling, and live grouped/expandable section changes. Repeat with zero, one, and many items and with stable duplicate-looking values to validate identity rather than display text.

Each callback must write its last payload into an on-screen event log. Expected results include coalesced animated snapshots, recycled cells rebinding without rebuilding their element tree, refresh ending only when `IsRefreshing` returns false, and unsupported layout-only features remaining inert rather than crashing.
## CollectionView<T>

A data-driven list, grid, or carousel.

- Source: `SkeleKit.iOS/Controls/CollectionView.cs`
- Inheritance/shape: `class CollectionView<T> : CollectionView<TItem, ISection<TItem>>`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UICollectionView` with diffable data source
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.CollectionView`1.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(CollectionView<string> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## CollectionView<TItem, TSection>

A data-driven list, grid, or carousel whose groups carry their own section model.

- Source: `SkeleKit.iOS/Controls/CollectionView.iOS.cs`
- Inheritance/shape: `class CollectionView<TItem, TSection>`
- Inherited API: [`View`](../../shared/view.md)
- Native counterpart: `UICollectionView` with diffable data source
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.CollectionView`2.ItemsSource` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The items to show. Changes animate into place when the list is an `ObservableCollection`. |
| Property | `SkeleKit.CollectionView`2.GroupedItemsSource` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Groups, each with its own header. Takes precedence over `CollectionView`2.ItemsSource`. |
| Property | `SkeleKit.CollectionView`2.ItemTemplate` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Builds the element tree for a cell. Called once per recycled cell, never per item. |
| Property | `SkeleKit.CollectionView`2.HeaderTemplate` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Builds a section header. Bound to the section model. |
| Property | `SkeleKit.CollectionView`2.FooterTemplate` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Builds a section footer. Bound to the section model. |
| Property | `SkeleKit.CollectionView`2.Layout` | public/protected as emitted | implementation-defined; inspect source | No | n/a | How the items are arranged. |
| Property | `SkeleKit.CollectionView`2.SectionLayout` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Gives each section its own layout, or null to arrange every section with `CollectionView`2.Layout`. Mixes arrangements in one collection, like a carousel row above a list. Every section shares the one `CollectionView`2.ItemTemplate`. |
| Property | `SkeleKit.CollectionView`2.SelectionCommand` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked with the tapped item. |
| Property | `SkeleKit.CollectionView`2.ShowsSeparators` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether rows draw their separator lines. List layouts only. |
| Property | `SkeleKit.CollectionView`2.SeparatorInsets` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Leading/trailing insets for the separator lines, or null for the system default. List layouts only. |
| Property | `SkeleKit.CollectionView`2.HighlightsSelection` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether a tapped row shows a highlight until the page is next appeared. |
| Property | `SkeleKit.CollectionView`2.HighlightColor` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The tapped row's highlight color, or null for the system gray. |
| Property | `SkeleKit.CollectionView`2.SectionIndexTitle` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Maps a section to its letter in the fast-scroll index, or null for no index. Grouped list layouts only. Tapping a letter jumps to that section. |
| Property | `SkeleKit.CollectionView`2.IndexTitles` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Explicit labels for the fast-scroll index, or null to show one per section. A tapped letter with no section jumps to the nearest one at or after it. Has no effect without `CollectionView`2.SectionIndexTitle`, which still supplies each section's letter. |
| Property | `SkeleKit.CollectionView`2.LoadMoreCommand` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked when the user scrolls within `CollectionView`2.LoadMoreThreshold` items of the end. Fires once per item count. |
| Property | `SkeleKit.CollectionView`2.LoadMoreThreshold` | public/protected as emitted | implementation-defined; inspect source | No | n/a | How many items from the end `CollectionView`2.LoadMoreCommand` fires at. |
| Property | `SkeleKit.CollectionView`2.EmptyView` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Shown instead of the items while the source is empty. |
| Property | `SkeleKit.CollectionView`2.RefreshCommand` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Command invoked when the user pulls to refresh. Setting it enables the refresh control. |
| Property | `SkeleKit.CollectionView`2.IsRefreshing` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the refresh spinner is showing. Two-way: the pull sets it true, the ViewModel sets it false when done. |
| Property | `SkeleKit.CollectionView`2.SwipeActions` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Actions revealed by swiping a row. List layouts only. |
| Property | `SkeleKit.CollectionView`2.ItemContextMenu` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Entries in a row's long-press context menu. Each command is invoked with the row's item. |
| Property | `SkeleKit.CollectionView`2.ItemPreview` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Builds the floating preview shown over a row's context menu, given the row's item. Without it the row itself is the preview. |
| Property | `SkeleKit.CollectionView`2.PreviewShape` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Shapes the row itself as the lifted platter: padding around the content and a corner radius. Null keeps the system shape. |
| Property | `SkeleKit.CollectionView`2.PreviewCommand` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked with the row's item when its context-menu preview is tapped. |
| Property | `SkeleKit.CollectionView`2.Prefetch` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Maps an item to the image url to warm before its row scrolls on. Setting it enables prefetching through the app's image loader. |
| Property | `SkeleKit.CollectionView`2.ReorderCommand` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked after a drag-to-reorder with an `ItemMove`1`. Setting it enables a long-press drag, unless a context menu owns that gesture; the edit-mode handle always drags. The move is already applied to the source when it fires. |
| Property | `SkeleKit.CollectionView`2.IsEditing` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the collection is in edit mode, showing selection circles and reorder handles. Two-way. |
| Property | `SkeleKit.CollectionView`2.SelectedItems` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The items checked while editing. Give it an `ObservableCollection`: taps keep it in sync, and mutating it moves the checkmarks. |
| Property | `SkeleKit.CollectionView`2.Scrolled` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked as the collection scrolls, with the vertical offset in points. |
| Method | `SkeleKit.CollectionView`2.ScrollTo(`0,SkeleKit.ScrollPosition,System.Boolean)` | public | n/a | n/a | n/a | Scrolls the list until `item` is visible, aligned to the given viewport edge. |
| Method | `SkeleKit.CollectionView`2.MeasureOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.CollectionView`2.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `ItemsSource`, `GroupedItemsSource`, `ItemTemplate`, `HeaderTemplate`, `FooterTemplate`, `Layout`, `SectionLayout`, `SelectionCommand`, `ShowsSeparators`, `SeparatorInsets`, `HighlightsSelection`, `HighlightColor`, `SectionIndexTitle`, `IndexTitles`, `LoadMoreCommand`, `LoadMoreThreshold`, `EmptyView`, `RefreshCommand`, `IsRefreshing`, `SwipeActions`, `ItemContextMenu`, `ItemPreview`, `PreviewShape`, `PreviewCommand`, `Prefetch`, `ReorderCommand`, `IsEditing`, `SelectedItems`, `Scrolled` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(CollectionView<string, ISection<string>> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

