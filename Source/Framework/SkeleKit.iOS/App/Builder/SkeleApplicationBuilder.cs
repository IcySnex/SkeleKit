using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace SkeleKit;

/// <summary>
/// Configures services, pages, appearance and the application shell.
/// </summary>
public sealed class SkeleApplicationBuilder
{
	internal SkeleApplicationBuilder()
	{
		Services.AddSingleton<IConfiguration>(Configuration);
		Services.AddLogging();
	}


	internal readonly ServiceCollection Services = [];
	internal readonly ConfigurationManager Configuration = new();
	internal readonly ViewRegistry Registry = new();

	internal SkeleApplication.ShellKind Shell = SkeleApplication.ShellKind.None;
	internal TabsBuilder? TabsBuilder;
	internal Type? RootView;
	internal readonly ThemeBuilder Theme = new();

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
	/// Adds sources to the shared application configuration.
	/// </summary>
	/// <param name="configure">Configures application configuration sources.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder ConfigureAppConfiguration(
		Action<IConfigurationBuilder> configure)
	{
		configure(Configuration);
		return this;
	}

	/// <summary>
	/// Configures Microsoft logging for application and framework services.
	/// </summary>
	/// <param name="configure">Adds and configures logging providers.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder ConfigureLogging(
		Action<ILoggingBuilder> configure)
	{
		Services.AddLogging(configure);
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
	/// Configures the app-wide theme inherited by windows, chrome and pages.
	/// </summary>
	/// <param name="configure">Sets the theme's initial values.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseTheme(
		Action<ThemeBuilder> configure)
	{
		configure(Theme);
		return this;
	}

	/// <summary>
	/// Registers a singleton application lifecycle service.
	/// </summary>
	/// <typeparam name="TLifecycle">The lifecycle service type.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseLifecycle<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TLifecycle>()
		where TLifecycle : class, IApplicationLifecycle
	{
		Services.TryAddSingleton<TLifecycle>();
		Services.AddSingleton<IApplicationLifecycle>(provider =>
			provider.GetRequiredService<TLifecycle>());

		return this;
	}

	/// <summary>
	/// Registers implicit styles applied to every view of a type as it is built.
	/// </summary>
	/// <param name="configure">Registers the implicit styles.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder UseStyles(
		Action<Styles> configure)
	{
		Styles.Use(configure);
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
	/// <returns>The builder instance for chaining calls.</returns>
	public SkeleApplicationBuilder Stack<TView>() where TView : ContentView
	{
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
