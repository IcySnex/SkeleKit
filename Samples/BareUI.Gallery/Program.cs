using BareUI;
using BareUI.Gallery.Services;
using BareUI.Gallery.ViewModels;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;
using BareUI.Gallery.Views.Demos;
using Microsoft.Extensions.DependencyInjection;

BareApp.Create()
	.UseServices(services =>
	{
		services.AddSingleton<IDemoCatalog, DemoCatalog>();
		services.AddSingleton<IMovieService, MovieService>();

		services.AddTransient<MenuViewModel>();
		services.AddTransient<MovieInfoViewModel>();
		services.AddTransient<BindingViewModel>();

		services.AddTransient<ButtonDemoViewModel>();
		services.AddTransient<TextFieldDemoViewModel>();
		services.AddTransient<TextEditorDemoViewModel>();
		services.AddTransient<SwitchDemoViewModel>();
		services.AddTransient<SliderDemoViewModel>();
		services.AddTransient<StepperDemoViewModel>();
		services.AddTransient<ProgressBarDemoViewModel>();
		services.AddTransient<ActivityIndicatorDemoViewModel>();
		services.AddTransient<DividerDemoViewModel>();
		services.AddTransient<PickerDemoViewModel>();
		services.AddTransient<ImageDemoViewModel>();
		services.AddTransient<NativeViewDemoViewModel>();
	})
	.UsePages(pages => pages
		.AddSingleton<MenuView>()
		.AddSingleton<BindingView>()
		.AddTransient<MovieInfoView>()
		.AddTransient<ButtonDemo>()
		.AddTransient<TextFieldDemo>()
		.AddTransient<TextEditorDemo>()
		.AddTransient<SwitchDemo>()
		.AddTransient<SliderDemo>()
		.AddTransient<StepperDemo>()
		.AddTransient<ProgressBarDemo>()
		.AddTransient<ActivityIndicatorDemo>()
		.AddTransient<DividerDemo>()
		.AddTransient<PickerDemo>()
		.AddTransient<ImageDemo>()
		.AddTransient<NativeViewDemo>())
	.Tabs(tabs => tabs
		.Tab<MenuView>("Controls", icon: "square.grid.2x2")
		.Tab<BindingView>("Bindings", icon: "link"))
	.Run(args);
