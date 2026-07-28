using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class FrameworkViewModel : CatalogViewModel
{
	public FrameworkViewModel(
		IGalleryCatalog catalog,
		INavigator navigator) : base(navigator, catalog.Framework)
	{ }
}
