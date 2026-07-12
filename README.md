# BareUI.iOS

A declarative, WPF-inspired UI library for **.NET for iOS** — no MAUI, no XAML. Native UIKit
controls behind C# object-initializer syntax with AOT-safe MVVM bindings. App code never touches
`UIViewController`, `NSLayoutConstraint`, or `AppDelegate` boilerplate.

- **100% native look & feel** — every control wraps the real UIKit control 1:1. BareUI owns
  composition and layout, never rendering.
- **WPF mental model, C# only** — `Grid`, `StackPanel`, `Margin`, `Alignment`, element trees via
  object initializers, MVVM with bindings and commands.
- **AOT-safe by construction** — iOS device builds are Mono full AOT + trimmed. No reflection,
  no expression trees, no runtime codegen anywhere. Compiled getter delegates +
  `[CallerArgumentExpression]` power the bindings.
- **Dark mode, Dynamic Type, safe areas, keyboard avoidance** handled by the framework.
- Works with plain `INotifyPropertyChanged` / `ICommand` — CommunityToolkit.Mvvm fits perfectly,
  no BareUI base ViewModel required.

## Quick start

```csharp
// Program.cs — the whole bootstrap
BareApp.Create()
	.UseServices(services =>
	{
		services.AddSingleton<ICounterService, CounterService>();
		services.AddTransient<CounterViewModel>();
	})
	.UsePages(pages => pages.AddSingleton<CounterView>())
	.SinglePage<CounterView>()
	.Run(args);
```

```csharp
// CounterView.cs — a page composes its tree in the constructor
public class CounterView : ContentView<CounterViewModel>
{
	public CounterView()
	{
		Title = "Counter";

		Content = new VStack
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
					Text = "Tap me",
					Kind = ButtonStyle.Filled,
					Command = Bind<ICommand?>(vm => vm.IncrementCommand)
				}
			}
		};
	}
}
```

```csharp
// CounterViewModel.cs — plain CommunityToolkit.Mvvm
public partial class CounterViewModel : ObservableObject
{
	[ObservableProperty]
	int count;

	[RelayCommand]
	void Increment() => Count++;
}
```

## Styling

There is no `ResourceDictionary` and no setter system: shared values are plain statics, and a
style is a typed action over the control — IntelliSense, compile-time checking, zero reflection.

```csharp
static class Palette
{
	public static readonly Color Card = Colors.SecondaryGroupedBackground;
}

static class Styles
{
	public static readonly Style<Label> Caption = new(label =>
	{
		label.TextStyle = TextStyle.Caption1;   // the native type hierarchy, Dynamic Type included
		label.TextColor = Colors.SecondaryLabel;
	});

	// BasedOn: Card runs first, then the overrides
	public static readonly Style<Border> Card = new(border =>
	{
		border.Background = Palette.Card;
		border.CornerRadius = 12;
	});
	public static readonly Style<Border> ProminentCard = new(Card, border =>
		border.Shadow = new(opacity: 0.2, radius: 8, offsetY: 4));
}

// Explicit — Style goes FIRST in the initializer: later lines override it
new Label { Style = Styles.Caption, Text = "Runtime" }

// Implicit — one app-global theme, applied to every view of the type as it is built
BareApp.Create()
	.UseTheme(theme => theme
		.Style(new Style<Label>(label => label.TextColor = Colors.Label))
		.Style(new Style<Button>(button => button.Kind = ButtonStyle.Tinted)))
```

Precedence, each source beating the one before it: control defaults → theme styles (base type
first) → explicit `Style` → whatever the initializer assigns after it.

## What's in the box

- **Layout**: `Grid` (star/auto/pixel, spans, spacing), `VStack`/`HStack`, `Overlay`, `Border`,
  `ScrollView`, per-view `IgnoresSafeArea`. Two-pass measure/arrange engine, unit-testable off-device.
- **Controls**: `Label`, `Button`, `Image` (async, cached), `TextField`, `SecureField`,
  `TextEditor`, `Switch`, `Slider`, `Stepper`, `ProgressBar`, `ActivityIndicator`, `Divider`,
  `Picker<T>`, and `NativeView` as the UIKit escape hatch.
- **Lists**: virtualized `CollectionView<T>` over `UICollectionView` + diffable data source —
  list (incl. inset-grouped), grid, carousel; sections + headers; pull-to-refresh, swipe actions,
  context menus, empty view, live `INotifyCollectionChanged` updates.
- **Bindings**: one-way / two-way / one-way-to-source / one-time, converters, update triggers,
  nested paths, `BindingContext` inheritance. Background-thread updates marshal to the UI thread.
- **Navigation**: ViewModel-first `INavigator` — push/pop, modals + sheets (detents),
  alert / confirm / action sheet. Shell in one line: `Tabs(...)`, `Stack<T>()`, `SinglePage<T>()`,
  `SidebarOnIPad()`.
- **Page chrome**: titles (incl. large), toolbar items, search bar, background styles, lifecycle
  hooks (`OnAppearing`, `OnLoaded`, ...).
- **Styling**: typed `Style<T>` with `BasedOn`, an app-global `Theme` of implicit styles, and
  `Label.TextStyle` for the native type hierarchy.
- **System integration**: dark mode (semantic + dynamic colors), Dynamic Type, VoiceOver
  (labels, hints, traits), haptics, keyboard avoidance and dismissal, gestures.

## Requirements

- .NET 10, `net10.0-ios`, iOS 18+
- No dependencies beyond `Microsoft.Extensions.DependencyInjection`

## Repository layout

| Path | What |
|---|---|
| `BareUI.iOS/` | The library (multi-targets a `net10.0` shim so the layout engine unit-tests without a simulator) |
| `BareUI.Tests/` | xunit tests for the layout + binding engines |
| `Samples/BareUI.Gallery/` | Gallery app: every control and layout, MVVM end to end |
| `Docs/` | Architecture, API sketch, ADRs |

## Escape hatches

UIKit is never required, but always reachable: wrap any `UIView` with `NativeView`, reach the
native control via `view.Native`, the hosting controller via `ContentView.Controller`, or attach
any `UIGestureRecognizer` with `view.AddGesture(...)`.
