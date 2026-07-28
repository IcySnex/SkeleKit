using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class ControlsViewModel : CatalogViewModel
{
	public ControlsViewModel(
		IGalleryCatalog catalog,
		INavigator navigator) : base(navigator, catalog.Controls)
	{ }
}
