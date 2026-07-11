using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A horizontally scrolling carousel of posters.
/// </summary>
public class CarouselDemo : ContentView<CarouselDemoViewModel>
{
	readonly CollectionView<Movie> movies = new()
	{
		Layout = CollectionLayout.Carousel(itemWidth: 130, spacing: 12, snap: CarouselSnap.LeadingBoundary),
		ItemTemplate = () => new MovieCell(),
		Height = 260
	};

	public CarouselDemo()
	{
		Title = "Carousel";

		Content = new VStack
		{
			Spacing = 16,
			Margin = new Thickness(0, 16),
			Children =
			{
				new Label
				{
					Text = "Swipe sideways — it settles on an item",
					Margin = new Thickness(16, 0),
					TextColor = Theme.Secondary,
					FontSize = 13
				},
				movies
			}
		};
	}

	protected override void OnViewModelAttached()
	{
		movies.ItemsSource = Bindable.From<IReadOnlyList<Movie>?>(ViewModel!.Movies);
		movies.SelectionCommand = ViewModel.OpenCommand;
	}

	protected override void OnAppearing() =>
		_ = ViewModel!.LoadAsync();
}
