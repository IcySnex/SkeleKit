using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Cells;

internal sealed class SectionHeaderView : ItemView<GallerySection>
{
	public SectionHeaderView()
	{
		Content = new Grid
		{
			Padding = new(16, 13, 16, 7),

			Children =
			{
				new Label
				{
					Text = Bind(section => section.Title),
					TextStyle = TextStyle.Subheadline,
					FontWeight = FontWeight.Semibold,
					TextColor = Colors.SecondaryLabel
				}
			}
		};
	}
}
