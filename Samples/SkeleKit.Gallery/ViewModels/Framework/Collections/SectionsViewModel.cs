using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record SectionEntry(
	string Title,
	string Layout,
	bool IsFeatured);

internal sealed record CollectionSection(
	string Title,
	string Footer,
	CollectionLayoutKind Layout,
	IReadOnlyList<SectionEntry> Items) : ISection<SectionEntry>;

internal sealed class SectionsViewModel : ShowcaseViewModel
{
	public IReadOnlyList<CollectionSection> Sections { get; } =
	[
		new(
			"Featured",
			"4 items",
			CollectionLayoutKind.Carousel,
			[
				new("Item 1", "Carousel", true),
				new("Item 2", "Carousel", true),
				new("Item 3", "Carousel", true),
				new("Item 4", "Carousel", true)
			]),
		new(
			"All items",
			"6 items",
			CollectionLayoutKind.List,
			[
				new("Item 5", "List row", false),
				new("Item 6", "List row", false),
				new("Item 7", "List row", false),
				new("Item 8", "List row", false),
				new("Item 9", "List row", false),
				new("Item 10", "List row", false)
			])
	];

	public IReadOnlyList<Span> SectionsCode { get; } =
		Code(
			"""
			CollectionView<SectionEntry, CollectionSection> collection = new()
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

			sealed record SectionEntry(
				string Title,
				string Layout,
				bool IsFeatured);

			sealed record CollectionSection(
				string Title,
				string Footer,
				CollectionLayoutKind Layout,
				IReadOnlyList<SectionEntry> Items) : ISection<SectionEntry>;

			IReadOnlyList<CollectionSection> Sections { get; } =
			[
				new(
					"Featured",
					"4 items",
					CollectionLayoutKind.Carousel,
					[
						new("Item 1", "Carousel", true),
						new("Item 2", "Carousel", true),
						new("Item 3", "Carousel", true),
						new("Item 4", "Carousel", true)
					]),
				new(
					"All items",
					"6 items",
					CollectionLayoutKind.List,
					[
						new("Item 5", "List row", false),
						new("Item 6", "List row", false),
						new("Item 7", "List row", false),
						new("Item 8", "List row", false),
						new("Item 9", "List row", false),
						new("Item 10", "List row", false)
					])
			];

			sealed class SectionCell : ItemView<SectionEntry>
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

				protected override void OnItemChanged(SectionEntry? item)
				{
					container.Background = item?.IsFeatured is true
						? Colors.Teal.WithAlpha(0.14)
						: null;
					container.CornerRadius = item?.IsFeatured is true ? 14 : 0;
				}
			}

			sealed class CollectionHeader : ItemView<CollectionSection>
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

			sealed class CollectionFooter : ItemView<CollectionSection>
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
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
