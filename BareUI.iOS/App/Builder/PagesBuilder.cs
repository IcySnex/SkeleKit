namespace BareUI;

/// <summary>
/// Registers the pages available in the application.
/// </summary>
public sealed class PagesBuilder
{
	readonly ViewRegistry registry;

	internal PagesBuilder(
		ViewRegistry registry)
	{
		this.registry = registry;
	}


	/// <summary>
	/// Registers a view that is recreated every time its ViewModel is navigated to.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type driving the view.</typeparam>
	/// <typeparam name="TView">The view type to register.</typeparam>
	/// <param name="create">Constructs the view around its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TViewModel, TView>(
		Func<TViewModel, TView> create)
		where TViewModel : class
		where TView : ContentView
	{
		registry.Add(create, singleton: false);

		return this;
	}

	/// <summary>
	/// Registers a view built once and kept for the app's lifetime, together with its ViewModel.
	/// </summary>
	/// <typeparam name="TViewModel">The ViewModel type driving the view.</typeparam>
	/// <typeparam name="TView">The view type to register.</typeparam>
	/// <param name="create">Constructs the view around its ViewModel.</param>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TViewModel, TView>(
		Func<TViewModel, TView> create)
		where TViewModel : class
		where TView : ContentView
	{
		registry.Add(create, singleton: true);

		return this;
	}
}
