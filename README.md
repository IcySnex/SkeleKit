# SkeleKit.iOS

A WPF-inspired UI library for **.NET for iOS** — no MAUI, no XAML. Native UIKit controls behind C# object-initializer syntax with AOT-safe MVVM bindings. App code never touches `UIViewController`, `NSLayoutConstraint`, or `AppDelegate` boilerplate.

- **Native UIKit, 1:1** — wraps the real UIKit controls. SkeleKit owns composition and layout, never rendering.
- **Clean C# syntax** — `Grid`, `StackPanel`, `Overlay`, `Border`, `Margin`, `Alignment`, MVVM bindings and commands. Element trees via object initializers.
- **AOT-safe by construction** — device builds are Mono full AOT + trimmed. No reflection, no expression trees, no runtime codegen. Bindings use compiled getters + `[CallerArgumentExpression]`.
- **Framework concerns handled** — dark mode, Dynamic Type, safe areas, keyboard avoidance.
- **No base ViewModel required** — works with plain `INotifyPropertyChanged` / `ICommand`.
  CommunityToolkit.Mvvm fits naturally.

## Quick start

```csharp
// Program.cs
SkeleApplication.CreateBuilder()
    .UseServices(services => services.AddTransient<CounterViewModel>())
    .UsePages(pages => pages.AddSingleton((CounterViewModel vm) => new CounterView(vm)))
    .SinglePage<CounterView>()
    .Build()
    .Run(args);

// CounterView.cs — a page composes its tree in the constructor
public class CounterView : ContentView<CounterViewModel>
{
    public CounterView(
        CounterViewModel viewModel) : base(viewModel)
    {
        Title = "Counter";

        Content = new StackPanel
        {
            Spacing = 12,
            Padding = new Thickness(16),
            Children =
            {
                new Label
                {
                    Text = Bind<int, string?>(vm => vm.Count, count => $"Tapped {count} times"),
                    FontSize = 34, FontWeight = FontWeight.Bold
                },
                new Button
                {
                    Text = "Tap me",
                    Kind = ButtonStyle.Filled,
                    Command = ViewModel.IncrementCommand      // off the injected ViewModel
                }
            }
        };
    }
}

// CounterViewModel.cs — plain CommunityToolkit.Mvvm
public partial class CounterViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int Count { get; set; }

    [RelayCommand]
    void Increment() => Count++;
}
```

## Styling

No `ResourceDictionary` and no setter system: shared values are plain statics, and a style is a
typed action over the control — IntelliSense, compile-time checking, zero reflection.

```csharp
static class Styles
{
    public static readonly Style<Label> Caption = new(label =>
    {
        label.TextStyle = TextStyle.Caption1;      // native type hierarchy, Dynamic Type included
        label.TextColor = Colors.SecondaryLabel;
    });

    // BasedOn: Card runs first, then the overrides
    public static readonly Style<Border> Card = new(border =>
    {
        border.Background = Colors.SecondaryGroupedBackground;
        border.CornerRadius = 12;
    });
    public static readonly Style<Border> ProminentCard = new(Card, border =>
        border.Shadow = new(opacity: 0.2, radius: 8, offsetY: 4));
}

new Label { Style = Styles.Caption, Text = "Runtime" };   // explicit: Style goes FIRST, later lines win

// implicit: one app-global theme, applied to every view of the type
SkeleApplication.CreateBuilder()
    .UseTheme(theme => theme.Style(new Style<Button>(b => b.Kind = ButtonStyle.Tinted)));
```

Precedence (each source beats the previous): control defaults → theme (base type first) → explicit
`Style` → the initializer after it.

## What's in the box

- **Layout**: `Grid` (star/auto/pixel, spans, spacing), `StackPanel`, `Overlay`, `Border`,
  `ScrollView`, per-view `IgnoresSafeArea`. Two-pass measure/arrange engine, unit-testable off-device.
- **Controls**: `Label`, `Button`, `Image` (async, cached), `TextField`, `SecureField`, `TextEditor`,
  `TextView` (rich text + links), `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`,
  `Divider`, `Picker<T>`, `SegmentedControl`, `DatePicker`, `PageControl`, `ColorWell`, `WebView`, and
  `NativeView` as the UIKit escape hatch.
- **Lists**: virtualized `CollectionView<T>` over `UICollectionView` + diffable data source — list
  (incl. inset-grouped), grid, carousel; sections + headers; pull-to-refresh, swipe actions, context
  menus, reorder, empty view, live `INotifyCollectionChanged` updates.
- **Bindings**: one-way / two-way / one-way-to-source / one-time, converters, update triggers, nested
  paths, `BindingContext` inheritance. Background-thread updates marshal to the UI thread.
- **Navigation**: ViewModel-first `INavigator` — push/pop, modals + sheets (detents), alert / confirm
  / action sheet. Shell in one line: `Tabs(...)`, `Stack<T>()`, `SinglePage<T>()`, `SidebarOnIPad()`.
- **Visual & animation**: `Brush` (solid / gradient / material), `Shadow`, `CornerRadius`; `Animation`
  + an interruptible, scrubbable `Animator`.
- **Styling**: typed `Style<T>` with `BasedOn`, an app-global `Theme`, `Label.TextStyle`.
- **System integration**: dark mode, Dynamic Type, VoiceOver, haptics, keyboard avoidance, gestures.

## Requirements

- .NET 10, `net10.0-ios`, iOS 18+
- No dependencies beyond `Microsoft.Extensions.DependencyInjection`

## Repository layout

| Path | What |
|---|---|
| `SkeleKit.iOS/` | The library (multi-targets a `net10.0` shim so the layout engine unit-tests without a simulator) |
| `SkeleKit.Tests/` | xunit tests for the layout + binding engines |
| `Samples/SkeleKit.Gallery/` | Gallery app: every control and layout, MVVM end to end |
| `Docs/` | Architecture, API sketch, ADRs |

## Escape hatches

UIKit is never required, but always reachable: wrap any `UIView` with `NativeView`, reach the native
control via `view.Native`, the hosting controller via `ContentView.Controller`, or attach a
`UIGestureRecognizer` with `view.AddGesture(...)`.
