using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

/// <summary>
/// A builder used to configure and construct a <see cref="BareApplication"/>.
/// </summary>
public sealed class BareApplicationBuilder
{
	internal readonly ServiceCollection Services = [];
	internal readonly ViewRegistry Registry = new();

	internal BareApplication.ShellKind Shell = BareApplication.ShellKind.None;
	internal bool PreferLargeTitles;
	internal TabsBuilder? TabsBuilder;
	internal Type? RootViewModel;

	internal BareApplicationBuilder() { }


	/// <summary>
	/// Registers core dependencies and application services into the container.
	/// </summary>
	/// <param name="configure">A delegate to configure.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder UseServices(
		Action<IServiceCollection> configure)
	{
		configure(Services);
		return this;
	}

	/// <summary>
	/// Sets how <c>Image</c> loads remote URLs. Plug in a caching loader here.
	/// </summary>
	/// <param name="loader">The loader to use.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder UseImageLoader(
		IImageLoader loader)
	{
		Image.Loader = loader;
		return this;
	}

	/// <summary>
	/// Sets the app-wide accent color every control tints with.
	/// </summary>
	/// <param name="accent">The accent color.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder UseAccent(
		Color accent)
	{
		View.AppAccent = accent;
		return this;
	}

	/// <summary>
	/// Registers app lifecycle hooks, invoked as the app leaves for and returns from the background.
	/// </summary>
	/// <param name="background">Invoked when the app enters the background, or null.</param>
	/// <param name="foreground">Invoked when the app returns to the foreground, or null.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder UseLifecycle(
		Action? background = null,
		Action? foreground = null)
	{
		LifecycleBackground = background;
		LifecycleForeground = foreground;
		return this;
	}

	internal Action? LifecycleBackground { get; private set; }
	internal Action? LifecycleForeground { get; private set; }

	/// <summary>
	/// Registers implicit styles applied to every view of a type as it is built.
	/// </summary>
	/// <param name="configure">A delegate to configure.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder UseTheme(
		Action<Theme> configure)
	{
		Theme.Use(configure);
		return this;
	}

	/// <summary>
	/// Registers the pages the app can show.
	/// </summary>
	/// <param name="configure">A delegate to configure.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder UsePages(
		Action<PagesBuilder> configure)
	{
		configure(new(Registry));

		return this;
	}


	/// <summary>
	/// Configures the app to use as a single page without navigation chrome.
	/// </summary>
	/// <typeparam name="TView">The type of the root view.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder SinglePage<TView>() where TView : ContentView
	{
		RootViewModel = Registry.ViewModelOf<TView>();
		Shell = BareApplication.ShellKind.SinglePage;

		return this;
	}

	/// <summary>
	/// Configures the app to use a stack-based navigation hierarchy.
	/// </summary>
	/// <typeparam name="TView">The type of the root view.</typeparam>
	/// <param name="preferLargeTitles">Whether to enable large, collapsing titles.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder Stack<TView>(
		bool preferLargeTitles = false) where TView : ContentView
	{
		PreferLargeTitles = preferLargeTitles;

		RootViewModel = Registry.ViewModelOf<TView>();
		Shell = BareApplication.ShellKind.Stack;

		return this;
	}

	/// <summary>
	/// Configures the app to use bottom navigation tabs with each tab having its own navigation stack.
	/// </summary>
	/// <param name="configure">The delegate to configure.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public BareApplicationBuilder Tabs(
		Action<TabsBuilder> configure)
	{
		TabsBuilder = new(Registry);
		configure(TabsBuilder);

		Shell = BareApplication.ShellKind.Tabs;

		return this;
	}


	/// <summary>
	/// Builds and returns the configured application instance.
	/// </summary>
	/// <returns>The fully built application.</returns>
	/// <exception cref="InvalidOperationException">Thrown if a shell layout style has not been configured.</exception>
	public BareApplication Build()
	{
		if (Shell == BareApplication.ShellKind.None)
			throw new InvalidOperationException("Call Tabs(), Stack<TView>() or SinglePage<TView>() before Build().");

		return new(this);
	}
}
