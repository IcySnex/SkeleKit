using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Abstract;

namespace SkeleKit.Gallery.Views;

[Page]
internal sealed class PlatformView : CatalogView<PlatformViewModel>
{
	public PlatformView(
		PlatformViewModel viewModel) : base(viewModel, "Platform", Colors.Green)
	{ }
}
