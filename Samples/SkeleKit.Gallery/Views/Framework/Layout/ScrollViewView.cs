using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Layout;

[Page]
internal sealed class ScrollViewView : ShowcaseView<ScrollViewViewModel>
{
	public ScrollViewView(
		ScrollViewViewModel viewModel) : base(viewModel, "ScrollView", Colors.Blue)
	{
		AddVerticalShowcase(viewModel);
		AddPagingShowcase(viewModel);
	}


	void AddVerticalShowcase(
		ScrollViewViewModel viewModel)
	{
		StackPanel rows = new()
		{
			Spacing = 8
		};

		for (int index = 1; index <= 7; index++)
			rows.Children.Add(Row($"Item {index}"));

		ScrollView scroll = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 280,
			Height = 220,
			Padding = 12,
			ShowsIndicator = viewModel.ShowsIndicator,
			Scrolled = value => viewModel.ScrollOffset = value,
			Content = rows
		};

		Switch indicator = new()
		{
			IsOn = Bind(
				model => model.ShowsIndicator,
				static (model, value) => model.ShowsIndicator = value),
			Toggled = value => scroll.ShowsIndicator = value
		};

		AddShowcase(
			"Vertical scrolling",
			"Drag the list to see its current offset and optionally hide the scroll indicator.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(scroll, 280),
				SettingRow(
					"Offset",
					new Label
					{
						Width = 64,
						Height = 20,
						VerticalAlignment = VerticalAlignment.Center,
						Text = Bind(model => model.OffsetLabel),
						TextStyle = TextStyle.Subheadline,
						FontDesign = FontDesign.Monospaced,
						TextAlignment = TextAlignment.Trailing,
						TextColor = Colors.SecondaryLabel
					}),
				SettingRow("Scroll indicator", indicator)),
			ShowcaseBox.Code(Bind(model => model.VerticalCode)));
	}

	void AddPagingShowcase(
		ScrollViewViewModel viewModel)
	{
		StackPanel pages = new()
		{
			Orientation = Orientation.Horizontal,

			Children =
			{
				Page("Page 1", 0.12),
				Page("Page 2", 0.18),
				Page("Page 3", 0.24)
			}
		};

		ScrollView pager = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 280,
			Height = 170,
			Orientation = Orientation.Horizontal,
			Paging = viewModel.Paging,
			ShowsIndicator = false,
			CornerRadius = 18,
			Content = pages
		};

		Switch paging = new()
		{
			IsOn = Bind(
				model => model.Paging,
				static (model, value) => model.Paging = value),
			Toggled = value => pager.Paging = value
		};

		AddShowcase(
			"Horizontal paging",
			"Swipe between viewport-sized pages, then turn snapping off to compare free scrolling.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(pager, 230),
				SettingRow("Snap to pages", paging)),
			ShowcaseBox.Code(Bind(model => model.PagingCode)));
	}


	static Border Row(
		string text) =>
		new()
		{
			Height = 50,
			Background = Colors.Blue.WithAlpha(0.14),
			CornerRadius = 12,

			Child = new Label
			{
				Margin = new Thickness(14, 0),
				VerticalAlignment = VerticalAlignment.Center,
				Text = text,
				TextStyle = TextStyle.Subheadline,
				FontWeight = FontWeight.Medium,
				TextColor = Colors.Blue
			}
		};

	static Border Page(
		string text,
		double alpha) =>
		new()
		{
			Width = 280,
			Height = 170,
			Background = Colors.Blue.WithAlpha(alpha),

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = text,
				TextStyle = TextStyle.Title2,
				FontWeight = FontWeight.Bold,
				TextColor = Colors.Blue
			}
		};
}
