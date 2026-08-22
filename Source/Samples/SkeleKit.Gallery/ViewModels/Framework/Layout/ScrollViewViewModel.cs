using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Layout;

internal sealed partial class ScrollViewViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(OffsetLabel))]
	double scrollOffset;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(VerticalCode))]
	bool showsIndicator = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PagingCode))]
	bool paging = true;


	public string OffsetLabel =>
		Number(ScrollOffset);

	public IReadOnlyList<Span> VerticalCode =>
		Code(
			$$"""
			Label offset = new()
			{
				Width = 64,
				Text = "0",
				FontDesign = FontDesign.Monospaced,
				TextAlignment = TextAlignment.Trailing
			};
			StackPanel items = new() { Spacing = 8 };

			for (int index = 1; index <= 7; index++)
			{
				items.Children.Add(new Label
				{
					Height = 50,
					Text = $"Item {index}"
				});
			}

			ScrollView scroll = new()
			{
				Width = 280,
				Height = 220,
				Padding = 12,
				ShowsIndicator = {{Boolean(ShowsIndicator)}},
				Scrolled = value => offset.Text = value.ToString("0"),
				Content = items
			};
			""");

	public IReadOnlyList<Span> PagingCode =>
		Code(
			$$"""
			StackPanel pages = new()
			{
				Orientation = Orientation.Horizontal
			};

			for (int index = 1; index <= 3; index++)
			{
				pages.Children.Add(new Border
				{
					Width = 280,
					Height = 170,
					Child = new Label { Text = $"Page {index}" }
				});
			}

			ScrollView pager = new()
			{
				Width = 280,
				Height = 170,
				Orientation = Orientation.Horizontal,
				Paging = {{Boolean(Paging)}},
				ShowsIndicator = false,
				CornerRadius = 18,
				Content = pages
			};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Boolean(
		bool value) =>
		value ? "true" : "false";

	static string Number(
		double value) =>
		value.ToString("0", CultureInfo.InvariantCulture);
}
