namespace SkeleKit;

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
	/// Like <see cref="LeadingBoundary"/>, but leaves a small slice of the previous item visible.
	/// </summary>
	LeadingBoundaryPeek,

	/// <summary>
	/// Settles on an item, leading edge aligned.
	/// </summary>
	Item,

	/// <summary>
	/// Like <see cref="Item"/>, but leaves a small slice of the previous item visible.
	/// </summary>
	ItemPeek,

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
	/// <param name="grouped">Whether to use the native inset-grouped style.</param>
	/// <returns>The list layout.</returns>
	public static CollectionLayout List(
		bool grouped = false) =>
		new(CollectionLayoutKind.List, 1, 0, 0, null, grouped, CarouselSnap.None, false, null);

	/// <summary>
	/// A grid of equal columns.
	/// </summary>
	/// <param name="columns">The number of columns per row.</param>
	/// <param name="spacing">The gap between items, in points.</param>
	/// <param name="itemAspectRatio">The item width divided by its height, or null to measure the item template.</param>
	/// <param name="outerSpacing">The spacing between the grid and the collection's edges, or null to match <paramref name="spacing"/>.</param>
	/// <returns>The grid layout.</returns>
	public static CollectionLayout Grid(
		int columns,
		double spacing = 8,
		double? itemAspectRatio = null,
		Thickness? outerSpacing = null)
	{
		ValidateAspectRatio(itemAspectRatio);
		ValidateOuterSpacing(outerSpacing);

		return new(CollectionLayoutKind.Grid, Math.Max(1, columns), spacing, 0, itemAspectRatio, false, CarouselSnap.None, false, outerSpacing);
	}

	/// <summary>
	/// A grid with fixed geometry that reads only the visible sections.
	/// </summary>
	/// <param name="columns">The number of columns per row.</param>
	/// <param name="spacing">The gap between items, in points.</param>
	/// <param name="itemAspectRatio">The item width divided by its height, or null to measure the first item template.</param>
	/// <param name="outerSpacing">The spacing between the grid and the collection's edges, or null to match <paramref name="spacing"/>.</param>
	/// <returns>The fixed grid layout.</returns>
	/// <remarks>
	/// SkeleKit measures the item template, the section header template, and the section footer template
	/// one time at each geometry build. All sections use the measured sizes.
	/// Use this layout with a virtualized source for a large or computed list.
	/// Use <see cref="Grid"/> for self-sizing items.
	/// </remarks>
	public static CollectionLayout FixedGrid(
		int columns,
		double spacing = 8,
		double? itemAspectRatio = null,
		Thickness? outerSpacing = null)
	{
		ValidateAspectRatio(itemAspectRatio);
		ValidateOuterSpacing(outerSpacing);

		return new(CollectionLayoutKind.Grid, Math.Max(1, columns), spacing, 0, itemAspectRatio, false, CarouselSnap.None, true, outerSpacing);
	}

	/// <summary>
	/// A horizontally scrolling row of fixed-width items, optionally snapping as it settles.
	/// </summary>
	/// <param name="itemWidth">The width of each item, in points.</param>
	/// <param name="spacing">The gap between items, in points.</param>
	/// <param name="snap">How scrolling settles when the drag ends.</param>
	/// <returns>The carousel layout.</returns>
	public static CollectionLayout Carousel(
		double itemWidth,
		double spacing = 8,
		CarouselSnap snap = CarouselSnap.None) =>
		new(CollectionLayoutKind.Carousel, 1, spacing, itemWidth, null, false, snap, false, null);


	static void ValidateAspectRatio(
		double? itemAspectRatio)
	{
		if (itemAspectRatio is double ratio && (!double.IsFinite(ratio) || ratio <= 0))
			throw new ArgumentOutOfRangeException(nameof(itemAspectRatio), "An item aspect ratio must be finite and greater than zero.");
	}

	static void ValidateOuterSpacing(
		Thickness? outerSpacing)
	{
		if (outerSpacing is not { } outer)
			return;

		if (!double.IsFinite(outer.Left) || !double.IsFinite(outer.Top)
			|| !double.IsFinite(outer.Right) || !double.IsFinite(outer.Bottom)
			|| outer.Left < 0 || outer.Top < 0 || outer.Right < 0 || outer.Bottom < 0)
			throw new ArgumentOutOfRangeException(nameof(outerSpacing), "Outer spacing values must be finite and non-negative.");
	}


	CollectionLayout(
		CollectionLayoutKind kind,
		int columns,
		double spacing,
		double itemWidth,
		double? itemAspectRatio,
		bool grouped,
		CarouselSnap snap,
		bool isFixedGeometry,
		Thickness? outerSpacing)
	{
		Kind = kind;
		Columns = columns;
		Spacing = spacing;
		ItemWidth = itemWidth;
		ItemAspectRatio = itemAspectRatio;
		Grouped = grouped;
		Snap = snap;
		IsFixedGeometry = isFixedGeometry;
		OuterSpacing = outerSpacing;
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
	/// The spacing between the grid and the collection's edges, or null to match <see cref="Spacing"/>.
	/// </summary>
	public Thickness? OuterSpacing { get; }

	/// <summary>
	/// Item width for a carousel, in points.
	/// </summary>
	public double ItemWidth { get; }

	/// <summary>
	/// Item width divided by item height for a grid, or null to measure the item template.
	/// </summary>
	/// <remarks>
	/// The sizing item's explicit height and minimum or maximum height constrain the derived row height.
	/// </remarks>
	public double? ItemAspectRatio { get; }

	/// <summary>
	/// Whether a list uses the native inset-grouped style.
	/// </summary>
	public bool Grouped { get; }

	/// <summary>
	/// How a carousel settles when the drag ends.
	/// </summary>
	public CarouselSnap Snap { get; }

	/// <summary>
	/// Whether the collection uses the visible-only fixed geometry layout.
	/// </summary>
	/// <remarks>
	/// Set by <see cref="FixedGrid"/>. All other factories return false.
	/// </remarks>
	public bool IsFixedGeometry { get; }
}
