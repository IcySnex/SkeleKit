using Xunit;

namespace SkeleKit.Tests.Elements;

public class MeasureCacheSlotTests
{
	[Fact]
	public void AlternatingSizes_StayCached()
	{
		StubLeaf leaf = new(10, 10);

		// what a Grid does: measure an auto child unconstrained, then at the resolved cell size
		leaf.Measure(new(double.PositiveInfinity, double.PositiveInfinity));
		leaf.Measure(new(100, 40));
		int before = leaf.MeasureCount;

		leaf.Measure(new(double.PositiveInfinity, double.PositiveInfinity));
		leaf.Measure(new(100, 40));

		Assert.Equal(before, leaf.MeasureCount);
	}

	[Fact]
	public void ThirdSize_Remeasures()
	{
		StubLeaf leaf = new(10, 10);

		leaf.Measure(new(100, 100));
		leaf.Measure(new(200, 100));
		int before = leaf.MeasureCount;

		leaf.Measure(new(300, 100));

		Assert.Equal(before + 1, leaf.MeasureCount);
	}

	[Fact]
	public void Invalidate_ClearsBothSlots()
	{
		StubLeaf leaf = new(10, 10);

		leaf.Measure(new(100, 100));
		leaf.Measure(new(200, 100));
		int before = leaf.MeasureCount;

		leaf.InvalidateMeasure();

		leaf.Measure(new(100, 100));
		leaf.Measure(new(200, 100));

		Assert.Equal(before + 2, leaf.MeasureCount);
	}

	[Fact]
	public void CachedHit_RestoresDesiredSize()
	{
		StubLeaf leaf = new(10, 10) { Margin = new Thickness(4) };

		leaf.Measure(new(100, 100));
		Size first = leaf.DesiredSize;

		leaf.Measure(new(200, 100));
		leaf.Measure(new(100, 100));

		Assert.Equal(first, leaf.DesiredSize);
	}
}
