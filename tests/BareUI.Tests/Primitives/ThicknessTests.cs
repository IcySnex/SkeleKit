using BareUI.Primitives;
using Xunit;

namespace BareUI.Tests.Primitives;

public class ThicknessTests
{
	[Fact]
	public void UniformConstructor_SetsAllSides()
	{
		Thickness thickness = new(8);

		Assert.Equal(8, thickness.Left);
		Assert.Equal(8, thickness.Top);
		Assert.Equal(8, thickness.Right);
		Assert.Equal(8, thickness.Bottom);
	}

	[Fact]
	public void SymmetricConstructor_SetsAxes()
	{
		Thickness thickness = new(16, 4);

		Assert.Equal(16, thickness.Left);
		Assert.Equal(16, thickness.Right);
		Assert.Equal(4, thickness.Top);
		Assert.Equal(4, thickness.Bottom);
	}

	[Fact]
	public void HorizontalAndVertical_SumSides()
	{
		Thickness thickness = new(1, 2, 3, 4);

		Assert.Equal(4, thickness.Horizontal);
		Assert.Equal(6, thickness.Vertical);
	}

	[Fact]
	public void ImplicitConversion_FromDouble_IsUniform()
	{
		Thickness thickness = 12;

		Assert.Equal(new Thickness(12), thickness);
	}
}
