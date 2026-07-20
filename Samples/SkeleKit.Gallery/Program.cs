using SkeleKit;
using SkeleKit.Gallery.Services;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.ViewModels.Demos;
using SkeleKit.Gallery.Views;
using SkeleKit.Gallery.Views.Demos;
using Microsoft.Extensions.DependencyInjection;

SkeleApplication.CreateBuilder()
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
		services.AddTransient<SystemPickerDemoViewModel>();
		services.AddTransient<ImageDemoViewModel>();
		services.AddTransient<WebViewDemoViewModel>();
		services.AddTransient<MapViewDemoViewModel>();
		services.AddTransient<NativeViewDemoViewModel>();
		services.AddTransient<KeyboardDemoViewModel>();
		services.AddTransient<GridDemoViewModel>();
		services.AddTransient<ListDemoViewModel>();
		services.AddTransient<ContactsDemoViewModel>();
		services.AddTransient<CarouselDemoViewModel>();
		services.AddTransient<MixedDemoViewModel>();
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
		.Accessory<PlayerBar>()
		.Minimizes()
		.Tab<MenuView>("Controls", "square.grid.2x2")
		.Tab<BindingView>("Bindings", "link")
		.Search<SearchTabDemo>()
		// .Bubble("List", "list.bullet", () => {})
		.OnPad(pad => pad
			.Sidebar()
			.PlaceTab<MenuView>(TabPlacement.Locked)
			.Group("Collections", "square.grid.2x2", collections => collections
				.Tab<GridDemo>("Grid", "square.grid.3x3")
				.Tab<CarouselDemo>("Carousel", "film"))
			.SidebarFooter<FooterCard>()))
	.Build()
	.Run(args);
