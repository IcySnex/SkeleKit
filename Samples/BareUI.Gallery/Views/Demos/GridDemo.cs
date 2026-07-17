using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Velura's Home poster grid: a virtualized CollectionView with recycled cells.
/// </summary>
[Page]
public class GridDemo : ContentView<GridDemoViewModel>
{
	const string Target = "Moon";

	readonly CollectionView<Movie> collection;


	public GridDemo(
		GridDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Grid";

		ToolbarItems.Add(new()
		{
			Icon = "scope",
			Menu =
			{
				new() { Text = $"Scroll to \"{Target}\" — align top", Icon = "arrow.up.to.line", Command = Command.From(() => Jump(ScrollPosition.Top)) },
				new() { Text = $"Scroll to \"{Target}\" — align centre", Icon = "arrow.down.and.line.horizontal.and.arrow.up", Command = Command.From(() => Jump(ScrollPosition.Center)) },
				new() { Text = $"Scroll to \"{Target}\" — align bottom", Icon = "arrow.down.to.line", Command = Command.From(() => Jump(ScrollPosition.Bottom)) }
			}
		});

		Content = collection = new CollectionView<Movie>
		{
			Layout = CollectionLayout.Grid(columns: 3, spacing: 12),
			ItemTemplate = () => new MovieCell(),
			ItemsSource = ViewModel.Movies,
			SelectionCommand = ViewModel.OpenCommand,
			LoadMoreCommand = ViewModel.LoadMoreCommand,
			HighlightsSelection = false,

			// warm the poster cache a screen ahead of the scroll
			Prefetch = movie => movie.PosterUrl,

			// long-press peek: a large poster card; tapping it opens the movie
			ItemPreview = movie => new StackPanel
			{
				Spacing = 8,
				Padding = new Thickness(16),
				Children =
				{
					new Image
					{
						Source = ImageSource.Url(movie.PosterUrl),
						Height = 360,
						CornerRadius = 12
					},
					new Label { Text = movie.Title, TextStyle = TextStyle.Headline }
				}
			},
			PreviewCommand = ViewModel.OpenCommand
		};
	}


	// the same poster every time: only where it lands changes
	void Jump(
		ScrollPosition position)
	{
		if (ViewModel.Movies.FirstOrDefault(movie => movie.Title == Target) is { } movie)
			collection.ScrollTo(movie, position);
	}

	protected override void OnAppearing() =>
		_ = ViewModel.LoadAsync();
}
