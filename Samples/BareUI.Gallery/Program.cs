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
		services.AddTransient<KeyboardDemoViewModel>();
		services.AddTransient<GridDemoViewModel>();
		services.AddTransient<ListDemoViewModel>();
		services.AddTransient<CarouselDemoViewModel>();
		services.AddTransient<LiveListDemoViewModel>();
	})
	.UsePages(pages =>
	{
		pages.AddSingleton<MenuView>();
		pages.AddTransient<MovieInfoView>();
		pages.AddSingleton<BindingView>();

		pages.AddTransient<ButtonDemo>();
		pages.AddTransient<TextFieldDemo>();
		pages.AddTransient<TextEditorDemo>();
		pages.AddTransient<SwitchDemo>();
		pages.AddTransient<SliderDemo>();
		pages.AddTransient<StepperDemo>();
		pages.AddTransient<ProgressBarDemo>();
		pages.AddTransient<ActivityIndicatorDemo>();
		pages.AddTransient<DividerDemo>();
		pages.AddTransient<PickerDemo>();
		pages.AddTransient<ImageDemo>();
		pages.AddTransient<NativeViewDemo>();
		pages.AddTransient<KeyboardDemo>();
		pages.AddTransient<GridDemo>();
		pages.AddTransient<ListDemo>();
		pages.AddTransient<CarouselDemo>();
		pages.AddTransient<LiveListDemo>();
	})
	.Tabs(tabs => tabs
		.Tab<MenuView>("Controls", icon: "square.grid.2x2")
		.Tab<BindingView>("Bindings", icon: "link"))
	.Run(args);
