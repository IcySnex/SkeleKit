using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Cells;

namespace SkeleKit.Gallery.Views.Shared;

internal abstract class CatalogView<TViewModel> : TintView<TViewModel>
	where TViewModel : CatalogViewModel
{
	protected CatalogView(
		TViewModel viewModel,
		string title,
		Color accent) : base(viewModel, accent)
	{
		Title = title;
		TitleStyle = TitleStyle.Large;
		BackgroundStyle = PageBackground.Grouped;

		ToolbarItems.Add(new()
		{
			Icon = "info.circle",
			Command = viewModel.ShowInfoCommand
		});

		Border glow = new()
		{
			VerticalAlignment = VerticalAlignment.Start,
			Height = 260,
			IgnoresSafeArea = SafeAreaEdges.Top | SafeAreaEdges.Leading | SafeAreaEdges.Trailing,
			Background = LinearGradient.Vertical(
				accent.WithAlpha(0.2),
				accent.WithAlpha(0))
		};

		CollectionView<GalleryTopic, GallerySection> collection = new()
		{
			GroupedItemsSource = Bind(model => model.Sections),
			ItemTemplate = static () => new TopicCell(),
			HeaderTemplate = static () => new SectionHeaderView(),
			Layout = CollectionLayout.List(grouped: true),
			SelectionCommand = viewModel.OpenTopicCommand,
			HighlightsSelection = true,
			IgnoresSafeArea = SafeAreaEdges.Top | SafeAreaEdges.Bottom,
			Scrolled = offset => glow.Opacity = Math.Clamp(1 - Math.Max(0, offset) / 120, 0, 1)
		};

		Content = new Overlay
		{
			Children =
			{
				glow,
				collection
			}
		};
	}
}
