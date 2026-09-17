using Xunit;

namespace SkeleKit.Tests.App;

public class TabsBuilderTests
{
	sealed class HomeView : ContentView;
	sealed class DetailView : ContentView;

	static ViewRegistry Registry()
	{
		ViewRegistry registry = new();
		new PagesBuilder(registry, replace: true)
			.AddTransient<HomeView>()
			.AddTransient<DetailView>();

		return registry;
	}


	[Fact]
	public void TabKeepsItsPlacement()
	{
		TabsBuilder tabs = new();
		tabs.Tab<HomeView>("Home", "house", TabPlacement.Fixed);

		tabs.Validate(Registry());

		TabsBuilder.Leaf tab = Assert.IsType<TabsBuilder.Leaf>(Assert.Single(tabs.Nodes));
		Assert.Equal(TabPlacement.Fixed, tab.Placement);
	}

	[Fact]
	public void SidebarOnlyDestinationRequiresSidebar()
	{
		TabsBuilder tabs = new();
		tabs.Tab<HomeView>("Home", "house", TabPlacement.SidebarOnly);

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => tabs.Validate(Registry()));

		Assert.Contains("Sidebar", exception.Message, StringComparison.OrdinalIgnoreCase);
	}

	[Fact]
	public void SidebarOnlyDestinationIsValidWithSidebar()
	{
		TabsBuilder tabs = new();
		tabs
			.Tab<HomeView>("Home", "house", TabPlacement.SidebarOnly)
			.Sidebar();

		tabs.Validate(Registry());
	}

	[Fact]
	public void GroupBelongsToTabHierarchy()
	{
		TabsBuilder tabs = new();
		tabs
			.Group("Collection", "folder", group => group
				.Tab<DetailView>("Detail", "doc"))
			.Sidebar();

		tabs.Validate(Registry());

		TabsBuilder.GroupNode group = Assert.IsType<TabsBuilder.GroupNode>(Assert.Single(tabs.Nodes));
		Assert.Equal(TabPlacement.SidebarOnly, group.Placement);
		Assert.IsType<TabsBuilder.Leaf>(Assert.Single(group.Children));
	}

	[Fact]
	public void GroupRequiresSidebar()
	{
		TabsBuilder tabs = new();
		tabs.Group("Collection", "folder", group => group
			.Tab<DetailView>("Detail", "doc"));

		InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
			() => tabs.Validate(Registry()));

		Assert.Contains("Sidebar", exception.Message, StringComparison.OrdinalIgnoreCase);
	}
}
