using Xunit;

namespace BareUI.Tests.Primitives;

public class LerpTests
{
	[Fact]
	public void Color_Lerp_MixesChannels()
	{
		Color mixed = Color.Lerp(new(0, 0, 0), new(1, 0.5, 0), 0.5)!.Value;

		Assert.Equal(0.5, mixed.Red, 3);
		Assert.Equal(0.25, mixed.Green, 3);
		Assert.Equal(0, mixed.Blue, 3);
		Assert.Equal(1, mixed.Alpha, 3);
	}

	[Fact]
	public void Color_Lerp_MixesBothAppearancesOfADynamicPair()
	{
		Color a = Color.Dynamic(new(0, 0, 0), new(1, 1, 1));
		Color b = Color.Dynamic(new(1, 1, 1), new(0, 0, 0));

		Color mixed = Color.Lerp(a, b, 0.5)!.Value;

		Assert.Equal(0.5, mixed.Red, 3);
		Assert.NotNull(mixed.Dark);
		Assert.Equal(0.5, mixed.Dark!.Value.Red, 3);
	}

	[Fact]
	public void Color_Lerp_RefusesASystemColor()
	{
		Assert.Null(Color.Lerp(Colors.Red, new(0, 0, 0), 0.5));
	}

	[Fact]
	public void Brush_Lerp_MixesSolids()
	{
		SolidBrush mixed = (SolidBrush)Brush.Lerp(new SolidBrush(new(0, 0, 0)), new SolidBrush(new(1, 1, 1)), 0.5)!;

		Assert.Equal(0.5, mixed.Color.Red, 3);
	}

	[Fact]
	public void Brush_Lerp_MixesShapeMatchedGradients()
	{
		LinearGradient a = LinearGradient.Vertical(new(0, 0, 0), new(1, 1, 1));
		LinearGradient b = LinearGradient.Horizontal(new(1, 1, 1), new(0, 0, 0));

		LinearGradient mixed = (LinearGradient)Brush.Lerp(a, b, 0.5)!;

		Assert.Equal(0.5, mixed.Stops[0].Color.Red, 3);
		Assert.Equal(0.5, mixed.Stops[1].Color.Red, 3);
		Assert.Equal(0.25, mixed.Start.X, 3);
	}

	[Fact]
	public void Brush_Lerp_RefusesMismatchedShapes()
	{
		LinearGradient two = LinearGradient.Vertical(new(0, 0, 0), new(1, 1, 1));
		LinearGradient three = LinearGradient.Vertical(new(0, 0, 0), new(0.5, 0.5, 0.5), new(1, 1, 1));

		Assert.Null(Brush.Lerp(two, three, 0.5));
		Assert.Null(Brush.Lerp(two, new Material(MaterialKind.Regular), 0.5));
	}

	[Fact]
	public void ViewState_Lerp_MixesLayoutLengths()
	{
		ViewState a = default(ViewState) with { Width = 100, Height = 50, Margin = new(0) };
		ViewState b = default(ViewState) with { Width = 200, Height = 150, Margin = new(20) };

		ViewState mixed = ViewState.Lerp(a, b, 0.5);

		Assert.Equal(150, mixed.Width);
		Assert.Equal(100, mixed.Height);
		Assert.Equal(new Thickness(10), mixed.Margin);
	}

	[Fact]
	public void ViewState_Lerp_SnapsAnAutoSizedLength()
	{
		ViewState a = default(ViewState) with { Width = double.NaN };
		ViewState b = default(ViewState) with { Width = 200 };

		Assert.True(double.IsNaN(ViewState.Lerp(a, b, 0.5).Width));
	}
}
