namespace BareUI;

/// <summary>
/// Per-child grid placement, stored in <see cref="View.LayoutParams"/>.
/// </summary>
sealed class GridChild
{
	public int Row { get; set; } = 0;
	public int Column { get; set; } = 0;
	public int RowSpan { get; set; } = 1;
	public int ColumnSpan { get; set; } = 1;

	/// <summary>
	/// Placement for a child with no explicit grid attributes: cell (0, 0), span 1×1.
	/// </summary>
	public static readonly GridChild Default = new();
}

/// <summary>
/// Fluent attached-property setters for placing a view inside a <see cref="Grid"/>.
/// </summary>
public static class GridExtensions
{
	static GridChild Placement(
		View view) =>
		view.LayoutParams as GridChild ?? (GridChild)(view.LayoutParams = new GridChild());


	/// <summary>
	/// Places the view in grid row <paramref name="row"/> (zero-based).
	/// </summary>
	public static T Row<T>(
		this T view,
		int row) where T : View
	{
		Placement(view).Row = row;
		return view;
	}

	/// <summary>
	/// Places the view in grid column <paramref name="column"/> (zero-based).
	/// </summary>
	public static T Column<T>(
		this T view,
		int column) where T : View
	{
		Placement(view).Column = column;
		return view;
	}

	/// <summary>
	/// Makes the view span <paramref name="span"/> rows.
	/// </summary>
	public static T RowSpan<T>(
		this T view,
		int span) where T : View
	{
		Placement(view).RowSpan = span;
		return view;
	}

	/// <summary>
	/// Makes the view span <paramref name="span"/> columns.
	/// </summary>
	public static T ColumnSpan<T>(
		this T view,
		int span) where T : View
	{
		Placement(view).ColumnSpan = span;
		return view;
	}
}
