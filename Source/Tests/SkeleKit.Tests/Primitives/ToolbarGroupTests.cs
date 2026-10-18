using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ToolbarGroupTests
{
	[Fact]
	public void Constructor_AddsActionsInOrder()
	{
		ToolbarButton first = new();
		ToolbarMenu second = new();

		ToolbarGroup group = new(first, second);

		Assert.Equal([first, second], group.Items);
	}

	[Fact]
	public void AddingAnAction_NotifiesChanged()
	{
		ToolbarGroup group = new();
		ToolbarItem? sender = null;
		group.Changed += item => sender = item;

		group.Items.Add(new ToolbarButton());

		Assert.Same(group, sender);
	}
}
