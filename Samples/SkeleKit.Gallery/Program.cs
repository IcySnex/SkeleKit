using Microsoft.Extensions.DependencyInjection;
using SkeleKit;
using SkeleKit.Gallery.Services;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Pages;

SkeleApplication.CreateBuilder()
	.UseServices(services =>
	{
		services.AddSingleton<IGalleryCatalog, GalleryCatalog>();
		services.AddTransient<ControlsViewModel>();
		services.AddTransient<FrameworkViewModel>();
		services.AddTransient<PlatformViewModel>();
		services.AddTransient<SearchViewModel>();
		services.AddTransient<AboutViewModel>();
	})
	.UseTint(Colors.Indigo)
	.UseAppearance(Appearance.System)
	.Tabs(tabs => tabs
		.LargeTitles()
		.Tab<FrameworkView>("Framework", "square.stack.3d.up")
		.Tab<ControlsView>("Controls", "switch.2")
		.Tab<PlatformView>("Platform", "iphone")
		.Search<SearchView>()
		.OnPad(pad => pad.Sidebar()))
	.Build()
	.Run(args);
