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
					itemWidth: 248,
					spacing: 8,
					snap: CarouselSnap.ItemPeek),
				_ => CollectionLayout.List()
			},
			HighlightsSelection = false,
			ShowsSeparators = false
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
			Height = 72,
			Padding = new Thickness(14, 0),

			Child = new Label
			{
				VerticalAlignment = VerticalAlignment.Center,
				Text = Bind(item => item.Title),
				TextStyle = TextStyle.Body,
				FontWeight = FontWeight.Semibold
			}
		};

		Content = container;
	}


	protected override void OnItemChanged(
		SectionEntry? item)
	{
		bool featured = item?.IsFeatured is true;
		container.Height = featured ? 76 : 64;
		container.Margin = featured ? Thickness.Zero : new(16, 3);
		container.Background = featured
			? Colors.Teal.WithAlpha(0.14)
			: Colors.SecondaryGroupedBackground;
		container.CornerRadius = 14;
	}
}

internal sealed class CollectionHeader : ItemView<CollectionSection>
{
	readonly Grid container;


	public CollectionHeader()
	{
		container = new()
		{
			Columns =
			{
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				new Label
				{
					Text = Bind(section => section.Title),
					TextStyle = TextStyle.Headline,
					FontWeight = FontWeight.Semibold
				},

				new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					Text = Bind<CollectionLayoutKind, string>(
						section => section.Layout,
						layout => layout is CollectionLayoutKind.Carousel ? "Carousel" : "List"),
					TextStyle = TextStyle.Footnote,
					TextColor = Colors.SecondaryLabel
				}.Column(1)
			}
		};
		Content = container;
	}


	protected override void OnItemChanged(
		CollectionSection? section) =>
		container.Margin = section?.Layout is CollectionLayoutKind.Carousel
			? new(0, 8, 8, 5)
			: new(16, 8, 16, 5);
}

internal sealed class CollectionFooter : ItemView<CollectionSection>
{
	readonly Label label;


	public CollectionFooter()
	{
		label = new()
		{
			Text = Bind<IReadOnlyList<SectionEntry>, string>(
				section => section.Items,
				items => $"{items.Count} items"),
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel
		};
		Content = label;
	}


	protected override void OnItemChanged(
		CollectionSection? section) =>
		label.Margin = section?.Layout is CollectionLayoutKind.Carousel
			? new(0, 3, 0, 3)
			: new(16, 3, 16, 3);
}
