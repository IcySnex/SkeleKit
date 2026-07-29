using Xunit;

namespace SkeleKit.Tests.App;

public class DetentTests
{
	[Fact]
	public void Medium_UsesSystemHeight()
	{
		Assert.Equal(DetentKind.Medium, Detent.Medium.Kind);
	}

	[Fact]
	public void Large_UsesSystemHeight()
	{
		Assert.Equal(DetentKind.Large, Detent.Large.Kind);
	}

	[Fact]
	public void Height_KeepsPoints()
	{
		Detent detent = Detent.Height(320);

		Assert.Equal(DetentKind.Height, detent.Kind);
		Assert.Equal(320, detent.Value);
	}

	[Fact]
	public void Fraction_KeepsProportion()
	{
		Detent detent = Detent.Fraction(0.35);

		Assert.Equal(DetentKind.Fraction, detent.Kind);
		Assert.Equal(0.35, detent.Value);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	public void Height_RejectsInvalidValues(
		double height)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => Detent.Height(height));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(1.1)]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	public void Fraction_RejectsInvalidValues(
		double fraction)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => Detent.Fraction(fraction));
	}
}
