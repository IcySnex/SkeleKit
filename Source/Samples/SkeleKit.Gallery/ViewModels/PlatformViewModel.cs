using CommunityToolkit.Mvvm.ComponentModel;
using SkeleKit.Gallery.Services.Abstract;
using SkeleKit.Gallery.ViewModels.Abstract;
using SkeleKit.Gallery.ViewModels.Platform;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class PlatformViewModel : CatalogViewModel
{
	public PlatformViewModel(
		IGalleryCatalog catalog,
		INavigator navigator,
		TabsIpadViewModel tabs) : base(navigator, catalog.Platform)
	{
		tabs.BadgeChanged += badge => TabBadge = badge;
	}


	[ObservableProperty]
	string? tabBadge;
}
