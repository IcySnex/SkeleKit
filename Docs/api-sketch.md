# BareUI.iOS — API Sketch (Velura before/after)

What *app* code looks like; no UIKit type appears anywhere. Every section is the shipped API.

## 1. App bootstrap

Replaces `Main.cs`, `AppDelegate.cs`, and the window/tab wiring in
`MainViewController.cs`:

```csharp
// Program.cs — the entire iOS head entry point
BareApp.Create()
    .UseServices(services =>
    {
        services.AddSingleton<Config>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddTransient<HomeViewModel>();
        // ... existing Velura service registrations unchanged
    })
    .UsePages(pages => pages
        .AddSingleton<HomeView>()          // keeps its state across navigations
        .AddSingleton<SearchView>()
        .AddSingleton<SettingsView>()
        .AddTransient<MovieInfoView>())    // fresh instance per push
    .Tabs(tabs => tabs
        .LargeTitles()
        .Tab<HomeView>("home_title", icon: "house")
        .Tab<SearchView>("search_title", icon: "magnifyingglass")
        .Tab<SettingsView>("settings_title", icon: "gear")
        .SidebarOnIPad())
    .Run(args);
```

## 2. A settings-style page

What `SettingsGroupViewController` + `SettingsGroupItemViewCell` +
`SettingsGroupHeaderView` (~460 lines of UIKit) collapse into:

```csharp
public class SettingsGroupView : ContentView<SettingsGroupViewModel>
{
    public SettingsGroupView()
    {
        Title = Bind(vm => vm.GroupName);

        Content = new CollectionView<SettingsEntry>
        {
            Layout = CollectionLayout.List(grouped: true),
            GroupedItemsSource = Bind<IReadOnlyList<Section<SettingsEntry>>?>(vm => vm.Sections),
            SelectionCommand = Bind<ICommand?>(vm => vm.OpenGroupCommand),
            ItemTemplate = () => new SettingsRow(),
            HeaderTemplate = () => new SectionHeader()
        };
    }
}

class SettingsRow : ItemView<SettingsEntry>
{
    public SettingsRow() => Content = new Grid
    {
        Columns = { GridLength.Auto, GridLength.Star, GridLength.Auto },
        ColumnSpacing = 12,
        Padding = new Thickness(16, 8),
        Children =
        {
            new Image
            {
                Source = Bind<string, ImageSource?>(i => i.IconName, n => ImageSource.Symbol(n)),
                Background = Bind(i => i.IconBackground),
                CornerRadius = 6,
                Width = 29, Height = 29,
            },
            new Label { Text = Bind(i => i.Name) }.Column(1),
            new Switch
            {
                IsOn = Bind(i => i.IsEnabled, (i, v) => i.IsEnabled = v),
                IsVisible = Bind(i => i.IsToggle),
            }.Column(2),
        }
    };
}
```

## 3. Layout-heavy content (MovieInfo top section)

The declarative part of `MovieInfoViewController` (constraint list lines 287–357 of the
original), composed in the page's constructor:

```csharp
Content = new ScrollView
{
    IgnoresSafeArea = SafeAreaEdges.All,      // full-bleed backdrop
    Content = new VStack
    {
        Children =
        {
            new Overlay                       // backdrop + poster stack
            {
                Children =
                {
                    new Image
                    {
                        Source = Bind<string, ImageSource?>(vm => vm.BackdropUrl, u => ImageSource.Url(u)),
                        Stretch = Stretch.UniformToFill,
                    },
                    new VStack
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(24, 0),
                        Spacing = 2,
                        Children =
                        {
                            // clipping and casting a shadow are mutually exclusive on one layer:
                            // the shadow goes on the outer view, the rounding on the inner one
                            new Border
                            {
                                Shadow = new(opacity: 0.5, radius: 12, offsetY: 6),
                                Margin = new Thickness(0, 0, 0, 32),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Child = new Image
                                {
                                    Source = Bind<string, ImageSource?>(vm => vm.PosterUrl, u => ImageSource.Url(u)),
                                    Width = 140, Height = 210,
                                    CornerRadius = 8,
                                },
                            },
                            new Label
                            {
                                Text = Bind(vm => vm.Title),
                                FontSize = 24, FontWeight = FontWeight.Bold,
                                TextColor = Colors.White,
                                MaxLines = 1, TextAlignment = TextAlignment.Center,
                            },
                            new Label
                            {
                                Text = Bind(vm => vm.SubtitleLine),   // genres • year • duration (VM concern now)
                                FontSize = 14,
                                TextColor = Colors.White.WithAlpha(0.375),
                                MaxLines = 2, TextAlignment = TextAlignment.Center,
                            },
                            new Button
                            {
                                Text = "media_play".L10N(),
                                Icon = "play.fill",
                                Kind = ButtonStyle.FilledCapsule,
                                Command = Bind<ICommand?>(vm => vm.PlayCommand),
                                Margin = new Thickness(0, 32, 0, 0),
                                HorizontalAlignment = HorizontalAlignment.Center,
                            },
                        }
                    },
                }
            },
            // bottom detail section...
        }
    }
};
```

The scroll-linked parallax/blur/alpha effects stay as one custom control
(`BackdropEffectView : Control`) using the documented custom-control API — BareUI removes
the ~200 lines of constraint and lifecycle noise around it, not the creative part.

## 4. Two-way + converters + commands (bindings summary)

```csharp
// OneWay (default)
Text = Bind(vm => vm.Title);

// TwoWay — explicit setter keeps it AOT-safe and compile-checked
IsOn = Bind(vm => vm.Config.Appearance.AnimateTabBar,
            (vm, v) => vm.Config.Appearance.AnimateTabBar = v);

// Converter / formatter
Text = Bind<TimeSpan, string?>(vm => vm.Duration, d => d.L10N());

// Numeric text field (replaces UINumberField)
Text = Bind(vm => vm.Port, (vm, v) => vm.Port = v,
            format: p => p.ToString(), parse: int.Parse);

// Update trigger
Text = Bind(vm => vm.Query, (vm, v) => vm.Query = v).On(UpdateTrigger.FocusLost);

// Commands — Bindable like everything else; explicit type arg because
// [RelayCommand] generates IRelayCommand and Bindable<T> is not covariant
Command = Bind<ICommand?>(vm => vm.CloseCommand);
TapCommand = Bind<ICommand?>(vm => vm.OpenMovieCommand);   // any view is tappable

// Literals for interface-typed properties (no implicit conversion from interfaces)
SelectionCommand = Bindable.From<ICommand?>(someCommand);
```

## 5. Navigation from a ViewModel

```csharp
public partial class HomeViewModel(INavigator navigator) : ObservableObject
{
    [RelayCommand]
    async Task OpenMovie(Movie movie) =>
        await navigator.PushAsync(new MovieInfoViewModel(movie));

    [RelayCommand]
    async Task ShowFilters() =>
        await navigator.PresentAsync<FilterViewModel>(ModalStyle.Sheet(Detent.Medium));

    [RelayCommand]
    async Task Delete() =>
        await navigator.ConfirmAsync("delete_title".L10N(), "delete_msg".L10N());
}
```

ViewModels stay CommunityToolkit.Mvvm — BareUI requires only `INotifyPropertyChanged` /
`ICommand`, nothing library-specific.

## 6. Styling & theming

See ADR-008. Styles are typed actions; resources are plain statics.

```csharp
// Styles.cs — the app's "resource dictionary" is a static class
static class Palette
{
    public static readonly Color Accent = Colors.Indigo;
    public static readonly Color Card = Colors.SecondaryGroupedBackground;
}

static class Styles
{
    public static readonly Style<Label> Caption = new(l =>
    {
        l.TextStyle = TextStyle.Caption1;      // native Dynamic Type curve
        l.TextColor = Colors.SecondaryLabel;
    });

    public static readonly Style<Border> Card = new(b =>
    {
        b.Background = Palette.Card;
        b.CornerRadius = 12;
        b.Padding = new Thickness(16);
    });

    // BasedOn: Card applies first, then the overrides
    public static readonly Style<Border> ProminentCard = new(Card, b =>
        b.Shadow = new(opacity: 0.2, radius: 8, offsetY: 4));
}

// Explicit use — Style goes FIRST in the initializer; later lines override it
new Label { Style = Styles.Caption, Text = "Runtime" }
new Border { Style = Styles.ProminentCard, Child = content }

// Implicit use — app-wide defaults, applied to every instance of the type
BareApp.Create()
    .UseTheme(theme => theme
        .Style(new Style<Label>(l => l.TextColor = Colors.Label))
        .Style(new Style<Button>(b => b.Kind = ButtonStyle.Tinted)))
    ...
```

Precedence (each later source wins): control defaults → theme styles (base type first) →
explicit `Style` → local values after it.
