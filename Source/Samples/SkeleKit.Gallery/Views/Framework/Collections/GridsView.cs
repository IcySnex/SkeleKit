using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class GridsView : ShowcaseView<GridsViewModel>
{
	public GridsView(
		GridsViewModel viewModel) : base(viewModel, "Grids", Colors.Teal)
	{
		AddCodePage("Grids code", () => viewModel.GridCode);

		ToolbarItems.Add(new ToolbarItem
		{
			Icon = ImageSource.Symbol("ellipsis.circle"),
			Menu =
			{
				new MenuAction
				{
					Text = "Add item",
					Command = viewModel.AddCommand
				},
				new MenuAction
				{
					Text = "Remove first item",
					Command = viewModel.RemoveCommand
				}
			}
		});

		Content = new CollectionView<GridEntry>
		{
			ItemsSource = viewModel.Items,
			ItemTemplate = static () => new GridCell(),
			Layout = CollectionLayout.Grid(columns: 3, spacing: 12),
			HighlightsSelection = false,

			EmptyView = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = "No items",
				TextStyle = TextStyle.Headline,
				TextColor = Colors.SecondaryLabel
			}
		};
	}
}

internal sealed class GridCell : ItemView<GridEntry>
{
	public GridCell() =>
		Content = new Border
		{
			Height = 112,
			Background = Colors.Teal.WithAlpha(0.14),
			CornerRadius = 16,

			Child = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Spacing = 3,

				Children =
				{
					new Label
					{
						Text = Bind(item => item.Number),
						TextStyle = TextStyle.Title2,
						FontWeight = FontWeight.Bold,
						TextColor = Colors.Teal
					},

					new Label
					{
						Text = Bind(item => item.Title),
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel
					}
				}
			}
		};
}
