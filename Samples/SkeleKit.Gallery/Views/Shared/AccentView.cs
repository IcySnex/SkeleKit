namespace SkeleKit.Gallery.Views.Shared;

internal abstract class AccentView<TViewModel> : ContentView<TViewModel>
	where TViewModel : class
{
	readonly Color accent;


	protected AccentView(
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
