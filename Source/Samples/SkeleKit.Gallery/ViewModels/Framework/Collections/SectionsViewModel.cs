using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record SectionEntry(
	string Title,
	bool IsFeatured);

internal sealed record CollectionSection(
	string Title,
	CollectionLayoutKind Layout,
	IReadOnlyList<SectionEntry> Items) : ISection<SectionEntry>;

internal sealed class SectionsViewModel : ShowcaseViewModel
{
	public IReadOnlyList<CollectionSection> Sections { get; } =
	[
		new("Featured", CollectionLayoutKind.Carousel, Entries(1, 6, true)),
		new("Recent", CollectionLayoutKind.List, Entries(7, 7, false))
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
						itemWidth: 248,
						spacing: 8,
						snap: CarouselSnap.ItemPeek),
					_ => CollectionLayout.List()
				},
				HighlightsSelection = false,
				ShowsSeparators = false
			};

			sealed record SectionEntry(string Title, bool IsFeatured);

			sealed record CollectionSection(
				string Title,
				CollectionLayoutKind Layout,
				IReadOnlyList<SectionEntry> Items) : ISection<SectionEntry>;

			sealed class SectionCell : ItemView<SectionEntry>
			{
				public SectionCell() =>
					Content = new Label
					{
						Text = Bind(item => item.Title)
					};
			}

			sealed class CollectionHeader : ItemView<CollectionSection>
			{
				public CollectionHeader() =>
					Content = new Label
					{
						Text = Bind(section => section.Title)
					};
			}

			sealed class CollectionFooter : ItemView<CollectionSection>
			{
				public CollectionFooter() =>
					Content = new Label
					{
						Text = Bind<IReadOnlyList<SectionEntry>, string>(
							section => section.Items,
							items => $"{items.Count} items")
					};
			}
			""");


	static IReadOnlyList<SectionEntry> Entries(
		int first,
		int count,
		bool featured) =>
		[
			.. Enumerable.Range(first, count).Select(
				index => new SectionEntry($"Item {index}", featured))
		];

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
