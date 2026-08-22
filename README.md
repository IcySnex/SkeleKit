> [!WARNING]
> SkeleKit is an early preview. It is usable for experimentation, but APIs may change before 1.0.

---

# SkeleKit.iOS

A WPF-inspired UI library for **.NET for iOS** - no MAUI, no XAML. Native UIKit controls behind C# object-initializer syntax with AOT-safe MVVM bindings. App code never touches `UIViewController`, `NSLayoutConstraint`, or `AppDelegate` boilerplate.

- **Native UIKit, 1:1**: wraps the real UIKit controls. SkeleKit owns composition and layout, never rendering.
- **Clean C# syntax**: `Grid`, `StackPanel`, `Overlay`, `Border`, `Margin`, `Alignment`, MVVM bindings and commands. Element trees via object initializers.
- **AOT-safe by construction**: device builds are Mono full AOT + trimmed. No reflection, no expression trees, no runtime codegen. Bindings use compiled getters + `[CallerArgumentExpression]`.
- **Framework concerns handled**: dark mode, Dynamic Type, safe areas, keyboard avoidance.
- **No base ViewModel required**: works with plain `INotifyPropertyChanged` / `ICommand`.
  CommunityToolkit.Mvvm fits naturally.

## Requirements

- macOS with Xcode and the .NET 10 iOS workload
- .NET 10, `net10.0-ios`, iOS 18+
- No dependencies beyond `Microsoft.Extensions.DependencyInjection`

## Quick start

```bash
dotnet new install SkeleKit.Templates
dotnet new skelekit-ios -n MyIosApp
cd MyIosApp
dotnet build -r iossimulator-arm64
```

The template creates the scene manifest, SkeleKit host, first `[Page]`, launch screen, entitlements, and app-icon catalog. It references `SkeleKit.iOS`, which also carries the page source generator and the simulator hot-reload build targets.

<details>

<summary>Create project manually</summary>

1. **Create the .NET project:**

   ```bash
   dotnet new ios -n MyIosApp
   ```

2. **Target iOS 18 or later:** set `<SupportedOSPlatformVersion>18.0</SupportedOSPlatformVersion>`
   in the project file.

3. **Add SkeleKit:**

   ```bash
   dotnet add package SkeleKit.iOS
   ```

4. **Use SkeleKit's scene delegate:** in `Info.plist`, set
   `UIApplicationSceneManifest` → `UISceneConfigurations` →
   `UIWindowSceneSessionRoleApplication` → item 0 → `UISceneDelegateClassName` to
   `SkeleWindowSceneDelegate`. Delete the generated `AppDelegate.cs` and `SceneDelegate.cs`.

5. **Create the first page:**

   ```cs
   using SkeleKit;

   [Page]
   public class MainView : ContentView
   {
       public MainView()
       {
           Content = new Label() { Text = "Hello World!" };
       }
   }
   ```

6. **Replace `Main.cs` with the SkeleKit host:**

   ```cs
   using SkeleKit;

   SkeleApplication.CreateBuilder()
        .SinglePage<MainView>()
        .Build()
        .Run(args);
   ```

</details>

## Getting started

The following ViewModel uses the optional `CommunityToolkit.Mvvm` package; SkeleKit itself does not require a base ViewModel type.

```csharp
// CounterView.cs
[Page]
public class CounterView : ContentView<CounterViewModel>
{
    public CounterView(CounterViewModel viewModel) : base(viewModel)
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
                    FontSize = 34,
                    FontWeight = FontWeight.Bold
                },
                new Button
                {
                    Text = "Click me",
                    Kind = ButtonStyle.Filled,
                    Command = viewModel.IncrementCommand
                }
            }
        };
    }
}

// CounterViewModel.cs
public partial class CounterViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int Count { get; set; }

    [RelayCommand]
    void Increment() => Count++;
}
```

## Styling

No `ResourceDictionary` and no setter system: shared values are plain statics, and a style is a typed action over the control: IntelliSense, compile-time checking, zero reflection.

```csharp
static class Styles
{
    public static readonly Style<Label> Caption = new(label =>
    {
        label.FontSize = 12;
        label.TextColor = Colors.SecondaryLabel;
    });

    // BasedOn: Card runs first, then the overrides
    public static readonly Style<Border> Card = new(border =>
    {
        border.Background = Colors.SecondaryGroupedBackground;
        border.CornerRadius = 12;
    });
    public static readonly Style<Border> ProminentCard = new(Card, border =>
    {
        border.Shadow = new(opacity: 0.2, radius: 8, offsetY: 4);
    });
}

new Label
{
    Style = Styles.Caption, // explicit: Style goes FIRST, later lines win
    Text = "Runtime"
};

// App-global themes, applied to every view of the type
SkeleApplication.CreateBuilder()
    .UseTheme(theme => theme.Style(new Style<Button>(b => b.Kind = ButtonStyle.Tinted)));
```

Precedence (each source beats the previous): control defaults → theme (base type first) → explicit `Style` → the initializer after it.

## What's in the box

- **Layout**: `Grid` (star/auto/pixel, spans, spacing), `StackPanel`, `Overlay`, `Border`, `ScrollView`, per-view `IgnoresSafeArea`. Two-pass measure/arrange engine, unit-testable off-device.
- **Controls**: `Label`, `Button`, `Image` (async, cached), `TextField`, `SecureField`, `TextEditor`, `TextView` (rich text + links), `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`, `Picker<T>`, `SegmentedControl`, `DatePicker`, `PageControl`, `ColorWell`, `MapView`, `WebView`, and `NativeView` as the UIKit escape hatch.
- **Lists**: virtualized `CollectionView<T>` over `UICollectionView` + diffable data source — list (incl. inset-grouped), grid, carousel; sections + headers; pull-to-refresh, swipe actions, context menus, reorder, empty view, live `INotifyCollectionChanged` updates.
- **Bindings**: one-way / two-way / one-way-to-source / one-time, converters, update triggers, nested paths, `BindingContext` inheritance. Background-thread updates marshal to the UI thread.
- **Navigation**: ViewModel-first `INavigator` — push/pop, modals + sheets (detents), alert / confirm / action sheet. Shells via `Tabs(...)`, `Stack<T>()`, or `SinglePage<T>()`; tab apps can opt into an iPad sidebar with `.OnPad(pad => pad.Sidebar())`.
- **Visual & animation**: `Brush` (solid / gradient / material), `Shadow`, `CornerRadius`; `Animation` + an interruptible, scrubbable `Animator`.
- **Styling**: typed `Style<T>` with `BasedOn`, an app-global `Theme`, `Label.TextStyle`.
- **System integration**: dark mode, Dynamic Type, VoiceOver, haptics, keyboard avoidance, gestures.

## Repository layout

| Path | What |
|---|---|
| `Source/SkeleKit.slnx` | Main .NET solution |
| `Source/Framework/SkeleKit.iOS/` | The library (multi-targets a `net10.0` shim so the layout engine unit-tests without a simulator) |
| `Source/Framework/SkeleKit.Generators/` | Source generators embedded in the `SkeleKit.iOS` package |
| `Source/Tests/SkeleKit.Tests/` | xunit tests for the layout + binding engines |
| `Source/Samples/SkeleKit.Gallery/` | Gallery app: every control and layout, MVVM end to end |
| `Source/Samples/SkeleKit.Template/` | Minimal app that is also the source for `dotnet new skelekit-ios` |
| `Source/Packaging/` | NuGet template-pack project |
| `Tools/` | Developer tooling, including the Rider plugin |
| `Docs/` | Architecture, API sketch, ADRs |
| `Assets/` | Brand and design source assets |

## Escape hatches

UIKit is never required, but always reachable: wrap any `UIView` with `NativeView`, reach the native control via `view.Native`, the hosting controller via `ContentView.Controller`, or attach a `UIGestureRecognizer` with `view.AddGesture(...)`.
