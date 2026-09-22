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

	[Fact]
	public void SolidBrush_UsesValueEquality()
	{
		Brush first = Colors.Red;
		Brush second = Colors.Red;
		Brush other = Colors.Blue;

		Assert.NotSame(first, second);
		Assert.Equal(first, second);
		Assert.Equal(first.GetHashCode(), second.GetHashCode());
		Assert.NotEqual(first, other);
	}

	[Fact]
	public void Material_UsesValueEquality()
	{
		Assert.Equal(new Material(MaterialKind.Chrome), new Material(MaterialKind.Chrome));
		Assert.NotEqual(new Material(MaterialKind.Chrome), new Material(MaterialKind.Thin));
	}

	[Fact]
	public void LinearGradient_UsesValueEquality()
	{
		Assert.Equal(
			LinearGradient.Vertical(Colors.Black, Colors.White),
			LinearGradient.Vertical(Colors.Black, Colors.White));

		Assert.NotEqual(
			LinearGradient.Vertical(Colors.Black, Colors.White),
			LinearGradient.Vertical(Colors.White, Colors.Black));
	}
}
