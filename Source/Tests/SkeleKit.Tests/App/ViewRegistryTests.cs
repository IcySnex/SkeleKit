using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SkeleKit.Tests.App;

public class ViewRegistryTests
{
	sealed class ViewModel
	{
	}

	sealed class ViewOnlyPage : ContentView
	{
		public string Source { get; init; } = "";
	}

	sealed class ViewModelPage(
		ViewModel viewModel) : ContentView<ViewModel>(viewModel);


	static ServiceProvider Services() =>
		new ServiceCollection()
			.AddTransient<ViewModel>()
			.BuildServiceProvider();


	[Fact]
	public void ViewOnlyTransientCreatesEachTime()
	{
		ViewRegistry registry = new();
		new PagesBuilder(registry, replace: true)
			.AddTransient(() => new ViewOnlyPage());

		using ServiceProvider services = Services();

		ContentView first = registry.CreatePage(typeof(ViewOnlyPage), services);
		ContentView second = registry.CreatePage(typeof(ViewOnlyPage), services);

		Assert.IsType<ViewOnlyPage>(first);
		Assert.NotSame(first, second);
	}

	[Fact]
	public void ViewOnlySingletonIsReused()
	{
		ViewRegistry registry = new();
		ViewOnlyPage instance = new();
		new PagesBuilder(registry, replace: true)
			.AddSingleton(instance);

		using ServiceProvider services = Services();

		ContentView first = registry.CreatePage(typeof(ViewOnlyPage), services);
		ContentView second = registry.CreatePage(typeof(ViewOnlyPage), services);

		Assert.Same(instance, first);
		Assert.Same(first, second);
	}

	[Fact]
	public void ViewModelNavigationAndViewNavigationShareRegistration()
	{
		ViewRegistry registry = new();
		new PagesBuilder(registry, replace: true)
			.AddTransient((ViewModel viewModel) => new ViewModelPage(viewModel));

		using ServiceProvider services = Services();
		ViewModel viewModel = services.GetRequiredService<ViewModel>();

		ContentView byView = registry.CreatePage(typeof(ViewModelPage), services);
		ContentView byViewModel = registry.CreatePage(viewModel, services);

		Assert.IsType<ViewModelPage>(byView);
		Assert.Same(viewModel, byViewModel.BindingContext);
	}

	[Fact]
	public void ManualRegistrationOverridesGeneratedDefault()
	{
		ViewRegistry registry = new();

		new PagesBuilder(registry, replace: false)
			.AddTransient(() => new ViewOnlyPage { Source = "generated" });

		new PagesBuilder(registry, replace: true)
			.AddTransient(() => new ViewOnlyPage { Source = "manual" });

		new PagesBuilder(registry, replace: false)
			.AddTransient(() => new ViewOnlyPage { Source = "generated again" });

		using ServiceProvider services = Services();
		ViewOnlyPage page = Assert.IsType<ViewOnlyPage>(
			registry.CreatePage(typeof(ViewOnlyPage), services));

		Assert.Equal("manual", page.Source);
	}

	[Fact]
	public void RecreateSupportsPagesWithoutBindingContext()
	{
		ViewRegistry registry = new();
		new PagesBuilder(registry, replace: true)
			.AddSingleton(() => new ViewOnlyPage());

		using ServiceProvider services = Services();
		ContentView original = registry.CreatePage(typeof(ViewOnlyPage), services);
		ContentView recreated = registry.RecreatePage(original, services);

		Assert.NotSame(original, recreated);
		Assert.Same(recreated, registry.CreatePage(typeof(ViewOnlyPage), services));
	}
}
