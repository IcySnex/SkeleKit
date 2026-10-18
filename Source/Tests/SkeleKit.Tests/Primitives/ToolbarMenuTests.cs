using System.Collections.ObjectModel;
using Xunit;

namespace SkeleKit.Tests.Primitives;

public class ToolbarMenuTests
{
	[Fact]
	public void AddingAnAction_NotifiesChanged()
	{
		ObservableCollection<MenuAction> source = [new MenuAction { Text = "First" }];
		ToolbarMenu menu = new() { Actions = source };
		menu.AttachBindings(null);

		ToolbarItem? sender = null;
		menu.Changed += item => sender = item;

		source.Add(new MenuAction { Text = "Add" });

		Assert.Same(menu, sender);
	}

	[Fact]
	public void Actions_AcceptCollectionExpression()
	{
		ToolbarMenu menu = new()
		{
			Actions = [new MenuAction { Text = "Add" }]
		};

		Assert.Single(menu.Actions);
	}
}
