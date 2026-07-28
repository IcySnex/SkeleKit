# Application, shell, and services

Classification: **Interactive lab + non-gallery reference**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## Application lab matrix

| Lab | APIs | Action | Expected observable behavior |
| --- | --- | --- | --- |
| Modal presentations | `INavigator.Present*`, `ModalStyle`, `ModalPresentation`, `Detent`, `PopoverArrow` | Present automatic, full-screen, sheet, and anchored popover variants; drag sheet detents and attempt guarded dismissal. | Correct iPhone/iPad presentation, detents, arrow selection, anchor behavior, and `ConfirmLeave` veto. |
| Dialogs | `AlertAsync`, `ConfirmAsync`, `PromptAsync`, `SelectAsync` | Trigger success, cancel, destructive, empty-input, and long-option states. | Awaited result matches the chosen action; iPad presentation remains anchored and keyboard/focus behavior is stable. |
| Sharing and picking | `ISharer`, `ShareContent`, `ISystemPicker`, `PickedAsset` | Share text/URL/image and pick image/file; cancel each picker once. | Share sheet contains only supplied content; pickers return deterministic name/data or null on cancel. Permission denial is explained, never represented as an empty successful result. |
| Haptics | `Haptics`, `HapticEvent`, `HapticStyle`, `HapticsNotification` | Trigger impact strengths, selection, notification outcomes, and a short custom pattern on a physical device. | Each action is perceivable without changing layout; simulator limitations are called out. |
| Navigation | `Push*`, `Pop*`, `Present*`, `DismissAsync`, `SelectTabAsync`, `OpenUrlAsync` | Exercise ViewModel-first and view-first overload families, cancellation/guard paths, tab selection, and an invalid URL fallback. | Stack/modal state and awaited completion match the call; overloads remain code-reference entries rather than fake visual controls. |
| Shell/page chrome | builders plus `ContentView` chrome properties | Build single-page, stack, tabs, iPad sidebar, search, toolbar, badge, status-bar, and bottom-accessory states. | iPhone/iPad arrangements, large-title collapse, search callbacks, badges, and bar coloring match the page configuration. |

Networking, Photos/files permissions, share extensions, haptics, URL opening, lifecycle events, and iPad popovers must use a real device/service path where required. Provide a visible unavailable/denied result beside the trigger.
## GroupBuilder

Declares the tabs inside a group.

- Source: `SkeleKit.iOS/App/Builder/GroupBuilder.cs`
- Inheritance/shape: `class GroupBuilder`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.GroupBuilder.Tab``1(System.String,System.String,SkeleKit.TabPlacement)` | public | n/a | n/a | n/a | Adds a tab page to the group. |
| Method | `SkeleKit.GroupBuilder.Group(System.String,System.String,System.Action{SkeleKit.GroupBuilder})` | public | n/a | n/a | n/a | Adds a nested group. |
| Method | `SkeleKit.GroupBuilder.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PadTabsBuilder

Everything iPad: the sidebar, placements and iPad-only destinations.

- Source: `SkeleKit.iOS/App/Builder/PadTabsBuilder.cs`
- Inheritance/shape: `class PadTabsBuilder`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.PadTabsBuilder.Sidebar` | public | n/a | n/a | n/a | Shows the tabs as a sidebar. |
| Method | `SkeleKit.PadTabsBuilder.PlaceTab``1(SkeleKit.TabPlacement)` | public | n/a | n/a | n/a | Overrides how a declared tab takes part in user customization. |
| Method | `SkeleKit.PadTabsBuilder.Tab``1(System.String,System.String,SkeleKit.TabPlacement)` | public | n/a | n/a | n/a | Adds an iPad-only tab. It does not exist on iPhone; reach the page there by navigation. |
| Method | `SkeleKit.PadTabsBuilder.Group(System.String,System.String,System.Action{SkeleKit.GroupBuilder})` | public | n/a | n/a | n/a | Adds a sidebar section: a group of tabs, always sidebar-only. |
| Method | `SkeleKit.PadTabsBuilder.SidebarFooter``1` | public | n/a | n/a | n/a | Shows a view of the given type at the sidebar's bottom. iOS 26 and later. |
| Method | `SkeleKit.PadTabsBuilder.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PagesBuilder

Registers the pages available in the application.

- Source: `SkeleKit.iOS/App/Builder/PagesBuilder.cs`
- Inheritance/shape: `class PagesBuilder`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.PagesBuilder.AddTransient``1` | public | n/a | n/a | n/a | Registers a view-only page that is recreated for each presentation. |
| Method | `SkeleKit.PagesBuilder.AddTransient``1(System.Func{``0})` | public | n/a | n/a | n/a | Registers a view-only page that is recreated for each presentation. |
| Method | `SkeleKit.PagesBuilder.AddTransient``1(System.Func{System.IServiceProvider,``0})` | public | n/a | n/a | n/a | Registers a view-only page that is recreated for each presentation. |
| Method | `SkeleKit.PagesBuilder.AddTransient``2(System.Func{``0,``1})` | public | n/a | n/a | n/a | Registers a ViewModel-backed page that is recreated for each presentation. |
| Method | `SkeleKit.PagesBuilder.AddTransient``2(System.Func{System.IServiceProvider,``0,``1})` | public | n/a | n/a | n/a | Registers a ViewModel-backed page that is recreated for each presentation. |
| Method | `SkeleKit.PagesBuilder.AddSingleton``1` | public | n/a | n/a | n/a | Registers a view-only page built once and kept for the application's lifetime. |
| Method | `SkeleKit.PagesBuilder.AddSingleton``1(``0)` | public | n/a | n/a | n/a | Registers an existing view-only page for the application's lifetime. |
| Method | `SkeleKit.PagesBuilder.AddSingleton``1(System.Func{``0})` | public | n/a | n/a | n/a | Registers a view-only page built once and kept for the application's lifetime. |
| Method | `SkeleKit.PagesBuilder.AddSingleton``1(System.Func{System.IServiceProvider,``0})` | public | n/a | n/a | n/a | Registers a view-only page built once and kept for the application's lifetime. |
| Method | `SkeleKit.PagesBuilder.AddSingleton``2(System.Func{``0,``1})` | public | n/a | n/a | n/a | Registers a ViewModel-backed page built once and kept for the application's lifetime. |
| Method | `SkeleKit.PagesBuilder.AddSingleton``2(System.Func{System.IServiceProvider,``0,``1})` | public | n/a | n/a | n/a | Registers a ViewModel-backed page built once and kept for the application's lifetime. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SkeleApplicationBuilder

A builder used to configure and construct a `SkeleApplication`.

- Source: `SkeleKit.iOS/App/Builder/SkeleApplicationBuilder.cs`
- Inheritance/shape: `class SkeleApplicationBuilder`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.SkeleApplicationBuilder.UseServices(System.Action{Microsoft.Extensions.DependencyInjection.IServiceCollection})` | public | n/a | n/a | n/a | Registers core dependencies and application services into the container. |
| Method | `SkeleKit.SkeleApplicationBuilder.UseImageLoader(SkeleKit.IImageLoader)` | public | n/a | n/a | n/a | Sets how `Image` loads remote URLs. Plug in a caching loader here. |
| Method | `SkeleKit.SkeleApplicationBuilder.UseAccent(SkeleKit.Color)` | public | n/a | n/a | n/a | Sets the app-wide accent color every control tints with. |
| Method | `SkeleKit.SkeleApplicationBuilder.UseLifecycle(System.Action,System.Action)` | public | n/a | n/a | n/a | Registers app lifecycle hooks, invoked as the app leaves for and returns from the background. |
| Method | `SkeleKit.SkeleApplicationBuilder.UseTheme(System.Action{SkeleKit.Theme})` | public | n/a | n/a | n/a | Registers implicit styles applied to every view of a type as it is built. |
| Method | `SkeleKit.SkeleApplicationBuilder.UsePages(System.Action{SkeleKit.PagesBuilder},System.Boolean)` | public | n/a | n/a | n/a | Registers or overrides pages by hand. |
| Method | `SkeleKit.SkeleApplicationBuilder.SinglePage``1` | public | n/a | n/a | n/a | Configures the app to use as a single page without navigation chrome. |
| Method | `SkeleKit.SkeleApplicationBuilder.Stack``1(System.Boolean)` | public | n/a | n/a | n/a | Configures the app to use a stack-based navigation hierarchy. |
| Method | `SkeleKit.SkeleApplicationBuilder.Tabs(System.Action{SkeleKit.TabsBuilder})` | public | n/a | n/a | n/a | Configures the app to use bottom navigation tabs with each tab having its own navigation stack. |
| Method | `SkeleKit.SkeleApplicationBuilder.BuildCore` | public | n/a | n/a | n/a | Builds and returns the configured application instance. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## TabsBuilder

Declares the application's tabs.

- Source: `SkeleKit.iOS/App/Builder/TabsBuilder.cs`
- Inheritance/shape: `class TabsBuilder`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.TabsBuilder.LargeTitles` | public | n/a | n/a | n/a | Enables large, expanding navigation titles for the tab pages. |
| Method | `SkeleKit.TabsBuilder.Tab``1(System.String,System.String)` | public | n/a | n/a | n/a | Adds a tab page to the navigation structure. |
| Method | `SkeleKit.TabsBuilder.Search``1` | public | n/a | n/a | n/a | Adds the system search tab: the separated bubble that morphs the bar into the search field. |
| Method | `SkeleKit.TabsBuilder.Bubble``1(System.String,System.String)` | public | n/a | n/a | n/a | Puts a destination page in the separated bubble: selecting it shows the page with native selection. |
| Method | `SkeleKit.TabsBuilder.Bubble(System.String,System.String,System.Action)` | public | n/a | n/a | n/a | Puts an action button in the separated bubble instead of search. The bubble is single: Search and Bubble exclude each other. |
| Method | `SkeleKit.TabsBuilder.Bubble``1(System.String,System.String,System.Func{``0,System.Windows.Input.ICommand})` | public | n/a | n/a | n/a | Puts an action button in the separated bubble, firing a command from a ViewModel resolved from the services. |
| Method | `SkeleKit.TabsBuilder.Minimizes(SkeleKit.TabBarMinimize)` | public | n/a | n/a | n/a | Lets the tab bar minimize as the content scrolls. iOS 26 and later. |
| Method | `SkeleKit.TabsBuilder.Accessory``1` | public | n/a | n/a | n/a | Shows a view of the given type in the tab bar's accessory slot. The view's IsVisible controls the slot. iOS 26 and later. |
| Method | `SkeleKit.TabsBuilder.OnPad(System.Action{SkeleKit.PadTabsBuilder})` | public | n/a | n/a | n/a | Configures everything iPad: the sidebar, tab placements and iPad-only destinations. Ignored on iPhone. |
| Method | `SkeleKit.TabsBuilder.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## HapticEvent

A single moment in a custom haptic pattern played through `Haptics.Play`.

- Source: `SkeleKit.iOS/App/Haptics/HapticEvent.cs`
- Inheritance/shape: `struct HapticEvent`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Intensity and sharpness range from 0 to 1 and are clamped. Intensity controls how strong the sensation feels, sharpness how crisp against dull.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.HapticEvent.Tap(System.Double,System.Double,System.Double)` | public static | n/a | n/a | n/a | Creates a brief, punctuated tap at the given time in the pattern. |
| Method | `SkeleKit.HapticEvent.Continuous(System.Double,System.Double,System.Double,System.Double)` | public static | n/a | n/a | n/a | Creates a sustained vibration spanning the given duration. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Haptics

Provides access to native device haptic feedback.

- Source: `SkeleKit.iOS/App/Haptics/Haptics.cs`
- Inheritance/shape: `class Haptics`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Haptics.Impact(SkeleKit.HapticStyle)` | public static | n/a | n/a | n/a | Triggers impact feedback to simulate physical weight or collisions. |
| Method | `SkeleKit.Haptics.Selection` | public static | n/a | n/a | n/a | Triggers subtle feedback indicating a user selection change. |
| Method | `SkeleKit.Haptics.Notify(SkeleKit.HapticsNotification)` | public static | n/a | n/a | n/a | Triggers notification feedback for successes, warnings, or errors. |
| Method | `SkeleKit.Haptics.Play(System.ReadOnlySpan{SkeleKit.HapticEvent})` | public static | n/a | n/a | n/a | Plays a custom haptic pattern built from taps and sustained vibrations. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## HapticsNotification

The type of notification event for haptic feedback.

- Source: `SkeleKit.iOS/App/Haptics/HapticsNotification.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.HapticsNotification.Success` | public | n/a | n/a | n/a | Indicates a task completed successfully. |
| Field/value | `SkeleKit.HapticsNotification.Warning` | public | n/a | n/a | n/a | Indicates a condition that requires user attention. |
| Field/value | `SkeleKit.HapticsNotification.Error` | public | n/a | n/a | n/a | Indicates a failed operation or critical error. |
| Field/value | `SkeleKit.HapticsNotification.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## HapticStyle

The weight and sharpness of haptic feedback.

- Source: `SkeleKit.iOS/App/Haptics/HapticStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.HapticStyle.Light` | public | n/a | n/a | n/a | A light tap. |
| Field/value | `SkeleKit.HapticStyle.Medium` | public | n/a | n/a | n/a | A medium tap. |
| Field/value | `SkeleKit.HapticStyle.Heavy` | public | n/a | n/a | n/a | A heavy tap. |
| Field/value | `SkeleKit.HapticStyle.Soft` | public | n/a | n/a | n/a | A soft tap. |
| Field/value | `SkeleKit.HapticStyle.Rigid` | public | n/a | n/a | n/a | A rigid tap. |
| Field/value | `SkeleKit.HapticStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## IImageLoader

Loads remote images for `Image`.

- Source: `SkeleKit.iOS/App/Media/IImageLoader.cs`
- Inheritance/shape: `interface IImageLoader`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.IImageLoader.LoadAsync(System.String,System.Threading.CancellationToken)` | public interface member | n/a | n/a | n/a | Loads the image at `url`, returning null on failure. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Detent

How a modal sheet's height is restricted.

- Source: `SkeleKit.iOS/App/Navigation/Detent.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.Detent.Medium` | public | n/a | n/a | n/a | Half height. |
| Field/value | `SkeleKit.Detent.Large` | public | n/a | n/a | n/a | Full height. |
| Field/value | `SkeleKit.Detent.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## INavigator

Manages application navigation, modal presentations, and native dialogs from a view model.

- Source: `SkeleKit.iOS/App/Navigation/INavigator.cs`
- Inheritance/shape: `interface INavigator`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.INavigator.SelectTabAsync(System.String)` | public interface member | n/a | n/a | n/a | Selects the tab with the given title, as declared on `Tab(title, ...)`. |
| Method | `SkeleKit.INavigator.PushAsync``1` | public interface member | n/a | n/a | n/a | Pushes a new page onto the stack, resolving its view model from the service container. |
| Method | `SkeleKit.INavigator.PushAsync(System.Type)` | public interface member | n/a | n/a | n/a | Pushes a new page onto the stack, resolving its view model by type from the service container. |
| Method | `SkeleKit.INavigator.PushAsync(System.Object)` | public interface member | n/a | n/a | n/a | Pushes a new page onto the stack using an existing view model instance. |
| Method | `SkeleKit.INavigator.PushViewAsync``1` | public interface member | n/a | n/a | n/a | Pushes a registered view, resolving any associated ViewModel from the service container. |
| Method | `SkeleKit.INavigator.PushViewAsync(System.Type)` | public interface member | n/a | n/a | n/a | Pushes a registered view by type, resolving any associated ViewModel from the service container. |
| Method | `SkeleKit.INavigator.PushViewAsync(SkeleKit.ContentView)` | public interface member | n/a | n/a | n/a | Pushes an existing page instance directly. Create a new instance per navigation. |
| Method | `SkeleKit.INavigator.PopAsync` | public interface member | n/a | n/a | n/a | Pops the top page off the current navigation stack. |
| Method | `SkeleKit.INavigator.PopToRootAsync` | public interface member | n/a | n/a | n/a | Pops all pages off the stack except for the root page. |
| Method | `SkeleKit.INavigator.PresentAsync``1(SkeleKit.ModalStyle)` | public interface member | n/a | n/a | n/a | Presents a modal page, resolving its view model from the service container. |
| Method | `SkeleKit.INavigator.PresentAsync(System.Type,SkeleKit.ModalStyle)` | public interface member | n/a | n/a | n/a | Presents a modal page, resolving its view model by type from the service container. |
| Method | `SkeleKit.INavigator.PresentAsync(System.Object,SkeleKit.ModalStyle)` | public interface member | n/a | n/a | n/a | Presents a modal page using an existing view model instance. |
| Method | `SkeleKit.INavigator.PresentViewAsync``1(SkeleKit.ModalStyle)` | public interface member | n/a | n/a | n/a | Presents a registered view, resolving any associated ViewModel from the service container. |
| Method | `SkeleKit.INavigator.PresentViewAsync(System.Type,SkeleKit.ModalStyle)` | public interface member | n/a | n/a | n/a | Presents a registered view by type, resolving any associated ViewModel from the service container. |
| Method | `SkeleKit.INavigator.PresentViewAsync(SkeleKit.ContentView,SkeleKit.ModalStyle)` | public interface member | n/a | n/a | n/a | Presents an existing page instance directly. Create a new instance per navigation. |
| Method | `SkeleKit.INavigator.DismissAsync` | public interface member | n/a | n/a | n/a | Dismisses the top-most active modal presentation layer. |
| Method | `SkeleKit.INavigator.OpenUrlAsync(System.String)` | public interface member | n/a | n/a | n/a | Opens a web address in an in-app Safari browser, with the system reader, share and done chrome. |
| Method | `SkeleKit.INavigator.AlertAsync(System.String,System.String,System.String)` | public interface member | n/a | n/a | n/a | Displays an alert dialog with a single button to dismiss it. |
| Method | `SkeleKit.INavigator.ConfirmAsync(System.String,System.String,System.String,System.String,System.Boolean)` | public interface member | n/a | n/a | n/a | Displays a confirmation dialog with accept and cancel actions. |
| Method | `SkeleKit.INavigator.PromptAsync(System.String,System.String,System.String,System.String,System.String,System.String)` | public interface member | n/a | n/a | n/a | Displays an alert with a single text field, for a name or another short answer. |
| Method | `SkeleKit.INavigator.SelectAsync(System.String,System.String,System.String[])` | public interface member | n/a | n/a | n/a | Displays an action sheet layout with multiple choices. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ModalPresentation

How a modal page is structured and presented on screen.

- Source: `SkeleKit.iOS/App/Navigation/ModalPresentation.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ModalPresentation.Automatic` | public | n/a | n/a | n/a | Let the system choose the best presentation style dynamically. |
| Field/value | `SkeleKit.ModalPresentation.PageSheet` | public | n/a | n/a | n/a | Fills the screen height but restricts width on large screens. |
| Field/value | `SkeleKit.ModalPresentation.FullScreen` | public | n/a | n/a | n/a | Covers the entire screen and unloads background views. |
| Field/value | `SkeleKit.ModalPresentation.OverFullScreen` | public | n/a | n/a | n/a | Covers the whole screen but keeps the background loaded for transparency. |
| Field/value | `SkeleKit.ModalPresentation.CurrentContext` | public | n/a | n/a | n/a | Presents inside the current view controller bounds instead of the full screen. |
| Field/value | `SkeleKit.ModalPresentation.OverCurrentContext` | public | n/a | n/a | n/a | Presents inside the parent view controller context while keeping its background visible. |
| Field/value | `SkeleKit.ModalPresentation.Popover` | public | n/a | n/a | n/a | A contextual floating bubble modal on large displays. |
| Field/value | `SkeleKit.ModalPresentation.FormSheet` | public | n/a | n/a | n/a | A centered card layout on iPad/desktop, and a full sheet on iPhone. |
| Field/value | `SkeleKit.ModalPresentation.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ModalStyle

The presentation style of a modal page.

- Source: `SkeleKit.iOS/App/Navigation/ModalStyle.cs`
- Inheritance/shape: `struct ModalStyle`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ModalStyle.Automatic` | public static get | C# default | No | No automatic invalidation | Let the system choose the best presentation style dynamically. |
| Property | `SkeleKit.ModalStyle.FullScreen` | public static get | C# default | No | No automatic invalidation | Covers the entire screen and unloads the background. |
| Property | `SkeleKit.ModalStyle.FormSheet` | public static get | C# default | No | No automatic invalidation | A centered card layout on iPad/desktop, and a full sheet on iPhone. |
| Property | `SkeleKit.ModalStyle.CurrentContext` | public static get | C# default | No | No automatic invalidation | Presents inside the parent bounds instead of the full screen. |
| Property | `SkeleKit.ModalStyle.OverFullScreen` | public static get | C# default | No | No automatic invalidation | Covers the whole screen but keeps the background loaded. |
| Property | `SkeleKit.ModalStyle.OverCurrentContext` | public static get | C# default | No | No automatic invalidation | Presents inside the parent bounds while keeping the background loaded. |
| Method | `SkeleKit.ModalStyle.Popover(SkeleKit.View,SkeleKit.PopoverArrow)` | public static | n/a | n/a | n/a | A contextual floating bubble anchored to a view on large displays. |
| Method | `SkeleKit.ModalStyle.Sheet(SkeleKit.Detent[])` | public static | n/a | n/a | n/a | An interactive, swipe-to-dismiss sheet. Pass more than one height to let the user drag between them, opening at the first. |
| Property | `SkeleKit.ModalStyle.Presentation` | public get | C# default | No | No automatic invalidation | How the modal is presented. |
| Property | `SkeleKit.ModalStyle.Detents` | public get | C# default | No | No automatic invalidation | The heights a sheet may rest at. It opens at the first and can be dragged between them; ignored for other presentations. |
| Property | `SkeleKit.ModalStyle.Anchor` | public get | null | No | No automatic invalidation | The view a popover points at, or null. Ignored for other presentations. |
| Property | `SkeleKit.ModalStyle.Arrows` | public get | C# default | No | No automatic invalidation | The directions a popover's arrow may point. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PageAttribute

Marks a view for automatic page registration.

- Source: `SkeleKit.iOS/App/Navigation/PageAttribute.cs`
- Inheritance/shape: `class PageAttribute : Attribute`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.PageAttribute.Singleton` | public get/set | false | No | No automatic invalidation | Whether one instance is kept for the app's lifetime. |
| Method | `SkeleKit.PageAttribute.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ISystemPicker

Presents the system photo and document pickers.

- Source: `SkeleKit.iOS/App/Picking/ISystemPicker.cs`
- Inheritance/shape: `interface ISystemPicker`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.ISystemPicker.PickImagesAsync(System.Int32)` | public interface member | n/a | n/a | n/a | Presents the photo library and returns the chosen image, or null if canceled. |
| Method | `SkeleKit.ISystemPicker.PickFileAsync(System.String[])` | public interface member | n/a | n/a | n/a | Presents the document browser and returns the chosen file, or null if canceled. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PickedAsset

A file or an image chosen from the device.

- Source: `SkeleKit.iOS/App/Picking/PickedAsset.cs`
- Inheritance/shape: `class PickedAsset`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.PickedAsset.Data` | public get | C# default | No | No automatic invalidation | The data represented as bytes. |
| Property | `SkeleKit.PickedAsset.Name` | public get | C# default | No | No automatic invalidation | The file name. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ISharer

Presents the system share sheet.

- Source: `SkeleKit.iOS/App/Sharing/ISharer.cs`
- Inheritance/shape: `interface ISharer`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.ISharer.ShareAsync(SkeleKit.ShareContent)` | public interface member | n/a | n/a | n/a | Presents the share sheet for a piece of content. A `string`, `Uri` or `ImageSource` converts to `ShareContent` implicitly for the common case. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ShareContent

What to hand the share sheet: an optional text, link and image.

- Source: `SkeleKit.iOS/App/Sharing/ShareContent.cs`
- Inheritance/shape: `class ShareContent`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.ShareContent.op_Implicit(System.String)~SkeleKit.ShareContent` | public static | n/a | n/a | n/a | Shares a string as plain text. |
| Method | `SkeleKit.ShareContent.op_Implicit(System.Uri)~SkeleKit.ShareContent` | public static | n/a | n/a | n/a | Shares a web address as a link. |
| Method | `SkeleKit.ShareContent.op_Implicit(SkeleKit.ImageSource)~SkeleKit.ShareContent` | public static | n/a | n/a | n/a | Shares an image. |
| Property | `SkeleKit.ShareContent.Text` | public get/set | null | No | No automatic invalidation | The text to share, which also titles the share sheet. |
| Property | `SkeleKit.ShareContent.Url` | public get/set | null | No | No automatic invalidation | The link to share, so the sheet offers its link-specific actions. |
| Property | `SkeleKit.ShareContent.Image` | public get/set | null | No | No automatic invalidation | The image to share, shown as the sheet's preview thumbnail. |
| Method | `SkeleKit.ShareContent.op_Implicit(System.String)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ShareContent.op_Implicit(System.Uri)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ShareContent.op_Implicit(SkeleKit.ImageSource)` | public static | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.ShareContent.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SkeleApplication

The core application instance that handles DI, navigation setup, and the app lifecycle.

- Source: `SkeleKit.iOS/App/SkeleApplication.cs`
- Inheritance/shape: `class SkeleApplication`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.SkeleApplication.Current` | public static get/private set | null | No | No automatic invalidation | The currently running application instance. |
| Method | `SkeleKit.SkeleApplication.CreateBuilder` | public static | n/a | n/a | n/a | Creates a new builder to configure services and the layout shell. |
| Property | `SkeleKit.SkeleApplication.Services` | public get | C# default | No | No automatic invalidation | The built-in service provider for resolving dependencies. |
| Method | `SkeleKit.SkeleApplication.Run(System.String[])` | public | n/a | n/a | n/a | Starts the native iOS main loop. |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

