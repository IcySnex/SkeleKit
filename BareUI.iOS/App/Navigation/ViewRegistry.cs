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


	// the factory keeps page construction reflection-free: the ViewModel goes in by constructor
	public void Add<TViewModel, TView>(
		Func<TViewModel, TView> create,
		bool singleton)
		where TViewModel : class
		where TView : ContentView
	{
		byViewModel[typeof(TViewModel)] = new(
			instance => create((TViewModel)instance),
			singleton);

		viewModelByView[typeof(TView)] = typeof(TViewModel);
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

		if (!byViewModel.TryGetValue(type, out PageRegistration? registration))
			throw new InvalidOperationException($"No page is registered for '{type.Name}'. Add its view in UsePages(...).");

		if (!registration.Singleton)
			return registration.Create(viewModel);

		return registration.Instance ??= registration.Create(viewModel);
	}

#pragma warning disable CA1822
	public object CreateViewModel(
		Type viewModel,
		IServiceProvider services) =>
		services.GetRequiredService(viewModel);
#pragma warning enable CA1822
}
