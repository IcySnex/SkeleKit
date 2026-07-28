using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Cells;

namespace SkeleKit.Gallery.Views.Pages;

internal abstract class GalleryListView<TViewModel> : ContentView<TViewModel>
	where TViewModel : GalleryListViewModel
{
	protected GalleryListView(
		TViewModel viewModel,
		string title,
		Color accent) : base(viewModel)
	{
		Title = title;
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;
		BarAccent = accent;

		ToolbarItems.Add(new()
		{
			Icon = "info.circle",
			Command = viewModel.ShowInfoCommand
		});

		Content = new CollectionView<GalleryTopic, GallerySection>
		{
			GroupedItemsSource = Bind(model => model.Sections),
			ItemTemplate = () => new TopicCell(accent),
			HeaderTemplate = () => new SectionHeaderView(accent),
			Layout = CollectionLayout.List(grouped: true),
			SelectionCommand = viewModel.OpenTopicCommand,
			HighlightsSelection = true,
			Tint = accent
		};
	}
}
