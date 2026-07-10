# BareUI.iOS — API Sketch (Velura before/after)

Illustrative only — final signatures may drift during implementation. All snippets are
what *app* code looks like; no UIKit type appears anywhere.

## 1. App bootstrap

Replaces `Main.cs`, `AppDelegate.cs`, and the window/tab wiring in
`MainViewController.cs`:

```csharp
// Program.cs — the entire iOS head entry point
BareApp.Create()
    .UseServices(services =>
    {
        services.AddSingleton<Config>();
        services.AddSingleton<IThemeManager, ThemeManager>();
        // ... existing Velura service registrations unchanged
    })
    .Map<HomeViewModel, HomeView>()
    .Map<SearchViewModel, SearchView>()
    .Map<SettingsViewModel, SettingsView>()
    .Map<MovieInfoViewModel, MovieInfoView>()
    .Tabs(tabs => tabs
        .LargeTitles()
        .SidebarOnIPad()
        .Tab<HomeViewModel>("home_title", icon: "house")
        .Tab<SearchViewModel>("search_title", icon: "magnifyingglass")
        .Tab<SettingsViewModel>("settings_title", icon: "gear"))
    .Run();
```

## 2. A settings-style page

What `SettingsGroupViewController` + `SettingsGroupItemViewCell` +
`SettingsGroupHeaderView` (~460 lines of UIKit) collapse into:

```csharp
public class SettingsGroupView : ContentView<SettingsGroupViewModel>
{
    protected override View Build() => new CollectionView<SettingsEntry>
    {
        Layout = CollectionLayout.List(grouped: true),
        Header = new SettingsHeader(),                       // reusable ContentView fragment
        GroupedItemsSource = Bind(vm => vm.Sections),
        SelectionCommand = ViewModel.OpenGroupCommand,
        ItemTemplate = () => new SettingsRow(),
    };

    public SettingsGroupView()
    {
        Title = Bind(vm => vm.GroupName);
        TitleRevealOnScroll = true;                          // built-in ConcealingTitleView behavior
    }
}

class SettingsRow : ItemView<SettingsEntry>
{
    protected override View Build() => new Grid
    {
        Columns = [Auto, Star, Auto],
        ColumnSpacing = 12,
        Padding = new(16, 8),
        Children =
        {
            new Image
            {
                Source     = Bind(i => i.IconName),
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
original) becomes:

```csharp
protected override View Build() => new ScrollView
{
    SafeAreaEdges = SafeArea.None,               // full-bleed backdrop
    Content = new StackPanel
    {
        Children =
        {
            new Overlay                           // backdrop + gradient + poster stack
            {
                Children =
                {
                    new Image
                    {
                        Source = Bind(vm => vm.BackdropUrl),
                        Stretch = Stretch.UniformToFill,
                    },
                    new VStack
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new(24, 0),
                        Spacing = 2,
                        Children =
                        {
                            new Image
                            {
                                Source = Bind(vm => vm.PosterUrl),
                                Width = 140, Height = 210,
                                CornerRadius = 8,
                                Shadow = new(opacity: 0.5f, radius: 12, offsetY: 6),
                                Margin = new(bottom: 32),
                                HorizontalAlignment = HorizontalAlignment.Center,
                            },
                            new Label
                            {
                                Text = Bind(vm => vm.Movie.Title),
                                FontSize = 24, FontWeight = FontWeight.Bold,
                                TextColor = Colors.White,
                                MaxLines = 1, TextAlignment = TextAlignment.Center,
                            },
                            new Label
                            {
                                Text = Bind(vm => vm.SubtitleLine),   // genres • year • duration (VM concern now)
                                FontSize = 14,
                                TextColor = Colors.White.WithAlpha(0.375f),
                                MaxLines = 2, TextAlignment = TextAlignment.Center,
                            },
                            new Button
                            {
                                Text = L10n.Get("media_play"),
                                Icon = "play.fill",
                                Style = ButtonStyle.FilledCapsule,
                                Command = ViewModel.PlayCommand,
                                Margin = new(top: 32),
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
Text = Bind(vm => vm.Movie.Duration, format: d => d.L10N());

// Numeric text field (replaces UINumberField)
Text = Bind(vm => vm.Port, (vm, v) => vm.Port = v,
            format: p => p.ToString(), parse: int.Parse);

// Update trigger
Text = Bind(vm => vm.Query, (vm, v) => vm.Query = v).On(UpdateTrigger.FocusLost);

// Commands
Command = ViewModel.CloseCommand;                    // direct — no binding needed
TapCommand = ViewModel.OpenMovieCommand;             // any view is tappable
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
