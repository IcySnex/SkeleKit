using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace SkeleKit;

/// <summary>
/// Declares the application's tabs.
/// </summary>
public sealed class TabsBuilder
{
	internal abstract record Node;

	internal sealed record Leaf(
		Type View,
		string Title,
		string Icon,
		TabPlacement Placement) : Node;

	internal sealed record GroupNode(
		string Title,
		string Icon,
		List<Node> Children) : Node;


	internal List<Node> Nodes { get; } = [];
	internal Type? SearchView { get; private set; }
	internal string? BubbleIcon { get; private set; }
	internal Func<IServiceProvider, Action>? BubbleFactory { get; private set; }
	internal Type? BubbleView { get; private set; }
	internal string? BubbleTitle { get; private set; }
	internal TabBarMinimize Minimize { get; private set; } = TabBarMinimize.Never;
	internal Func<View>? AccessoryFactory { get; private set; }
	internal PadTabsBuilder? Pad { get; private set; }
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
		Nodes.Add(new Leaf(typeof(TView), title, icon, TabPlacement.Automatic));

		return this;
	}

	/// <summary>
	/// Adds the system search tab: the separated bubble that morphs the bar into the search field.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Search<TView>() where TView : ContentView
	{
		SearchView = typeof(TView);

		return this;
	}

	/// <summary>
	/// Puts a destination page in the separated bubble: selecting it shows the page with native selection.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the bubble.</typeparam>
	/// <param name="title">The title, shown in the sidebar and read by VoiceOver.</param>
	/// <param name="icon">The SF Symbol shown in the bubble.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Bubble<TView>(
		string title,
		string icon) where TView : ContentView
	{
		BubbleTitle = title;
		BubbleIcon = icon;
		BubbleView = typeof(TView);

		return this;
	}

	/// <summary>
	/// Puts an action button in the separated bubble instead of search.
	/// </summary>
	/// <remarks>
	/// The bubble is single: Search and Bubble exclude each other.
	/// </remarks>
	/// <param name="title">The title, shown in the sidebar and read by VoiceOver.</param>
	/// <param name="icon">The SF Symbol shown in the bubble.</param>
	/// <param name="tapped">Runs on tap.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Bubble(
		string title,
		string icon,
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
	/// <param name="icon">The SF Symbol shown in the bubble.</param>
	/// <param name="command">Picks the command off the ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Bubble<TViewModel>(
		string title,
		string icon,
		Func<TViewModel, ICommand> command)
		where TViewModel : class
	{
		BubbleTitle = title;
		BubbleIcon = icon;
		BubbleFactory = services =>
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
	/// Lets the tab bar minimize as the content scrolls.
	/// </summary>
	/// <remarks>
	/// iOS 26 and later.
	/// </remarks>
	/// <param name="minimize">When the bar minimizes.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Minimizes(
		TabBarMinimize minimize = TabBarMinimize.OnScrollDown)
	{
		Minimize = minimize;

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
	/// Configures everything iPad: the sidebar, tab placements and iPad-only destinations.
	/// </summary>
	/// <remarks>
	/// Ignored on iPhone.
	/// </remarks>
	/// <param name="configure">Configures the iPad layout.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder OnPad(
		Action<PadTabsBuilder> configure)
	{
		Pad = new();
		configure(Pad);

		return this;
	}


	static IEnumerable<Type> Views(
		IEnumerable<Node> nodes)
	{
		foreach (Node node in nodes)
		{
			if (node is Leaf leaf)
				yield return leaf.View;
			else if (node is GroupNode group)
			{
				foreach (Type view in Views(group.Children))
					yield return view;
			}
		}
	}

	internal void Validate(
		ViewRegistry registry)
	{
		foreach (Type view in Views(Nodes)
			.Concat(Pad is null ? [] : Views(Pad.Nodes))
			.Append(SearchView)
			.Append(BubbleView)
			.OfType<Type>()
			.Distinct())
			registry.EnsureRegistered(view);
	}
}
