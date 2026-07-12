namespace BareUI;

/// <summary>
/// Declares the app's tabs. Each tab hosts its own navigation stack.
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
	/// Adds a tab rooted at <typeparamref name="TView"/>, which must be registered in UsePages.
	/// </summary>
	public TabsBuilder Tab<TView>(
		string title,
		string icon)
		where TView : ContentView
	{
		Definitions.Add(new(registry.ViewModelOf<TView>(), title, icon));

		return this;
	}

	/// <summary>
	/// Uses large navigation-bar titles.
	/// </summary>
	public TabsBuilder LargeTitles()
	{
		UseLargeTitles = true;

		return this;
	}

	/// <summary>
	/// On iPad, shows the tabs as a sidebar instead of a tab bar.
	/// </summary>
	public TabsBuilder SidebarOnIPad()
	{
		UseSidebar = true;

		return this;
	}
}
