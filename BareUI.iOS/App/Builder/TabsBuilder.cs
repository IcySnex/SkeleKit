using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

/// <summary>
/// Declares the application's tabs.
/// </summary>
public sealed class TabsBuilder
{
	internal abstract record Node;

	internal sealed record Leaf(
		Type ViewModel,
		string Title,
		string Icon,
		TabPlacement Placement) : Node;

	internal sealed record GroupNode(
		string Title,
		string Icon,
		List<Node> Children,
		TabPlacement Placement) : Node;


	readonly ViewRegistry registry;

	internal TabsBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}

	internal List<Node> Nodes { get; } = [];

	internal Type? SearchViewModel { get; private set; }

	internal string? ActionIcon { get; private set; }

	internal Func<IServiceProvider, Action>? ActionFactory { get; private set; }

	internal TabBarMinimize Minimize { get; private set; } = TabBarMinimize.Never;

	internal Func<View>? AccessoryFactory { get; private set; }

	internal IPadTabsBuilder? IPad { get; private set; }

	internal bool UseLargeTitles { get; private set; }


	/// <summary>
	/// Enables large, expanding navigation titles for the tab pages.
	/// </summary>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder LargeTitles()
	{
		UseLargeTitles = true;

		return this;
	}

	/// <summary>
	/// Adds a tab page to the navigation structure.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="title">The text displayed on the tab bar item.</param>
	/// <param name="icon">The name or path of the icon resource for the tab.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Tab<TView>(
		string title,
		string icon) where TView : ContentView
	{
		Nodes.Add(new Leaf(registry.ViewModelOf<TView>(), title, icon, TabPlacement.Automatic));

		return this;
	}

	/// <summary>
	/// Adds a group of tabs: a collapsible section in the iPad sidebar, a drill-in tab on iPhone.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="icon">The group's SF Symbol.</param>
	/// <param name="children">Declares the tabs inside the group.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Group(
		string title,
		string icon,
		Action<GroupBuilder> children)
	{
		GroupBuilder group = new(registry);
		children(group);

		Nodes.Add(new GroupNode(title, icon, group.Nodes, TabPlacement.Automatic));

		return this;
	}

	/// <summary>
	/// Adds the system search tab: the separated bubble that morphs the bar into the search field.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Search<TView>() where TView : ContentView
	{
		SearchViewModel = registry.ViewModelOf<TView>();

		return this;
	}

	/// <summary>
	/// Puts an action button in the separated bubble instead of search. The bubble is single: Search and Action exclude each other.
	/// </summary>
	/// <param name="icon">The SF Symbol shown in the bubble.</param>
	/// <param name="tapped">Runs on tap.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Action(
		string icon,
		Action tapped)
	{
		ActionIcon = icon;
		ActionFactory = _ => tapped;

		return this;
	}

	/// <summary>
	/// Puts an action button in the separated bubble, firing a command from a ViewModel resolved from the services.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type carrying the command.</typeparam>
	/// <param name="icon">The SF Symbol shown in the bubble.</param>
	/// <param name="command">Picks the command off the ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Action<TViewModel>(
		string icon,
		Func<TViewModel, ICommand> command)
		where TViewModel : class
	{
		ActionIcon = icon;
		ActionFactory = services =>
		{
			ICommand resolved = command(services.GetRequiredService<TViewModel>());

			return () =>
			{
				if (resolved.CanExecute(null))
					resolved.Execute(null);
			};
		};

		return this;
	}

	/// <summary>
	/// Lets the tab bar minimize as the content scrolls. iOS 26 and later.
	/// </summary>
	/// <param name="minimize">When the bar minimizes.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Minimizes(
		TabBarMinimize minimize = TabBarMinimize.OnScrollDown)
	{
		Minimize = minimize;

		return this;
	}

	/// <summary>
	/// Shows a view of the given type in the tab bar's accessory slot. The view's IsVisible controls the slot. iOS 26 and later.
	/// </summary>
	/// <typeparam name="TView">The view type to host.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Accessory<TView>()
		where TView : View, new()
	{
		AccessoryFactory = () => new TView();

		return this;
	}

	/// <summary>
	/// Configures everything iPad: the sidebar, tab placements and iPad-only destinations. Ignored on iPhone.
	/// </summary>
	/// <param name="configure">A delegate to configure.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder OnIPad(
		Action<IPadTabsBuilder> configure)
	{
		IPad = new(registry);
		configure(IPad);

		return this;
	}
}

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
	/// <returns>The builder instance for chaining calls.</returns>
	public GroupBuilder Tab<TView>(
		string title,
		string icon) where TView : ContentView
	{
		Nodes.Add(new TabsBuilder.Leaf(registry.ViewModelOf<TView>(), title, icon, TabPlacement.Automatic));

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

		Nodes.Add(new TabsBuilder.GroupNode(title, icon, group.Nodes, TabPlacement.Automatic));

		return this;
	}
}

/// <summary>
/// Everything iPad: the sidebar, placements and iPad-only destinations.
/// </summary>
public sealed class IPadTabsBuilder
{
	readonly ViewRegistry registry;

	internal IPadTabsBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}

	internal bool UseSidebar { get; private set; }

	internal Dictionary<Type, TabPlacement> Placements { get; } = [];

	internal Dictionary<string, TabPlacement> GroupPlacements { get; } = [];

	internal List<TabsBuilder.Node> Nodes { get; } = [];

	internal Func<View>? FooterFactory { get; private set; }


	/// <summary>
	/// Shows the tabs as a sidebar.
	/// </summary>
	/// <returns>The builder instance for chaining calls.</returns>
	public IPadTabsBuilder Sidebar()
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
	public IPadTabsBuilder PlaceTab<TView>(
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
	public IPadTabsBuilder Tab<TView>(
		string title,
		string icon,
		TabPlacement placement = TabPlacement.SidebarOnly) where TView : ContentView
	{
		Nodes.Add(new TabsBuilder.Leaf(registry.ViewModelOf<TView>(), title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds an iPad-only group.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="icon">The group's SF Symbol.</param>
	/// <param name="children">Declares the tabs inside the group.</param>
	/// <param name="placement">How the group takes part in user customization.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public IPadTabsBuilder Group(
		string title,
		string icon,
		Action<GroupBuilder> children,
		TabPlacement placement = TabPlacement.SidebarOnly)
	{
		GroupBuilder group = new(registry);
		children(group);

		Nodes.Add(new TabsBuilder.GroupNode(title, icon, group.Nodes, placement));

		return this;
	}

	/// <summary>
	/// Overrides how a declared group takes part in user customization, by its title.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="placement">The placement to apply.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public IPadTabsBuilder PlaceGroup(
		string title,
		TabPlacement placement)
	{
		GroupPlacements[title] = placement;

		return this;
	}

	/// <summary>
	/// Shows a view of the given type at the sidebar's bottom. iOS 26 and later.
	/// </summary>
	/// <typeparam name="TView">The view type to host.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public IPadTabsBuilder SidebarFooter<TView>()
		where TView : View, new()
	{
		FooterFactory = () => new TView();

		return this;
	}
}
