using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Cells;

namespace SkeleKit.Gallery.Views.Pages;

internal abstract class GalleryListView<TViewModel> : ContentView<TViewModel>
	where TViewModel : GalleryListViewModel
{
	protected GalleryListView(
		TViewModel viewModel,
		string title) : base(viewModel)
	{
		Title = title;
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;

		Content = new CollectionView<GalleryTopic, GallerySection>
		{
			GroupedItemsSource = Bind(model => model.Sections),
			ItemTemplate = static () => new TopicCell(),
			HeaderTemplate = static () => new SectionHeaderView(),
			Layout = CollectionLayout.List(grouped: true),
			SelectionCommand = viewModel.OpenTopicCommand,
			HighlightsSelection = true
		};
	}
}
