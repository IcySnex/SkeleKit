using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

// ViewModel type -> page factory. Explicit, so no scanning and no reflection.
sealed class ViewRegistry
{
	readonly Dictionary<Type, Func<object, ContentView>> factories = [];

	// a probe instance reports its ViewModel type, so Map needs only the view's type
	public Type? Map<TView>()
		where TView : ContentView, new()
	{
		TView probe = new();

		if (probe.ViewModelType is not { } viewModel)
			return null;

		factories[viewModel] = instance =>
		{
			TView view = new();
			view.AttachViewModel(instance);

			return view;
		};

		return viewModel;
	}

	public ContentView CreatePage(
		object viewModel)
	{
		Type type = viewModel.GetType();

		if (!factories.TryGetValue(type, out Func<object, ContentView>? factory))
			throw new InvalidOperationException(
				$"No page is mapped to '{type.Name}'. Call BareApp.Map<YourView>() during startup.");

		return factory(viewModel);
	}

	public object CreateViewModel(
		Type viewModel,
		IServiceProvider services) =>
		services.GetRequiredService(viewModel);
}
