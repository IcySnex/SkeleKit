using SkeleKit.Gallery.Services.Abstract;
using SkeleKit.Gallery.ViewModels.Abstract;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class ControlsViewModel(
	IGalleryCatalog catalog,
	INavigator navigator) : CatalogViewModel(navigator, catalog.Controls);
