using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace SkeleKit;

/// <summary>
/// Declares the application's tabs.
/// </summary>
public sealed class TabsBuilder
{
	internal abstract record Node(
		TabPlacement Placement);

	internal sealed record Leaf(
		Type View,
		string Title,
		ImageSource Icon,
		TabPlacement Placement) : Node(Placement);

	internal sealed record SplitLeaf(
		SplitViewBuilder Split,
		string Title,
		ImageSource Icon,
		TabPlacement Placement) : Node(Placement);

	internal sealed record GroupNode(
		string Title,
		ImageSource Icon,
		List<Node> Children,
		TabPlacement Placement) : Node(Placement);


	internal List<Node> Nodes { get; } = [];
	internal Type? SearchView { get; private set; }
	internal bool SearchBubble { get; private set; } = true;
	internal ImageSource? BubbleIcon { get; private set; }
	internal Func<IServiceProvider, Action>? BubbleFactory { get; private set; }
	internal Type? BubbleView { get; private set; }
	internal string? BubbleTitle { get; private set; }
	internal Func<View>? AccessoryFactory { get; private set; }
	internal SidebarBuilder? SidebarConfiguration { get; private set; }


	/// <summary>
	/// Adds a tab page to the navigation structure.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="title">The text displayed on the tab bar item.</param>
	/// <param name="icon">The local icon shown on the tab.</param>
	/// <param name="placement">How the destination participates in the tab bar and sidebar.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Tab<TView>(
		string title,
		ImageSource icon,
		TabPlacement placement = TabPlacement.Automatic) where TView : ContentView
	{
		Nodes.Add(new Leaf(typeof(TView), title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds a group of related destinations to the tab hierarchy.
	/// </summary>
	/// <param name="title">The group's title.</param>
	/// <param name="icon">The local icon shown for the group.</param>
	/// <param name="children">Declares the destinations inside the group.</param>
	/// <param name="placement">How the group participates in the tab bar and sidebar.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Group(
		string title,
		ImageSource icon,
		Action<GroupBuilder> children,
		TabPlacement placement = TabPlacement.SidebarOnly)
	{
		GroupBuilder group = new();
		children(group);

		Nodes.Add(new GroupNode(title, icon, group.Nodes, placement));

		return this;
	}

	/// <summary>
	/// Adds a native split view as a tab destination.
	/// </summary>
	/// <param name="title">The text displayed on the tab bar item.</param>
	/// <param name="icon">The local icon shown on the tab.</param>
	/// <param name="configure">Declares the split view's columns and native configuration.</param>
	/// <param name="placement">How the destination takes part in tab and sidebar customization.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Split(
		string title,
		ImageSource icon,
		Action<SplitViewBuilder> configure,
		TabPlacement placement = TabPlacement.Automatic)
	{
		SplitViewBuilder split = new();
		configure(split);
		Nodes.Add(new SplitLeaf(split, title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds the system search destination. It is a regular tab on iOS 18 and uses the separated search presentation on iOS 26.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="bubble">Whether to put the search tab in the system bubble.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Search<TView>(
		bool bubble = true) where TView : ContentView
	{
		SearchView = typeof(TView);
		SearchBubble = bubble;

		return this;
	}

	/// <summary>
	/// Puts a destination page in the separated bubble on iOS 26, with a regular tab fallback on earlier versions.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the bubble.</typeparam>
	/// <param name="title">The title, shown in the sidebar and read by VoiceOver.</param>
	/// <param name="icon">The local icon shown in the bubble.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Bubble<TView>(
		string title,
		ImageSource icon) where TView : ContentView
	{
		BubbleTitle = title;
		BubbleIcon = icon;
		BubbleView = typeof(TView);

		return this;
	}

	/// <summary>
	/// Puts an action button in the separated bubble instead of search, with a regular tab-shaped action on earlier versions.
	/// </summary>
	/// <remarks>
	/// The bubble is single: Search and Bubble exclude each other.
	/// </remarks>
	/// <param name="title">The title, shown in the sidebar and read by VoiceOver.</param>
	/// <param name="icon">The local icon shown in the bubble.</param>
	/// <param name="tapped">Runs on tap.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Bubble(
		string title,
		ImageSource icon,
		Action tapped)
	{
		BubbleTitle = title;
		BubbleIcon = icon;
		BubbleFactory = _ => tapped;

		return this;
	}

	/// <summary>
	/// Puts an action button in the separated bubble, firing a command from a ViewModel resolved from the services.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type carrying the command.</typeparam>
	/// <param name="title">The title, shown in the sidebar and read by VoiceOver.</param>
	/// <param name="icon">The local icon shown in the bubble.</param>
	/// <param name="command">Picks the command off the ViewModel.</param>
	/// <param name="commandParameter">The parameter passed to the command.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Bubble<TViewModel>(
		string title,
		ImageSource icon,
		Func<TViewModel, ICommand> command,
		object? commandParameter = null)
		where TViewModel : class
	{
		BubbleTitle = title;
		BubbleIcon = icon;
		BubbleFactory = services =>
		{
			ICommand resolved = command(services.GetRequiredService<TViewModel>());

			return () =>
			{
				if (resolved.CanExecute(commandParameter))
					resolved.Execute(commandParameter);
			};
		};

		return this;
	}

	/// <summary>
	/// Shows a view of the given type in the tab bar's accessory slot.
	/// </summary>
	/// <remarks>
	/// The view's IsVisible controls the slot. iOS 26 and later.
	/// </remarks>
	/// <typeparam name="TView">The view type to host.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Accessory<TView>()
		where TView : View, new()
	{
		AccessoryFactory = () => new TView();

		return this;
	}

	/// <summary>
	/// Enables the adaptive sidebar and configures its presentation.
	/// </summary>
	/// <remarks>
	/// Compact environments keep the tab bar. When a sidebar is available, UIKit switches to it without rebuilding the tabs.
	/// </remarks>
	/// <param name="configure">Optionally configures the sidebar layout.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Sidebar(
		Action<SidebarBuilder>? configure = null)
	{
		SidebarConfiguration = new();
		configure?.Invoke(SidebarConfiguration);

		return this;
	}


	static IEnumerable<Type> Views(
		IEnumerable<Node> nodes)
	{
		foreach (Node node in nodes)
		{
			switch (node)
			{
				case Leaf leaf:
					yield return leaf.View;
					break;
				case SplitLeaf split:
					foreach (Type view in split.Split.Views)
						yield return view;
					break;
				case GroupNode group:
					{
						foreach (Type view in Views(group.Children))
							yield return view;
						break;
					}
			}
		}
	}

	static IEnumerable<SplitViewBuilder> Splits(
		IEnumerable<Node> nodes)
	{
		foreach (Node node in nodes)
		{
			switch (node)
			{
				case SplitLeaf split:
					yield return split.Split;
					break;
				case GroupNode group:
					foreach (SplitViewBuilder child in Splits(group.Children))
						yield return child;
					break;
			}
		}
	}

	internal void Validate(
		ViewRegistry registry)
	{
		if (SidebarConfiguration is null)
		{
			if (Nodes.Any(node => node is GroupNode))
				throw new InvalidOperationException("Tab groups require Sidebar() to enable the adaptive sidebar.");
			if (Nodes.Any(node => node.Placement is TabPlacement.SidebarOnly))
				throw new InvalidOperationException("A sidebar-only destination requires Sidebar() to enable the adaptive sidebar.");
		}

		foreach (SplitViewBuilder split in Splits(Nodes))
			split.Validate(registry);

		foreach (Type view in Views(Nodes)
			.Append(SearchView)
			.Append(BubbleView)
			.OfType<Type>()
			.Distinct())
			registry.EnsureRegistered(view);
	}
}
