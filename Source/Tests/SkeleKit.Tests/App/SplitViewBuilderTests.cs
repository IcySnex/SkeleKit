using Xunit;

namespace SkeleKit.Tests.App;

public class SplitViewBuilderTests
{
	sealed class PrimaryView : ContentView;
	sealed class SupplementaryView : ContentView;
	sealed class SecondaryView : ContentView;
	sealed class CompactView : ContentView;

	static ViewRegistry Registry()
	{
		ViewRegistry registry = new();
		new PagesBuilder(registry, replace: true)
			.AddTransient<PrimaryView>()
			.AddTransient<SupplementaryView>()
			.AddTransient<SecondaryView>()
			.AddTransient<CompactView>();

		return registry;
	}


	[Fact]
	public void DoubleColumnRegistersItsPages()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Secondary<SecondaryView>()
			.Compact<CompactView>()
			.NavigationColumn(SplitViewColumn.Primary);

		split.Validate(registry);

		Assert.Equal(SplitViewStyle.DoubleColumn, split.SplitStyle);
		Assert.Equal(typeof(PrimaryView), split.Columns[SplitViewColumn.Primary]);
		Assert.Equal(typeof(SecondaryView), split.Columns[SplitViewColumn.Secondary]);
		Assert.Equal(typeof(CompactView), split.Columns[SplitViewColumn.Compact]);
		Assert.Equal(SplitViewColumn.Primary, split.NavigationTarget);
	}

	[Fact]
	public void DefaultsKeepNativeAdaptiveLayout()
	{
		SplitViewBuilder split = new();

		Assert.Equal(SplitViewEdge.Leading, split.PrimaryColumnEdge);
		Assert.Equal(SplitViewBehavior.Automatic, split.BehaviorPreference);
		Assert.Equal(SplitViewDisplay.Automatic, split.DisplayPreference);
		Assert.Empty(split.ColumnWeights);
	}

	[Fact]
	public void ColumnsStoreRelativeWeights()
	{
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>(weight: 1)
			.Secondary<SecondaryView>(weight: 2)
			.PrimaryEdge(SplitViewEdge.Trailing)
			.Behavior(SplitViewBehavior.SideBySide)
			.Display(SplitViewDisplay.TwoColumns);

		Assert.Equal(1, split.ColumnWeights[SplitViewColumn.Primary]);
		Assert.Equal(2, split.ColumnWeights[SplitViewColumn.Secondary]);
		Assert.Equal(SplitViewEdge.Trailing, split.PrimaryColumnEdge);
		Assert.Equal(SplitViewBehavior.SideBySide, split.BehaviorPreference);
		Assert.Equal(SplitViewDisplay.TwoColumns, split.DisplayPreference);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	public void ColumnWeightsMustBePositiveAndFinite(
		double weight)
	{
		SplitViewBuilder split = new();

		Assert.Throws<ArgumentOutOfRangeException>(
			() => split.Primary<PrimaryView>(weight));
	}

	[Fact]
	public void CompactColumnRejectsWeight()
	{
		SplitViewBuilder split = new();

		Assert.Throws<ArgumentException>(
			() => split.Column<CompactView>(SplitViewColumn.Compact, weight: 1));
	}

	[Fact]
	public void ReplacingColumnWithoutWeightRestoresAutomaticWeight()
	{
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>(weight: 2)
			.Primary<SupplementaryView>();

		Assert.False(split.ColumnWeights.ContainsKey(SplitViewColumn.Primary));
	}

	[Fact]
	public void TripleColumnRequiresSupplementaryPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Style(SplitViewStyle.TripleColumn)
			.Primary<PrimaryView>()
			.Secondary<SecondaryView>();

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("supplementary", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void NavigationColumnMustContainPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Secondary<SecondaryView>()
			.NavigationColumn(SplitViewColumn.Supplementary);

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("navigation column", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void TripleColumnAcceptsSupplementaryPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Style(SplitViewStyle.TripleColumn)
			.Primary<PrimaryView>()
			.Supplementary<SupplementaryView>()
			.Secondary<SecondaryView>();

		split.Validate(registry);

		Assert.Equal(typeof(SupplementaryView), split.Columns[SplitViewColumn.Supplementary]);
	}

	[Fact]
	public void DoubleColumnRejectsSupplementaryPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Supplementary<SupplementaryView>()
			.Secondary<SecondaryView>();

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("supplementary", exception.Message, StringComparison.OrdinalIgnoreCase);
		Assert.Contains("triple", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void PrimaryColumnIsRequired()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split.Secondary<SecondaryView>();

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("primary", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void SecondaryColumnIsRequired()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split.Primary<PrimaryView>();

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("secondary", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void InspectorNavigationColumnRequiresInspectorPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Secondary<SecondaryView>()
			.NavigationColumn(SplitViewColumn.Inspector);

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("navigation column", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void InspectorNavigationColumnAcceptsInspectorPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Secondary<SecondaryView>()
			.Inspector<CompactView>()
			.NavigationColumn(SplitViewColumn.Inspector);

		split.Validate(registry);

		Assert.Equal(typeof(CompactView), split.Columns[SplitViewColumn.Inspector]);
		Assert.Equal(SplitViewColumn.Inspector, split.NavigationTarget);
	}

	[Fact]
	public void CompactColumnCannotBeNavigationColumn()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Secondary<SecondaryView>()
			.Compact<CompactView>()
			.NavigationColumn(SplitViewColumn.Compact);

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => split.Validate(registry));

		Assert.Contains("compact", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void ColumnReplacesExistingPage()
	{
		ViewRegistry registry = Registry();
		SplitViewBuilder split = new();
		split
			.Primary<PrimaryView>()
			.Primary<SupplementaryView>()
			.Secondary<SecondaryView>();

		split.Validate(registry);

		Assert.Equal(typeof(SupplementaryView), split.Columns[SplitViewColumn.Primary]);
		Assert.Equal(
			new[] { typeof(SecondaryView), typeof(SupplementaryView) },
			split.Views.OrderBy(view => view.Name));
	}
}
