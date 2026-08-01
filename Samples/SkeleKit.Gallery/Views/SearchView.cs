using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Abstract;
using SkeleKit.Gallery.Views.Cells;

namespace SkeleKit.Gallery.Views;

[Page]
internal sealed class SearchView : TintView<SearchViewModel>
{
	public SearchView(
		SearchViewModel viewModel) : base(viewModel, Colors.Label)
	{
		Title = "Search";
		BackgroundStyle = PageBackground.Grouped;
		SearchPlaceholder = "Search SkeleKit";
		HidesSearchScopesWhenEmpty = true;
		SearchChanged = viewModel.SearchCommand.Execute;
		SearchScopeChanged = viewModel.SelectScopeCommand.Execute;
		SearchCanceled = () => viewModel.CancelSearchCommand.Execute(null);

		SearchScopes.Add("All");
		SearchScopes.Add("Controls");
		SearchScopes.Add("Framework");
		SearchScopes.Add("Platform");

		ToolbarItems.Add(new()
		{
			Icon = "info.circle",
			Command = viewModel.ShowInfoCommand
		});

		Content = new CollectionView<GalleryTopic>
		{
			ItemsSource = Bind(model => model.Results),
			ItemTemplate = static () => new TopicCell(showsArea: true),
			Layout = CollectionLayout.List(grouped: true),
			SelectionCommand = viewModel.OpenTopicCommand,
			HighlightsSelection = true,

			EmptyView = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				MaxWidth = 310,
				Spacing = 8,

				Children =
				{
					new Border
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Width = 68,
						Height = 68,
						CornerRadius = 20,
						Background = Colors.Label.WithAlpha(0.12),

						Child = new Image
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Source = ImageSource.Symbol("magnifyingglass"),
							SymbolSize = 28,
							SymbolWeight = FontWeight.Semibold,
							Tint = Colors.Label
						}
					},

					new Label
					{
						Text = Bind(model => model.EmptyTitle),
						TextStyle = TextStyle.Headline,
						FontWeight = FontWeight.Semibold,
						TextAlignment = TextAlignment.Center
					},

					new Label
					{
						Text = Bind(model => model.EmptySummary),
						TextStyle = TextStyle.Subheadline,
						TextColor = Colors.SecondaryLabel,
						TextAlignment = TextAlignment.Center,
						MaxLines = 3
					}
				}
			}
		};
	}
}
