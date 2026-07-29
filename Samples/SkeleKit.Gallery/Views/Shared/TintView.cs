namespace SkeleKit.Gallery.Views.Shared;

internal abstract class TintView<TViewModel> : ContentView<TViewModel>
	where TViewModel : class
{
	readonly Color tint;


	protected TintView(
		TViewModel viewModel,
		Color tint) : base(viewModel)
	{
		this.tint = tint;
	}


	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (SkeleApplication.Current is SkeleApplication app)
			app.Tint = tint;
	}
}
