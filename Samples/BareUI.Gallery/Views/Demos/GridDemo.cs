using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Velura's Home poster grid: a virtualized CollectionView with recycled cells.
/// </summary>
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
				new() { Text = $"{Target} to top", Icon = "arrow.up.to.line", Command = Command.From(() => Jump(ScrollPosition.Top)) },
				new() { Text = $"{Target} to centre", Icon = "arrow.down.and.line.horizontal.and.arrow.up", Command = Command.From(() => Jump(ScrollPosition.Center)) },
				new() { Text = $"{Target} to bottom", Icon = "arrow.down.to.line", Command = Command.From(() => Jump(ScrollPosition.Bottom)) }
			}
		});

		Content = collection = new CollectionView<Movie>
		{
			Layout = CollectionLayout.Grid(columns: 3, spacing: 12),
			ItemTemplate = () => new MovieCell(),
			ItemsSource = ViewModel.Movies,
			SelectionCommand = ViewModel.OpenCommand,
			LoadMoreCommand = ViewModel.LoadMoreCommand,
			HighlightsSelection = false
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
