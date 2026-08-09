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
				Height = 20,
				Text = "0",
				TextStyle = TextStyle.Subheadline,
				FontDesign = FontDesign.Monospaced,
				TextAlignment = TextAlignment.Trailing,
				TextColor = Colors.SecondaryLabel
			};

			ScrollView scroll = new()
			{
				Width = 280,
				Height = 220,
				Padding = 12,
				ShowsIndicator = {{Boolean(ShowsIndicator)}},
				Scrolled = value => offset.Text = value.ToString("0"),

				Content = new StackPanel
				{
					Spacing = 8,
					Children =
					{
						Row("Item 1"),
						Row("Item 2"),
						Row("Item 3"),
						Row("Item 4"),
						Row("Item 5"),
						Row("Item 6"),
						Row("Item 7")
					}
				}
			};

			static Border Row(string text) =>
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
			""");

	public IReadOnlyList<Span> PagingCode =>
		Code(
			$$"""
			ScrollView pager = new()
			{
				Width = 280,
				Height = 170,
				Orientation = Orientation.Horizontal,
				Paging = {{Boolean(Paging)}},
				ShowsIndicator = false,
				CornerRadius = 18,

				Content = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Children =
					{
						Page("Page 1", 0.12),
						Page("Page 2", 0.18),
						Page("Page 3", 0.24)
					}
				}
			};

			static Border Page(string text, double alpha) =>
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
