using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A horizontally scrolling carousel of posters.
/// </summary>
[Page]
public class CarouselDemo : ContentView<CarouselDemoViewModel>
{
	public CarouselDemo(
		CarouselDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Carousel";

		Content = new StackPanel
		{
			Spacing = 16,
			Margin = new Thickness(0, 16),
			Children =
			{
				new Label
				{
					Style = Styles.Caption,
					Text = "Swipe sideways — it settles on an item",
					Margin = new Thickness(16, 0)
				},
				new CollectionView<Movie>
				{
					Layout = CollectionLayout.Carousel(itemWidth: 130, spacing: 12, snap: CarouselSnap.LeadingBoundaryPeek),
					ItemTemplate = () => new MovieCell(),
					ItemsSource = ViewModel.Movies,
					SelectionCommand = ViewModel.OpenCommand,
					Height = 260,
					HighlightsSelection = false,

					// the other peek flavour: a fully custom card instead of the lifted tile
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
					PreviewCommand = ViewModel.OpenCommand,

					// only the row bleeds: it scrolls under the notch, the posters stay inside the safe area
					IgnoresSafeArea = SafeAreaEdges.Leading | SafeAreaEdges.Trailing
				}
			}
		};
	}

	protected override void OnAppearing() =>
		_ = ViewModel.LoadAsync();
}
