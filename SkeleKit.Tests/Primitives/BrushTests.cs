using Xunit;

namespace SkeleKit.Tests.Primitives;

public class BrushTests
{
	// the implicit conversion is what keeps every existing Background = Colors.X call site compiling
	[Fact]
	public void Color_ConvertsToASolidBrush()
	{
		Brush brush = Colors.Red;

		SolidBrush solid = Assert.IsType<SolidBrush>(brush);
		Assert.Equal(Colors.Red, solid.Color);
	}

	[Fact]
	public void Vertical_SpreadsStopsEvenly()
	{
		LinearGradient gradient = LinearGradient.Vertical(Colors.Black, Colors.Gray, Colors.White);

		Assert.Equal([0, 0.5, 1], gradient.Stops.Select(stop => stop.Offset));
		Assert.Equal(new Point(0.5, 0), gradient.Start);
		Assert.Equal(new Point(0.5, 1), gradient.End);
	}

	[Fact]
	public void Vertical_WithOneColor_PlacesItAtTheStart()
	{
		LinearGradient gradient = LinearGradient.Vertical(Colors.Black);

		Assert.Equal(0, Assert.Single(gradient.Stops).Offset);
	}

	[Fact]
	public void Horizontal_RunsLeadingToTrailing()
	{
		LinearGradient gradient = LinearGradient.Horizontal(Colors.Black, Colors.White);

		Assert.Equal(new Point(0, 0.5), gradient.Start);
		Assert.Equal(new Point(1, 0.5), gradient.End);
	}

	[Fact]
	public void Material_KeepsItsKind()
	{
		Material material = new(MaterialKind.Chrome);

		Assert.Equal(MaterialKind.Chrome, material.Kind);
	}
}
