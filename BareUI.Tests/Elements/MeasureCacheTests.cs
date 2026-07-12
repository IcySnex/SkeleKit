using Xunit;

namespace BareUI.Tests.Elements;

public class MeasureCacheTests
{
	[Fact]
	public void Measure_SameAvailable_MeasuresOnce()
	{
		StubLeaf leaf = new(10, 10);

		leaf.Measure(new(100, 100));
		leaf.Measure(new(100, 100));
		leaf.Measure(new(100, 100));

		Assert.Equal(1, leaf.MeasureCount);
	}

	[Fact]
	public void Measure_DifferentAvailable_MeasuresAgain()
	{
		StubLeaf leaf = new(10, 10);

		leaf.Measure(new(100, 100));
		leaf.Measure(new(200, 100));

		Assert.Equal(2, leaf.MeasureCount);
	}

	[Fact]
	public void InvalidateMeasure_ForcesRemeasure()
	{
		StubLeaf leaf = new(10, 10);
		leaf.Measure(new(100, 100));

		leaf.InvalidateMeasure();
		leaf.Measure(new(100, 100));

		Assert.Equal(2, leaf.MeasureCount);
	}

	[Fact]
	public void PropertyChange_InvalidatesMeasure()
	{
		StubLeaf leaf = new(10, 10);
		leaf.Measure(new(100, 100));

		leaf.Margin = new Thickness(8);
		leaf.Measure(new(100, 100));

		Assert.Equal(2, leaf.MeasureCount);
	}

	[Fact]
	public void PropertyChange_SameValue_KeepsCache()
	{
		StubLeaf leaf = new(10, 10) { Margin = new Thickness(8) };
		leaf.Measure(new(100, 100));

		leaf.Margin = new Thickness(8);
		leaf.Measure(new(100, 100));

		Assert.Equal(1, leaf.MeasureCount);
	}

	[Fact]
	public void PanelRemeasure_SkipsCleanChildren()
	{
		StubLeaf first = new(10, 10);
		StubLeaf second = new(10, 10);
		StackPanel stack = new() { Children = { first, second } };

		stack.Measure(new(100, 100));
		int before = first.MeasureCount;

		// a fresh pass over an unchanged tree must not re-measure the children
		stack.Measure(new(100, 100));

		Assert.Equal(before, first.MeasureCount);
		Assert.Equal(before, second.MeasureCount);
	}

	[Fact]
	public void ChildChange_InvalidatesAncestors()
	{
		StubLeaf leaf = new(10, 10);
		StackPanel inner = new() { Children = { leaf } };
		StackPanel outer = new() { Children = { inner } };

		outer.Measure(new(100, 100));
		double before = outer.DesiredSize.Height;

		inner.Children.Add(new StubLeaf(10, 25));
		outer.Measure(new(100, 100));

		Assert.Equal(before + 25, outer.DesiredSize.Height);
	}

	[Fact]
	public void GrandchildChange_InvalidatesRoot()
	{
		StubLeaf leaf = new(10, 10);
		StackPanel inner = new() { Children = { leaf } };
		StackPanel outer = new() { Children = { inner } };

		outer.Measure(new(100, 100));

		leaf.Height = 40;
		outer.Measure(new(100, 100));

		Assert.Equal(40, outer.DesiredSize.Height);
	}
}
