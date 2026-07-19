namespace BareUI;

/// <summary>
/// How a <c>CollectionView</c> arranges its items.
/// </summary>
public enum CollectionLayoutKind
{
	/// <summary>
	/// A vertical list of full-width rows.
	/// </summary>
	List,

	/// <summary>
	/// A vertical grid of equal columns.
	/// </summary>
	Grid,

	/// <summary>
	/// A horizontally scrolling row.
	/// </summary>
	Carousel
}

/// <summary>
/// How a carousel settles when the drag ends.
/// </summary>
/// <remarks>
/// Mirrors SwiftUI's scroll target behavior.
/// </remarks>
public enum CarouselSnap
{
	/// <summary>
	/// Free scrolling; stops wherever the drag ends.
	/// </summary>
	None,

	/// <summary>
	/// Free scrolling, but the resting offset lands on an item's leading edge.
	/// </summary>
	LeadingBoundary,

	/// <summary>
	/// Settles on an item, leading edge aligned.
	/// </summary>
	Item,

	/// <summary>
	/// Settles on an item, centered.
	/// </summary>
	ItemCentered,

	/// <summary>
	/// Settles a full page at a time.
	/// </summary>
	Page
}

/// <summary>
/// The layout of a <c>CollectionView</c>: a list, a grid, or a carousel.
/// </summary>
public readonly struct CollectionLayout
{
	/// <summary>
	/// A list of full-width rows; <paramref name="grouped"/> uses the native inset-grouped style.
	/// </summary>
	public static CollectionLayout List(
		bool grouped = false) =>
		new(CollectionLayoutKind.List, 1, 0, 0, grouped, CarouselSnap.None);

	/// <summary>
	/// A grid of equal columns.
	/// </summary>
	public static CollectionLayout Grid(
		int columns,
		double spacing = 8) =>
		new(CollectionLayoutKind.Grid, Math.Max(1, columns), spacing, 0, false, CarouselSnap.None);

	/// <summary>
	/// A horizontally scrolling row of fixed-width items, optionally snapping as it settles.
	/// </summary>
	public static CollectionLayout Carousel(
		double itemWidth,
		double spacing = 8,
		CarouselSnap snap = CarouselSnap.None) =>
		new(CollectionLayoutKind.Carousel, 1, spacing, itemWidth, false, snap);


	CollectionLayout(
		CollectionLayoutKind kind,
		int columns,
		double spacing,
		double itemWidth,
		bool grouped,
		CarouselSnap snap)
	{
		Kind = kind;
		Columns = columns;
		Spacing = spacing;
		ItemWidth = itemWidth;
		Grouped = grouped;
		Snap = snap;
	}


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

	/// <summary>
	/// How a carousel settles when the drag ends.
	/// </summary>
	public CarouselSnap Snap { get; }
}
