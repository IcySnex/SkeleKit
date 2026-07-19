using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// One poster in the grid. Built once per recycled cell, rebound as it scrolls.
/// </summary>
public class MovieCell : ItemView<Movie>
{
	public MovieCell() =>
		Content = new StackPanel
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
					TextStyle = TextStyle.Footnote,
					Text = Bind(vm => vm.Title),
					Bold = true,
					MaxLines = 1
				},

				new Label
				{
					Style = Styles.Caption,
					Text = Bind(vm => vm.Year, year => year.ToString())
				}
			}
		};
}
