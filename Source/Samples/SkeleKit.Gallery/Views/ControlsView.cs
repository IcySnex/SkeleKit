using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Abstract;

namespace SkeleKit.Gallery.Views;

[Page]
internal sealed class ControlsView : CatalogView<ControlsViewModel>
{
	public ControlsView(
		ControlsViewModel viewModel) : base(viewModel, "Controls", Colors.Pink)
	{ }
}
