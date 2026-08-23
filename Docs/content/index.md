---
title: Home
navigation: false
description: Native, code-first UI for .NET for iOS. UIKit underneath, clean C# on top.
---

::landing-hero
```csharp [MainView.cs]
[Page]
public class MainView : ContentView
{
    public MainView()
    {
        Content = new StackPanel
        {
            Spacing = 12,
            Padding = new Thickness(20),
            Children =
            {
                new Label { Text = "Hello, iOS" },
                new Button { Text = "Continue" }
            }
        };
    }
}
```
::

::landing-features
::
