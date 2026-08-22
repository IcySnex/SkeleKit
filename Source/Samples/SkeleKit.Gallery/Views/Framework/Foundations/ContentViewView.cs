using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Foundations;

[Page]
internal sealed class ContentViewView : ShowcaseView<ContentViewViewModel>
{
	public ContentViewView(
		ContentViewViewModel viewModel) : base(viewModel, "ContentView", Colors.Indigo)
	{
		AddCompositionShowcase(viewModel);
		AddLifecycleShowcase(viewModel);
	}


	void AddCompositionShowcase(
		ContentViewViewModel viewModel)
	{
		Button open = DemoButton(
			"Open content page",
			() => _ = Navigator.PushViewAsync(new ContentViewCompositionDemo()));

		AddShowcase(
			"Content composition",
			"Build a page by assigning a view tree to Content and let ContentView host it in the app shell.",
			ShowcaseBox.Canvas(open, 140),
			Code(model => model.CompositionCode));
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

internal sealed class ContentViewCompositionDemo : ContentView
{
	public ContentViewCompositionDemo()
	{
		Title = "Content";
		BackgroundStyle = PageBackground.Grouped;

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
						Text = "ContentView owns the page tree.",
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold
					},

					new Label
					{
						Text = "Compose any SkeleKit view hierarchy in Content, then let the navigation shell host it.",
						TextStyle = TextStyle.Subheadline,
						TextColor = Colors.SecondaryLabel,
						MaxLines = 3
					},

					new Border
					{
						Padding = 16,
						Background = Colors.SecondaryBackground,
						CornerRadius = 16,
						Child = new StackPanel
						{
							Spacing = 8,
							Children =
							{
								new Label
								{
									Text = "Content",
									TextStyle = TextStyle.Headline,
									FontWeight = FontWeight.Semibold
								},

								new Label
								{
									Text = "A StackPanel inside a ScrollView inside this page.",
									TextStyle = TextStyle.Subheadline,
									TextColor = Colors.SecondaryLabel
								}
							}
						}
					}
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
