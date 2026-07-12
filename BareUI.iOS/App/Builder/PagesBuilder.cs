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
	/// Registers a view that is recreated every time it is resolved.
	/// </summary>
	/// <typeparam name="TView">The view type to register.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddTransient<TView>()
		where TView : ContentView, new()
	{
		registry.Add<TView>(singleton: false);

		return this;
	}

	/// <summary>
	/// Registers a view that keeps a single shared instance throughout the lifecycle.
	/// </summary>
	/// <typeparam name="TView">The view type to register.</typeparam>
	/// <returns>The builder instance for chaining calls.</returns>
	public PagesBuilder AddSingleton<TView>()
		where TView : ContentView, new()
	{
		registry.Add<TView>(singleton: true);

		return this;
	}
}
