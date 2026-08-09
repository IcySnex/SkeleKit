using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Layout;

internal sealed partial class GridViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(FixedWidthLabel))]
	[NotifyPropertyChangedFor(nameof(GridCode))]
	double fixedColumnWidth = 72;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(SpanCount))]
	[NotifyPropertyChangedFor(nameof(SpanLabel))]
	[NotifyPropertyChangedFor(nameof(GridCode))]
	int spanIndex = 1;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(GridCode))]
	int columnIndex;


	internal int SpanCount =>
		Math.Clamp(SpanIndex + 1, 1, 3);

	public string FixedWidthLabel =>
		$"{Number(FixedColumnWidth)} pt";

	public string SpanLabel =>
		$"Span {SpanCount}";

	public IReadOnlyList<Span> SimpleGridCode { get; } =
		Code(
			"""
			Grid grid = new()
			{
				Width = 280,
				Height = 280,
				ColumnSpacing = 6,
				RowSpacing = 6
			};

			for (int index = 0; index < 5; index++)
			{
				grid.Columns.Add(GridLength.Star);
				grid.Rows.Add(GridLength.Star);
			}

			for (int row = 0; row < 5; row++)
			{
				for (int column = 0; column < 5; column++)
				{
					grid.Children.Add(
						Cell(row * 5 + column + 1)
							.Row(row)
							.Column(column));
				}
			}

			static Border SimpleCell(int number) =>
				new()
				{
					Background = Colors.Blue.WithAlpha(0.18),
					CornerRadius = 8,
					Child = new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = number.ToString(),
						TextColor = Colors.Blue
					}
				};
			""");

	public IReadOnlyList<Span> GridCode =>
		Code(
			$$"""
			Grid grid = new()
			{
				Width = 300,
				Height = 156,
				Padding = 10,
				ColumnSpacing = 8,
				RowSpacing = 8,
				Columns =
				{
					GridLength.Auto,
					{{Number(FixedColumnWidth)}},
					GridLength.Star,
					GridLength.Star,
					GridLength.Star
				},
				Rows =
				{
					GridLength.Auto,
					GridLength.Star
				},
				Children =
				{
					Cell("Auto").Column(0).Row(0),
					Cell("{{FixedWidthLabel}}").Column(1).Row(0),
					Cell("Star").Column(2).Row(0).ColumnSpan(3),
					Cell("{{SpanLabel}}", filled: true)
						.Column({{ColumnIndex}}).Row(1).ColumnSpan({{SpanCount}})
				}
			};

			static Border Cell(string text, bool filled = false) =>
				new()
				{
					Background = filled
						? Colors.Blue
						: Colors.Blue.WithAlpha(0.18),
					CornerRadius = 10,
					Child = new Label
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Text = text,
						TextColor = filled ? Colors.White : Colors.Blue
					}
				};
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
