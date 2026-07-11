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
	.Map<MovieInfoView>()
	.Tabs(tabs => tabs
		.Tab<MenuView>("Controls", icon: "square.grid.2x2")
		.Tab<BindingView>("Bindings", icon: "link"))
	.Run(args);
