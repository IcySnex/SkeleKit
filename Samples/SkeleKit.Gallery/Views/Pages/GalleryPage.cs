namespace SkeleKit.Gallery.Views.Pages;

internal abstract class GalleryPage<TViewModel> : ContentView<TViewModel>
	where TViewModel : class
{
	readonly Color accent;


	protected GalleryPage(
		TViewModel viewModel,
		Color accent) : base(viewModel)
	{
		this.accent = accent;
	}


	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (SkeleApplication.Current is SkeleApplication app)
			app.Accent = accent;
	}
}
