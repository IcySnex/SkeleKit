using BareUI.Primitives;
using Xunit;

namespace BareUI.Tests.Primitives;

public class SizeTests
{
	[Fact]
	public void Infinity_IsNotFinite()
	{
		Assert.False(Size.Infinity.IsFinite);
		Assert.True(new Size(10, 20).IsFinite);
	}

	[Fact]
	public void Deflate_ShrinksBothAxes()
	{
		Size result = new Size(100, 50).Deflate(new Thickness(10, 5));

		Assert.Equal(new Size(80, 40), result);
	}

	[Fact]
	public void Deflate_ClampsAtZero()
	{
		Size result = new Size(10, 10).Deflate(new Thickness(20));

		Assert.Equal(Size.Zero, result);
	}

	[Fact]
	public void Inflate_GrowsBothAxes()
	{
		Size result = new Size(80, 40).Inflate(new Thickness(10, 5));

		Assert.Equal(new Size(100, 50), result);
	}
}
