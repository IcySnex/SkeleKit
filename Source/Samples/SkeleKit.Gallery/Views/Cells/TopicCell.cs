using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Cells;

internal sealed class TopicCell : ItemView<GalleryTopic>
{
	readonly Border iconBackground;
	readonly Image icon;


	public TopicCell(
		bool showsArea = false)
	{
		icon = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Source = Bind(topic => topic.Symbol, symbol => (ImageSource?)ImageSource.Symbol(symbol)),
			SymbolSize = 19,
			SymbolWeight = FontWeight.Semibold
		};

		iconBackground = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 38,
			Height = 38,
			CornerRadius = 10,
			Child = icon
		};

		Content = new Grid
		{
			Padding = new(16, 11),
			ColumnSpacing = 12,

			Columns =
			{
				40,
				GridLength.Star,
				16
			},

			Children =
			{
				iconBackground.Column(0),

				new StackPanel
				{
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 2,

					Children =
					{
						new Label
						{
							Text = Bind(topic => topic.Title),
							TextStyle = TextStyle.Body,
							FontWeight = FontWeight.Medium
						},

						new Label
						{
							Text = showsArea
								? Bind(topic => topic.SearchSummary)
								: Bind(topic => topic.Summary),
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel,
							MaxLines = 2
						}
					}
				}.Column(1),

				new Image
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Source = ImageSource.Symbol("chevron.right"),
					SymbolSize = 12,
					SymbolWeight = FontWeight.Semibold,
					Tint = Colors.TertiaryLabel
				}.Column(2)
			}
		};
	}


	protected override void OnItemChanged(
		GalleryTopic? item)
	{
		if (item is not GalleryTopic)
			return;

		icon.Tint = item.Accent;
		iconBackground.Background = item.Accent.WithAlpha(0.14);
	}
}
