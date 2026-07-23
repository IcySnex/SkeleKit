namespace SkeleKit;

/// <summary>
/// A grid placing children into cells of <see cref="Rows"/> and <see cref="Columns"/> (absolute, auto, or star).
/// </summary>
public class Grid : Panel
{
	static readonly GridLength[] SingleStar = [GridLength.Star];


	static IReadOnlyList<GridLength> EffectiveTracks(
		List<GridLength> declared) =>
		declared.Count > 0 ? declared : SingleStar;

	static (int Start, int Span) AxisPlacement(
		View child,
		bool horizontal,
		int trackCount)
	{
		GridChild placement = child.LayoutParams as GridChild ?? GridChild.Default;

		int start = horizontal ? placement.Column : placement.Row;
		int span = horizontal ? placement.ColumnSpan : placement.RowSpan;

		return (Math.Clamp(start, 0, Math.Max(0, trackCount - 1)), Math.Max(1, span));
	}

	static GridChild PlacementOf(
		View child,
		int columnCount,
		int rowCount)
	{
		GridChild placement = child.LayoutParams as GridChild ?? GridChild.Default;

		return new()
		{
			Row = Math.Clamp(placement.Row, 0, Math.Max(0, rowCount - 1)),
			Column = Math.Clamp(placement.Column, 0, Math.Max(0, columnCount - 1)),
			RowSpan = Math.Max(1, placement.RowSpan),
			ColumnSpan = Math.Max(1, placement.ColumnSpan)
		};
	}

	static double SpanSize(
		double[] sizes,
		int start,
		int span,
		double spacing)
	{
		double total = 0;
		int end = Math.Min(sizes.Length, start + span);
		for (int i = start; i < end; i++)
			total += sizes[i];

		total += spacing * Math.Max(0, Math.Min(span, sizes.Length - start) - 1);
		return total;
	}

	static double[] Offsets(
		double[] sizes,
		double spacing)
	{
		double[] offsets = new double[sizes.Length];
		double running = 0;
		for (int i = 0; i < sizes.Length; i++)
		{
			offsets[i] = running;
			running += sizes[i] + spacing;
		}

		return offsets;
	}

	static double Sum(
		double[] values)
	{
		double total = 0;
		foreach (double value in values)
			total += value;

		return total;
	}


	// resolved tracks, shared measure -> arrange
	double[] columnWidths = [];
	double[] rowHeights = [];


	/// <summary>
	/// The row definitions, top to bottom.
	/// </summary>
	/// <remarks>
	/// Empty means a single star row.
	/// </remarks>
	public List<GridLength> Rows { get; } = [];

	/// <summary>
	/// The column definitions, leading to trailing.
	/// </summary>
	/// <remarks>
	/// Empty means a single star column.
	/// </remarks>
	public List<GridLength> Columns { get; } = [];

	/// <summary>
	/// The gap in points inserted between rows.
	/// </summary>
	public double RowSpacing { get; set; }

	/// <summary>
	/// The gap in points inserted between columns.
	/// </summary>
	public double ColumnSpacing { get; set; }


	protected override Size MeasureOverride(
		Size availableSize)
	{
		availableSize = availableSize.Deflate(Padding);

		IReadOnlyList<GridLength> columns = EffectiveTracks(Columns);
		IReadOnlyList<GridLength> rows = EffectiveTracks(Rows);

		double columnGaps = ColumnSpacing * (columns.Count - 1);
		double rowGaps = RowSpacing * (rows.Count - 1);

		columnWidths = ResolveTracks(columns, availableSize.Width - columnGaps, horizontal: true);
		rowHeights = ResolveTracks(rows, availableSize.Height - rowGaps, horizontal: false);

		// remeasure at final cell size so arrange has DesiredSize
		foreach (View child in Children)
		{
			GridChild placement = PlacementOf(child, columns.Count, rows.Count);
			Size cell = new(
				SpanSize(columnWidths, placement.Column, placement.ColumnSpan, ColumnSpacing),
				SpanSize(rowHeights, placement.Row, placement.RowSpan, RowSpacing));

			child.Measure(cell);
		}

		return new Size(
			Sum(columnWidths) + columnGaps,
			Sum(rowHeights) + rowGaps).Inflate(Padding);
	}

	protected override Size ArrangeOverride(
		Size finalSize)
	{
		IReadOnlyList<GridLength> columns = EffectiveTracks(Columns);
		IReadOnlyList<GridLength> rows = EffectiveTracks(Rows);

		double[] columnOffsets = Offsets(columnWidths, ColumnSpacing);
		double[] rowOffsets = Offsets(rowHeights, RowSpacing);

		foreach (View child in Children)
		{
			GridChild placement = PlacementOf(child, columns.Count, rows.Count);

			Rect cell = new(
				Padding.Left + columnOffsets[placement.Column],
				Padding.Top + rowOffsets[placement.Row],
				SpanSize(columnWidths, placement.Column, placement.ColumnSpan, ColumnSpacing),
				SpanSize(rowHeights, placement.Row, placement.RowSpan, RowSpacing));

			child.Arrange(cell);
		}

		return finalSize;
	}


	double[] ResolveTracks(
		IReadOnlyList<GridLength> tracks,
		double available,
		bool horizontal)
	{
		// one axis: absolute = value, auto = fit children, star = split rest

		double[] sizes = new double[tracks.Count];
		double used = 0;
		double totalStars = 0;

		for (int i = 0; i < tracks.Count; i++)
		{
			GridLength track = tracks[i];
			if (track.IsAbsolute)
			{
				sizes[i] = track.Value;
				used += sizes[i];
			}
			else if (track.IsStar)
				totalStars += track.Value;
		}

		// auto: fit single-span children, unconstrained on this axis
		for (int i = 0; i < tracks.Count; i++)
		{
			if (!tracks[i].IsAuto)
				continue;

			double max = 0;
			foreach (View child in Children)
			{
				(int start, int span) = AxisPlacement(child, horizontal, tracks.Count);
				if (start != i || span != 1)
					continue;

				// columns already resolved: constrain width so wrapping content reports real height
				Size probe = horizontal
					? Size.Infinity
					: new(CellWidth(child), double.PositiveInfinity);

				child.Measure(probe);
				max = Math.Max(max, horizontal ? child.DesiredSize.Width : child.DesiredSize.Height);
			}

			sizes[i] = max;
			used += max;
		}

		// star: split remainder by weight
		double remaining = Math.Max(0, available - used);
		if (totalStars > 0)
		{
			for (int i = 0; i < tracks.Count; i++)
			{
				if (tracks[i].IsStar)
					sizes[i] = remaining * tracks[i].Value / totalStars;
			}
		}

		return sizes;
	}

	double CellWidth(
		View child)
	{
		(int start, int span) = AxisPlacement(child, horizontal: true, columnWidths.Length);
		return SpanSize(columnWidths, start, span, ColumnSpacing);
	}
}
