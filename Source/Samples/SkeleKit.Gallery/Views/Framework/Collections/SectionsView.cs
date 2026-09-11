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
			Header = new CollectionPageHeader(),
			Footer = new CollectionPageFooter(),
			GroupedItemsSource = Bind(vm => vm.Sections),
			ItemTemplate = static () => new SectionCell(),
			SectionHeaderTemplate = static () => new CollectionHeader(),
			SectionFooterTemplate = static () => new CollectionFooter(),
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

internal sealed class CollectionPageHeader : Border
{
	public CollectionPageHeader()
	{
		Margin = new(16, 12, 16, 4);
		Padding = 16;
		CornerRadius = 16;
		Background = Colors.Teal.WithAlpha(0.14);

		Child = new StackPanel
		{
			Spacing = 3,

			Children =
			{
				new Label
				{
					Text = "Collection header",
					TextStyle = TextStyle.Title3,
					FontWeight = FontWeight.Semibold
				},

				new Label
				{
					Text = "One view above every section, scrolling with the collection.",
					TextStyle = TextStyle.Subheadline,
					TextColor = Colors.SecondaryLabel,
					MaxLines = 2
				}
			}
		};
	}
}

internal sealed class CollectionPageFooter : Border
{
	public CollectionPageFooter()
	{
		Margin = new(16, 8, 16, 16);
		Padding = 12;
		CornerRadius = 14;
		Background = Colors.SecondaryGroupedBackground;

		Child = new Label
		{
			Text = "Collection footer",
			TextStyle = TextStyle.Footnote,
			TextColor = Colors.SecondaryLabel,
			TextAlignment = TextAlignment.Center
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
					Text = Bind(section => section.Layout)
						.ConvertTo(layout => layout is CollectionLayoutKind.Carousel ? "Carousel" : "List"),
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
			Text = Bind(section => section.Items)
				.ConvertTo(items => $"{items.Count} items"),
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
