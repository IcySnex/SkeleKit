using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record CarouselEntry(
	string Number,
	string Title);

internal sealed partial class CarouselsViewModel : ShowcaseViewModel
{
	public ObservableCollection<CarouselEntry> Items { get; } =
	[
		.. Enumerable.Range(1, 8).Select(
			index => new CarouselEntry(index.ToString("00"), $"Card {index}"))
	];

	[ObservableProperty]
	int snapIndex = 1;

	public IReadOnlyList<Span> CarouselCode { get; } =
		Code(
			"""
			CollectionView<CarouselEntry> CreateCarousel(CarouselSnap snap) =>
				new()
				{
					ItemsSource = viewModel.Items,
					ItemTemplate = static () => new CarouselCell(),
					Layout = CollectionLayout.Carousel(
						itemWidth: 260,
						spacing: 16,
						snap: snap),
					HighlightsSelection = false
				};

			Border host = new()
			{
				Child = CreateCarousel(CarouselSnap.ItemPeek)
			};

			SegmentedControl snapping = new()
			{
				SelectionChanged = index =>
					host.Child = CreateCarousel(SnapFor(index))
			};
			snapping.Items.Add("Free");
			snapping.Items.Add("Peek");
			snapping.Items.Add("Centered");

			CarouselSnap SnapFor(int index) =>
				index switch
				{
					0 => CarouselSnap.None,
					2 => CarouselSnap.ItemCentered,
					_ => CarouselSnap.ItemPeek
				};

			sealed record CarouselEntry(string Number, string Title);

			sealed class CarouselCell : ItemView<CarouselEntry>
			{
				public CarouselCell() =>
					Content = new StackPanel
					{
						Children =
						{
							new Label { Text = Bind(item => item.Number) },
							new Label { Text = Bind(item => item.Title) }
						}
					};
			}
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
