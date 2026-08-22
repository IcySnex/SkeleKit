using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class ListsView : ShowcaseView<ListsViewModel>
{
	public ListsView(
		ListsViewModel viewModel) : base(viewModel, "Lists", Colors.Teal)
	{
		AddCodePage("Lists code", () => viewModel.ListCode);

		ToolbarItems.Add(new ToolbarItem
		{
			Icon = "ellipsis.circle",
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

		Content = new CollectionView<ListEntry>
		{
			ItemsSource = viewModel.Items,
			ItemTemplate = static () => new ListCell(),
			Layout = CollectionLayout.List(),
			ItemCommand = viewModel.SelectCommand,
			ShowsSeparators = true,

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

internal sealed class ListCell : ItemView<ListEntry>
{
	public ListCell() =>
		Content = new Border
		{
			Height = 52,

			Child = new Label
			{
				Margin = new Thickness(16, 0),
				VerticalAlignment = VerticalAlignment.Center,
				Text = Bind(item => item.Title),
				TextStyle = TextStyle.Body
			}
		};
}
