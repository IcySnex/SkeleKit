using BareUI.Primitives;
using Xunit;

namespace BareUI.Tests.Primitives;

public class GridLengthTests
{
	[Fact]
	public void Auto_IsAutoOnly()
	{
		Assert.True(GridLength.Auto.IsAuto);
		Assert.False(GridLength.Auto.IsStar);
		Assert.False(GridLength.Auto.IsAbsolute);
	}

	[Fact]
	public void Star_HasWeightOne()
	{
		Assert.True(GridLength.Star.IsStar);
		Assert.Equal(1, GridLength.Star.Value);
	}

	[Fact]
	public void Stars_CarriesWeight()
	{
		GridLength length = GridLength.Stars(2.5);

		Assert.True(length.IsStar);
		Assert.Equal(2.5, length.Value);
	}

	[Fact]
	public void Pixels_IsAbsolute()
	{
		GridLength length = GridLength.Pixels(120);

		Assert.True(length.IsAbsolute);
		Assert.Equal(120, length.Value);
	}

	[Fact]
	public void ImplicitConversion_FromDouble_IsPixels()
	{
		GridLength length = 200;

		Assert.Equal(GridLength.Pixels(200), length);
	}
}
