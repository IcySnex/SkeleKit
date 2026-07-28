namespace SkeleKit.Gallery;

internal sealed class CategoryView : ContentView
{
	public CategoryView(
		GalleryCategory category)
	{
		Title = category.Title;
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;
		BackButtonStyle = BackButtonStyle.Generic;

		StackPanel topics = new()
		{
			Spacing = 8
		};

		foreach (string component in category.Components)
			topics.Children.Add(BuildTopic(component, category.Accent));

		Content = new ScrollView
		{
			Content = new StackPanel
			{
				Padding = new(16, 12, 16, 32),
				Spacing = 20,

				Children =
				{
					new Border
					{
						Background = category.Accent.WithAlpha(0.12),
						CornerRadius = 20,
						Padding = 18,

						Child = new Grid
						{
							ColumnSpacing = 14,

							Columns =
							{
								48,
								GridLength.Star
							},

							Children =
							{
								new Image
								{
									HorizontalAlignment = HorizontalAlignment.Center,
									VerticalAlignment = VerticalAlignment.Center,
									Source = ImageSource.Symbol(category.Symbol),
									SymbolSize = 30,
									Tint = category.Accent
								}.Column(0),

								new StackPanel
								{
									VerticalAlignment = VerticalAlignment.Center,
									Spacing = 4,

									Children =
									{
										new Label
										{
											Text = category.Title,
											TextStyle = TextStyle.Title2,
											FontWeight = FontWeight.Bold
										},

										new Label
										{
											Text = category.Description,
											TextStyle = TextStyle.Subheadline,
											TextColor = Colors.SecondaryLabel,
											MaxLines = 3
										}
									}
								}.Column(1)
							}
						}
					},

					new Label
					{
						Text = "Topics",
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold
					},

					topics
				}
			}
		};
	}


	static Border BuildTopic(
		string title,
		Color accent) =>
		new()
		{
			Background = Colors.SecondaryGroupedBackground,
			CornerRadius = 13,
			Padding = new(14, 12),

			Child = new Grid
			{
				ColumnSpacing = 12,

				Columns =
				{
					22,
					GridLength.Star
				},

				Children =
				{
					new Image
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Source = ImageSource.Symbol("circle.fill"),
						SymbolSize = 8,
						Tint = accent
					}.Column(0),

					new Label
					{
						VerticalAlignment = VerticalAlignment.Center,
						Text = title,
						TextStyle = TextStyle.Body
					}.Column(1)
				}
			}
		};
}
