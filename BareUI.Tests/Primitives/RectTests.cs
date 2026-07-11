using BareUI.Primitives;
using Xunit;

namespace BareUI.Tests.Primitives;

public class RectTests
{
	[Fact]
	public void Edges_ComputeFromLocationAndSize()
	{
		Rect rect = new(10, 20, 100, 50);

		Assert.Equal(10, rect.Left);
		Assert.Equal(20, rect.Top);
		Assert.Equal(110, rect.Right);
		Assert.Equal(70, rect.Bottom);
	}

	[Fact]
	public void LocationAndSize_RoundTrip()
	{
		Rect rect = new(new Point(3, 4), new Size(5, 6));

		Assert.Equal(new Point(3, 4), rect.Location);
		Assert.Equal(new Size(5, 6), rect.Size);
	}

	[Fact]
	public void Deflate_InsetsAndShrinks()
	{
		Rect result = new Rect(0, 0, 100, 100).Deflate(new Thickness(10, 20, 30, 40));

		Assert.Equal(new Rect(10, 20, 60, 40), result);
	}

	[Fact]
	public void Deflate_ClampsSizeAtZero()
	{
		Rect result = new Rect(0, 0, 10, 10).Deflate(new Thickness(20));

		Assert.Equal(new Rect(20, 20, 0, 0), result);
	}
}
