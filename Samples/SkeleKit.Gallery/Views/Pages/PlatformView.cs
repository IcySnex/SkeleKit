using SkeleKit.Gallery.ViewModels;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class PlatformView : GalleryListView<PlatformViewModel>
{
	public PlatformView(
		PlatformViewModel viewModel) : base(viewModel, "Platform", Colors.Green)
	{ }
}
