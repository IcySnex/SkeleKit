using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class SectionsView : ShowcaseView<SectionsViewModel>
{
	public SectionsView(
		SectionsViewModel viewModel) : base(viewModel, "Sections", Colors.Teal)
	{
		AddCodePage("Sections code", () => viewModel.SectionsCode);

		Content = new CollectionView<SectionEntry, CollectionSection>
		{
			GroupedItemsSource = Bind(model => model.Sections),
			ItemTemplate = static () => new SectionCell(),
			HeaderTemplate = static () => new CollectionHeader(),
			FooterTemplate = static () => new CollectionFooter(),
			Layout = CollectionLayout.List(),
			SectionLayout = section => section.Layout switch
			{
				CollectionLayoutKind.Carousel => CollectionLayout.Carousel(
					itemWidth: 220,
					spacing: 12,
					snap: CarouselSnap.ItemPeek),
				_ => CollectionLayout.List()
			},
			HighlightsSelection = false
		};
	}
}

internal sealed class SectionCell : ItemView<SectionEntry>
{
	readonly Border container;


	public SectionCell()
	{
		container = new()
		{
			Height = 68,
			Padding = new Thickness(14, 0),

			Child = new StackPanel
			{
				VerticalAlignment = VerticalAlignment.Center,
				Spacing = 2,

				Children =
				{
					new Label
					{
						Text = Bind(item => item.Title),
						TextStyle = TextStyle.Body,
						FontWeight = FontWeight.Semibold
					},

					new Label
					{
						Text = Bind(item => item.Layout),
						TextStyle = TextStyle.Footnote,
						TextColor = Colors.SecondaryLabel
					}
				}
			}
		};

		Content = container;
	}


	protected override void OnItemChanged(
		SectionEntry? item)
	{
		container.Background = item?.IsFeatured is true
			? Colors.Teal.WithAlpha(0.14)
			: null;
		container.CornerRadius = item?.IsFeatured is true ? 14 : 0;
	}
}

internal sealed class CollectionHeader : ItemView<CollectionSection>
{
	public CollectionHeader() =>
		Content = new Label
		{
			Margin = new Thickness(16, 12, 16, 6),
			Text = Bind(section => section.Title),
			TextStyle = TextStyle.Headline,
			FontWeight = FontWeight.Semibold
		};
}

internal sealed class CollectionFooter : ItemView<CollectionSection>
{
	public CollectionFooter() =>
		Content = new Label
		{
			Margin = new Thickness(16, 6, 16, 12),
			Text = Bind(section => section.Footer),
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel
		};
}
