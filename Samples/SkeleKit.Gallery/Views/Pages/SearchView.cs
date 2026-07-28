using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Cells;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class SearchView : ContentView<SearchViewModel>
{
	public SearchView(
		SearchViewModel viewModel) : base(viewModel)
	{
		Title = "Search";
		BackgroundStyle = PageBackground.Grouped;
		SearchPlaceholder = "Search SkeleKit";
		HidesSearchBarWhenScrolling = false;
		SearchObscuresBackground = false;
		SearchChanged = viewModel.Search;
		SearchCanceled = () => viewModel.Search("");

		Content = new CollectionView<GalleryTopic>
		{
			ItemsSource = Bind(model => model.Results),
			ItemTemplate = static () => new TopicCell(),
			Layout = CollectionLayout.List(grouped: true),
			SelectionCommand = viewModel.OpenTopicCommand,
			HighlightsSelection = true,

			EmptyView = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Spacing = 10,

				Children =
				{
					new Image
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Source = ImageSource.Symbol("magnifyingglass"),
						SymbolSize = 34,
						Tint = Colors.SecondaryLabel
					},

					new Label
					{
						Text = "Search every component and platform API",
						TextStyle = TextStyle.Body,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center,
						MaxLines = 2
					}
				}
			}
		};
	}
}
