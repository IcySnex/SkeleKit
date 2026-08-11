using Microsoft.Extensions.DependencyInjection;
using SkeleKit;
using SkeleKit.Gallery.Services;
using SkeleKit.Gallery.Services.Abstract;
using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.ViewModels.Controls.ActionsSelection;
using SkeleKit.Gallery.ViewModels.Controls.MediaContent;
using SkeleKit.Gallery.ViewModels.Controls.TextInput;
using SkeleKit.Gallery.ViewModels.Controls.ValuesStatus;
using SkeleKit.Gallery.ViewModels.Framework.Collections;
using SkeleKit.Gallery.ViewModels.Framework.Foundations;
using SkeleKit.Gallery.ViewModels.Framework.Layout;
using SkeleKit.Gallery.ViewModels.Framework.StylingMotion;
using SkeleKit.Gallery.ViewModels.Platform;
using SkeleKit.Gallery.Views;

SkeleApplication.CreateBuilder()
	.UseServices(services =>
	{
		services.AddSingleton<IGalleryCatalog, GalleryCatalog>();

		services.AddTransient<ControlsViewModel>();
		services.AddTransient<FrameworkViewModel>();
		services.AddTransient<NavigationViewModel>();
		services.AddTransient<PlatformViewModel>();
		services.AddTransient<SearchViewModel>();

		services.AddTransient<AboutViewModel>();
		services.AddTransient<PageChromeViewModel>();
		services.AddTransient<BindingViewModel>();
		services.AddTransient<ContentViewViewModel>();
		services.AddTransient<PanelsViewModel>();
		services.AddTransient<ViewViewModel>();
		services.AddTransient<BorderViewModel>();
		services.AddTransient<GridViewModel>();
		services.AddTransient<OverlayViewModel>();
		services.AddTransient<ScrollViewViewModel>();
		services.AddTransient<StackPanelViewModel>();
		services.AddTransient<ListsViewModel>();
		services.AddTransient<GridsViewModel>();
		services.AddTransient<CarouselsViewModel>();
		services.AddTransient<SectionsViewModel>();
		services.AddTransient<CollectionInteractionsViewModel>();
		services.AddTransient<ColorsBrushesViewModel>();
		services.AddTransient<MaterialsShadowsViewModel>();
		services.AddTransient<StylesThemesViewModel>();
		services.AddTransient<AnimationViewModel>();

		services.AddTransient<ActivityIndicatorViewModel>();
		services.AddTransient<ButtonViewModel>();
		services.AddTransient<ColorWellViewModel>();
		services.AddTransient<DatePickerViewModel>();
		services.AddTransient<DividerViewModel>();
		services.AddTransient<ImageViewModel>();
		services.AddTransient<LabelViewModel>();
		services.AddTransient<PageControlViewModel>();
		services.AddTransient<PickerViewModel>();
		services.AddTransient<ProgressBarViewModel>();
		services.AddTransient<SecureFieldViewModel>();
		services.AddTransient<SegmentedControlViewModel>();
		services.AddTransient<SliderViewModel>();
		services.AddTransient<StepperViewModel>();
		services.AddTransient<SwitchViewModel>();
		services.AddTransient<TextEditorViewModel>();
		services.AddTransient<TextFieldViewModel>();
		services.AddTransient<TextViewViewModel>();
		services.AddTransient<WebViewModel>();
		services.AddTransient<MapViewModel>();
		services.AddTransient<NativeViewModel>();
	})
	.UseTheme(theme => theme.Style(GalleryStyles.ImplicitCard))
	.UseTint(Colors.Indigo)
	.Tabs(tabs => tabs
		.LargeTitles()
		.Tab<FrameworkView>("Framework", "square.stack.3d.up")
		.Tab<ControlsView>("Controls", "switch.2")
		.Tab<PlatformView>("Platform", "iphone")
		.Search<SearchView>()
		.OnPad(pad => pad.Sidebar()))
	.Build()
	.Run(args);
