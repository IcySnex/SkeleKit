using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

sealed record PageRegistration(
	Func<object, ContentView> Create,
	bool Singleton)
{
	public ContentView? Instance { get; set; }
}

// ViewModel type -> page factory. Explicit, so no scanning and no reflection.
sealed class ViewRegistry
{
	readonly Dictionary<Type, PageRegistration> byViewModel = [];
	readonly Dictionary<Type, Type> viewModelByView = [];

	// a probe instance reports its ViewModel type, so registration needs only the view's type
	public void Add<TView>(
		bool singleton)
		where TView : ContentView, new()
	{
		TView probe = new();
		Type viewModel = probe.ViewModelType;

		byViewModel[viewModel] = new(
			instance =>
			{
				TView view = new();
				view.AttachViewModel(instance);

				return view;
			},
			singleton);

		viewModelByView[typeof(TView)] = viewModel;
	}

	/// <summary>
	/// The ViewModel a registered view renders.
	/// </summary>
	public Type ViewModelOf<TView>()
		where TView : ContentView
	{
		if (!viewModelByView.TryGetValue(typeof(TView), out Type? viewModel))
			throw new InvalidOperationException(
				$"'{typeof(TView).Name}' is not registered. Add it in UsePages(...).");

		return viewModel;
	}

	public ContentView CreatePage(
		object viewModel)
	{
		Type type = viewModel.GetType();

		if (!byViewModel.TryGetValue(type, out PageRegistration? registration))
			throw new InvalidOperationException(
				$"No page is registered for '{type.Name}'. Add its view in UsePages(...).");

		if (!registration.Singleton)
			return registration.Create(viewModel);

		return registration.Instance ??= registration.Create(viewModel);
	}

	public object CreateViewModel(
		Type viewModel,
		IServiceProvider services) =>
		services.GetRequiredService(viewModel);
}
