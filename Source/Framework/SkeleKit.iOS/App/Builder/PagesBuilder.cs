namespace SkeleKit;

/// <summary>
/// Registers the pages available in the application.
/// </summary>
public sealed class PagesBuilder
{
	readonly ViewRegistry registry;
	readonly bool replace;

	internal PagesBuilder(
		ViewRegistry registry,
		bool replace)
	{
		this.registry = registry;
		this.replace = replace;
	}


	/// <summary>
	/// Registers a view-only page that is recreated for each presentation.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TView>()
		where TView : ContentView, new() =>
		AddTransient(() => new TView());

	/// <summary>
	/// Registers a view-only page that is recreated for each presentation.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TView>(
		Func<TView> create)
		where TView : ContentView =>
		AddTransient<TView>(_ => create());

	/// <summary>
	/// Registers a view-only page that is recreated for each presentation.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page from the application's services.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TView>(
		Func<IServiceProvider, TView> create)
		where TView : ContentView
	{
		registry.Add(create, singleton: false, replace);

		return this;
	}

	/// <summary>
	/// Registers a ViewModel-backed page that is recreated for each presentation.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type.</typeparam>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page from its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TViewModel, TView>(
		Func<TViewModel, TView> create)
		where TViewModel : class
		where TView : ContentView =>
		AddTransient<TViewModel, TView>((_, viewModel) => create(viewModel));

	/// <summary>
	/// Registers a ViewModel-backed page that is recreated for each presentation.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type.</typeparam>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page from the application's services and its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TViewModel, TView>(
		Func<IServiceProvider, TViewModel, TView> create)
		where TViewModel : class
		where TView : ContentView
	{
		registry.Add(create, singleton: false, replace);

		return this;
	}

	/// <summary>
	/// Registers a view-only page built once and kept for the application's lifetime.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TView>()
		where TView : ContentView, new() =>
		AddSingleton(() => new TView());

	/// <summary>
	/// Registers an existing view-only page for the application's lifetime.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="instance">The page instance to register.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TView>(
		TView instance)
		where TView : ContentView
	{
		ArgumentNullException.ThrowIfNull(instance);

		return AddSingleton(() => instance);
	}

	/// <summary>
	/// Registers a view-only page built once and kept for the application's lifetime.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TView>(
		Func<TView> create)
		where TView : ContentView =>
		AddSingleton<TView>(_ => create());

	/// <summary>
	/// Registers a view-only page built once and kept for the application's lifetime.
	/// </summary>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page from the application's services.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TView>(
		Func<IServiceProvider, TView> create)
		where TView : ContentView
	{
		registry.Add(create, singleton: true, replace);

		return this;
	}

	/// <summary>
	/// Registers a ViewModel-backed page built once and kept for the application's lifetime.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type.</typeparam>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page from its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TViewModel, TView>(
		Func<TViewModel, TView> create)
		where TViewModel : class
		where TView : ContentView =>
		AddSingleton<TViewModel, TView>((_, viewModel) => create(viewModel));

	/// <summary>
	/// Registers a ViewModel-backed page built once and kept for the application's lifetime.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type.</typeparam>
	/// <typeparam name="TView">The page type.</typeparam>
	/// <param name="create">Creates the page from the application's services and its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TViewModel, TView>(
		Func<IServiceProvider, TViewModel, TView> create)
		where TViewModel : class
		where TView : ContentView
	{
		registry.Add(create, singleton: true, replace);

		return this;
	}
}
