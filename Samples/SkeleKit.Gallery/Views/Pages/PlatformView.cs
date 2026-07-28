using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Shared;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class PlatformView : CatalogView<PlatformViewModel>
{
	public PlatformView(
		PlatformViewModel viewModel) : base(viewModel, "Platform", Colors.Green)
	{ }
}
