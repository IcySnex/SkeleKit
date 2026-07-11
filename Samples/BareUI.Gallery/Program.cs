using BareUI;
using BareUI.Gallery;
using Microsoft.Extensions.DependencyInjection;

BareApp.Create()
	.UseServices(services =>
	{
		services.AddTransient<MenuViewModel>();
		services.AddTransient<BindingViewModel>();
	})
	.Map<MenuViewModel, MenuPage>()
	.Map<DemoViewModel, DemoPage>()
	.Map<BindingViewModel, BindingPage>()
	.Tabs(tabs => tabs
		.Tab<MenuViewModel>("Controls", icon: "square.grid.2x2")
		.Tab<BindingViewModel>("Binding", icon: "link"))
	.Run(args);
