namespace BareUI;

/// <summary>
/// Registers every page the app can show. Shells (tabs, stacks) only reference pages registered here.
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
	/// A fresh page instance per navigation. The usual choice.
	/// </summary>
	public PagesBuilder AddTransient<TView>()
		where TView : ContentView, new()
	{
		registry.Add<TView>(singleton: false);

		return this;
	}

	/// <summary>
	/// One page instance reused for the app's lifetime, so it keeps its state (scroll position, input).
	/// </summary>
	public PagesBuilder AddSingleton<TView>()
		where TView : ContentView, new()
	{
		registry.Add<TView>(singleton: true);

		return this;
	}
}
