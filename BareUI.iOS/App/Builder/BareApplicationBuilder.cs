using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

public sealed class BareApplicationBuilder
{
	internal readonly ServiceCollection Services = [];
	internal readonly ViewRegistry Registry = new();

	internal BareApplication.ShellKind Shell = BareApplication.ShellKind.None;
	internal bool PreferLargeTitles = false;
	internal TabsBuilder? TabsBuilder;
	internal Type? RootViewModel;

	internal BareApplicationBuilder() { }


	/// <summary>
	/// Registers services. Use explicit factories to stay trim-safe.
	/// </summary>
	public BareApplicationBuilder UseServices(
		Action<IServiceCollection> configure)
	{
		configure(Services);
		return this;
	}

	/// <summary>
	/// Sets how <c>Image</c> loads remote URLs. Plug in a caching loader here.
	/// </summary>
	public BareApplicationBuilder UseImageLoader(
		IImageLoader loader)
	{
		Image.Loader = loader;
		return this;
	}

	/// <summary>
	/// Registers implicit styles applied to every view of a type as it is built. One theme per app.
	/// </summary>
	public BareApplicationBuilder UseTheme(
		Action<Theme> configure)
	{
		Theme.Use(configure);
		return this;
	}

	/// <summary>
	/// Registers the pages the app can show. Every navigable page goes here, once.
	/// </summary>
	public BareApplicationBuilder UsePages(
		Action<PagesBuilder> configure)
	{
		configure(new(Registry));

		return this;
	}


	/// <summary>
	/// A single page with no navigation bar.
	/// </summary>
	public BareApplicationBuilder SinglePage<TView>() where TView : ContentView
	{
		RootViewModel = Registry.ViewModelOf<TView>();
		Shell = BareApplication.ShellKind.SinglePage;

		return this;
	}

	/// <summary>
	/// One navigation stack rooted at <typeparamref name="TView"/>.
	/// </summary>
	public BareApplicationBuilder Stack<TView>(
		bool preferLargeTitles = false) where TView : ContentView
	{
		PreferLargeTitles = preferLargeTitles;

		RootViewModel = Registry.ViewModelOf<TView>();
		Shell = BareApplication.ShellKind.Stack;

		return this;
	}

	/// <summary>
	/// A tab bar; each tab gets its own navigation stack.
	/// </summary>
	public BareApplicationBuilder Tabs(
		Action<TabsBuilder> configure)
	{
		TabsBuilder = new(Registry);
		configure(TabsBuilder);

		Shell = BareApplication.ShellKind.Tabs;

		return this;
	}


	/// <summary>
	/// Freezes configuration and constructs the runtime App instance.
	/// </summary>
	public BareApplication Build()
	{
		if (Shell == BareApplication.ShellKind.None)
			throw new InvalidOperationException("Call Tabs(), Stack<TView>() or SinglePage<TView>() before Build().");

		return new(this);
	}
}
