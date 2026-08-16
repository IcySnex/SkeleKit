# Page, toolbar, and tab primitives

Classification: **Interactive lab + non-gallery reference**. Platform: **iOS 18.0+**; APIs explicitly described as iPad- or iOS-version-specific retain that narrower requirement.

This is the canonical declaration inventory for the types below. Inherited `View` behavior is linked instead of repeated. `Bindable<T>` properties accept either a literal or a binding expression. Defaults are implementation defaults: an explicit initializer/backing-field initializer is shown when present; otherwise the C# zero/null/default value applies.

## AccessibilityTraits

How VoiceOver describes and treats a view.

- Source: `SkeleKit.iOS/Primitives/AccessibilityTraits.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference
- Behavior note: Combines with the control's own traits.

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.AccessibilityTraits.None` | public | n/a | n/a | n/a | No extra traits. |
| Field/value | `SkeleKit.AccessibilityTraits.Button` | public | n/a | n/a | n/a | Acts like a button. |
| Field/value | `SkeleKit.AccessibilityTraits.Link` | public | n/a | n/a | n/a | Opens a link. |
| Field/value | `SkeleKit.AccessibilityTraits.Header` | public | n/a | n/a | n/a | A heading that divides content. |
| Field/value | `SkeleKit.AccessibilityTraits.Image` | public | n/a | n/a | n/a | An image with no text. |
| Field/value | `SkeleKit.AccessibilityTraits.Selected` | public | n/a | n/a | n/a | Currently selected. |
| Field/value | `SkeleKit.AccessibilityTraits.StaticText` | public | n/a | n/a | n/a | Static text that never changes. |
| Field/value | `SkeleKit.AccessibilityTraits.Adjustable` | public | n/a | n/a | n/a | Adjustable with swipe up/down (a slider). |
| Field/value | `SkeleKit.AccessibilityTraits.UpdatesFrequently` | public | n/a | n/a | n/a | Updates its value on its own (a progress bar). |
| Field/value | `SkeleKit.AccessibilityTraits.NotEnabled` | public | n/a | n/a | n/a | Present but not interactable. |
| Field/value | `SkeleKit.AccessibilityTraits.PlaysSound` | public | n/a | n/a | n/a | Plays a sound on activation. |
| Field/value | `SkeleKit.AccessibilityTraits.StartsMediaSession` | public | n/a | n/a | n/a | Starts a media session on activation. |
| Field/value | `SkeleKit.AccessibilityTraits.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Use the accessibility lab. Verify header, image, and selected traits with VoiceOver or Accessibility Inspector, including traits added to controls with their own native semantics.

## ButtonStyle

The visual treatment of a `Button`.

- Source: `SkeleKit.iOS/Primitives/ButtonStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ButtonStyle.Plain` | public | n/a | n/a | n/a | Borderless button with tinted text and no background. |
| Field/value | `SkeleKit.ButtonStyle.Gray` | public | n/a | n/a | n/a | Gray translucent background. |
| Field/value | `SkeleKit.ButtonStyle.Tinted` | public | n/a | n/a | n/a | Tinted translucent background. |
| Field/value | `SkeleKit.ButtonStyle.Filled` | public | n/a | n/a | n/a | Solid filled background. |
| Field/value | `SkeleKit.ButtonStyle.FilledCapsule` | public | n/a | n/a | n/a | Solid filled background with fully rounded (capsule) corners. |
| Field/value | `SkeleKit.ButtonStyle.Glass` | public | n/a | n/a | n/a | A Liquid Glass capsule. Plain on earlier systems. |
| Field/value | `SkeleKit.ButtonStyle.ProminentGlass` | public | n/a | n/a | n/a | A prominent, tinted Liquid Glass capsule. Filled on earlier systems. |
| Field/value | `SkeleKit.ButtonStyle.ClearGlass` | public | n/a | n/a | n/a | Invisible Liquid Glass: flat at rest, lights up and swells under the finger. For buttons on a glass bar. Plain on earlier systems. |
| Field/value | `SkeleKit.ButtonStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ButtonSize

The built-in size classes of a `Button`.

- Source: `SkeleKit.iOS/Primitives/ButtonStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ButtonSize.Medium` | public | n/a | n/a | n/a | The standard size. |
| Field/value | `SkeleKit.ButtonSize.Mini` | public | n/a | n/a | n/a | The smallest size. |
| Field/value | `SkeleKit.ButtonSize.Small` | public | n/a | n/a | n/a | Slightly smaller than standard. |
| Field/value | `SkeleKit.ButtonSize.Large` | public | n/a | n/a | n/a | A prominent, call-to-action size. |
| Field/value | `SkeleKit.ButtonSize.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## IconPlacement

Where a `Button`'s icon sits relative to its text.

- Source: `SkeleKit.iOS/Primitives/ButtonStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.IconPlacement.Leading` | public | n/a | n/a | n/a | Before the text. |
| Field/value | `SkeleKit.IconPlacement.Trailing` | public | n/a | n/a | n/a | After the text. |
| Field/value | `SkeleKit.IconPlacement.Top` | public | n/a | n/a | n/a | Above the text. |
| Field/value | `SkeleKit.IconPlacement.Bottom` | public | n/a | n/a | n/a | Below the text. |
| Field/value | `SkeleKit.IconPlacement.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## Coordinate

A geographic location in degrees.

- Source: `SkeleKit.iOS/Primitives/Coordinate.cs`
- Inheritance/shape: `record Coordinate`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Method | `SkeleKit.Coordinate.#ctor(System.Double,System.Double)` | public (compiled) | n/a | n/a | n/a | A geographic location in degrees. |
| Property | `SkeleKit.Coordinate.Latitude` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Degrees north of the equator, negative for south. |
| Property | `SkeleKit.Coordinate.Longitude` | public/protected as emitted | implementation-defined; inspect source | No | n/a | Degrees east of the prime meridian, negative for west. |
| Method | `SkeleKit.Coordinate.ToString` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Coordinate.op_Inequality(SkeleKit.Coordinate,SkeleKit.Coordinate)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Coordinate.op_Equality(SkeleKit.Coordinate,SkeleKit.Coordinate)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Coordinate.GetHashCode` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Coordinate.Equals(System.Object)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Coordinate.Equals(SkeleKit.Coordinate)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |
| Method | `SkeleKit.Coordinate.Deconstruct(System.Double@,System.Double@)` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## DatePickerMode

What a `DatePicker` lets the user pick.

- Source: `SkeleKit.iOS/Primitives/DatePickerStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.DatePickerMode.Date` | public | n/a | n/a | n/a | A calendar date. |
| Field/value | `SkeleKit.DatePickerMode.Time` | public | n/a | n/a | n/a | A time of day. |
| Field/value | `SkeleKit.DatePickerMode.DateAndTime` | public | n/a | n/a | n/a | A date and a time together. |
| Field/value | `SkeleKit.DatePickerMode.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## DatePickerStyle

How a `DatePicker` presents itself.

- Source: `SkeleKit.iOS/Primitives/DatePickerStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.DatePickerStyle.Compact` | public | n/a | n/a | n/a | A compact pill that expands into a popover. The default. |
| Field/value | `SkeleKit.DatePickerStyle.Inline` | public | n/a | n/a | n/a | The full calendar or clock, laid out inline. |
| Field/value | `SkeleKit.DatePickerStyle.Wheels` | public | n/a | n/a | n/a | The classic spinning wheels. |
| Field/value | `SkeleKit.DatePickerStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## TitleStyle

How the navigation bar shows the page's title.

- Source: `SkeleKit.iOS/Primitives/PageStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.TitleStyle.Inline` | public | n/a | n/a | n/a | The standard inline title. |
| Field/value | `SkeleKit.TitleStyle.Large` | public | n/a | n/a | n/a | A large title that collapses to inline as the content scrolls. |
| Field/value | `SkeleKit.TitleStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PageBackground

The page's background.

- Source: `SkeleKit.iOS/Primitives/PageStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.PageBackground.Default` | public | n/a | n/a | n/a | The system background. |
| Field/value | `SkeleKit.PageBackground.Grouped` | public | n/a | n/a | n/a | The grouped background, for settings-style pages. |
| Field/value | `SkeleKit.PageBackground.None` | public | n/a | n/a | n/a | No background at all. |
| Field/value | `SkeleKit.PageBackground.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## BackButtonStyle

How the next pushed page's back button represents this page.

- Source: `SkeleKit.iOS/Primitives/PageStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.BackButtonStyle.Default` | public | n/a | n/a | n/a | The page title, shortened to "Back" when space runs out. |
| Field/value | `SkeleKit.BackButtonStyle.Generic` | public | n/a | n/a | n/a | Always the generic "Back", never the title. |
| Field/value | `SkeleKit.BackButtonStyle.Minimal` | public | n/a | n/a | n/a | The chevron alone. |
| Field/value | `SkeleKit.BackButtonStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## StatusBarStyle

The status bar look a page asks for.

- Source: `SkeleKit.iOS/Primitives/PageStyle.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.StatusBarStyle.Default` | public | n/a | n/a | n/a | Follows the system appearance. |
| Field/value | `SkeleKit.StatusBarStyle.Light` | public | n/a | n/a | n/a | White content, for dark page backgrounds. |
| Field/value | `SkeleKit.StatusBarStyle.Dark` | public | n/a | n/a | n/a | Black content, for light page backgrounds. |
| Field/value | `SkeleKit.StatusBarStyle.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## PopoverArrow

The directions a popover's arrow may point.

- Source: `SkeleKit.iOS/Primitives/PopoverArrow.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.PopoverArrow.Up` | public | n/a | n/a | n/a | Points up, from a popover below its anchor. |
| Field/value | `SkeleKit.PopoverArrow.Down` | public | n/a | n/a | n/a | Points down, from a popover above its anchor. |
| Field/value | `SkeleKit.PopoverArrow.Left` | public | n/a | n/a | n/a | Points left. |
| Field/value | `SkeleKit.PopoverArrow.Right` | public | n/a | n/a | n/a | Points right. |
| Field/value | `SkeleKit.PopoverArrow.Any` | public | n/a | n/a | n/a | Any direction the system prefers. |
| Field/value | `SkeleKit.PopoverArrow.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SymbolEffect

A built-in animation an SF Symbol can perform.

- Source: `SkeleKit.iOS/Primitives/Symbols.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.SymbolEffect.None` | public | n/a | n/a | n/a | No effect. |
| Field/value | `SkeleKit.SymbolEffect.Bounce` | public | n/a | n/a | n/a | Scales the symbol up and back, like a tap acknowledgement. |
| Field/value | `SkeleKit.SymbolEffect.Pulse` | public | n/a | n/a | n/a | Fades the symbol's opacity in and out. |
| Field/value | `SkeleKit.SymbolEffect.VariableColor` | public | n/a | n/a | n/a | Steps through the symbol's variable layers, like an ongoing transfer. |
| Field/value | `SkeleKit.SymbolEffect.Breathe` | public | n/a | n/a | n/a | Smoothly scales the symbol up and down, like a calm breath. |
| Field/value | `SkeleKit.SymbolEffect.Wiggle` | public | n/a | n/a | n/a | Rocks the symbol side to side, drawing attention. |
| Field/value | `SkeleKit.SymbolEffect.Rotate` | public | n/a | n/a | n/a | Spins the symbol's rotatable parts. |
| Field/value | `SkeleKit.SymbolEffect.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## SymbolScale

The relative size an SF Symbol is drawn at within its font metrics.

- Source: `SkeleKit.iOS/Primitives/Symbols.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.SymbolScale.Default` | public | n/a | n/a | n/a | The symbol's own default. |
| Field/value | `SkeleKit.SymbolScale.Small` | public | n/a | n/a | n/a | Small. |
| Field/value | `SkeleKit.SymbolScale.Medium` | public | n/a | n/a | n/a | Medium. |
| Field/value | `SkeleKit.SymbolScale.Large` | public | n/a | n/a | n/a | Large. |
| Field/value | `SkeleKit.SymbolScale.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## TabBarMinimize

When the tab bar minimizes as content scrolls.

- Source: `SkeleKit.iOS/Primitives/TabBarMinimize.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.TabBarMinimize.Never` | public | n/a | n/a | n/a | The bar always stays at full size. |
| Field/value | `SkeleKit.TabBarMinimize.OnScrollDown` | public | n/a | n/a | n/a | Minimizes when scrolling down. |
| Field/value | `SkeleKit.TabBarMinimize.OnScrollUp` | public | n/a | n/a | n/a | Minimizes when scrolling up. |
| Field/value | `SkeleKit.TabBarMinimize.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## TabPlacement

How a tab takes part in iPad user customization.

- Source: `SkeleKit.iOS/Primitives/TabPlacement.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.TabPlacement.Automatic` | public | n/a | n/a | n/a | The system default: fully customizable. |
| Field/value | `SkeleKit.TabPlacement.Locked` | public | n/a | n/a | n/a | Exempt from customization: cannot be hidden or moved. |
| Field/value | `SkeleKit.TabPlacement.Pinned` | public | n/a | n/a | n/a | Anchored at the trailing end of the bar. |
| Field/value | `SkeleKit.TabPlacement.SidebarOnly` | public | n/a | n/a | n/a | Shown only in the sidebar, never in the tab bar. |
| Field/value | `SkeleKit.TabPlacement.Optional` | public | n/a | n/a | n/a | Hidden until the user adds it through Edit. |
| Field/value | `SkeleKit.TabPlacement.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ToolbarSide

Which side of the navigation bar a toolbar item sits on.

- Source: `SkeleKit.iOS/Primitives/ToolbarItem.cs`
- Inheritance/shape: `enum, struct, delegate, or interface; see declaration`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Field/value | `SkeleKit.ToolbarSide.Trailing` | public | n/a | n/a | n/a | Trailing edge (the right, in a left-to-right layout). |
| Field/value | `SkeleKit.ToolbarSide.Leading` | public | n/a | n/a | n/a | Leading edge. |
| Field/value | `SkeleKit.ToolbarSide.value__` | public | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.

## ToolbarItem

A button in the page's navigation bar.

- Source: `SkeleKit.iOS/Primitives/ToolbarItem.cs`
- Inheritance/shape: `class ToolbarItem`
- Native counterpart: value/configuration type or implementation-selected UIKit peer
- Gallery role: Code-only/non-gallery reference

| Kind | API / exact documentation ID | Access | Default / semantics | Bindable | Layout | Behavior |
| --- | --- | --- | --- | --- | --- | --- |
| Property | `SkeleKit.ToolbarItem.Text` | public get/set | null | No | No automatic invalidation | The item's text, or null when it shows only an icon. Setting it updates the bar live. |
| Property | `SkeleKit.ToolbarItem.Icon` | public get/set | null | No | No automatic invalidation | An SF Symbol name, or null for a text-only item. Setting it updates the bar live. |
| Property | `SkeleKit.ToolbarItem.IsVisible` | public get/set | true | No | No automatic invalidation | Whether the item is in the bar at all. Contextual actions toggle it live, like a Delete that only exists in edit mode. |
| Property | `SkeleKit.ToolbarItem.Side` | public get/set | ToolbarSide.Trailing | No | No automatic invalidation | Which side of the bar the item sits on. |
| Property | `SkeleKit.ToolbarItem.IsPrimary` | public get/set | false | No | No automatic invalidation | Whether the item is rendered as the prominent action. |
| Property | `SkeleKit.ToolbarItem.Tint` | public get/set | null; inherits the page or app tint | No | Visual/interaction only | The item's tint. Setting it updates the bar live. |
| Property | `SkeleKit.ToolbarItem.Menu` | public get | [] | No | No automatic invalidation | Menu entries shown on tap instead of invoking `ToolbarItem.Command`. Empty for a plain item. |
| Property | `SkeleKit.ToolbarItem.Command` | public get/set | null | No | No automatic invalidation | Invoked when the item is tapped; its CanExecute drives the enabled state. |
| Property | `SkeleKit.ToolbarItem.CommandParameter` | public get/set | null | No | No automatic invalidation | The parameter passed to `ToolbarItem.Command`. |
| Method | `SkeleKit.ToolbarItem.#ctor` | public (compiled) | n/a | n/a | n/a | _Exported in compiled metadata but absent from the XML documentation baseline._ |

### Gallery treatment

Non-gallery/reference entry. Exercise this API through the application, tooling, or code-only labs described by its behavior rather than inventing a visual specimen.
