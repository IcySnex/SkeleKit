using BareUI.Gallery.Models;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// One poster in the grid. Built once per recycled cell, rebound as it scrolls.
/// </summary>
public class MovieCell : ItemView<Movie>
{
	public MovieCell() =>
		Content = new VStack
		{
			Spacing = 6,
			Children =
			{
				new Image
				{
					Source = Bind<string, ImageSource?>(vm => vm.PosterUrl, url => ImageSource.Url(url)),
					Height = 180,
					CornerRadius = 8,
					Stretch = Stretch.UniformToFill
				},

				new Label
				{
					Text = Bind(vm => vm.Title),
					FontSize = 13,
					Bold = true,
					MaxLines = 1
				},

				new Label
				{
					Text = Bind(vm => vm.Year, year => year.ToString()),
					FontSize = 12,
					TextColor = Theme.Secondary
				}
			}
		};
}
