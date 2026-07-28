using System.Windows.Input;

namespace SkeleKit.Gallery;

internal sealed class CategoryCard : Border
{
	public CategoryCard(
		GalleryCategory category,
		ICommand command)
	{
		Category = category;

		Background = Colors.SecondaryGroupedBackground;
		CornerRadius = 16;
		Padding = 14;
		PointerEffect = PointerEffect.Automatic;
		TapCommand = command;
		TapCommandParameter = category;
		AccessibilityLabel = $"{category.Title}, {category.ComponentCount}";
		AccessibilityHint = "Opens the category";
		AccessibilityTraits = AccessibilityTraits.Button;
		Pressed = pressed => Scale = pressed ? 0.98 : 1;

		Child = new Grid
		{
			ColumnSpacing = 12,

			Columns =
			{
				44,
				GridLength.Star,
				20
			},

			Children =
			{
				new Border
				{
					Width = 44,
					Height = 44,
					Background = category.Accent.WithAlpha(0.14),
					CornerRadius = 12,

					Child = new Image
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Source = ImageSource.Symbol(category.Symbol),
						SymbolSize = 21,
						Tint = category.Accent
					}
				}.Column(0),

				new StackPanel
				{
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 3,

					Children =
					{
						new Label
						{
							Text = category.Title,
							TextStyle = TextStyle.Headline
						},

						new Label
						{
							Text = category.Description,
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel,
							MaxLines = 2
						},

						new Label
						{
							Text = category.ComponentCount,
							TextStyle = TextStyle.Caption1,
							TextColor = category.Accent
						}
					}
				}.Column(1),

				new Image
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Source = ImageSource.Symbol("chevron.right"),
					SymbolSize = 13,
					SymbolWeight = FontWeight.Semibold,
					Tint = Colors.TertiaryLabel
				}.Column(2)
			}
		};
	}


	public GalleryCategory Category { get; }
}
