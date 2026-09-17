namespace SkeleKit;

/// <summary>
/// Declares the pages hosted by a native split view controller.
/// </summary>
public sealed partial class SplitViewBuilder
{
	internal Dictionary<SplitViewColumn, Type> Columns { get; } = [];
	internal Dictionary<SplitViewColumn, double> ColumnWeights { get; } = [];
	internal SplitViewStyle SplitStyle { get; private set; } = SplitViewStyle.DoubleColumn;
	internal SplitViewColumn NavigationTarget { get; private set; } = SplitViewColumn.Secondary;
	internal SplitViewEdge PrimaryColumnEdge { get; private set; } = SplitViewEdge.Leading;
	internal SplitViewBehavior BehaviorPreference { get; private set; } = SplitViewBehavior.Automatic;
	internal SplitViewDisplay DisplayPreference { get; private set; } = SplitViewDisplay.Automatic;


	/// <summary>
	/// Selects the split view's two- or three-column arrangement.
	/// </summary>
	/// <param name="style">The arrangement to create.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder Style(
		SplitViewStyle style)
	{
		SplitStyle = style;

		return this;
	}

	/// <summary>
	/// Adds or replaces a page in a split view column.
	/// </summary>
	/// <typeparam name="TView">The page type hosted by the column.</typeparam>
	/// <param name="column">The column to configure.</param>
	/// <param name="weight">
	/// The column's relative share of the available width. Once any expanded column supplies a weight,
	/// columns without one use a weight of <c>1</c>.
	/// </param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder Column<TView>(
		SplitViewColumn column,
		double? weight = null) where TView : ContentView
	{
		if (column is SplitViewColumn.Compact && weight is not null)
			throw new ArgumentException("The compact column doesn't participate in expanded width distribution.", nameof(weight));
		if (weight is not null && (!double.IsFinite(weight.Value) || weight.Value <= 0))
			throw new ArgumentOutOfRangeException(nameof(weight), weight, "A split view column weight must be finite and greater than zero.");

		Columns[column] = typeof(TView);
		if (weight is double value)
			ColumnWeights[column] = value;
		else
			ColumnWeights.Remove(column);

		return this;
	}

	/// <summary>
	/// Adds or replaces the primary page.
	/// </summary>
	public SplitViewBuilder Primary<TView>(
		double? weight = null) where TView : ContentView =>
		Column<TView>(SplitViewColumn.Primary, weight);

	/// <summary>
	/// Adds or replaces the supplementary page of a triple-column split view.
	/// </summary>
	public SplitViewBuilder Supplementary<TView>(
		double? weight = null) where TView : ContentView =>
		Column<TView>(SplitViewColumn.Supplementary, weight);

	/// <summary>
	/// Adds or replaces the secondary or detail page.
	/// </summary>
	public SplitViewBuilder Secondary<TView>(
		double? weight = null) where TView : ContentView =>
		Column<TView>(SplitViewColumn.Secondary, weight);

	/// <summary>
	/// Adds or replaces the page UIKit uses when the split view is collapsed.
	/// </summary>
	public SplitViewBuilder Compact<TView>() where TView : ContentView =>
		Column<TView>(SplitViewColumn.Compact);

	/// <summary>
	/// Adds or replaces the trailing inspector page on iOS 26 and later.
	/// </summary>
	/// <remarks>
	/// The inspector is omitted when the app runs on an earlier system version.
	/// </remarks>
	public SplitViewBuilder Inspector<TView>(
		double? weight = null) where TView : ContentView =>
		Column<TView>(SplitViewColumn.Inspector, weight);

	/// <summary>
	/// Chooses the edge that hosts the primary column.
	/// </summary>
	/// <param name="edge">The primary column edge.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder PrimaryEdge(
		SplitViewEdge edge)
	{
		PrimaryColumnEdge = edge;

		return this;
	}

	/// <summary>
	/// Chooses how visible columns share constrained space.
	/// </summary>
	/// <param name="behavior">The preferred adaptive presentation.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder Behavior(
		SplitViewBehavior behavior)
	{
		BehaviorPreference = behavior;

		return this;
	}

	/// <summary>
	/// Chooses the preferred number of visible columns.
	/// </summary>
	/// <param name="display">The preferred column display.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder Display(
		SplitViewDisplay display)
	{
		DisplayPreference = display;

		return this;
	}

	/// <summary>
	/// Chooses which column receives ViewModel-first stack navigation and tab reselection behavior.
	/// </summary>
	/// <remarks>
	/// The default is <see cref="SplitViewColumn.Secondary"/>. Use primary for canvas-style apps whose main
	/// content belongs in the primary column.
	/// </remarks>
	/// <param name="column">The column whose navigation stack is considered active.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder NavigationColumn(
		SplitViewColumn column)
	{
		NavigationTarget = column;

		return this;
	}


	internal IEnumerable<Type> Views =>
		Columns.Values.Distinct();

	internal void Validate(
		ViewRegistry registry)
	{
		if (!Columns.ContainsKey(SplitViewColumn.Primary))
			throw new InvalidOperationException("A split view requires a primary column.");
		if (!Columns.ContainsKey(SplitViewColumn.Secondary))
			throw new InvalidOperationException("A split view requires a secondary column.");
		if (SplitStyle is SplitViewStyle.TripleColumn
			&& !Columns.ContainsKey(SplitViewColumn.Supplementary))
			throw new InvalidOperationException("A triple-column split view requires a supplementary column.");
		if (SplitStyle is SplitViewStyle.DoubleColumn
			&& Columns.ContainsKey(SplitViewColumn.Supplementary))
			throw new InvalidOperationException("A supplementary column requires a triple-column split view.");
		if (NavigationTarget is SplitViewColumn.Inspector
			&& !Columns.ContainsKey(SplitViewColumn.Inspector))
			throw new InvalidOperationException("The split view navigation column must contain a page.");
		if (NavigationTarget is SplitViewColumn.Compact)
			throw new InvalidOperationException("The compact column can't be the navigation column; it only shows while the split view is collapsed.");
		if (NavigationTarget is not SplitViewColumn.Inspector
			&& !Columns.ContainsKey(NavigationTarget))
			throw new InvalidOperationException("The split view navigation column must contain a page.");

		foreach (Type view in Views)
			registry.EnsureRegistered(view);
	}
}
