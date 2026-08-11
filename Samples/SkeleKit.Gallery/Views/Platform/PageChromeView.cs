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
			SelectedItem = Bind(
				model => model.SelectedTitleStyle,
				static (model, value) => model.SelectedTitleStyle = value!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeBackgroundOption> background = new()
		{
			ItemsSource = viewModel.Backgrounds,
			SelectedItem = Bind(
				model => model.SelectedBackground,
				static (model, value) => model.SelectedBackground = value!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeStatusBarOption> statusBar = new()
		{
			ItemsSource = viewModel.StatusBars,
			SelectedItem = Bind(
				model => model.SelectedStatusBar,
				static (model, value) => model.SelectedStatusBar = value!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeSafeAreaOption> safeArea = new()
		{
			ItemsSource = viewModel.SafeAreas,
			SelectedItem = Bind(
				model => model.SelectedSafeArea,
				static (model, value) => model.SelectedSafeArea = value!),
			ItemTitle = static option => option.Title
		};

		Picker<PageChromeColorOption> accent = new()
		{
			ItemsSource = viewModel.AccentColors,
			SelectedItem = Bind(
				model => model.SelectedAccentColors,
				static (model, value) => model.SelectedAccentColors = value!),
			ItemTitle = static option => option.Title
		};

		Switch prompt = Toggle(model => model.ShowsPrompt, static (model, value) => model.ShowsPrompt = value);
		Switch scrolling = Toggle(model => model.ScrollsUnderBars, static (model, value) => model.ScrollsUnderBars = value);
		Switch navigationBar = Toggle(model => model.HidesNavigationBar, static (model, value) => model.HidesNavigationBar = value);
		Switch tabBar = Toggle(model => model.HidesTabBar, static (model, value) => model.HidesTabBar = value);
		Switch toolbar = Toggle(model => model.HasToolbar, static (model, value) => model.HasToolbar = value);
		Switch bottomToolbar = Toggle(model => model.HasBottomToolbar, static (model, value) => model.HasBottomToolbar = value);

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
				SettingRow("Scroll under bars", scrolling),
				SettingRow("Hide navigation bar", navigationBar),
				SettingRow("Hide tab bar", tabBar),
				SettingRow("Toolbar actions", toolbar),
				SettingRow("Bottom toolbar", bottomToolbar)),
			Code(model => model.PageCode));
	}

	void AddSearchShowcase(
		PageChromeViewModel viewModel)
	{
		Switch collapsing = Toggle(
			model => model.HidesSearchBarWhenScrolling,
			static (model, value) => model.HidesSearchBarWhenScrolling = value);

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
			Code(model => model.SearchCode));
	}


	static Switch Toggle(
		Func<PageChromeViewModel, bool> getter,
		Action<PageChromeViewModel, bool> setter,
		[CallerArgumentExpression(nameof(getter))] string? path = null) =>
		new()
		{
			IsOn = Bind(getter, setter, path)
		};

	static Button ActionButton(
		string text,
		string icon,
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
		Text = "Ready",
		TextStyle = TextStyle.Headline,
		FontWeight = FontWeight.Semibold,
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
		ScrollsUnderBars = configuration.ScrollsUnderBars;
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
				Text = "Action",
				Icon = "plus",
				IsPrimary = true,
				Command = Command.From(() => status.Text = "Top action tapped")
			});

			ToolbarItems.Add(new ToolbarItem
			{
				Icon = "ellipsis.circle",
				Menu =
				{
					new MenuAction
					{
						Text = "Reset status",
						Command = Command.From(() => status.Text = "Ready")
					}
				}
			});
		}

		if (configuration.HasBottomToolbar)
		{
			BottomToolbarItems.Add(new ToolbarItem
			{
				Text = "Refresh",
				Icon = "arrow.clockwise",
				Command = Command.From(() => status.Text = "Bottom action tapped")
			});

			BottomToolbarItems.Add(new ToolbarItem
			{
				Text = "Done",
				Icon = "checkmark",
				IsPrimary = true,
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
					new Label
					{
						Text = "Page-owned chrome",
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold
					},

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
