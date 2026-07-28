using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class PlatformViewModel : CatalogViewModel
{
	public PlatformViewModel(
		IGalleryCatalog catalog,
		INavigator navigator) : base(navigator, catalog.Platform)
	{ }
}
