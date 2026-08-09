using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Foundations;

[Page]
internal sealed class ContentViewView : ShowcaseView<ContentViewViewModel>
{
	public ContentViewView(
		ContentViewViewModel viewModel) : base(viewModel, "ContentView", Colors.Indigo)
	{
		AddChromeShowcase(viewModel);
		AddSearchShowcase(viewModel);
		AddLifecycleShowcase(viewModel);
	}


	void AddChromeShowcase(
		ContentViewViewModel viewModel)
	{
		SegmentedControl titleStyle = new()
		{
			SelectedIndex = Bind(
				model => model.TitleStyleIndex,
				static (model, value) => model.TitleStyleIndex = value)
		};
		titleStyle.Items.Add("Large");
		titleStyle.Items.Add("Inline");

		Switch prompt = new()
		{
			IsOn = Bind(
				model => model.ShowsPrompt,
				static (model, value) => model.ShowsPrompt = value)
		};

		Switch tabBar = new()
		{
			IsOn = Bind(
				model => model.HidesTabBar,
				static (model, value) => model.HidesTabBar = value)
		};

		Button open = DemoButton(
			"Open chrome page",
			() => _ = Navigator.PushViewAsync(new ContentViewChromeDemo(
				viewModel.SelectedTitleStyle,
				viewModel.ShowsPrompt,
				viewModel.HidesTabBar)));

		AddShowcase(
			"Composition & chrome",
			"See how the selected title, prompt and tab-bar options affect a real page.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(open, 140),
				LabeledControl("Title style", titleStyle),
				SettingRow("Show prompt", prompt),
				SettingRow("Hide tab bar", tabBar)),
			Code(model => model.ChromeCode));
	}

	void AddSearchShowcase(
		ContentViewViewModel viewModel)
	{
		Button open = DemoButton(
			"Open search page",
			() => _ = Navigator.PushViewAsync(new ContentViewSearchDemo()));

		AddShowcase(
			"Navigation search",
			"Try typing, changing scope, submitting and cancelling in the native navigation search field.",
			ShowcaseBox.Canvas(open, 140),
			Code(model => model.SearchCode));
	}

	void AddLifecycleShowcase(
		ContentViewViewModel viewModel)
	{
		Button open = DemoButton(
			"Open lifecycle page",
			() => _ = Navigator.PushViewAsync(new ContentViewLifecycleDemo()));

		AddShowcase(
			"Lifecycle & leave guard",
			"Cover and uncover a page to observe its lifecycle, or enable confirmation before leaving it.",
			ShowcaseBox.Canvas(open, 140),
			Code(model => model.LifecycleCode));
	}


	static Button DemoButton(
		string text,
		Action action) =>
		new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Text = text,
			Kind = ButtonStyle.Tinted,
			Size = ButtonSize.Medium,
			Command = Command.From(action)
		};
}

internal sealed class ContentViewChromeDemo : ContentView
{
	public ContentViewChromeDemo() : this(
		TitleStyle.Large,
		showsPrompt: false,
		hidesTabBar: true)
	{ }

	public ContentViewChromeDemo(
		TitleStyle titleStyle,
		bool showsPrompt,
		bool hidesTabBar)
	{
		Title = "Page chrome";
		TitleStyle = titleStyle;
		Prompt = showsPrompt ? "ContentView" : null;
		HidesTabBar = hidesTabBar;
		BackgroundStyle = PageBackground.Grouped;
		BackButtonStyle = BackButtonStyle.Generic;
		ScrollsUnderBars = true;

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
						Text = "Selected options",
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold
					},

					new Label
					{
						Text = "These match the options on the previous screen.",
						TextStyle = TextStyle.Subheadline,
						TextColor = Colors.SecondaryLabel,
						MaxLines = 2
					},

					ConfigurationCard(
						("Title style", titleStyle.ToString()),
						("Prompt", showsPrompt ? "Visible" : "Hidden"),
						("Tab bar", hidesTabBar ? "Hidden" : "Visible"))
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

internal sealed class ContentViewSearchDemo : ContentView
{
	readonly Label status = new()
	{
		Text = "Activate search to begin.",
		TextStyle = TextStyle.Headline,
		FontWeight = FontWeight.Semibold,
		TextAlignment = TextAlignment.Center,
		MaxLines = 3
	};


	public ContentViewSearchDemo()
	{
		Title = "Search";
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;
		SearchPlaceholder = "Search gallery";
		HidesSearchBarWhenScrolling = false;
		SearchObscuresBackground = false;
		HidesSearchScopesWhenEmpty = false;

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

		Content = new StackPanel
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MaxWidth = 300,
			Padding = 20,
			Spacing = 8,

			Children =
			{
				status,

				new Label
				{
					Text = "The text reflects the native search callbacks.",
					TextStyle = TextStyle.Subheadline,
					TextColor = Colors.SecondaryLabel,
					TextAlignment = TextAlignment.Center,
					MaxLines = 2
				}
			}
		};
	}
}

internal sealed class ContentViewLifecycleDemo : ContentView
{
	readonly List<string> events = [];

	readonly Label lifecycle = new()
	{
		TextStyle = TextStyle.Headline,
		FontWeight = FontWeight.Semibold,
		TextAlignment = TextAlignment.Center,
		MaxLines = 6
	};


	public ContentViewLifecycleDemo()
	{
		Title = "Lifecycle";
		TitleStyle = TitleStyle.Inline;
		BackgroundStyle = PageBackground.Grouped;
		HidesTabBar = true;

		Switch guard = new()
		{
			Toggled = GuardLeaving
		};

		Content = new StackPanel
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Center,
			Padding = 20,
			Spacing = 18,

			Children =
			{
				lifecycle,

				new Button
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					Text = "Cover this page",
					Kind = ButtonStyle.Tinted,
					Command = Command.From(() => _ = Navigator.PushViewAsync(new ContentViewLifecycleCoverDemo()))
				},

				new Grid
				{
					ColumnSpacing = 12,
					Columns =
					{
						GridLength.Star,
						GridLength.Auto
					},

					Children =
					{
						new Label
						{
							VerticalAlignment = VerticalAlignment.Center,
							Text = "Confirm before leaving",
							TextStyle = TextStyle.Subheadline,
							FontWeight = FontWeight.Medium
						},

						guard.Column(1)
					}
				}
			}
		};
	}


	protected override void OnLoaded()
	{
		Record("Loaded");
		base.OnLoaded();
	}

	protected override void OnUnloaded()
	{
		Record("Unloaded");
		base.OnUnloaded();
	}

	protected override void OnAppearing()
	{
		Record("Appearing");
		base.OnAppearing();
	}

	protected override void OnAppeared()
	{
		Record("Appeared");
		base.OnAppeared();
	}

	protected override void OnDisappearing()
	{
		Record("Disappearing");
		base.OnDisappearing();
	}

	protected override void OnDisappeared()
	{
		Record("Disappeared");
		base.OnDisappeared();
	}


	void GuardLeaving(
		bool enabled) =>
		ConfirmLeave = enabled ? ConfirmLeaveAsync : null;

	Task<bool> ConfirmLeaveAsync() =>
		Navigator.ConfirmAsync(
			"Leave page?",
			"Leave confirmation is enabled.",
			"Leave",
			"Stay");

	void Record(
		string name)
	{
		if (events.Count == 6)
			events.RemoveAt(0);

		events.Add(name);
		lifecycle.Text = string.Join(" → ", events);
	}
}

internal sealed class ContentViewLifecycleCoverDemo : ContentView
{
	public ContentViewLifecycleCoverDemo()
	{
		Title = "Cover page";
		HidesTabBar = true;
		BackgroundStyle = PageBackground.Grouped;

		Content = new Label
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			MaxWidth = 280,
			Text = "Go back to see the disappearance and appearance callbacks.",
			TextStyle = TextStyle.Headline,
			TextAlignment = TextAlignment.Center,
			MaxLines = 3
		};
	}
}
