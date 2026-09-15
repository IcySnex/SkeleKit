namespace SkeleKit;

/// <summary>
/// Configures adaptive sidebar placements and destinations.
/// </summary>
public sealed class SidebarBuilder
{
	internal Dictionary<Type, TabPlacement> Placements { get; } = [];
	internal List<TabsBuilder.Node> Nodes { get; } = [];
	internal Func<View>? FooterFactory { get; private set; }


	/// <summary>
	/// Overrides how a declared tab takes part in sidebar customization.
	/// </summary>
	/// <typeparam name="TView">The view type of the tab to place.</typeparam>
	/// <param name="placement">The placement to apply.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SidebarBuilder PlaceTab<TView>(
		TabPlacement placement) where TView : ContentView
	{
		Placements[typeof(TView)] = placement;

		return this;
	}

	/// <summary>
	/// Adds a destination that is shown only when the sidebar is available by default.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="title">The text displayed for the destination.</param>
	/// <param name="icon">The local icon shown for the destination.</param>
	/// <param name="placement">How the destination takes part in sidebar customization.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SidebarBuilder Tab<TView>(
		string title,
		ImageSource icon,
		TabPlacement placement = TabPlacement.SidebarOnly) where TView : ContentView
	{
		Nodes.Add(new TabsBuilder.Leaf(typeof(TView), title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds a sidebar section: a group of tabs, always sidebar-only.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="icon">The local icon shown for the group.</param>
	/// <param name="children">Declares the tabs inside the group.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SidebarBuilder Group(
		string title,
		ImageSource icon,
		Action<GroupBuilder> children)
	{
		GroupBuilder group = new();
		children(group);

		Nodes.Add(new TabsBuilder.GroupNode(title, icon, group.Nodes));

		return this;
	}

	/// <summary>
	/// Shows a view of the given type at the sidebar's bottom. iOS 26 and later.
	/// </summary>
	/// <typeparam name="TView">The view type to host.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public SidebarBuilder Footer<TView>()
		where TView : View, new()
	{
		FooterFactory = () => new TView();

		return this;
	}
}
