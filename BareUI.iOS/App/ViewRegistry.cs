using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

// ViewModel type -> page factory. Explicit, so no scanning and no reflection.
sealed class ViewRegistry
{
	readonly Dictionary<Type, Func<object, ContentView>> factories = [];

	public void Map<TViewModel, TView>()
		where TViewModel : class
		where TView : ContentView<TViewModel>, new() =>
		factories[typeof(TViewModel)] = viewModel => new TView { ViewModel = (TViewModel)viewModel };

	public ContentView CreatePage(
		object viewModel)
	{
		Type type = viewModel.GetType();

		if (!factories.TryGetValue(type, out Func<object, ContentView>? factory))
			throw new InvalidOperationException(
				$"No page is mapped to '{type.Name}'. Call BareApp.Map<{type.Name}, YourPage>() during startup.");

		return factory(viewModel);
	}

	public object CreateViewModel<TViewModel>(
		IServiceProvider services)
		where TViewModel : class =>
		services.GetRequiredService<TViewModel>();
}
