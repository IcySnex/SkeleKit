namespace SkeleKit;

/// <summary>
/// Declares the pages hosted by a native split view controller.
/// </summary>
public sealed partial class SplitViewBuilder
{
	internal Dictionary<SplitViewColumn, Type> Columns { get; } = [];
	internal SplitViewStyle SplitStyle { get; private set; } = SplitViewStyle.DoubleColumn;
	internal SplitViewColumn NavigationTarget { get; private set; } = SplitViewColumn.Secondary;


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
	/// <returns>The builder instance for chaining calls.</returns>
	public SplitViewBuilder Column<TView>(
		SplitViewColumn column) where TView : ContentView
	{
		Columns[column] = typeof(TView);

		return this;
	}

	/// <summary>
	/// Adds or replaces the primary page.
	/// </summary>
	public SplitViewBuilder Primary<TView>() where TView : ContentView =>
		Column<TView>(SplitViewColumn.Primary);

	/// <summary>
	/// Adds or replaces the supplementary page of a triple-column split view.
	/// </summary>
	public SplitViewBuilder Supplementary<TView>() where TView : ContentView =>
		Column<TView>(SplitViewColumn.Supplementary);

	/// <summary>
	/// Adds or replaces the secondary or detail page.
	/// </summary>
	public SplitViewBuilder Secondary<TView>() where TView : ContentView =>
		Column<TView>(SplitViewColumn.Secondary);

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
	public SplitViewBuilder Inspector<TView>() where TView : ContentView =>
		Column<TView>(SplitViewColumn.Inspector);

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
		if (NavigationTarget is SplitViewColumn.Inspector
			&& !Columns.ContainsKey(SplitViewColumn.Inspector))
			throw new InvalidOperationException("The split view navigation column must contain a page.");
		if (NavigationTarget is not SplitViewColumn.Inspector
			&& !Columns.ContainsKey(NavigationTarget))
			throw new InvalidOperationException("The split view navigation column must contain a page.");

		foreach (Type view in Views)
			registry.EnsureRegistered(view);
	}
}
