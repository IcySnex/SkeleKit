using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class ControlsViewModel : GalleryListViewModel
{
	public ControlsViewModel(
		IGalleryCatalog catalog,
		INavigator navigator) : base(navigator, catalog.Controls)
	{ }
}
