using System.Runtime.CompilerServices;
using System.Windows.Input;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Platform;

[Page]
internal sealed class PageChromeView : ShowcaseView<PageChromeViewModel>
{
	public PageChromeView(
		PageChromeViewModel viewModel) : base(viewModel, "Page Chrome", Colors.Green)
	{
		AddPageShowcase(viewModel);
		AddSearchShowcase(viewModel);
	}


	void AddPageShowcase(
		PageChromeViewModel viewModel)
	{
		Picker<PageChromeTitleOption> title = new()
		{
			ItemsSource = viewModel.TitleStyles,
			SelectedItem = Bind(vm => vm.SelectedTitleStyle)
				.TwoWay((vm, val) => vm.SelectedTitleStyle = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeBackgroundOption> background = new()
		{
			ItemsSource = viewModel.Backgrounds,
			SelectedItem = Bind(vm => vm.SelectedBackground)
				.TwoWay((vm, val) => vm.SelectedBackground = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeStatusBarOption> statusBar = new()
		{
			ItemsSource = viewModel.StatusBars,
			SelectedItem = Bind(vm => vm.SelectedStatusBar)
				.TwoWay((vm, val) => vm.SelectedStatusBar = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeSafeAreaOption> safeArea = new()
		{
			ItemsSource = viewModel.SafeAreas,
			SelectedItem = Bind(vm => vm.SelectedSafeArea)
				.TwoWay((vm, val) => vm.SelectedSafeArea = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeColorOption> accent = new()
		{
			ItemsSource = viewModel.AccentColors,
			SelectedItem = Bind(vm => vm.SelectedAccentColors)
				.TwoWay((vm, val) => vm.SelectedAccentColors = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeNavigationMinimizeOption> navigationMinimize = new()
		{
			IsEnabled = Bind(vm => vm.AllowsNavigationMinimization),
			ItemsSource = viewModel.NavigationMinimizeBehaviors,
			SelectedItem = Bind(vm => vm.SelectedNavigationMinimizeBehavior)
				.TwoWay((vm, val) => vm.SelectedNavigationMinimizeBehavior = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeNavigationSafeAreaOption> navigationSafeArea = new()
		{
			IsEnabled = Bind(vm => vm.AllowsNavigationMinimization),
			ItemsSource = viewModel.NavigationMinimizeSafeAreas,
			SelectedItem = Bind(vm => vm.SelectedNavigationMinimizeSafeArea)
				.TwoWay((vm, val) => vm.SelectedNavigationMinimizeSafeArea = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeNavigationRestoreOption> navigationRestore = new()
		{
			IsEnabled = Bind(vm => vm.AllowsNavigationMinimization),
			ItemsSource = viewModel.NavigationMinimizeRestoreBehaviors,
			SelectedItem = Bind(vm => vm.SelectedNavigationMinimizeRestoreBehavior)
				.TwoWay((vm, val) => vm.SelectedNavigationMinimizeRestoreBehavior = val!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeScrollEdgeOption> topScrollEdge = new()
		{
			ItemsSource = viewModel.TopScrollEdgeStyles,
			SelectedItem = Bind(vm => vm.SelectedTopScrollEdgeStyle)
				.TwoWay((vm, val) => vm.SelectedTopScrollEdgeStyle = val!),
			ItemTitle = static option => option.Title
		};

		Switch prompt = Toggle(vm => vm.ShowsPrompt, (vm, val) => vm.ShowsPrompt = val);
		Switch navigationBar = Toggle(vm => vm.HidesNavigationBar, (vm, val) => vm.HidesNavigationBar = val);
		Switch tabBar = Toggle(vm => vm.HidesTabBar, (vm, val) => vm.HidesTabBar = val);
		Switch toolbar = Toggle(vm => vm.HasToolbar, (vm, val) => vm.HasToolbar = val);
		Switch bottomToolbar = Toggle(vm => vm.HasBottomToolbar, (vm, val) => vm.HasBottomToolbar = val);
		Switch navigationAccessory = Toggle(vm => vm.HasNavigationAccessory, (vm, val) => vm.HasNavigationAccessory = val);

		Button open = ActionButton(
			"Open configured page",
			"rectangle.portrait.and.arrow.forward");
		open.Command = Command.From(() => _ = Navigator.PushViewAsync(new PageChromeDemo(viewModel.Configuration)));

		AddShowcase(
			"Navigation shell",
			"Page-owned chrome. Resize an opened page to test toolbar priority, or turn off its accessory to test minimization.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(open, 140),
				SettingRow("Title", title),
				SettingRow("Prompt", prompt),
				SettingRow("Background", background),
				SettingRow("Status bar", statusBar),
				SettingRow("Accent colors", accent),
				SettingRow("Safe area", safeArea),
				SettingRow("Navigation minimize", navigationMinimize),
				SettingRow("Minimize safe area", navigationSafeArea),
				SettingRow("Restore behavior", navigationRestore),
				SettingRow("Navigation accessory", navigationAccessory),
				SettingRow("Top scroll edge", topScrollEdge),
				SettingRow("Hide navigation bar", navigationBar),
				SettingRow("Hide tab bar", tabBar),
				SettingRow("Toolbar actions", toolbar),
				SettingRow("Bottom toolbar", bottomToolbar)),
			Code(vm => vm.PageCode));
	}

	void AddSearchShowcase(
		PageChromeViewModel viewModel)
	{
		Switch collapsing = Toggle(
			vm => vm.HidesSearchBarWhenScrolling,
			(vm, val) => vm.HidesSearchBarWhenScrolling = val);

		Button open = ActionButton(
			"Open search page",
			"magnifyingglass");
		open.Command = Command.From(() => _ = Navigator.PushViewAsync(new PageChromeSearchDemo(viewModel.SearchConfiguration)));

		AddShowcase(
			"Search chrome",
			"Search field, scopes and callbacks.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(open, 140),
				SettingRow("Collapse while scrolling", collapsing)),
			Code(vm => vm.SearchCode));
	}


	static Switch Toggle(
		Func<PageChromeViewModel, bool> getter,
		Action<PageChromeViewModel, bool> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		new()
		{
			IsOn = Bind(getter, path).TwoWay(setter)
		};

	static Button ActionButton(
		string text,
		ImageSource icon,
		ICommand? command = null) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = text,
			Icon = icon,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = command
		};
}

internal sealed class PageChromeDemo : ContentView
{
	readonly Label status = new()
	{
		Text = "No action pressed yet",
		TextStyle = TextStyle.Headline,
		TextAlignment = TextAlignment.Center,
		MaxLines = 2
	};


	public PageChromeDemo(
		PageChromeConfiguration configuration)
	{
		Title = "Page chrome";
		NavigationTitleStyle = configuration.NavigationTitleStyle;
		NavigationBarMinimizeBehavior = configuration.NavigationMinimizeBehavior;
		NavigationBarMinimizeSafeAreaAdjustment = configuration.NavigationMinimizeSafeArea;
		NavigationBarMinimizeRestorationBehavior = configuration.NavigationMinimizeRestoreBehavior;
		TopScrollEdgeStyle = configuration.TopScrollEdgeStyle;
		Prompt = configuration.ShowsPrompt ? "ContentView" : null;
		SafeAreaEdges = configuration.SafeAreaEdges;
		HidesNavigationBar = configuration.HidesNavigationBar;
		Background = configuration.Background.Value;
		StatusBar = configuration.StatusBar;
		BarTint = configuration.AccentColor;
		TitleColor = configuration.AccentColor;
		LargeTitleColor = configuration.AccentColor;
		HidesTabBar = configuration.HidesTabBar;

		if (configuration.HasNavigationAccessory)
		{
			NavigationAccessory = new Border
			{
				Padding = new(16, 8),
				Child = new Label
				{
					Text = "Navigation accessory",
					TextStyle = TextStyle.Footnote,
					FontWeight = FontWeight.Semibold,
					TextAlignment = TextAlignment.Center
				}
			};
		}

		if (configuration.HasToolbar)
		{
			void AddAction(
				string text,
				ToolbarVisibilityPriority priority,
				bool primary = false) =>
				ToolbarItems.Add(new ToolbarItem
				{
					Text = text,
					IsPrimary = primary,
					VisibilityPriority = priority,
					Command = Command.From(() => status.Text = $"{text} tapped")
				});

			AddAction("Save", ToolbarVisibilityPriority.High, true);

			AddAction("Share", ToolbarVisibilityPriority.Standard);
			AddAction("Undo", ToolbarVisibilityPriority.Standard);
			AddAction("Redo", ToolbarVisibilityPriority.Standard);

			AddAction("Duplicate", ToolbarVisibilityPriority.Low);
			AddAction("Tag", ToolbarVisibilityPriority.Low);
			AddAction("Archive", ToolbarVisibilityPriority.Low);
			AddAction("Delete", ToolbarVisibilityPriority.Low);
		}

		if (configuration.HasBottomToolbar)
		{
			BottomToolbarItems.Add(new ToolbarItem
			{
				Text = "Refresh",
				Icon = ImageSource.Symbol("arrow.clockwise"),
				Command = Command.From(() => status.Text = "Bottom action tapped")
			});

			BottomToolbarItems.Add(new ToolbarItem
			{
				Text = "Done",
				Icon = ImageSource.Symbol("checkmark"),
				IsPrimary = true,
				Tint = Colors.Green,
				Command = Command.From(() => status.Text = "Bottom action tapped")
			});
		}

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Padding = new(16, 18, 16, 32),
				Spacing = 14,

				Children =
				{
					status,

					ConfigurationCard(
						("Title", configuration.NavigationTitleStyle?.ToString() ?? "Inherit"),
						("Minimize", configuration.NavigationMinimizeBehavior.ToString()),
						("Minimize safe area", configuration.NavigationMinimizeSafeArea.ToString()),
						("Restore", configuration.NavigationMinimizeRestoreBehavior.ToString()),
						("Accessory", configuration.HasNavigationAccessory ? "Visible" : "Hidden"),
						("Top scroll edge", configuration.TopScrollEdgeStyle.ToString()),
						("Prompt", configuration.ShowsPrompt ? "Visible" : "Hidden"),
						("Background", configuration.Background.Title),
						("Safe area", configuration.SafeAreaEdges.ToString()),
						("Tab bar", configuration.HidesTabBar ? "Hidden" : "Visible")),

					new Border
					{
						Height = 360,
						Padding = 16,
						Background = Colors.SecondaryBackground,
						CornerRadius = 16
					}
				}
			}
		};
	}


	static Border ConfigurationCard(
		params (string Name, string Value)[] values)
	{
		StackPanel rows = new()
		{
			Spacing = 12
		};

		for (int index = 0; index < values.Length; index++)
		{
			(string name, string value) = values[index];
			rows.Children.Add(ConfigurationRow(name, value));

			if (index < values.Length - 1)
				rows.Children.Add(new Divider());
		}

		return new()
		{
			Padding = 16,
			Background = Colors.SecondaryBackground,
			CornerRadius = 16,
			Child = rows
		};
	}

	static Grid ConfigurationRow(
		string name,
		string value) =>
		new()
		{
			ColumnSpacing = 16,
			Columns =
			{
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				new Label
				{
					Text = name,
					TextStyle = TextStyle.Subheadline,
					FontWeight = FontWeight.Medium
				},

				new Label
				{
					Text = value,
					TextStyle = TextStyle.Subheadline,
					TextColor = Colors.SecondaryLabel
				}.Column(1)
			}
		};
}

internal sealed class PageChromeSearchDemo : ContentView
{
	readonly Label status = new()
	{
		Text = "Ready",
		TextStyle = TextStyle.Headline,
		FontWeight = FontWeight.Semibold,
		TextAlignment = TextAlignment.Center,
		MaxLines = 3
	};


	public PageChromeSearchDemo(
		PageChromeSearchConfiguration configuration)
	{
		Title = "Search";
		NavigationTitleStyle = TitleStyle.Large;
		Background = Colors.GroupedBackground;
		SearchPlaceholder = "Search gallery";
		HidesSearchBarWhenScrolling = configuration.HidesSearchBarWhenScrolling;

		SearchScopes.Add("All");
		SearchScopes.Add("Recent");
		SearchScopes.Add("Saved");

		SearchChanged = query =>
			status.Text = string.IsNullOrWhiteSpace(query) ? "Type a search term." : $"Typing: {query}";
		SearchScopeChanged = index =>
			status.Text = $"Scope: {SearchScopes[index]}";
		SearchCommand = Command.From<string>(query =>
			status.Text = $"Submitted: {query}");
		SearchCanceled = () =>
			status.Text = "Search cancelled";

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Padding = new(16, 20, 16, 32),
				Spacing = 14,

				Children =
				{
					status,

					new Border
					{
						Height = 640,
						Padding = 16,
						Background = Colors.SecondaryBackground,
						CornerRadius = 16
					}
				}
			}
		};
	}
}
