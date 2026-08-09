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
			Border carouselHost = new();

			CollectionView<CarouselEntry> CreateCarousel(
				CarouselSnap snap) =>
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

			carouselHost.Child = CreateCarousel(SnapFor(viewModel.SnapIndex));

			SegmentedControl snapping = new()
			{
				SelectedIndex = Bind(
					model => model.SnapIndex,
					static (model, value) => model.SnapIndex = value),
				SelectionChanged = index =>
					carouselHost.Child = CreateCarousel(SnapFor(index))
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

			ObservableCollection<CarouselEntry> Items { get; } =
			[
				.. Enumerable.Range(1, 8).Select(
					index => new CarouselEntry(index.ToString("00"), $"Card {index}"))
			];

			sealed class CarouselCell : ItemView<CarouselEntry>
			{
				public CarouselCell() =>
					Content = new Border
					{
						Height = 320,
						VerticalAlignment = VerticalAlignment.Center,
						Background = Colors.Teal.WithAlpha(0.14),
						CornerRadius = 24,
						Child = new StackPanel
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Spacing = 4,
							Children =
							{
								new Label
								{
									Text = Bind(item => item.Number),
									TextStyle = TextStyle.LargeTitle,
									FontWeight = FontWeight.Bold,
									TextColor = Colors.Teal
								},
								new Label
								{
									Text = Bind(item => item.Title),
									TextStyle = TextStyle.Body,
									TextColor = Colors.SecondaryLabel
								}
							}
						}
					};
			}
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
