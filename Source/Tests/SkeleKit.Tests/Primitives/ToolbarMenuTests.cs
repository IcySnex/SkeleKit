using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ToolbarMenuTests
{
	[Fact]
	public void AddingAnAction_NotifiesChanged()
	{
		ToolbarMenu menu = new();
		ToolbarItem? sender = null;
		menu.Changed += item => sender = item;

		menu.Actions.Add(new MenuAction { Text = "Add" });

		Assert.Same(menu, sender);
	}
}
