using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Shared;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class ControlsView : CatalogView<ControlsViewModel>
{
	public ControlsView(
		ControlsViewModel viewModel) : base(viewModel, "Controls", Colors.Pink)
	{ }
}
