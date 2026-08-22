---
title: Home
navigation: false
description: Native, code-first UI for .NET for iOS. UIKit underneath, clean C# on top.
---

::hero
---
announcement:
  title: 'SkeleKit 0.1.0 is available'
  icon: 'lucide:sparkles'
  to: https://github.com/IcySnex/SkeleKit/releases/tag/v0.1.0
  target: _blank
actions:
  - name: Get started
    to: /getting-started/introduction
  - name: API Reference
    leftIcon: 'lucide:book-open'
    variant: outline
    to: /reference
  - name: GitHub
    variant: outline
    to: https://github.com/IcySnex/SkeleKit
    leftIcon: 'lucide:github'
    target: _blank
---

#title
Native iOS UI. Just C#.

#description
Build UIKit applications with clean object-initializer syntax, AOT-safe MVVM bindings, layout, navigation and controls—without MAUI or XAML.
::

::card-group{:cols="3"}
  ::card
  ---
  title: Native UIKit
  icon: lucide:smartphone
  ---
  Real UIKit controls and behavior. SkeleKit owns composition and layout, not rendering.
  ::

  ::card
  ---
  title: Clean C#
  icon: lucide:braces
  ---
  Compose grids, stacks, controls and bindings with ordinary C# object initializers.
  ::

  ::card
  ---
  title: AOT-safe MVVM
  icon: lucide:shield-check
  ---
  Compiled binding paths without reflection, expression trees or runtime code generation.
  ::

  ::card
  ---
  title: Complete layout
  icon: lucide:panels-top-left
  ---
  Grid, StackPanel, Overlay, Border and safe-area-aware scrolling with a two-pass layout engine.
  ::

  ::card
  ---
  title: Navigation included
  icon: lucide:route
  ---
  Stacks, tabs, sheets, dialogs and iPad sidebars through a ViewModel-first navigator.
  ::

  ::card
  ---
  title: Start from a template
  icon: lucide:rocket
  to: /getting-started/installation
  ---
  Create a configured native iOS project with one `dotnet new` command.
  ::
::
