using BareUI;
using BareUI.Gallery.Services;
using BareUI.Gallery.ViewModels;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;
using BareUI.Gallery.Views.Demos;
using Microsoft.Extensions.DependencyInjection;

BareApplication.CreateBuilder()
	.UseServices(services =>
	{
		services.AddSingleton<IDemoCatalog, DemoCatalog>();
		services.AddSingleton<IMovieService, MovieService>();
		services.AddSingleton<PlayerBarViewModel>();
	})
	.UseTheme(theme => theme
		.Style(new Style<Label>(label => label.TextColor = Colors.Label))
		.Style(new Style<Button>(button => button.Kind = ButtonStyle.Tinted)))
	.UsePages()
	.Tabs(tabs => tabs
		.LargeTitles()
		.Accessory<PlayerBar>()
		.Tab<MenuView>("Controls", "square.grid.2x2")
		.Tab<BindingView>("Bindings", "link"))
	.Build()
	.Run(args);
