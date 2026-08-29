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

		Switch prompt = Toggle(vm => vm.ShowsPrompt, (vm, val) => vm.ShowsPrompt = val);
		Switch navigationBar = Toggle(vm => vm.HidesNavigationBar, (vm, val) => vm.HidesNavigationBar = val);
		Switch tabBar = Toggle(vm => vm.HidesTabBar, (vm, val) => vm.HidesTabBar = val);
		Switch toolbar = Toggle(vm => vm.HasToolbar, (vm, val) => vm.HasToolbar = val);
		Switch bottomToolbar = Toggle(vm => vm.HasBottomToolbar, (vm, val) => vm.HasBottomToolbar = val);

		Button open = ActionButton(
			"Open configured page",
			"rectangle.portrait.and.arrow.forward");
		open.Command = Command.From(() => _ = Navigator.PushViewAsync(new PageChromeDemo(viewModel.Configuration)));

		AddShowcase(
			"Navigation shell",
			"Page-owned navigation, status and toolbar chrome.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(open, 140),
				SettingRow("Title", title),
				SettingRow("Prompt", prompt),
				SettingRow("Background", background),
				SettingRow("Status bar", statusBar),
				SettingRow("Accent colors", accent),
				SettingRow("Safe area", safeArea),
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
		TitleStyle = configuration.TitleStyle;
		Prompt = configuration.ShowsPrompt ? "ContentView" : null;
		SafeAreaEdges = configuration.SafeAreaEdges;
		HidesNavigationBar = configuration.HidesNavigationBar;
		BackgroundStyle = configuration.BackgroundStyle;
		StatusBar = configuration.StatusBar;
		BarTint = configuration.AccentColor;
		TitleColor = configuration.AccentColor;
		LargeTitleColor = configuration.AccentColor;
		HidesTabBar = configuration.HidesTabBar;

		if (configuration.HasToolbar)
		{
			ToolbarItems.Add(new ToolbarItem
			{
				Icon = ImageSource.Symbol("plus"),
				IsPrimary = true,
				Command = Command.From(() => status.Text = "Top action tapped")
			});

			ToolbarItems.Add(new ToolbarItem
			{
				Icon = ImageSource.Symbol("ellipsis.circle"),
				Menu =
				{
					new MenuAction
					{
						Text = "Reset status",
						Command = Command.From(() => status.Text = "No action pressed yet")
					}
				}
			});
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
						("Title", configuration.TitleStyle.ToString()),
						("Prompt", configuration.ShowsPrompt ? "Visible" : "Hidden"),
						("Background", configuration.BackgroundStyle.ToString()),
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
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;
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
