using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Velura's Home poster grid: a virtualized CollectionView with recycled cells.
/// </summary>
public class GridDemo : ContentView<GridDemoViewModel>
{
	public GridDemo(
		GridDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Grid";

		Content = new CollectionView<Movie>
		{
			Layout = CollectionLayout.Grid(columns: 3, spacing: 12),
			ItemTemplate = () => new MovieCell(),
			ItemsSource = ViewModel.Movies,
			SelectionCommand = ViewModel.OpenCommand
		};
	}

	protected override void OnAppearing() =>
		_ = ViewModel.LoadAsync();
}
