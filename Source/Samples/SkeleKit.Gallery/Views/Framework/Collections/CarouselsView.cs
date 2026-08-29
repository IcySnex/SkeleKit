using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Collections;

[Page]
internal sealed class CarouselsView : ShowcaseView<CarouselsViewModel>
{
	public CarouselsView(
		CarouselsViewModel viewModel) : base(viewModel, "Carousels", Colors.Teal)
	{
		AddCodePage("Carousels code", () => viewModel.CarouselCode);

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
			SelectedIndex = Bind(vm => vm.SnapIndex)
				.TwoWay((vm, val) => vm.SnapIndex = val),
			SelectionChanged = index =>
				carouselHost.Child = CreateCarousel(SnapFor(index))
		};
		snapping.Items.Add("Free");
		snapping.Items.Add("Peek");
		snapping.Items.Add("Centered");

		Content = new Grid
		{
			Rows =
			{
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				carouselHost,

				new Border
				{
					Padding = new(16, 14, 16, 16),
					Background = Colors.SecondaryGroupedBackground,

					Child = new StackPanel
					{
						Spacing = 8,

						Children =
						{
							new Label
							{
								Text = "Snapping",
								TextStyle = TextStyle.Subheadline,
								FontWeight = FontWeight.Semibold
							},

							snapping
						}
					}
				}.Row(1)
			}
		};
	}


	static CarouselSnap SnapFor(
		int index) =>
		index switch
		{
			0 => CarouselSnap.None,
			2 => CarouselSnap.ItemCentered,
			_ => CarouselSnap.ItemPeek
		};
}

internal sealed class CarouselCell : ItemView<CarouselEntry>
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
