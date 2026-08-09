using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record SectionEntry(
	string Title,
	string Layout,
	bool IsFeatured);

internal sealed record CollectionSection(
	string Title,
	CollectionLayoutKind Layout,
	IReadOnlyList<SectionEntry> Items) : ISection<SectionEntry>;

internal sealed class SectionsViewModel : ShowcaseViewModel
{
	public IReadOnlyList<CollectionSection> Sections { get; } =
	[
		new("Featured", CollectionLayoutKind.Carousel, Entries(1, 6, "Carousel item", true)),
		new("Recent", CollectionLayoutKind.List, Entries(7, 7, "List item", false)),
		new("Recommended", CollectionLayoutKind.Carousel, Entries(14, 6, "Carousel item", true)),
		new("Archive", CollectionLayoutKind.List, Entries(20, 12, "List item", false))
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
						spacing: 12,
						snap: CarouselSnap.ItemPeek),
					_ => CollectionLayout.List()
				},
				HighlightsSelection = false,
				ShowsSeparators = false
			};

			sealed record SectionEntry(
				string Title,
				string Layout,
				bool IsFeatured);

			sealed record CollectionSection(
				string Title,
				CollectionLayoutKind Layout,
				IReadOnlyList<SectionEntry> Items) : ISection<SectionEntry>;

			IReadOnlyList<CollectionSection> Sections { get; } =
			[
				new("Featured", CollectionLayoutKind.Carousel, Entries(1, 6, "Carousel item", true)),
				new("Recent", CollectionLayoutKind.List, Entries(7, 7, "List item", false)),
				new("Recommended", CollectionLayoutKind.Carousel, Entries(14, 6, "Carousel item", true)),
				new("Archive", CollectionLayoutKind.List, Entries(20, 12, "List item", false))
			];

			static IReadOnlyList<SectionEntry> Entries(
				int first,
				int count,
				string layout,
				bool featured) =>
				[
					.. Enumerable.Range(first, count).Select(
						index => new SectionEntry($"Item {index}", layout, featured))
				];

			sealed class SectionCell : ItemView<SectionEntry>
			{
				readonly Border container;

				public SectionCell()
				{
					container = new()
					{
						Height = 72,
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
					bool featured = item?.IsFeatured is true;
					container.Height = featured ? 76 : 64;
					container.Margin = featured ? Thickness.Zero : new(16, 3);
					container.Background = featured
						? Colors.Teal.WithAlpha(0.14)
						: Colors.SecondaryGroupedBackground;
					container.CornerRadius = 14;
				}
			}

			sealed class CollectionHeader : ItemView<CollectionSection>
			{
				readonly Label label;

				public CollectionHeader()
				{
					label = new()
					{
						Text = Bind(section => section.Title),
						TextStyle = TextStyle.Headline,
						FontWeight = FontWeight.Semibold
					};
					Content = label;
				}

				protected override void OnItemChanged(CollectionSection? section) =>
					label.Margin = section?.Layout is CollectionLayoutKind.Carousel
						? new(0, 8, 0, 5)
						: new(16, 8, 16, 5);
			}

			sealed class CollectionFooter : ItemView<CollectionSection>
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

				protected override void OnItemChanged(CollectionSection? section) =>
					label.Margin = section?.Layout is CollectionLayoutKind.Carousel
						? new(0, 3, 0, 3)
						: new(16, 3, 16, 3);
			}
			""");


	static IReadOnlyList<SectionEntry> Entries(
		int first,
		int count,
		string layout,
		bool featured) =>
		[
			.. Enumerable.Range(first, count).Select(
				index => new SectionEntry($"Item {index}", layout, featured))
		];

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
