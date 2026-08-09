using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.Views.Showcase;

namespace SkeleKit.Gallery.Views.Framework.Layout;

[Page]
internal sealed class GridView : ShowcaseView<GridViewModel>
{
	public GridView(
		GridViewModel viewModel) : base(viewModel, "Grid", Colors.Blue)
	{
		AddSimpleGridShowcase(viewModel);
		AddGridShowcase(viewModel);
	}


	void AddSimpleGridShowcase(
		GridViewModel viewModel)
	{
		Grid grid = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
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
					SimpleCell(row * 5 + column + 1)
						.Row(row)
						.Column(column));
			}
		}

		AddShowcase(
			"Rows & columns",
			"Place 25 cells in five equal rows and five equal columns.",
			ShowcaseBox.Canvas(grid, 320),
			ShowcaseBox.Code(Bind(model => model.SimpleGridCode)));
	}

	void AddGridShowcase(
		GridViewModel viewModel)
	{
		Border spanningCell = Cell(
			Bind(model => model.SpanLabel),
			filled: true)
			.Column(viewModel.ColumnIndex)
			.Row(1)
			.ColumnSpan(viewModel.SpanCount);

		Grid grid = new()
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Width = 300,
			Height = 156,
			Padding = 10,
			ColumnSpacing = 8,
			RowSpacing = 8,
			Background = Colors.Blue.WithAlpha(0.08),
			CornerRadius = 16,
			Columns =
			{
				GridLength.Auto,
				viewModel.FixedColumnWidth,
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
				Cell(Bind(model => model.FixedWidthLabel)).Column(1).Row(0),
				Cell("Star").Column(2).Row(0),
				spanningCell
			}
		};

		Slider fixedWidth = new()
		{
			Minimum = 56,
			Maximum = 96,
			Step = 4,
			Value = Bind(
				model => model.FixedColumnWidth,
				static (model, value) => model.FixedColumnWidth = value),
			ValueChanged = value => grid.Columns[1] = value
		};

		Border spanHost = new();

		SegmentedControl CreateSpanControl()
		{
			SegmentedControl control = new()
			{
				SelectedIndex = Bind(
					model => model.SpanIndex,
					static (model, value) => model.SpanIndex = value),
				SelectionChanged = index =>
				{
					spanningCell.ColumnSpan(index + 1);
					grid.InvalidateMeasure();
				}
			};

			for (int value = 1; value <= viewModel.MaxSpan; value++)
				control.Items.Add(value.ToString());

			return control;
		}

		spanHost.Child = CreateSpanControl();

		SegmentedControl column = new()
		{
			SelectedIndex = Bind(
				model => model.ColumnIndex,
				static (model, value) => model.ColumnIndex = value),
			SelectionChanged = index =>
			{
				spanningCell
					.Column(index)
					.ColumnSpan(viewModel.SpanCount);
				grid.InvalidateMeasure();
				spanHost.Child = CreateSpanControl();
			}
		};
		column.Items.Add("0");
		column.Items.Add("1");
		column.Items.Add("2");

		AddShowcase(
			"Tracks & span",
			"Compare track sizes, then change the lower cell's starting column index and span.",
			PreviewWithSettings(
				ShowcaseBox.Canvas(grid, 220),
				LabeledSlider("Fixed column", Bind(model => model.FixedWidthLabel), fixedWidth),
				LabeledControl("Column index", column),
				LabeledControl("Column span", spanHost)),
			ShowcaseBox.Code(Bind(model => model.GridCode)));
	}


	static Border Cell(
		Bindable<string?> text,
		bool filled = false) =>
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
				TextStyle = TextStyle.Subheadline,
				FontWeight = FontWeight.Semibold,
				TextColor = filled ? Colors.White : Colors.Blue,
				MaxLines = 1
			}
		};

	static Border SimpleCell(
		int number) =>
		new()
		{
			Background = Colors.Blue.WithAlpha(0.18),
			CornerRadius = 8,

			Child = new Label
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = number.ToString(),
				TextStyle = TextStyle.Footnote,
				FontWeight = FontWeight.Semibold,
				TextColor = Colors.Blue
			}
		};
}
