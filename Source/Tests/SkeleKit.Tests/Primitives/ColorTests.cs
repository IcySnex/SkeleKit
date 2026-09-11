using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ColorTests
{
	[Fact]
	public void FromHex_ReadsRgb()
	{
		Color color = Color.FromHex(0xFF3B30);

		Assert.Equal(1.0, color.Red, 2);
		Assert.Equal(0x3B / 255.0, color.Green, 2);
		Assert.Equal(0x30 / 255.0, color.Blue, 2);
		Assert.Equal(1.0, color.Alpha);
	}

	[Fact]
	public void SystemColor_CarriesLightFallback()
	{
		Color red = Colors.Red;

		Assert.Equal(SystemColor.Red, red.System);
		Assert.Equal(Color.FromHex(0xFF3B30) with { System = SystemColor.Red }, red);
	}

	[Fact]
	public void WithAlpha_PreservesSystemColor()
	{
		Color faded = Colors.Label.WithAlpha(0.5);

		Assert.Equal(SystemColor.Label, faded.System);
		Assert.Equal(0.5, faded.Alpha);
	}

	[Fact]
	public void Transparent_HasZeroAlpha()
	{
		Color transparent = Colors.Transparent;

		Assert.Equal(0, transparent.Alpha);
	}

	[Fact]
	public void Dynamic_KeepsBothAppearances()
	{
		Color color = Color.Dynamic(Colors.White, Colors.Black);

		Assert.Null(color.System);
		Assert.Equal(1.0, color.Red);
		Assert.Equal(Colors.White, color.Pair!.Light);
		Assert.Equal(Colors.Black, color.Pair.Dark);
	}

	[Fact]
	public void Dynamic_PreservesSystemColors()
	{
		Color color = Color.Dynamic(Colors.Background, Colors.SecondaryBackground);

		Assert.Null(color.System);
		Assert.Equal(SystemColor.Background, color.Pair!.Light.System);
		Assert.Equal(SystemColor.SecondaryBackground, color.Pair.Dark.System);
	}

	[Fact]
	public void Dynamic_PreservesNestedDynamicColors()
	{
		Color nested = Color.Dynamic(Colors.Red, Colors.Blue);
		Color color = Color.Dynamic(Colors.White, nested);

		Assert.Equal(nested, color.Pair!.Dark);
	}

	[Fact]
	public void WithAlpha_AppliesToBothAppearances()
	{
		Color color = Color.Dynamic(Colors.White, Colors.Black).WithAlpha(0.5);

		Assert.Equal(0.5, color.Alpha);
		Assert.Equal(0.5, color.Pair!.Light.Alpha);
		Assert.Equal(0.5, color.Pair.Dark.Alpha);
	}
}
