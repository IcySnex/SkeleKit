# Panels, pages, and item views

Classification: **Visual showcase + advanced escape hatch**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Control

Base for native control wrappers: measurement delegates to the control's own SizeThatFits.

- Source: `SkeleKit.iOS/Controls/Control.cs`
- Inheritance/shape: `class Control : View`
- Inherited API: [`View`](view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Control.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Control.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Baseline | _(type has no declared documented properties)_ | Render or invoke the type in the smallest owning control and verify its documented behavior. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Control specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ContentView

A full screen: compose its tree into `ContentView.Content` in the constructor.

- Source: `SkeleKit.iOS/Elements/ContentView.iOS.cs`
- Inheritance/shape: `class ContentView`
- Inherited API: [`View`](view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ContentView.Title` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The navigation bar title. |
| Property | `SkeleKit.ContentView.SafeAreaEdges` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Which edges the page keeps clear of the safe area. |
| Property | `SkeleKit.ContentView.ScrollsUnderBars` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether scrolling content passes under the navigation bar so the bar blurs over it. |
| Property | `SkeleKit.ContentView.TitleStyle` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the title is shown large and collapses as the content scrolls. |
| Property | `SkeleKit.ContentView.HidesNavigationBar` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Hides the navigation bar for this page. |
| Property | `SkeleKit.ContentView.BackgroundStyle` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The page's background style. |
| Property | `SkeleKit.ContentView.BackButtonTitle` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The back button title the next pushed page shows, or null for this page's title. |
| Property | `SkeleKit.ContentView.BackButtonStyle` | public/protected as emitted | implementation-defined; inspect source | No | n/a | How the next pushed page's back button represents this page. |
| Property | `SkeleKit.ContentView.Prompt` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The small line of text above the navigation title, or null for none. |
| Property | `SkeleKit.ContentView.StatusBar` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The status bar look for this page. |
| Property | `SkeleKit.ContentView.BarTint` | public get/set | null; inherits the app tint | No | Visual/interaction only | The tint for this page's bar buttons and back button. |
| Property | `SkeleKit.ContentView.TitleColor` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The navigation title's color, or null for the system default. |
| Property | `SkeleKit.ContentView.LargeTitleColor` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The expanded large title's color, or null for the system default. |
| Property | `SkeleKit.ContentView.ConfirmLeave` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Asked before the page is left, so unsaved changes can veto leaving. Fires for the back button, a sheet swipe or a popover tap-out; return `false` to stay. Leave it null while nothing needs guarding, which also disables the interactive pop-back swipe. |
| Property | `SkeleKit.ContentView.HidesTabBar` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Hides the tab bar while this page is on top of the stack. |
| Property | `SkeleKit.ContentView.TabReselected` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked when this page's tab is tapped while already selected, replacing the default pop-to-root / scroll-to-top. |
| Property | `SkeleKit.ContentView.TabBadge` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The badge on this page's tab bar item, or null for none. Applies even while the tab was never opened. |
| Property | `SkeleKit.ContentView.TabBadgeColor` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The badge's background color, or null for the system red. |
| Property | `SkeleKit.ContentView.ToolbarItems` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Buttons in the navigation bar. |
| Property | `SkeleKit.ContentView.BottomToolbarItems` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Buttons in a persistent bar along the screen's bottom edge. Above a visible tab bar they float as its accessory; everywhere else they form the classic bottom toolbar. |
| Property | `SkeleKit.ContentView.SearchPlaceholder` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Placeholder for the navigation bar's search field. Setting it shows the search bar. |
| Property | `SkeleKit.ContentView.HidesSearchBarWhenScrolling` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the search bar collapses into the bar as the content scrolls. |
| Property | `SkeleKit.ContentView.SearchObscuresBackground` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Whether the content dims behind an active search. |
| Property | `SkeleKit.ContentView.SearchScopes` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Titles of the scope buttons under an active search field. Empty for none. |
| Property | `SkeleKit.ContentView.SearchChanged` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked as the user types in the search field. |
| Property | `SkeleKit.ContentView.SearchScopeChanged` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked with the selected index when the user switches search scope. |
| Property | `SkeleKit.ContentView.SearchCanceled` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Invoked when the user cancels out of the search field. |
| Property | `SkeleKit.ContentView.Content` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The page's element tree. |
| Method | `SkeleKit.ContentView.OnLoaded` | protected virtual | n/a | n/a | n/a | Raised once, the first time the page is realized. |
| Method | `SkeleKit.ContentView.OnUnloaded` | protected virtual | n/a | n/a | n/a | Raised when the page's tree is torn down. |
| Method | `SkeleKit.ContentView.OnAppearing` | protected virtual | n/a | n/a | n/a | Raised before the page appears on screen. |
| Method | `SkeleKit.ContentView.OnAppeared` | protected virtual | n/a | n/a | n/a | Raised after the page appears on screen. |
| Method | `SkeleKit.ContentView.OnDisappearing` | protected virtual | n/a | n/a | n/a | Raised before the page leaves the screen. |
| Method | `SkeleKit.ContentView.OnDisappeared` | protected virtual | n/a | n/a | n/a | Raised after the page leaves the screen. |
| Property | `SkeleKit.ContentView.Navigator` | protected get | C# default | No | No automatic invalidation | The application's navigator, for navigation from page code. ViewModels take `INavigator` by constructor instead. |
| Property | `SkeleKit.ContentView.Sharer` | protected get | C# default | No | No automatic invalidation | The application's share sheet, for sharing from page code. ViewModels take `ISharer` by constructor instead. |
| Property | `SkeleKit.ContentView.SystemPicker` | protected get | C# default | No | No automatic invalidation | The application's photo and document pickers, for picking from page code. ViewModels take `ISystemPicker` by constructor instead. |
| Property | `SkeleKit.ContentView.Controller` | public get | null | No | No automatic invalidation | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ContentView.MeasureOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ContentView.ArrangeOverride(SkeleKit.Size)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ContentView.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Title`, `SafeAreaEdges`, `ScrollsUnderBars`, `TitleStyle`, `HidesNavigationBar`, `BackgroundStyle`, `BackButtonTitle`, `BackButtonStyle`, `Prompt`, `StatusBar`, `BarTint`, `TitleColor`, `LargeTitleColor`, `ConfirmLeave`, `HidesTabBar`, `TabReselected`, `TabBadge`, `TabBadgeColor`, `ToolbarItems`, `BottomToolbarItems`, `SearchPlaceholder`, `HidesSearchBarWhenScrolling`, `SearchObscuresBackground`, `SearchScopes`, `SearchChanged`, `SearchScopeChanged`, `SearchCanceled`, `Content`, `Navigator`, `Sharer`, `SystemPicker`, `Controller` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |
| Page lifecycle | `OnLoaded`, `OnUnloaded`, `OnAppearing`, `OnAppeared`, `OnDisappearing`, `OnDisappeared` | Record each callback while pushing, covering, uncovering, popping, and reloading a page. Loading occurs once per realization; appearing/disappearing callbacks bracket each native transition in order; appeared/disappeared callbacks run after it completes. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ContentView specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ContentView<T>

A page bound to a typed ViewModel: bind with `Bind(...)`.

- Source: `SkeleKit.iOS/Elements/ContentView.cs`
- Inheritance/shape: `class ContentView<T> : ContentView`
- Inherited API: [`View`](view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.ContentView`1.Bind``1(System.Func{`0,``0},System.String)` | protected static | n/a | n/a | n/a | Binds one way to a ViewModel property. |
| Method | `SkeleKit.ContentView`1.Bind``1(System.Func{`0,``0},System.Action{`0,``0},System.String)` | protected static | n/a | n/a | n/a | Binds two ways; `setter` writes the control's value back. |
| Method | `SkeleKit.ContentView`1.Bind``2(System.Func{`0,``0},System.Func{``0,``1},System.String)` | protected static | n/a | n/a | n/a | Binds one way through a converter. |
| Method | `SkeleKit.ContentView`1.Bind``2(System.Func{`0,``0},System.Action{`0,``0},System.Func{``0,``1},System.Func{``1,``0},System.String)` | protected static | n/a | n/a | n/a | Binds two ways through converters: `format` out, `parse` back in, as for a numeric text field. |
| Method | `SkeleKit.ContentView`1.BindToSource``1(System.Func{`0,``0},System.Action{`0,``0},System.String)` | protected static | n/a | n/a | n/a | Binds control to source only: the control writes, and never reads back. |
| Method | `SkeleKit.ContentView`1.BindOnce``1(System.Func{`0,``0},System.String)` | protected static | n/a | n/a | n/a | Reads the value once when the ViewModel attaches, then never again. |
| Method | `SkeleKit.ContentView`1.#ctor(`0)` | protected | n/a | n/a | n/a | Stores the ViewModel, so the derived constructor composes its tree against it directly. |
| Property | `SkeleKit.ContentView`1.ViewModel` | public get | C# default | No | No automatic invalidation | The ViewModel this page was built around. Bindings resolve against it. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `ViewModel` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ContentView<object> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ItemView<T>

The element tree for one item in a `CollectionView`.

- Source: `SkeleKit.iOS/Elements/ItemView.cs`
- Inheritance/shape: `class ItemView<T> : Panel`
- Inherited API: [`View`](view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ItemView`1.Item` | public get/set | null | No | No automatic invalidation | The item this cell shows. Swapped on reuse; the bindings re-fire. |
| Property | `SkeleKit.ItemView`1.Content` | public get/set | null | No | No automatic invalidation | The cell's element tree. |
| Method | `SkeleKit.ItemView`1.Bind``1(System.Func{`0,``0},System.String)` | protected static | n/a | n/a | n/a | Binds one way to an item property. |
| Method | `SkeleKit.ItemView`1.Bind``2(System.Func{`0,``0},System.Func{``0,``1},System.String)` | protected static | n/a | n/a | n/a | Binds one way through a converter. |
| Method | `SkeleKit.ItemView`1.MeasureOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemView`1.ArrangeOverride(SkeleKit.Size)` | protected override | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ItemView`1.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Item`, `Content` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ItemView<object> specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## Panel

A `View` that lays out one or more children, hosted by a native LayoutHost.

- Source: `SkeleKit.iOS/Elements/Panel.iOS.cs`
- Inheritance/shape: `class Panel`
- Inherited API: [`View`](view.md)
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.Panel.Children` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The panel's children. Collection-initializer friendly. |
| Property | `SkeleKit.Panel.Padding` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Empty space between the panel's edge and its children. |
| Method | `SkeleKit.Panel.#ctor` | public (compiled) | n/a | n/a | n/a | Creates the panel and its `Panel.Children` collection. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Children`, `Padding` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(Panel specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```

## ViewCollection

The children of a `Panel`, raising a change callback so the panel can relayout.

- Source: `SkeleKit.iOS/Elements/ViewCollection.cs`
- Inheritance/shape: `class ViewCollection : IEnumerable<View>`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Visual showcase; add an interactive lab when callbacks, focus, selection, scrolling, loading, or presentation are observable.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ViewCollection.Count` | public get | 0 | No | No automatic invalidation | The number of children. |
| Property | `SkeleKit.ViewCollection.Item(System.Int32)` | public/protected as emitted | implementation-defined; inspect source | No | n/a | The child at `index`. |
| Method | `SkeleKit.ViewCollection.Add(SkeleKit.View)` | public | n/a | n/a | n/a | Adds a child to the panel. |
| Method | `SkeleKit.ViewCollection.Remove(SkeleKit.View)` | public | n/a | n/a | n/a | Removes a child from the panel. |
| Method | `SkeleKit.ViewCollection.Clear` | public | n/a | n/a | n/a | Removes all children. |
| Method | `SkeleKit.ViewCollection.GetEnumerator` | public | n/a | n/a | n/a | Returns an enumerator over the children. |

### Showcase matrix

| Scenario | Declared properties covered | Interaction and expected result |
| --- | --- | --- |
| Deliberate property/state matrix | `Count`, `Item` | Give every listed property at least one nondefault or semantic-edge state. Toggle each independently, preserve focus/selection where relevant, and match the default/semantics table. Repeat enabled/disabled, empty/populated, focused/unfocused, selected/unselected, light/dark, Dynamic Type, and iPad presentation where supported. |

```csharp
// Compile this specimen inside a SkeleKit page; each matrix row supplies a deliberate value.
static void Showcase(ViewCollection specimen)
{
	_ = specimen; // configure the documented properties for the selected matrix row
}
```
