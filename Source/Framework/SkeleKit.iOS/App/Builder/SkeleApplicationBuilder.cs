using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace SkeleKit;

/// <summary>
/// Configures services, pages, appearance and the application shell.
/// </summary>
public sealed class SkeleApplicationBuilder
{
	internal SkeleApplicationBuilder() { }


	internal readonly ServiceCollection Services = [];
	internal readonly ViewRegistry Registry = new();

	internal SkeleApplication.ShellKind Shell = SkeleApplication.ShellKind.None;
	internal bool PreferLargeTitles;
	internal TabsBuilder? TabsBuilder;
	internal Type? RootView;
	internal Color? Tint;
	internal Appearance Appearance;

	internal Action? LifecycleBackground { get; private set; }
	internal Action? LifecycleForeground { get; private set; }


	/// <summary>
	/// Registers core dependencies and application services into the container.
	/// </summary>
	/// <param name="configure">Adds services to the container.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseServices(
		Action<IServiceCollection> configure)
	{
		configure(Services);
		return this;
	}

	/// <summary>
	/// Sets how <c>Image</c> loads remote URLs.
	/// </summary>
	/// <param name="loader">The loader to use.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseImageLoader(
		IImageLoader loader)
	{
		Image.Loader = loader;
		return this;
	}

	/// <summary>
	/// Sets the initial app-wide tint inherited by windows, chrome and views.
	/// </summary>
	/// <param name="tint">The tint color.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseTint(
		Color tint)
	{
		Tint = tint;
		return this;
	}

	/// <summary>
	/// Sets the initial app-wide light or dark appearance.
	/// </summary>
	/// <param name="appearance">The initial appearance.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseAppearance(
		Appearance appearance)
	{
		Appearance = appearance;
		return this;
	}

	/// <summary>
	/// Registers app lifecycle hooks, invoked as the app leaves for and returns from the background.
	/// </summary>
	/// <param name="background">Invoked when the app enters the background, or null.</param>
	/// <param name="foreground">Invoked when the app returns to the foreground, or null.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseLifecycle(
		Action? background = null,
		Action? foreground = null)
	{
		LifecycleBackground = background;
		LifecycleForeground = foreground;
		return this;
	}

	/// <summary>
	/// Registers implicit styles applied to every view of a type as it is built.
	/// </summary>
	/// <param name="configure">Registers the implicit styles.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseTheme(
		Action<Theme> configure)
	{
		Theme.Use(configure);
		return this;
	}

	/// <summary>
	/// Registers or overrides pages by hand.
	/// </summary>
	/// <param name="configure">Registers the pages.</param>
	/// <param name="replace">Whether existing pages should be replaced.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UsePages(
		Action<PagesBuilder> configure,
		bool replace = true)
	{
		configure(new(Registry, replace));

		return this;
	}

	/// <summary>
	/// Configures the app to use as a single page without navigation chrome.
	/// </summary>
	/// <typeparam name="TView">The type of the root view.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder SinglePage<TView>() where TView : ContentView
	{
		RootView = typeof(TView);
		Shell = SkeleApplication.ShellKind.SinglePage;

		return this;
	}

	/// <summary>
	/// Configures the app to use a stack-based navigation hierarchy.
	/// </summary>
	/// <typeparam name="TView">The type of the root view.</typeparam>
	/// <param name="preferLargeTitles">Whether to enable large, collapsing titles.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder Stack<TView>(
		bool preferLargeTitles = false) where TView : ContentView
	{
		PreferLargeTitles = preferLargeTitles;

		RootView = typeof(TView);
		Shell = SkeleApplication.ShellKind.Stack;

		return this;
	}

	/// <summary>
	/// Configures the app to use bottom navigation tabs with each tab having its own navigation stack.
	/// </summary>
	/// <param name="configure">Declares the tabs.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder Tabs(
		Action<TabsBuilder> configure)
	{
		TabsBuilder = new();
		configure(TabsBuilder);

		Shell = SkeleApplication.ShellKind.Tabs;

		return this;
	}


	/// <summary>
	/// Builds and returns the configured application instance.
	/// </summary>
	/// <returns>The fully built application.</returns>
	/// <exception cref="InvalidOperationException">Thrown if a shell layout style has not been configured.</exception>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SkeleApplication BuildCore()
	{
		if (Shell == SkeleApplication.ShellKind.None)
			throw new InvalidOperationException("Call Tabs(), Stack<TView>() or SinglePage<TView>() before Build().");

		if (RootView is Type root)
			Registry.EnsureRegistered(root);

		TabsBuilder?.Validate(Registry);

		return new(this);
	}
}
