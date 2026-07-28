using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Cells;

internal sealed class SectionHeaderView : ItemView<GallerySection>
{
	public SectionHeaderView(
		Color accent)
	{
		Content = new Grid
		{
			Padding = new(16, 13, 16, 7),
			ColumnSpacing = 8,

			Columns =
			{
				18,
				GridLength.Star
			},

			Children =
			{
				new Image
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Source = Bind(section => section.Symbol, symbol => (ImageSource?)ImageSource.Symbol(symbol)),
					SymbolSize = 13,
					SymbolWeight = FontWeight.Semibold,
					Tint = accent
				}.Column(0),

				new Label
				{
					Text = Bind(section => section.Title),
					TextStyle = TextStyle.Subheadline,
					FontWeight = FontWeight.Semibold,
					TextColor = accent
				}.Column(1)
			}
		};
	}
}
