using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class ListsView : ShowcaseView<ListsViewModel>
{
	public ListsView(
		ListsViewModel viewModel) : base(viewModel, "Lists", Colors.Teal)
	{
		AddListShowcase(viewModel);
	}


	void AddListShowcase(
		ListsViewModel viewModel)
	{
		CollectionView<ListEntry> list = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 300,
			Height = 248,
			ItemsSource = viewModel.Items,
			ItemTemplate = static () => new ListCell(),
			Layout = CollectionLayout.List(),
			ItemCommand = viewModel.SelectCommand,
			ShowsSeparators = true,
			Background = Colors.SecondaryBackground,
			CornerRadius = 16
		};

		Stepper count = new()
		{
			Minimum = 1,
			Maximum = 6,
			Step = 1,
			Value = Bind(
				model => model.ItemCount,
				static (model, value) => model.ItemCount = value)
		};

		AddShowcase(
			"Items & selection",
			"Change the observable source, then tap a native list row to select it.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(list, 300),
				SettingRow(
					"Items",
					new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Spacing = 10,

						Children =
						{
							new Label
							{
								VerticalAlignment = VerticalAlignment.Center,
								Text = Bind(model => model.ItemCountLabel),
								TextStyle = TextStyle.Subheadline,
								TextColor = Colors.SecondaryLabel
							},
							count
						}
					}),
				SettingRow(
					"Selected",
					new Label
					{
						Width = 100,
						Height = 20,
						VerticalAlignment = VerticalAlignment.Center,
						Text = Bind(model => model.SelectedTitle),
						TextStyle = TextStyle.Subheadline,
						TextAlignment = TextAlignment.Trailing,
						TextColor = Colors.SecondaryLabel
					})),
			ShowcaseBox.Code(Bind(model => model.ListCode)));
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
