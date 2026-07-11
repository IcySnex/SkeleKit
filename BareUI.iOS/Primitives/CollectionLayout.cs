namespace BareUI;

/// <summary>
/// How a <c>CollectionView</c> arranges its items.
/// </summary>
public enum CollectionLayoutKind
{
	/// <summary>A vertical list of full-width rows.</summary>
	List,

	/// <summary>A vertical grid of equal columns.</summary>
	Grid,

	/// <summary>A horizontally scrolling row.</summary>
	Carousel
}

/// <summary>
/// The layout of a <c>CollectionView</c>: a list, a grid, or a carousel.
/// </summary>
public readonly struct CollectionLayout
{
	/// <summary>
	/// Which arrangement this is.
	/// </summary>
	public CollectionLayoutKind Kind { get; }

	/// <summary>
	/// Columns per row, for a grid.
	/// </summary>
	public int Columns { get; }

	/// <summary>
	/// Gap between items, in points.
	/// </summary>
	public double Spacing { get; }

	/// <summary>
	/// Item width for a carousel, in points.
	/// </summary>
	public double ItemWidth { get; }

	/// <summary>
	/// Whether a list uses the native inset-grouped style.
	/// </summary>
	public bool Grouped { get; }

	CollectionLayout(
		CollectionLayoutKind kind,
		int columns,
		double spacing,
		double itemWidth,
		bool grouped)
	{
		Kind = kind;
		Columns = columns;
		Spacing = spacing;
		ItemWidth = itemWidth;
		Grouped = grouped;
	}


	/// <summary>
	/// A list of full-width rows; <paramref name="grouped"/> uses the native inset-grouped style.
	/// </summary>
	public static CollectionLayout List(
		bool grouped = false) =>
		new(CollectionLayoutKind.List, 1, 0, 0, grouped);

	/// <summary>
	/// A grid of equal columns.
	/// </summary>
	public static CollectionLayout Grid(
		int columns,
		double spacing = 8) =>
		new(CollectionLayoutKind.Grid, Math.Max(1, columns), spacing, 0, false);

	/// <summary>
	/// A horizontally scrolling row of fixed-width items.
	/// </summary>
	public static CollectionLayout Carousel(
		double itemWidth,
		double spacing = 8) =>
		new(CollectionLayoutKind.Carousel, 1, spacing, itemWidth, false);
}
