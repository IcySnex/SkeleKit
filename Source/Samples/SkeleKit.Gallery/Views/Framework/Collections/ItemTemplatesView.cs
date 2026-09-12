using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class ItemTemplatesView : ShowcaseView<ItemTemplatesViewModel>
{
	public ItemTemplatesView(
		ItemTemplatesViewModel viewModel) : base(viewModel, "Item Templates", Colors.Teal)
	{
		AddCodePage("Item templates code", () => viewModel.TemplatesCode);

		Content = new CollectionView<TemplateEntry>
		{
			Padding = new(0, 16, 0, 32),
			ItemsSource = viewModel.Items,
			ItemTemplateSelector = new ItemTemplateSelector<TemplateEntry>()
				.Add<TemplateNavigationEntry>(static () => new TemplateNavigationCell())
				.Add<TemplateToggleEntry>(static () => new TemplateToggleCell())
				.Add<TemplateActionEntry>(static () => new TemplateActionCell()),
			ItemCommand = viewModel.ActivateCommand,
			Layout = CollectionLayout.List(grouped: true),
			RetainsSelection = false,
			SeparatorInsets = new(52, 0, 12, 0),

			Header = new StackPanel
			{
				Padding = new(0, 0, 0, 16),
				Spacing = 4,

				Children =
				{
					new Label
					{
						Text = "Different models, different reusable cells",
						TextStyle = TextStyle.Headline,
						FontWeight = FontWeight.Semibold
					},

					new Label
					{
						Text = Bind(vm => vm.Status),
						TextStyle = TextStyle.Subheadline,
						TextColor = Colors.SecondaryLabel
					}
				}
			}
		};
	}
}

internal sealed class TemplateNavigationCell : ItemView<TemplateNavigationEntry>
{
	public TemplateNavigationCell()
	{
		Background = Colors.SecondaryGroupedBackground;

		Content = new Grid
		{
			Padding = new(16, 0),
			ColumnSpacing = 12,
			Columns =
			{
				24,
				GridLength.Star,
				GridLength.Auto,
				10
			},

			Children =
			{
				Icon(Bind(item => item.Symbol)),
				Title(Bind(item => item.Title)).Column(1),
				new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					Text = Bind(item => item.Detail),
					TextStyle = TextStyle.Body,
					TextColor = Colors.SecondaryLabel,
					MaxLines = 1
				}.Column(2),
				new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					Source = ImageSource.Symbol("chevron.right", weight: FontWeight.Semibold, colors: [Colors.TertiaryLabel])
				}.Column(3)
			}
		};
	}


	static Image Icon<TSource, TOwner>(
		BindingExpression<TSource, TOwner, string> symbol)
		where TSource : class
		where TOwner : class? =>
		new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			Height = 22,
			Width = 22,
			Source = symbol.ConvertTo(value => ImageSource.Symbol(value!, colors: [Colors.Label]))
		};

	static Label Title<TSource, TOwner>(
		BindingExpression<TSource, TOwner, string> text)
		where TSource : class
		where TOwner : class? =>
		new()
		{
			VerticalAlignment = VerticalAlignment.Center,
			Text = text,
			TextStyle = TextStyle.Body,
			MaxLines = 1
		};
}

internal sealed class TemplateToggleCell : ItemView<TemplateToggleEntry>
{
	public TemplateToggleCell()
	{
		Background = Colors.SecondaryGroupedBackground;
		HighlightBackground = null;

		Content = new Grid
		{
			Padding = new(16, 0),
			ColumnSpacing = 12,
			Columns =
			{
				24,
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					Height = 22,
					Width = 22,
					Source = Bind(item => item.Symbol)
						.ConvertTo(value => ImageSource.Symbol(value, colors: [Colors.Label]))
				},
				new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					Text = Bind(item => item.Title),
					TextStyle = TextStyle.Body,
					MaxLines = 1
				}.Column(1),
				new Switch
				{
					VerticalAlignment = VerticalAlignment.Center,
					IsOn = Bind(item => item.Value).TwoWay(
						static (item, value) => item.Value = value)
				}.Column(2)
			}
		};
	}
}

internal sealed class TemplateActionCell : ItemView<TemplateActionEntry>
{
	public TemplateActionCell()
	{
		Background = Colors.SecondaryGroupedBackground;

		Content = new Grid
		{
			Padding = new(16, 0),
			ColumnSpacing = 12,
			Columns =
			{
				24,
				GridLength.Star
			},

			Children =
			{
				new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					Height = 22,
					Width = 22,
					Source = Bind(item => item.Symbol)
						.ConvertTo(value => ImageSource.Symbol(value, colors: [Colors.Red]))
				},
				new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					Text = Bind(item => item.Title),
					TextStyle = TextStyle.Body,
					TextColor = Colors.Red,
					MaxLines = 1
				}.Column(1)
			}
		};
	}
}
