using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class FrameworkViewModel : GalleryListViewModel
{
	public FrameworkViewModel(
		IGalleryCatalog catalog,
		INavigator navigator) : base(navigator, catalog.Framework)
	{ }
}
