using System.Windows.Input;
using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// Velura's Home poster grid: a virtualized CollectionView with recycled cells.
/// </summary>
public class GridDemo : ContentView<GridDemoViewModel>
{
	readonly CollectionView<Movie> movies = new()
	{
		Layout = CollectionLayout.Grid(columns: 3, spacing: 12),
		ItemTemplate = () => new MovieCell()
	};

	public GridDemo(
		GridDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Grid";

		Content = movies;

		AttachViewModel();
		}

	void AttachViewModel()
	{
		movies.ItemsSource = Bindable.From<IReadOnlyList<Movie>?>(ViewModel!.Movies);
		movies.SelectionCommand = ViewModel.OpenCommand;
	}

	protected override void OnAppearing() =>
		_ = ViewModel!.LoadAsync();
}
