namespace BareUI;

/// <summary>
/// Everything iPad: the sidebar, placements and iPad-only destinations.
/// </summary>
public sealed class PadTabsBuilder
{
	readonly ViewRegistry registry;

	internal PadTabsBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}

	
	internal bool UseSidebar { get; private set; }
	internal Dictionary<Type, TabPlacement> Placements { get; } = [];
	internal List<TabsBuilder.Node> Nodes { get; } = [];
	internal Func<View>? FooterFactory { get; private set; }


	/// <summary>
	/// Shows the tabs as a sidebar.
	/// </summary>
	/// <returns>The builder instance for chaining calls.</returns>
	public PadTabsBuilder Sidebar()
	{
		UseSidebar = true;

		return this;
	}

	/// <summary>
	/// Overrides how a declared tab takes part in user customization.
	/// </summary>
	/// <typeparam name="TView">The view type of the tab to place.</typeparam>
	/// <param name="placement">The placement to apply.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PadTabsBuilder PlaceTab<TView>(
		TabPlacement placement) where TView : ContentView
	{
		Placements[registry.ViewModelOf<TView>()] = placement;

		return this;
	}

	/// <summary>
	/// Adds an iPad-only tab. It does not exist on iPhone; reach the page there by navigation.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="title">The text displayed on the tab bar item.</param>
	/// <param name="icon">The name or path of the icon resource for the tab.</param>
	/// <param name="placement">How the tab takes part in user customization.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PadTabsBuilder Tab<TView>(
		string title,
		string icon,
		TabPlacement placement = TabPlacement.SidebarOnly) where TView : ContentView
	{
		Nodes.Add(new TabsBuilder.Leaf(registry.ViewModelOf<TView>(), title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds a sidebar section: a group of tabs, always sidebar-only.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="icon">The group's SF Symbol.</param>
	/// <param name="children">Declares the tabs inside the group.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PadTabsBuilder Group(
		string title,
		string icon,
		Action<GroupBuilder> children)
	{
		GroupBuilder group = new(registry);
		children(group);

		Nodes.Add(new TabsBuilder.GroupNode(title, icon, group.Nodes));

		return this;
	}

	/// <summary>
	/// Shows a view of the given type at the sidebar's bottom. iOS 26 and later.
	/// </summary>
	/// <typeparam name="TView">The view type to host.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public PadTabsBuilder SidebarFooter<TView>()
		where TView : View, new()
	{
		FooterFactory = () => new TView();

		return this;
	}
}
