using Microsoft.Extensions.DependencyInjection;

namespace SkeleKit;

internal sealed class ViewRegistry
{
	sealed record PageRegistration(
		Type View,
		Type? ViewModel,
		Func<IServiceProvider, object?, ContentView> Create,
		bool Singleton)
	{
		public ContentView? Instance { get; set; }
	}


	readonly Dictionary<Type, PageRegistration> byView = [];
	readonly Dictionary<Type, PageRegistration> byViewModel = [];


	void Store(
		PageRegistration registration,
		bool replace)
	{
		if (!replace && byView.ContainsKey(registration.View))
			return;

		if (byView.TryGetValue(registration.View, out PageRegistration? previous)
			&& previous.ViewModel is Type previousViewModel
			&& byViewModel.TryGetValue(previousViewModel, out PageRegistration? mapped)
			&& ReferenceEquals(previous, mapped))
			byViewModel.Remove(previousViewModel);

		byView[registration.View] = registration;

		if (registration.ViewModel is Type viewModel)
			byViewModel[viewModel] = registration;
	}

	public void Add<TView>(
		Func<IServiceProvider, TView> create,
		bool singleton,
		bool replace)
		where TView : ContentView =>
		Store(
			new(
				typeof(TView),
				null,
				(services, _) => create(services),
				singleton),
			replace);

	public void Add<TViewModel, TView>(
		Func<IServiceProvider, TViewModel, TView> create,
		bool singleton,
		bool replace)
		where TViewModel : class
		where TView : ContentView =>
		Store(
			new(
				typeof(TView),
				typeof(TViewModel),
				(services, viewModel) => create(services, (TViewModel)viewModel!),
				singleton),
			replace);

	public void EnsureRegistered(
		Type view)
	{
		if (!byView.ContainsKey(view))
			throw new InvalidOperationException($"'{view.Name}' is not registered. Add [Page] or register it in UsePages(...).");
	}

	ContentView Create(
		PageRegistration registration,
		IServiceProvider services,
		object? viewModel = null,
		bool recreate = false)
	{
		if (!recreate && registration.Singleton && registration.Instance is ContentView existing)
			return existing;

		if (registration.ViewModel is Type viewModelType && viewModel is null)
			viewModel = services.GetRequiredService(viewModelType);

		ContentView page = registration.Create(services, viewModel);

		if (registration.Singleton)
			registration.Instance = page;

		return page;
	}

	public ContentView CreatePage(
		Type view,
		IServiceProvider services)
	{
		if (!byView.TryGetValue(view, out PageRegistration? registration))
			throw new InvalidOperationException($"'{view.Name}' is not registered. Add [Page] or register it in UsePages(...).");

		return Create(registration, services);
	}

	public ContentView CreatePage(
		object viewModel,
		IServiceProvider services)
	{
		Type type = viewModel.GetType();

		if (!byViewModel.TryGetValue(type, out PageRegistration? registration))
			throw new InvalidOperationException($"No page is registered for '{type.Name}'. Add [Page] or register its view in UsePages(...).");

		return Create(registration, services, viewModel);
	}

	public ContentView RecreatePage(
		ContentView page,
		IServiceProvider services)
	{
		Type type = page.GetType();

		if (!byView.TryGetValue(type, out PageRegistration? registration))
			throw new InvalidOperationException($"'{type.Name}' is not registered. Add [Page] or register it in UsePages(...).");

		object? viewModel = registration.ViewModel is null
			? null
			: page.BindingContext;

		return Create(registration, services, viewModel, recreate: true);
	}

#pragma warning disable CA1822
	public object CreateViewModel(
		Type viewModel,
		IServiceProvider services) =>
		services.GetRequiredService(viewModel);
#pragma warning restore CA1822
}
