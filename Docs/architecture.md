# SkeleKit.iOS — Architecture

Five layers, each depending only on the ones below it:

```
5. App model      SkeleApplication, INavigator, PageHost, ContentView<TVm>, Haptics
4. Binding        Bindable<T>, BindingExpression<T>, Binding<T>, commands, MainThread
3. Controls       Label, Button, Image, TextField… (1:1 native wrappers)
2. Layout         Grid, StackPanel, ScrollView, Border, Overlay + measure/arrange
1. Element model  View (wraps one UIView), Panel, LayoutHost (UIView subclass)
```

UIKit is an implementation detail of layers 1–3 and the hosting glue in layer 5. Nothing above
layer 1 exposes a UIKit type in its public API (except the `NativeView`/`.Native` escape hatches).

## 1. Element model

Every element is a `View` that **owns exactly one native `UIView`**, created lazily. Elements are
plain C# objects — cheap to construct in object-initializer trees, alive for the screen's lifetime
(retained mode like WPF, not MVU rebuilds).

```
View (abstract)
 ├─ owns:  UIView Native            (created on first realize)
 ├─ layout:  Margin, Width/Height, Min/Max, HorizontalAlignment, VerticalAlignment
 ├─ visual:  IsVisible, Opacity, Background, CornerRadius, ClipsToBounds, Shadow, Tint
 ├─ interaction: IsEnabled, TapCommand, gestures
 └─ lifecycle:   Realize()/Unrealize(), OnLoaded/OnUnloaded
Panel : View     └─ Children : ViewCollection
Control : View   (base for native wrappers)
```

- **Realization**: constructing the tree builds no UIKit objects. On present, the tree realizes
  top-down (create native view, apply pending properties, activate bindings). Unrealize tears down
  bindings deterministically — no finalizers.
- **Property storage**: values live on the C# element and forward to the native view once realized,
  so object initializers are order-independent and trees build off-screen. No dependency-property
  system — plain properties + `Set(ref field, value)` which forwards to native and invalidates layout.
- **Styling** (ADR-008): a `Style<T>` is an `Action<T>` over the typed view — no registry, no boxing,
  no reflection. `View.Style` applies one in its setter; `Theme` (`UseTheme`) applies its implicit
  styles in the `View` base ctor, base-most first. Precedence (defaults → theme → `Style` → local) is
  just C# construction order; nothing tracks a value's source.

## 2. Layout engine

Custom two-pass **measure/arrange** (WPF semantics), computing frames directly (ADR-002).

- **Host**: each `Panel`'s native view is a `LayoutHost : UIView` overriding `LayoutSubviews()`
  (arrange) and `SizeThatFits(CGSize)` (measure), so SkeleKit panels and native controls interoperate
  through the standard UIKit sizing protocol.
- **Measure** (`Size Measure(Size available)`): panels recurse; leaf controls delegate to the native
  `SizeThatFits`. Cached per available-size; `InvalidateMeasure()` bubbles to the root → `SetNeedsLayout`.
- **Arrange** (`void Arrange(Rect final)`): applies margin, alignment, min/max, then positions by
  bounds+center. No constraints, no solver.
- **Safe area — one regime**: a page sits inside the safe area (`PageHost` insets the root). A view
  escapes it with `IgnoresSafeArea` (edge flags); a *scrolling* view turns that bleed into a content
  inset along its scroll axis, so the scroll passes under the bar but its content never does. SkeleKit
  owns every scroll inset (`ContentInsetAdjustmentBehavior = Never`).
- **Environment changes**: `PageHost` observes Dynamic Type (→ `InvalidateSubtree`) and
  `UITraitUserInterfaceStyle` (→ `ReapplyVisuals`, re-resolving CGColor snapshots). Keyboard frame
  changes shrink the page (SwiftUI semantics) with a slide-up fallback.

### Panels (v1)

| Panel | Semantics |
|---|---|
| `Grid` | `Rows`/`Columns` of `GridLength` (`Auto`, `Star`/`N*`, pixels); `Row()/Column()/RowSpan()/ColumnSpan()` extension methods store into the child's `LayoutParams`; `RowSpacing`/`ColumnSpacing` |
| `StackPanel` | `Orientation`, `Spacing`; measures unconstrained on the stacking axis |
| `ScrollView` | wraps `UIScrollView`; content measured unconstrained on scroll axis; owns keyboard-avoidance |
| `Border` | single child + `Padding`, `Stroke`, `StrokeThickness`, `CornerRadius`; also the generic padding wrapper |
| `Overlay` | z-stack; children positioned by alignment + margin |

`Padding` lives on panels and `Border`; `Margin` lives on every view.

## 3. Controls

Thin 1:1 wrappers — **configure the native view, never draw**. Each is ~50–150 lines: property
forwarding + binding hookup + measure delegation.

| SkeleKit | UIKit | Notes |
|---|---|---|
| `Label` | `UILabel` | `Text`/`Spans`, `TextStyle`, `FontSize`/`FontWeight`/`FontDesign` (`UIFontMetrics`-scaled), `TextColor`, `MaxLines`, `Truncation`, `TextAlignment` |
| `Button` | `UIButton` (config) | `Text`, `Icon` (SF Symbol), `Kind`, `Command`/`CommandParameter`, `Menu` |
| `Image` | `UIImageView` | `Source` (`ImageSource.Symbol/Bundle/Url`); default `IImageLoader` caches, dedups in-flight, pre-decodes; swap via `UseImageLoader`. SF Symbol styling + effects |
| `TextField`/`SecureField`/`TextEditor` | `UITextField`/`UITextView` | `Text` (TwoWay default), traits, keyboard toolbar/accessory |
| `TextView` | `UITextView` | read-only rich text with tappable `Link` runs |
| `Switch`/`Slider`/`Stepper`/`ProgressBar`/`ActivityIndicator`/`PageControl` | matching UIKit | |
| `Picker` | `UIButton` + `UIMenu` | `ItemsSource`/`SelectedItem` (typed) |
| `SegmentedControl`/`DatePicker`/`ColorWell`/`WebView` | matching UIKit | |
| `Divider` | hairline `UIView` | |
| `NativeView` | any | escape hatch; every `View` also exposes `.Native` after realize |

Numeric entry = `TextField` with `Keyboard = Numeric` + a converter on the binding — no dedicated control.

## 4. Binding system (AOT-safe — ADR-003)

Surface syntax (inside a `ContentView<TVm>`, where `Bind` is a protected helper):

```csharp
Text    = Bind(vm => vm.Title);                                      // OneWay
Text    = Bind(vm => vm.Name, (vm, v) => vm.Name = v);               // TwoWay
Text    = Bind<TimeSpan, string?>(vm => vm.Duration, d => d.L10N()); // converter
IsOn    = Bind(vm => vm.Config.Appearance.AnimateTabBar,
               (vm, v) => vm.Config.Appearance.AnimateTabBar = v);   // nested path, TwoWay
Command = ViewModel.PlayCommand;                                     // commands never bindable (ADR-012)
```

- Bindable control properties are `Bindable<T>` with implicit conversions from `T` and from
  `BindingExpression<T>`, so `Text = "Hi"` and `Text = Bind(...)` both type-check.
- `Bind` captures the getter as a compiled delegate — no reflection. The change-notification path
  comes from `[CallerArgumentExpression]`, parsed once (split after `=>`, then on `.`) into segments.
- **Nested paths**: the expression carries parsed segments; on attach the binding subscribes
  `PropertyChanged` on each intermediate `INotifyPropertyChanged`, re-resolving replaced intermediates
  on change. `BindingFactory.BindPath` is the explicit-path fallback.
- **Modes**: OneTime / OneWay (default) / TwoWay / OneWayToSource. Triggers: property-changed / focus-lost.
- **Threading**: a source may notify from any thread; the refresh marshals to main (`MainThread.Post`).
- **Target→source**: each control wires its own native change event; no global mapper.
- **Teardown**: bindings register with the owning `ContentView`; unrealize disposes them and
  unsubscribes INPC. `ICommand.CanExecuteChanged` drives `IsEnabled`.

## 5. App model, MVVM, navigation

### ContentView

```csharp
public class MovieInfoView : ContentView<MovieInfoViewModel>
{
    public MovieInfoView(MovieInfoViewModel vm) : base(vm) =>
        Content = new Grid { ... };   // composed in the ctor, against vm/ViewModel directly
}
```

- The `ViewModel` is **ctor-injected** (`: base(viewModel)`, resolved through DI) — no
  `OnViewModelAttached`; compose the tree against it directly and bind values/commands.
- Lifecycle: `OnLoaded` / `OnAppearing` / `OnDisappearing` / `OnUnloaded`.
- Page chrome as properties, not UIKit calls: `Title` (bindable), `TitleStyle`, `ToolbarItems`,
  `HidesNavigationBar`, `BackgroundStyle`, `SearchPlaceholder`/`SearchChanged`, `ScrollsUnderBars`.
- A hidden `PageHost : UIViewController` hosts the page (`ContentView.Controller` escape hatch).

### Navigation (view-centric registration, ViewModel-first navigation)

```csharp
SkeleApplication.CreateBuilder()
    .UseServices(s => s.AddSingleton<IMovieService, MovieService>())
    .Tabs(t => t
        .Tab<HomeView>("Home", icon: "house")
        .Tab<SearchView>("Search", icon: "magnifyingglass")
        .SidebarOnIPad())
    .Build()
    .Run(args);
```

- `[Page]` generates registration for both `ContentView` and `ContentView<TViewModel>`; `Build()`
  applies it automatically without runtime scanning. `UsePages(...)` can override generated defaults.
- `INavigator` (injectable, ViewModel-first): `PushAsync<TVm>()`/`PushAsync(vm)`/`PopAsync()`/
  `PopToRootAsync()`; registered views use `PushViewAsync<TView>()`, and existing page instances use
  `PushViewAsync(page)`. Modal equivalents are `PresentAsync<TVm>` and `PresentViewAsync<TView>`.
  Dialogs include `AlertAsync`/`ConfirmAsync`/`PromptAsync`/`SelectAsync`; `OpenUrlAsync` opens URLs.
- Shells: `Tabs(...)` (incl. iPad sidebar), `Stack<TView>()`, `SinglePage<TView>()`.
- `SkeleApplication` hides `Main`/`AppDelegate`/scene wiring and hosts the DI `IServiceProvider`.

### CollectionView (virtualization)

Built on `UICollectionView` + compositional layout + diffable data source:

```csharp
new CollectionView<Movie>
{
    Layout       = CollectionLayout.Grid(columns: 3, spacing: 12),   // or .List(grouped:), .Carousel()
    ItemsSource  = Bind<IReadOnlyList<Movie>?>(vm => vm.Movies),
    ItemTemplate = () => new PosterCell(),                           // built once per recycled cell
    SelectionCommand = ViewModel.OpenMovieCommand
}
```

- `ItemTemplate` returns an `ItemView<T>` subtree bound against an item context
  (`Bind(item => item.Title)`); on reuse only the context swaps and bindings re-fire.
- `ItemsSource` accepts `IReadOnlyList<T>`; if it's also `INotifyCollectionChanged`, changes flow
  through the diffable snapshot with animations. Sections via `GroupedItemsSource` + header templates;
  `.List` uses `UICollectionLayoutListConfiguration` for native inset-grouped lists.
- Also: pull-to-refresh, swipe actions, context menus, reorder, prefetch, `EmptyView`, `ScrollTo`,
  `Scrolled`, `CarouselSnap`. Snapshots coalesce onto the next run-loop turn; one cached `ItemKey`
  per item roots the identifier peers.

## System integration

- **Dark mode**: `Colors` palette + semantics resolve live UIKit dynamic colors;
  `Color.Dynamic(light, dark)` for custom pairs; CGColor snapshots re-resolve on theme change.
- **Dynamic Type**: fonts go through `UIFontMetrics`; a text-size change invalidates every measurement.
- **VoiceOver**: `AccessibilityLabel`/`AccessibilityValue` (bindable), `AccessibilityHint`,
  `AccessibilityIdentifier`, `AccessibilityTraits`, `IsAccessibilityElement`.
- **Misc**: `Haptics`, `View.Animate`, `View.AddGesture`, `View.Focus()/Unfocus()/IsFocused`,
  `ScrollView.KeyboardDismiss`.

## Interop & escape hatches

1. `NativeView` — wrap any `UIView` as a SkeleKit child.
2. `view.Native` — the wrapped UIKit view after realize.
3. `ContentView.Controller` — the hosting `UIViewController`.
4. Custom controls — subclass `Control`, override `CreateNative()` (+ measure if needed).
