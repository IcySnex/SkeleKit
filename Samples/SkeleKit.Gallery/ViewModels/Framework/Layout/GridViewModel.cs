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
	[NotifyPropertyChangedFor(nameof(MaxSpan))]
	[NotifyPropertyChangedFor(nameof(SpanCount))]
	[NotifyPropertyChangedFor(nameof(SpanLabel))]
	[NotifyPropertyChangedFor(nameof(GridCode))]
	int columnIndex;


	internal int MaxSpan =>
		3 - Math.Clamp(ColumnIndex, 0, 2);

	internal int SpanCount =>
		Math.Clamp(SpanIndex + 1, 1, MaxSpan);

	public string FixedWidthLabel =>
		$"{Number(FixedColumnWidth)} pt";

	public string SpanLabel =>
		$"Span {SpanCount}";

	partial void OnColumnIndexChanged(
		int value)
	{
		int maxSpanIndex = 2 - Math.Clamp(value, 0, 2);

		if (SpanIndex > maxSpanIndex)
			SpanIndex = maxSpanIndex;
	}

	public IReadOnlyList<Span> SimpleGridCode { get; } =
		Code(
			"""
			Grid grid = new()
			{
				Width = 280,
				Height = 280,
				ColumnSpacing = 6,
				RowSpacing = 6,
				Columns =
				{
					GridLength.Star, GridLength.Star, GridLength.Star,
					GridLength.Star, GridLength.Star
				},
				Rows =
				{
					GridLength.Star, GridLength.Star, GridLength.Star,
					GridLength.Star, GridLength.Star
				}
			};

			for (int index = 0; index < 25; index++)
			{
				grid.Children.Add(
					new Border
				{
						Child = new Label { Text = (index + 1).ToString() }
					}
					.Row(index / 5)
					.Column(index % 5));
			}
			""");

	public IReadOnlyList<Span> GridCode =>
		Code(
			$$"""
			int column = {{ColumnIndex}};
			int span = {{SpanCount}};

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
					GridLength.Star
				},
				Rows =
				{
					GridLength.Auto,
					GridLength.Star
				},
				Children =
				{
					new Label { Text = "Auto" }.Column(0).Row(0),
					new Label { Text = "{{FixedWidthLabel}}" }.Column(1).Row(0),
					new Label { Text = "Star" }.Column(2).Row(0),
					new Border { Child = new Label { Text = "{{SpanLabel}}" } }
						.Column(column).Row(1).ColumnSpan(span)
				}
			};

			int maximumSpan = grid.Columns.Count - column;
			SegmentedControl spanPicker = new();
			for (int value = 1; value <= maximumSpan; value++)
				spanPicker.Items.Add(value.ToString());
			""");


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Number(
		double value) =>
		value.ToString("0.##", CultureInfo.InvariantCulture);
}
