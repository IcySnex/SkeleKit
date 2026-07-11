using Xunit;

namespace BareUI.Tests.Elements;

public class ViewPropertyTests
{
	[Fact]
	public void Add_SetsParent()
	{
		StubLeaf child = new(10, 10);
		VStack stack = new() { Children = { child } };

		Assert.Same(stack, child.Parent);
	}

	[Fact]
	public void Remove_ClearsParent()
	{
		StubLeaf child = new(10, 10);
		VStack stack = new() { Children = { child } };

		stack.Children.Remove(child);

		Assert.Null(child.Parent);
	}

	[Fact]
	public void Clear_ClearsParent()
	{
		StubLeaf child = new(10, 10);
		VStack stack = new() { Children = { child } };

		stack.Children.Clear();

		Assert.Null(child.Parent);
	}

	[Fact]
	public void Parent_ChainsToRoot()
	{
		StubLeaf leaf = new(10, 10);
		VStack inner = new() { Children = { leaf } };
		VStack outer = new() { Children = { inner } };

		Assert.Same(inner, leaf.Parent);
		Assert.Same(outer, inner.Parent);
		Assert.Null(outer.Parent);
	}

	[Fact]
	public void Set_RoundTripsValue()
	{
		StubLeaf leaf = new(10, 10)
		{
			Width = 42,
			Margin = new Thickness(4),
			Opacity = 0.5
		};

		Assert.Equal(42, leaf.Width);
		Assert.Equal(new Thickness(4), leaf.Margin);
		Assert.Equal(0.5, leaf.Opacity);
	}

	[Fact]
	public void InvalidateMeasure_OnUnrealizedTree_DoesNotThrow()
	{
		StubLeaf leaf = new(10, 10);
		_ = new VStack { Children = { leaf } };

		leaf.InvalidateMeasure();
	}
}
