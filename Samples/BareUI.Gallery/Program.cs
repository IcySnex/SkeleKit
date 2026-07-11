using BareUI;
using BareUI.Gallery.Services;
using BareUI.Gallery.ViewModels;
using BareUI.Gallery.Views;
using Microsoft.Extensions.DependencyInjection;

BareApp.Create()
	.UseServices(services =>
	{
		services.AddSingleton<IDemoCatalog, DemoCatalog>();
		services.AddSingleton<IMovieService, MovieService>();

		services.AddTransient<MenuViewModel>();
		services.AddTransient<MovieInfoViewModel>();
		services.AddTransient<BindingViewModel>();
	})
	.Map<MenuViewModel, MenuView>()
	.Map<DemoViewModel, DemoView>()
	.Map<MovieInfoViewModel, MovieInfoView>()
	.Map<BindingViewModel, BindingView>()
	.Tabs(tabs => tabs
		.Tab<MenuViewModel>("Controls", icon: "square.grid.2x2")
		.Tab<BindingViewModel>("Bindings", icon: "link"))
	.Run(args);
