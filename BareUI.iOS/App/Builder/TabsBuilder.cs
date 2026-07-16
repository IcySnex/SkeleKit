namespace BareUI;

/// <summary>
/// Declares the application's tabs.
/// </summary>
public sealed class TabsBuilder
{
	internal sealed record Definition(
		Type ViewModel,
		string Title,
		string Icon,
		TabPlacement Placement);


	readonly ViewRegistry registry;

	internal TabsBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}

	internal List<Definition> Definitions { get; } = [];

	internal Type? SearchViewModel { get; private set; }

	internal TabBarMinimize Minimize { get; private set; } = TabBarMinimize.Never;


	internal bool UseLargeTitles { get; private set; }

	internal bool UseSidebar { get; private set; }


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
	/// Transforms the tab layout into a sidebar layout on iPad.
	/// </summary>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder SidebarOnIPad()
	{
		UseSidebar = true;

		return this;
	}

	/// <summary>
	/// Adds a tab page to the navigation structure.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <param name="title">The text displayed on the tab bar item.</param>
	/// <param name="icon">The name or path of the icon resource for the tab.</param>
	/// <param name="placement">How the tab takes part in iPad user customization.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Tab<TView>(
		string title,
		string icon,
		TabPlacement placement = TabPlacement.Automatic) where TView : ContentView
	{
		Definitions.Add(new(registry.ViewModelOf<TView>(), title, icon, placement));

		return this;
	}

	/// <summary>
	/// Adds the system search tab: it sits apart from the other tabs and morphs the bar into the search field.
	/// </summary>
	/// <typeparam name="TView">The type of the content view to host in the tab.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder SearchTab<TView>() where TView : ContentView
	{
		SearchViewModel = registry.ViewModelOf<TView>();

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

	internal Func<View>? AccessoryFactory { get; private set; }

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
}
