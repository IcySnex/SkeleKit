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
		services.AddTransient<MenuViewModel>();
		services.AddTransient<MovieInfoViewModel>();
		services.AddTransient<BindingViewModel>();
		services.AddTransient<StylingDemoViewModel>();
		services.AddTransient<ChromeDemoViewModel>();
		services.AddTransient<AccessoryDemoViewModel>();
		services.AddTransient<AnimationDemoViewModel>();
		services.AddTransient<ButtonDemoViewModel>();
		services.AddTransient<TextFieldDemoViewModel>();
		services.AddTransient<TextEditorDemoViewModel>();
		services.AddTransient<SwitchDemoViewModel>();
		services.AddTransient<SegmentedDemoViewModel>();
		services.AddTransient<DatePickerDemoViewModel>();
		services.AddTransient<TintDemoViewModel>();
		services.AddTransient<PageControlDemoViewModel>();
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
		services.AddSingleton<PlayerBarViewModel>();
		services.AddTransient<SearchTabDemoViewModel>();
	})
	.UseTheme(theme => theme
		.Style(new Style<Label>(label => label.TextColor = Colors.Label))
		.Style(new Style<Button>(button => button.Kind = ButtonStyle.Tinted)))
	.UsePages()
	.Tabs(tabs => tabs
		.LargeTitles()
		.SidebarOnIPad()
		.Accessory<PlayerBar>()
		.Minimizes()
		.SidebarOnIPad()
		.Tab<MenuView>("Controls", "square.grid.2x2", TabPlacement.Locked)
		.Tab<BindingView>("Bindings", "link")
		.SearchTab<SearchTabDemo>())
	.Build()
	.Run(args);
