# BareUI.iOS — API Sketch

What app code looks like; no UIKit type appears anywhere. Every section is the shipped API.

## 1. App bootstrap

Replaces `Main`, `AppDelegate`, and the window/tab wiring:

```csharp
BareApplication.CreateBuilder()
    .UseServices(services =>
    {
        services.AddSingleton<Config>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddTransient<HomeViewModel>();
    })
    .UsePages(pages => pages                                       // a factory lambda per page: reflection-free
        .AddSingleton((HomeViewModel vm) => new HomeView(vm))      // keeps state across navigations
        .AddSingleton((SearchViewModel vm) => new SearchView(vm))
        .AddTransient((MovieInfoViewModel vm) => new MovieInfoView(vm)))  // fresh instance per push
    .Tabs(tabs => tabs
        .LargeTitles()
        .Tab<HomeView>("home_title", icon: "house")
        .Tab<SearchView>("search_title", icon: "magnifyingglass")
        .SidebarOnIPad())
    .Build()
    .Run(args);
```

## 2. A settings-style page

```csharp
public class SettingsGroupView : ContentView<SettingsGroupViewModel>
{
    public SettingsGroupView(
        SettingsGroupViewModel viewModel) : base(viewModel)
    {
        Title = Bind(vm => vm.GroupName);

        Content = new CollectionView<SettingsEntry, SettingsSection>
        {
            Layout = CollectionLayout.List(grouped: true),
            GroupedItemsSource = Bind<IReadOnlyList<SettingsSection>?>(vm => vm.Sections),
            SelectionCommand = ViewModel.OpenGroupCommand,        // command off the injected ViewModel
            ItemTemplate = () => new SettingsRow(),
            HeaderTemplate = () => new SectionHeader()
        };
    }
}

// the section model is the app's own; the library only asks for Items
record SettingsSection(
    string Title,
    IReadOnlyList<SettingsEntry> Items) : ISection<SettingsEntry>;

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
                Width = 29, Height = 29, CornerRadius = 6,
            },
            new Label { Text = Bind(i => i.Name) }.Column(1),
            new Switch { IsOn = Bind(i => i.IsEnabled, (i, v) => i.IsEnabled = v) }.Column(2),
        }
    };
}
```

## 3. Layout-heavy content

Composed in the page's constructor — `ScrollView` → `Overlay` (backdrop + poster) → `Border`
(shadow) → `Image`/`Label`/`Button`:

```csharp
Content = new ScrollView
{
    IgnoresSafeArea = SafeAreaEdges.All,      // full-bleed backdrop
    Content = new Overlay
    {
        Children =
        {
            new Image
            {
                Source = Bind<string, ImageSource?>(vm => vm.BackdropUrl, u => ImageSource.Url(u)),
                Stretch = Stretch.UniformToFill,
            },
            new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(24, 0),
                Children =
                {
                    // round and cast a shadow: shadow on the outer view, radius on the inner one
                    new Border
                    {
                        Shadow = new(opacity: 0.5, radius: 12, offsetY: 6),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Child = new Image
                        {
                            Source = Bind<string, ImageSource?>(vm => vm.PosterUrl, u => ImageSource.Url(u)),
                            Width = 140, Height = 210, CornerRadius = 8,
                        },
                    },
                    new Label
                    {
                        Text = Bind(vm => vm.Title),
                        FontSize = 24, FontWeight = FontWeight.Bold, TextColor = Colors.White,
                        MaxLines = 1, TextAlignment = TextAlignment.Center,
                    },
                    new Button
                    {
                        Text = "media_play".L10N(), Icon = "play.fill",
                        Kind = ButtonStyle.FilledCapsule,
                        Command = ViewModel.PlayCommand,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    },
                }
            },
        }
    }
};
```

Scroll-linked parallax/blur effects stay as one custom control (`: Control`) using the
custom-control API.

## 4. Bindings summary

```csharp
Text = Bind(vm => vm.Title);                                     // OneWay (default)
IsOn = Bind(vm => vm.Config.Appearance.AnimateTabBar,            // TwoWay (explicit setter)
            (vm, v) => vm.Config.Appearance.AnimateTabBar = v);
Text = Bind<TimeSpan, string?>(vm => vm.Duration, d => d.L10N()); // converter
Text = Bind(vm => vm.Port, (vm, v) => vm.Port = v,               // numeric field (format/parse)
            format: p => p.ToString(), parse: int.Parse);
Text = Bind(vm => vm.Query, (vm, v) => vm.Query = v).On(UpdateTrigger.FocusLost);

Command     = ViewModel.CloseCommand;                            // commands never bindable (ADR-012)
TapCommand  = ViewModel.OpenMovieCommand;                        // any view is tappable
Command     = Command.From(Close);                              // ... or a view-local handler

SelectionCommand = Bindable.From<ICommand?>(someCommand);        // literal for an interface-typed prop
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

ViewModels stay CommunityToolkit.Mvvm — BareUI requires only `INotifyPropertyChanged` / `ICommand`.

## 6. Styling & theming

Styles are typed actions; resources are plain statics (ADR-008).

```csharp
static class Styles
{
    public static readonly Style<Label> Caption = new(l =>
    {
        l.TextStyle = TextStyle.Caption1;
        l.TextColor = Colors.SecondaryLabel;
    });

    public static readonly Style<Border> Card = new(b =>
    {
        b.Background = Colors.SecondaryGroupedBackground;
        b.CornerRadius = 12;
        b.Padding = new Thickness(16);
    });

    // BasedOn: Card applies first, then the overrides
    public static readonly Style<Border> ProminentCard = new(Card, b =>
        b.Shadow = new(opacity: 0.2, radius: 8, offsetY: 4));
}

// explicit — Style goes FIRST in the initializer; later lines override it
new Label { Style = Styles.Caption, Text = "Runtime" };

// implicit — app-wide defaults for every instance of the type
BareApplication.CreateBuilder()
    .UseTheme(theme => theme
        .Style(new Style<Label>(l => l.TextColor = Colors.Label))
        .Style(new Style<Button>(b => b.Kind = ButtonStyle.Tinted)));
```

Precedence (each later source wins): control defaults → theme (base type first) → explicit `Style`
→ local values.
