using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// A Home-style screen mixing a carousel, a grid, and a list in one scrolling collection.
/// </summary>
[Page]
public class MixedDemo : ContentView<MixedDemoViewModel>
{
	public MixedDemo(
		MixedDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Mixed sections";

		Content = new CollectionView<Movie, HomeRow>
		{
			// the collection scrolls vertically; each row picks its own arrangement off the section model
			Layout = CollectionLayout.List(),
			SectionLayout = row => row.Layout switch
			{
				CollectionLayoutKind.Carousel => CollectionLayout.Carousel(itemWidth: 130, spacing: 12, snap: CarouselSnap.ItemPeek),
				CollectionLayoutKind.Grid => CollectionLayout.Grid(columns: 3, spacing: 12),
				_ => CollectionLayout.List()
			},
			ItemTemplate = () => new MovieCell(),
			HeaderTemplate = () => new RowHeader(),
			GroupedItemsSource = viewModel.Rows,
			ItemCommand = viewModel.OpenCommand,
			HighlightsSelection = false,
			Prefetch = movie => movie.PosterUrl
		};
	}

	protected override void OnAppearing() =>
		_ = ViewModel.LoadAsync();
}

file class RowHeader : ItemView<HomeRow>
{
	public RowHeader() =>
		Content = new Label
		{
			Style = Styles.SectionHeader,
			Text = Bind(row => row.Title),
			HorizontalAlignment = HorizontalAlignment.Start,
			Margin = new Thickness(16, 8, 16, 4)
		};
}
