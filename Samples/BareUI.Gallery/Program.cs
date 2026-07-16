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
		services.AddSingleton<PlayerBarViewModel>();
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
	})
	.UseTheme(theme => theme
		.Style(new Style<Label>(label => label.TextColor = Colors.Label))
		.Style(new Style<Button>(button => button.Kind = ButtonStyle.Tinted)))
	.UsePages(pages =>
	{
		pages.AddSingleton((MenuViewModel vm) => new MenuView(vm));
		pages.AddTransient((MovieInfoViewModel vm) => new MovieInfoView(vm));
		pages.AddSingleton((BindingViewModel vm) => new BindingView(vm));

		pages.AddTransient((StylingDemoViewModel vm) => new StylingDemo(vm));
		pages.AddTransient((ChromeDemoViewModel vm) => new ChromeDemo(vm));
		pages.AddTransient((AccessoryDemoViewModel vm) => new AccessoryDemo(vm));
		pages.AddTransient((AnimationDemoViewModel vm) => new AnimationDemo(vm));
		pages.AddTransient((ButtonDemoViewModel vm) => new ButtonDemo(vm));
		pages.AddTransient((TextFieldDemoViewModel vm) => new TextFieldDemo(vm));
		pages.AddTransient((TextEditorDemoViewModel vm) => new TextEditorDemo(vm));
		pages.AddTransient((SwitchDemoViewModel vm) => new SwitchDemo(vm));
		pages.AddTransient((SegmentedDemoViewModel vm) => new SegmentedDemo(vm));
		pages.AddTransient((DatePickerDemoViewModel vm) => new DatePickerDemo(vm));
		pages.AddTransient((TintDemoViewModel vm) => new TintDemo(vm));
		pages.AddTransient((PageControlDemoViewModel vm) => new PageControlDemo(vm));
		pages.AddTransient((SliderDemoViewModel vm) => new SliderDemo(vm));
		pages.AddTransient((StepperDemoViewModel vm) => new StepperDemo(vm));
		pages.AddTransient((ProgressBarDemoViewModel vm) => new ProgressBarDemo(vm));
		pages.AddTransient((ActivityIndicatorDemoViewModel vm) => new ActivityIndicatorDemo(vm));
		pages.AddTransient((DividerDemoViewModel vm) => new DividerDemo(vm));
		pages.AddTransient((PickerDemoViewModel vm) => new PickerDemo(vm));
		// singleton on purpose: exercises re-realize on every push
		pages.AddSingleton((ImageDemoViewModel vm) => new ImageDemo(vm));
		pages.AddTransient((NativeViewDemoViewModel vm) => new NativeViewDemo(vm));
		pages.AddTransient((KeyboardDemoViewModel vm) => new KeyboardDemo(vm));
		pages.AddTransient((GridDemoViewModel vm) => new GridDemo(vm));
		pages.AddTransient((ListDemoViewModel vm) => new ListDemo(vm));
		pages.AddTransient((CarouselDemoViewModel vm) => new CarouselDemo(vm));
		pages.AddTransient((LiveListDemoViewModel vm) => new LiveListDemo(vm));
	})
	.Tabs(tabs => tabs
		.LargeTitles()
		.Accessory((PlayerBarViewModel vm) => new PlayerBar())
		.Tab<MenuView>("Controls", "square.grid.2x2")
		.Tab<BindingView>("Bindings", "link"))
	.Build()
	.Run(args);
