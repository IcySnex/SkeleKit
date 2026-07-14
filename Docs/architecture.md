# BareUI.iOS — Architecture

Five layers, each depending only on the ones below it:

```
┌─────────────────────────────────────────────────────┐
│ 5. App model      BareApplication, INavigator,      │
│                   PageHost, ContentView<TVm>,       │
│                   Haptics                           │
├─────────────────────────────────────────────────────┤
│ 4. Binding        Bindable<T>, BindingExpression<T>,│
│                   Binding<T>, commands, MainThread  │
├─────────────────────────────────────────────────────┤
│ 3. Controls       Label, Button, Image, TextField…  │
│                   (1:1 native wrappers)             │
├─────────────────────────────────────────────────────┤
│ 2. Layout         Grid, StackPanel, ScrollView,     │
│                   Border, Overlay + measure/arrange │
├─────────────────────────────────────────────────────┤
│ 1. Element model  View (wraps one UIView), Panel,   │
│                   LayoutHost (UIView subclass)      │
└─────────────────────────────────────────────────────┘
```

UIKit is an implementation detail of layers 1–3 and the hosting glue in layer 5.
Nothing above layer 1 exposes a UIKit type in its public API (except the explicit
`NativeView` escape hatch).

---

## 1. Element model

Every BareUI element is a `View` that **owns exactly one native `UIView`** created lazily.
BareUI elements are plain C# objects — cheap to construct in object-initializer trees,
alive for the lifetime of the screen (retained mode, like WPF; *not* MVU rebuilds).

```
View (abstract)
 ├─ owns:  UIView Native            (created on first realize)
 ├─ layout props: Margin, Width, Height, MinWidth/MaxWidth, MinHeight/MaxHeight,
 │                HorizontalAlignment, VerticalAlignment (Start|Center|End|Stretch)
 ├─ visual props: IsVisible, Opacity, Background, CornerRadius, ClipsToBounds
 ├─ interaction:  IsEnabled, TapCommand, GestureRecognizers (escape hatch)
 └─ lifecycle:    Realize() / Unrealize(), OnLoaded / OnUnloaded

Panel : View (abstract)
 └─ Children : ViewCollection   (collection-initializer friendly)

Control : View (abstract base for native wrappers)
```

**Realization**: constructing the tree builds no UIKit objects. When a screen is presented,
the tree is *realized* top-down: each `View` creates its native view, applies pending
property values, activates bindings. Unrealize tears down bindings deterministically
(no finalizer-based cleanup).

**Property storage**: properties are stored on the C# element and forwarded to the native
view when realized. This keeps object initializers order-independent and lets trees be
built off-screen. No dependency-property system — plain properties + a small
`Set(ref field, value)` helper that forwards to native and invalidates layout when needed.

**Styling** (ADR-008): a `Style<T>` is an `Action<T>` over the typed view, so a setter block is
ordinary C# — no property registry, no boxing, no reflection. `View.Style` applies one in its
setter; the app-global `Theme` applies its implicit styles in the `View` base constructor, walking
the inheritance chain base-most first. Because C# runs field initializers before base constructor
bodies and object initializers after them, value precedence (defaults → theme → explicit `Style` →
local values) is just construction order — nothing tracks a value's source.

## 2. Layout engine

Custom two-pass **measure/arrange** (WPF semantics), computing frames directly.
See ADR-002 for why (vs Auto Layout translation).

- **Host**: each `Panel`'s native view is a `LayoutHost : UIView` that overrides
  `LayoutSubviews()` (arrange pass) and `SizeThatFits(CGSize)` (measure pass). This means
  BareUI panels compose correctly even when embedded *inside* arbitrary UIKit hierarchies,
  and native controls compose inside BareUI — the two worlds interoperate through the
  standard UIKit sizing protocol.
- **Measure**: `Size Measure(Size available)` — panels recurse; leaf controls delegate to
  the native control's `SizeThatFits` (so a `UILabel` wraps/sizes exactly like native).
  Results cached per available-size; invalidated by property changes
  (`InvalidateMeasure()` bubbles to the root, which calls `SetNeedsLayout`).
- **Arrange**: `void Arrange(Rect final)` — applies `Margin`, alignment, min/max clamping,
  then sets `Native.Frame`. No constraints, no solver, no `TranslatesAutoresizingMaskIntoConstraints`.
- **Safe area — one regime**: a page always sits *inside* the safe area (`PageHost` insets
  the root frame). A view escapes it with `IgnoresSafeArea` (edge flags) and grows back out;
  a *scrolling* view turns that bleed into a content inset along its scroll axis, so the
  scroll passes under the bar but its content never does. BareUI owns every scroll inset
  (`ContentInsetAdjustmentBehavior = Never` everywhere — UIKit's guesses are wrong both ways).
- **Environment changes**: `PageHost` observes `ContentSizeCategoryChangedNotification`
  (Dynamic Type → `InvalidateSubtree`, every cached measurement dropped) and registers for
  `UITraitUserInterfaceStyle` changes (theme → `ReapplyVisuals` walk re-resolving CGColor
  snapshots like border strokes and shadows; dynamic `UIColor`s adapt on their own).
  Keyboard frame changes shrink the page like a safe-area change (SwiftUI semantics), with
  a slide-up fallback for non-adaptive layouts.

### Panels (v1)

| Panel | Semantics |
|---|---|
| `Grid` | `Rows`/`Columns` of `GridLength` (`Auto`, `Star`/`N*`, pixels), `Row()/Column()/RowSpan()/ColumnSpan()` attached via extension methods storing into the child's `LayoutParams` bag; `RowSpacing`/`ColumnSpacing` |
| `StackPanel` | `Orientation`, `Spacing`; measures unconstrained on the stacking axis |
| `ScrollView` | wraps `UIScrollView`; content measured unconstrained on scroll axis; owns keyboard-avoidance (content inset on keyboard frame notifications) |
| `Border` | single child + `Padding`, `Stroke`, `StrokeThickness`, `CornerRadius` — also the generic "padding wrapper" (replaces Velura's `UIPaddedView`) |
| `Overlay` | z-stack; children positioned by alignment + margin (poster-over-backdrop scenarios) |

`Padding` lives on panels and `Border` (WPF-style); `Margin` lives on every view.

## 3. Controls

Thin 1:1 wrappers. Rule: **a control may configure its native view, never draw**.
Each wrapper is ~50–150 lines: property forwarding + binding hookup + measure delegation.

v1 set and native mapping:

| BareUI | UIKit | Notes |
|---|---|---|
| `Label` | `UILabel` | `Text`, `TextStyle` (the native type hierarchy), `FontSize`/`FontWeight`/`FontDesign` (maps to `UIFontMetrics`-scaled dynamic fonts by default), `TextColor`, `MaxLines`, `Truncation`, `TextAlignment` |
| `Button` | `UIButton` (UIButtonConfiguration) | `Text`, `Icon` (SF Symbol name), `Kind` (Plain/Tinted/Filled/Capsule…), `Command`/`CommandParameter` |
| `Image` | `UIImageView` | `Source` (`ImageSource.Symbol/Bundle/Url`); the default `IImageLoader` caches (`NSCache`), dedups in-flight downloads and pre-decodes; swap it via `UseImageLoader` |
| `TextField` | `UITextField` | `Text` (TwoWay default), `Placeholder`, `Keyboard`, `ReturnKey`, `OnSubmit` |
| `SecureField` | `UITextField` | secure entry preset |
| `TextEditor` | `UITextView` | multi-line |
| `Switch` | `UISwitch` | `IsOn` (TwoWay default) |
| `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator` | matching UIKit | |
| `Picker` | `UIButton` + `UIMenu` | `ItemsSource`/`SelectedItem` — replaces Velura's `UISelectionButton` |
| `Divider` | hairline `UIView` | |
| `NativeView` | any | escape hatch: `new NativeView(myUIView)`; also every `View` exposes `.Native` after realize |

Numeric entry (Velura's `UINumberField`) = `TextField` with `Keyboard = Numeric` + a
`Func<string,T>` converter on the binding — no dedicated control needed.

## 4. Binding system (AOT-safe — see ADR-003)

**Surface syntax** (inside a `ContentView<TVm>`, where `Bind` is a protected helper):

```csharp
Text    = Bind(vm => vm.Title);                                  // OneWay
Text    = Bind(vm => vm.Name, (vm, v) => vm.Name = v);           // TwoWay (explicit setter)
Text    = Bind<TimeSpan, string?>(vm => vm.Duration, d => d.L10N()); // converter
IsOn    = Bind(vm => vm.Config.Appearance.AnimateTabBar,
               (vm, v) => vm.Config.Appearance.AnimateTabBar = v); // nested path, TwoWay
// commands are never bindable (ADR-012) — assigned from the ctor-injected ViewModel
Command = ViewModel.PlayCommand;
```

**Mechanics**:

- Bindable control properties are typed `Bindable<T>` with implicit conversions from `T`
  (literal) and from `BindingExpression<T>` (what `Bind(...)` returns). So
  `Text = "Hi"` and `Text = Bind(...)` both compile with full type checking.
- `Bind` captures the getter as a **compiled delegate** (`Func<TVm,T>`) — evaluation never
  touches reflection. The *property path* for change notification comes from
  `[CallerArgumentExpression]`: the literal string `"vm => vm.Config.Appearance.AnimateTabBar"`
  is parsed once (split after `=>`, then on `.`) into segments
  `["Config","Appearance","AnimateTabBar"]`.
- **Nested paths**: a `BindingExpression<T>` carries the parsed segments, each with an
  optional step delegate. On attach the binding walks the chain, subscribing
  `PropertyChanged` on every intermediate `INotifyPropertyChanged`; when any watched
  segment fires it re-attaches (re-resolving intermediates that were replaced) and
  re-applies the leaf getter. `BindingFactory.BindPath` is the explicit-path fallback.
- **Modes**: OneTime / OneWay (default) / TwoWay (when setter supplied) / OneWayToSource.
  Update triggers: property-changed (default) / focus-lost.
- **Threading**: a source may notify from any thread; the binding marshals the refresh to
  the main thread (`MainThread.Post` — inline on the neutral test TFM, `DispatchQueue.MainQueue`
  on iOS). Same helper backs `CanExecuteChanged` and async image completion.
- **Target→source**: each control wires its own native change event (`UISwitch.ValueChanged`,
  `UITextField.EditingChanged`, …) in its wrapper — no global mapper registry.
- **Ownership & teardown**: bindings register with the owning `ContentView`; unrealize
  disposes all bindings and unsubscribes INPC handlers. No finalizers.
- `ICommand.CanExecuteChanged` → `IsEnabled` on `Button` (and `TapCommand` hosts).

## 5. App model, MVVM, navigation

### ContentView

```csharp
public class MovieInfoView : ContentView<MovieInfoViewModel>
{
    public MovieInfoView() => Content = new Grid { ... };   // composed in the ctor (XAML-compatible)
}

// The ViewModel is attached *after* construction, so bind values/commands rather than reading
// ViewModel directly. Anything that truly needs the instance goes in OnViewModelAttached().
```

- Typed `ViewModel` attached by the navigator after construction (resolved through DI).
- Lifecycle: `OnLoaded` / `OnAppearing` / `OnDisappearing` / `OnUnloaded` +
  `OnViewModelAttached`.
- Page-level chrome as properties, not UIKit calls: `Title` (bindable), `TitleStyle`
  (incl. large titles), `ToolbarItems` (leading/trailing bar buttons with `Icon` +
  `Command`), `HidesNavigationBar`, `BackgroundStyle`, `SearchPlaceholder`/`SearchChanged`,
  `ScrollsUnderBars`.
- Internally a hidden `PageHost : UIViewController` hosts the page; app code never sees it
  (`ContentView.Controller` escape hatch exists).

### Navigation (ViewModel-first, AOT-safe)

```csharp
BareApplication.CreateBuilder()
    .UseServices(s => { s.AddSingleton<IMovieService, MovieService>(); ... })
    .UsePages(pages => pages
        .AddSingleton((HomeViewModel vm) => new HomeView(vm))          // explicit registry — no scanning,
        .AddTransient((MovieInfoViewModel vm) => new MovieInfoView(vm))) // AOT-safe, reflection-free
    .Tabs(t => t
        .Tab<HomeView>("Home", icon: "house")
        .Tab<SearchView>("Search", icon: "magnifyingglass")
        .Tab<SettingsView>("Settings", icon: "gear")
        .SidebarOnIPad())
    .Build()
    .Run(args);
```

- `INavigator` (injectable into ViewModels, ViewModel-first only):
  - `PushAsync<TVm>()` / `PushAsync(vmInstance)` / `PopAsync()` / `PopToRootAsync()`
  - `PresentAsync<TVm>(ModalStyle)` — sheet (with detents), full-screen, form sheet
  - `AlertAsync(...)`, `ConfirmAsync(...)`, `PromptAsync(...)`, `ActionSheetAsync(...)`
- Shells: `Tabs(...)` (`UITabBarController`, incl. iPadOS sidebar via `SidebarOnIPad`),
  `Stack<TView>()` (`UINavigationController`), `SinglePage<TView>()`.
- `BareApplication` hides `Main.cs`/`AppDelegate`/`UIWindow` scene wiring; hosts the
  `IServiceProvider` (Microsoft.Extensions.DependencyInjection). `UseImageLoader` swaps the
  image pipeline.

### CollectionView (virtualization)

Built on `UICollectionView` + compositional layout + diffable data source:

```csharp
new CollectionView<Movie>
{
    Layout      = CollectionLayout.Grid(columns: 3, spacing: 12),   // or .List(grouped: true), .Carousel()
    ItemsSource = Bind<IReadOnlyList<Movie>?>(vm => vm.Movies),
    ItemTemplate = () => new PosterCell(),                     // element tree built once per recycled cell
    SelectionCommand = ViewModel.OpenMovieCommand              // receives the tapped item
}
```

- `ItemTemplate` returns a `View` subtree with bindings against an **item context**
  (`ItemView<T>` with its own `Bind(item => item.Title)`); on cell reuse only the context
  swaps and bindings re-fire — the native/element tree is never rebuilt while scrolling.
- `ItemsSource` accepts `IReadOnlyList<T>`; if it also implements
  `INotifyCollectionChanged` (incl. Velura's `ObservableRangeCollection`), changes flow
  through the diffable data source snapshot with animations.
- Sections (`GroupedItemsSource`) + header templates; `.List` layout uses
  `UICollectionLayoutListConfiguration` so Settings-style inset-grouped lists are native.
- Also: pull-to-refresh (`RefreshCommand`), native swipe actions, context menus,
  `EmptyView`, `ScrollTo(item)`, `Scrolled`, `CarouselSnap`. Snapshots coalesce onto the
  next run-loop turn, so an `Add` loop is one diff; one cached `ItemKey` per item roots
  the identifier peers and avoids per-snapshot allocation.

## System integration

- **Dark mode**: the `Colors` palette + semantic colors (`Label`, `Separator`,
  backgrounds, ...) resolve live UIKit dynamic colors; `Color.Dynamic(light, dark)` for
  custom pairs. CGColor snapshots re-resolve on theme change (see layer 2).
- **Dynamic Type**: fonts go through `UIFontMetrics`; a text-size change invalidates every
  cached measurement.
- **VoiceOver**: `AccessibilityLabel`/`AccessibilityValue` (bindable), `AccessibilityHint`,
  `AccessibilityIdentifier`, `AccessibilityTraits` (OR'd onto the control's own),
  `IsAccessibilityElement`.
- **Haptics** (`Impact`/`Selection`/`Notify`), `View.Animate`, `View.AddGesture`,
  `View.Focus()/Unfocus()/IsFocused`, `KeyboardDismiss` modes on `ScrollView`.

## Interop & escape hatches

1. `NativeView` — wrap any `UIView` as a BareUI child.
2. `view.Native` — reach the wrapped UIKit view after realize (custom tweaks, animations).
3. `ContentView.Controller` — the hosting `UIViewController` for edge cases.
4. Custom controls — subclass `Control`, override `CreateNative()` + measure if needed;
   the same API BareUI's own controls use (documented pattern for things like Velura's
   `ExpandableTextView`, gradient layers, scroll-linked effects).

Rule of thumb: exotic screens like Velura's `MovieInfoViewController` scroll-parallax keep
their custom pieces as custom controls; BareUI still removes all the layout/binding noise
around them.
