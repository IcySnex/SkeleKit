using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class CollectionInteractionsView : ShowcaseView<CollectionInteractionsViewModel>
{
	public CollectionInteractionsView(
		CollectionInteractionsViewModel viewModel) : base(viewModel, "Interactions", Colors.Teal)
	{
		AddCodePage("Collection interactions code", () => viewModel.InteractionsCode);

		ToolbarItem edit = new()
		{
			Text = "Edit",
			Command = viewModel.ToggleEditingCommand
		};

		viewModel.PropertyChanged += (_, args) =>
		{
			if (args.PropertyName == nameof(viewModel.IsEditing))
				edit.Text = viewModel.IsEditing ? "Done" : "Edit";
		};

		ToolbarItems.Add(edit);

		Content = new CollectionView<ContactEntry>
		{
			ItemsSource = viewModel.Items,
			ItemTemplate = static () => new ContactCell(),
			Layout = CollectionLayout.List(),
			HighlightsSelection = false,
			SeparatorInsets = new Thickness(66, 0, 0, 0),
			RefreshCommand = viewModel.RefreshCommand,
			IsRefreshing = Bind(
				model => model.IsRefreshing,
				static (model, value) => model.IsRefreshing = value),
			ReorderCommand = viewModel.ReorderCommand,
			IsEditing = Bind(
				model => model.IsEditing,
				static (model, value) => model.IsEditing = value),

			SwipeActions =
			{
				new SwipeAction
				{
					Text = "Delete",
					IsDestructive = true,
					Command = viewModel.DeleteCommand
				}
			},

			ItemContextMenu =
			{
				new MenuAction
				{
					Text = "Move to top",
					Command = viewModel.MoveToTopCommand
				},
				new MenuAction
				{
					Text = "Delete",
					IsDestructive = true,
					Command = viewModel.DeleteCommand
				}
			},

			EmptyView = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = "No contacts",
				TextStyle = TextStyle.Headline,
				TextColor = Colors.SecondaryLabel
			}
		};
	}
}

internal sealed class ContactCell : ItemView<ContactEntry>
{
	public ContactCell() =>
		Content = new Grid
		{
			Height = 60,
			Padding = new Thickness(16, 0),
			ColumnSpacing = 12,
			Columns =
			{
				GridLength.Auto,
				GridLength.Star
			},

			Children =
			{
				new Border
				{
					Width = 38,
					Height = 38,
					VerticalAlignment = VerticalAlignment.Center,
					Background = Colors.Teal.WithAlpha(0.16),
					CornerRadius = 19,

					Child = new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = Bind(item => item.Initials),
						TextStyle = TextStyle.Footnote,
						FontWeight = FontWeight.Semibold,
						TextColor = Colors.Teal
					}
				},

				new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					Text = Bind(item => item.Name),
					TextStyle = TextStyle.Body
				}.Column(1)
			}
		};
}
