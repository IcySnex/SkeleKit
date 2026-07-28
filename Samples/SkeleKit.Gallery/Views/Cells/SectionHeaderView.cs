using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Cells;

internal sealed class SectionHeaderView : ItemView<GallerySection>
{
	public SectionHeaderView()
	{
		Content = new Grid
		{
			Padding = new(16, 10, 16, 6),

			Children =
			{
				new Label
				{
					Text = Bind(section => section.Title),
					TextStyle = TextStyle.Headline,
					TextColor = Colors.SecondaryLabel
				}
			}
		};
	}
}
