using SkeleKit.Gallery.Services.Abstract;
using SkeleKit.Gallery.ViewModels.Abstract;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class PlatformViewModel(
	IGalleryCatalog catalog,
	INavigator navigator) : CatalogViewModel(navigator, catalog.Platform);
