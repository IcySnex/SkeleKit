namespace SkeleKit.Gallery;

[Page]
internal sealed class MainView : ContentView<MainViewModel>
{
	readonly List<CategoryCard> categoryCards = [];


	public MainView(
		MainViewModel viewModel) : base(viewModel)
	{
		Title = "SkeleKit";
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;
		SearchPlaceholder = "Search components";
		HidesSearchBarWhenScrolling = true;
		SearchObscuresBackground = false;
		SearchChanged = Filter;
		SearchCanceled = () => Filter("");

		StackPanel categories = new()
		{
			Spacing = 10
		};

		foreach (GalleryCategory category in viewModel.Categories)
		{
			CategoryCard card = new(category, viewModel.OpenCategoryCommand);
			categoryCards.Add(card);
			categories.Children.Add(card);
		}

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Padding = new(16, 12, 16, 32),
				Spacing = 20,

				Children =
				{
					BuildHero(),

					new Label
					{
						Text = "Explore",
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold
					},

					categories,

					new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Margin = new(0, 8, 0, 0),
						Text = "Native iOS controls, composed with C#",
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel
					}
				}
			}
		};
	}


	static Border BuildHero() =>
		new()
		{
			Background = LinearGradient.Horizontal(
				Color.FromHex(0x5856D6),
				Color.FromHex(0x007AFF)),
			CornerRadius = 22,
			Padding = 20,

			Child = new Grid
			{
				ColumnSpacing = 16,

				Columns =
				{
					GridLength.Star,
					56
				},

				Children =
				{
					new StackPanel
					{
						VerticalAlignment = VerticalAlignment.Center,
						Spacing = 6,

						Children =
						{
							new Label
							{
								Text = "Build native iOS interfaces",
								TextStyle = TextStyle.Title2,
								FontWeight = FontWeight.Bold,
								TextColor = Colors.White,
								MaxLines = 2
							},

							new Label
							{
								Text = "Browse every SkeleKit component, pattern and platform integration.",
								TextStyle = TextStyle.Subheadline,
								TextColor = Colors.White.WithAlpha(0.82),
								MaxLines = 3
							}
						}
					}.Column(0),

					new Image
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Source = ImageSource.Symbol("square.grid.2x2.fill"),
						SymbolSize = 42,
						Tint = Colors.White
					}.Column(1)
				}
			}
		};

	void Filter(
		string query)
	{
		foreach (CategoryCard card in categoryCards)
			card.IsVisible = card.Category.Matches(query);
	}
}
