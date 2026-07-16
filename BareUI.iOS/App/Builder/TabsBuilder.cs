using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

/// <summary>
/// Declares the application's tabs.
/// </summary>
public sealed class TabsBuilder
{
	internal sealed record Definition(
		Type ViewModel,
		string Title,
		string Icon);


	readonly ViewRegistry registry;

	internal TabsBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}

	internal List<Definition> Definitions { get; } = [];


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
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Tab<TView>(
		string title,
		string icon) where TView : ContentView
	{
		Definitions.Add(new(registry.ViewModelOf<TView>(), title, icon));

		return this;
	}

	internal Func<IServiceProvider, View>? AccessoryFactory { get; private set; }

	/// <summary>
	/// Hosts a view in the tab bar's accessory slot — the app-global bar floating above the tabs, like Music's mini player. Toggle it with <see cref="INavigator.AccessoryVisible"/>. iOS 26 and later.
	/// </summary>
	/// <param name="create">Builds the accessory's view.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Accessory(
		Func<View> create)
	{
		AccessoryFactory = _ => create();

		return this;
	}

	/// <summary>
	/// Hosts a view in the tab bar's accessory slot, built around a ViewModel resolved from the services. iOS 26 and later.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type driving the accessory.</typeparam>
	/// <param name="create">Builds the accessory's view around its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public TabsBuilder Accessory<TViewModel>(
		Func<TViewModel, View> create)
		where TViewModel : class
	{
		AccessoryFactory = services => create(services.GetRequiredService<TViewModel>());

		return this;
	}
}
