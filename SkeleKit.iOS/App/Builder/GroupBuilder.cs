namespace SkeleKit;

/// <summary>
/// Declares the tabs inside a group.
/// </summary>
public sealed class GroupBuilder
{
	readonly ViewRegistry registry;

	internal GroupBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}

	internal List<TabsBuilder.Node> Nodes { get; } = [];


	/// <summary>
	/// Adds a tab page to the group.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="title">The text displayed on the tab bar item.</param>
	/// <param name="icon">The name or path of the icon resource for the tab.</param>
	/// <param name="placement">How the tab takes part in user customization.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public GroupBuilder Tab<TView>(
		string title,
		string icon,
		TabPlacement placement = TabPlacement.Automatic) where TView : ContentView
	{
		Nodes.Add(new TabsBuilder.Leaf(registry.ViewModelOf<TView>(), title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds a nested group.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="icon">The group's SF Symbol.</param>
	/// <param name="children">Declares the tabs inside the group.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public GroupBuilder Group(
		string title,
		string icon,
		Action<GroupBuilder> children)
	{
		GroupBuilder group = new(registry);
		children(group);

		Nodes.Add(new TabsBuilder.GroupNode(title, icon, group.Nodes));

		return this;
	}
}
