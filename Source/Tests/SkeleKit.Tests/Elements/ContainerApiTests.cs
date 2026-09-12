using SkeleKit;
using Xunit;

namespace SkeleKit.Tests.Elements;

public class ContainerApiTests
{
	sealed class Page : ContentView;

	sealed class Item : ItemView<object>;


	[Theory]
	[InlineData(typeof(Border), "Child")]
	[InlineData(typeof(ScrollView), "Content")]
	[InlineData(typeof(Page), "Content")]
	[InlineData(typeof(Item), "Content")]
	public void SingleContentContainers_ExposeOnlyTheirSemanticSlot(
		Type type,
		string slot)
	{
		Assert.NotNull(type.GetProperty(slot));
		Assert.Null(type.GetProperty("Children"));
	}

	[Theory]
	[InlineData(typeof(Grid))]
	[InlineData(typeof(StackPanel))]
	[InlineData(typeof(Overlay))]
	public void Panels_ExposeChildren(
		Type type) =>
		Assert.NotNull(type.GetProperty("Children"));
}
