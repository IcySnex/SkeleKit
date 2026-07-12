namespace BareUI;

internal sealed class GridChild
{
	public static readonly GridChild Default = new();

	public int Row { get; set; } = 0;
	public int Column { get; set; } = 0;
	public int RowSpan { get; set; } = 1;
	public int ColumnSpan { get; set; } = 1;
}

/// <summary>
/// Fluent attached-property setters for placing a view inside a <see cref="Grid"/>.
/// </summary>
public static class GridExtensions
{
	static GridChild Placement(
		View view) =>
		view.LayoutParams as GridChild ?? (GridChild)(view.LayoutParams = new GridChild());

	/// <param name="view">The view target being placed.</param>
	/// <typeparam name="T">The type of the view.</typeparam>
	extension<T>(
		T view) where T : View
	{
		/// <summary>
		/// Places the view in grid row <paramref name="row"/> (zero-based).
		/// </summary>
		/// <param name="row">The row index allocation.</param>
		/// <returns>The view instance for fluent layout chaining.</returns>
		public T Row(
			int row)
		{
			Placement(view).Row = row;
			return view;
		}

		/// <summary>
		/// Places the view in grid column <paramref name="column"/> (zero-based).
		/// </summary>
		/// <param name="column">The column index allocation.</param>
		/// <returns>The view instance for fluent layout chaining.</returns>
		public T Column(
			int column)
		{
			Placement(view).Column = column;
			return view;
		}

		/// <summary>
		/// Makes the view span <paramref name="span"/> rows.
		/// </summary>
		/// <param name="span">The total number of rows to bridge across.</param>
		/// <returns>The view instance for fluent layout chaining.</returns>
		public T RowSpan(
			int span)
		{
			Placement(view).RowSpan = span;
			return view;
		}

		/// <summary>
		/// Makes the view span <paramref name="span"/> columns.
		/// </summary>
		/// <param name="span">The total number of columns to bridge across.</param>
		/// <returns>The view instance for fluent layout chaining.</returns>
		public T ColumnSpan(
			int span)
		{
			Placement(view).ColumnSpan = span;
			return view;
		}
	}
}
