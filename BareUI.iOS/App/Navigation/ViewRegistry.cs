#pragma warning disable CA1822

using Microsoft.Extensions.DependencyInjection;

namespace BareUI;

internal sealed class ViewRegistry
{
	sealed record PageRegistration(
		Func<object, ContentView> Create,
		bool Singleton)
	{
		public ContentView? Instance { get; set; }
	}


	readonly Dictionary<Type, PageRegistration> byViewModel = [];
	readonly Dictionary<Type, Type> viewModelByView = [];


	public void Add<TView>(
		bool singleton) where TView : ContentView, new()
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


	public Type ViewModelOf<TView>() where TView : ContentView
	{
		if (!viewModelByView.TryGetValue(typeof(TView), out Type? viewModel))
			throw new InvalidOperationException($"'{typeof(TView).Name}' is not registered. Add it in UsePages(...).");

		return viewModel;
	}


	public ContentView CreatePage(
		object viewModel)
	{
		Type? type = viewModel.GetType();

		if (type is null || !byViewModel.TryGetValue(type, out PageRegistration? registration))
			throw new InvalidOperationException($"No page is registered for '{type?.Name}'. Add its view in UsePages(...).");

		if (!registration.Singleton)
			return registration.Create(viewModel);

		return registration.Instance ??= registration.Create(viewModel);
	}

	public object CreateViewModel(
		Type viewModel,
		IServiceProvider services) =>
		services.GetRequiredService(viewModel);
}
