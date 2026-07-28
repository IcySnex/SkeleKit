using SkeleKit.Gallery.ViewModels;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class FrameworkView : GalleryListView<FrameworkViewModel>
{
	public FrameworkView(
		FrameworkViewModel viewModel) : base(viewModel, "Framework", Colors.Indigo)
	{ }
}
