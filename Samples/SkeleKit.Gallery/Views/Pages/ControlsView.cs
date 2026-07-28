using SkeleKit.Gallery.ViewModels;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class ControlsView : GalleryListView<ControlsViewModel>
{
	public ControlsView(
		ControlsViewModel viewModel) : base(viewModel, "Controls", Colors.Purple)
	{ }
}
