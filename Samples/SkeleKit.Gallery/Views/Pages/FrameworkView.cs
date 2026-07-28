using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Shared;

namespace SkeleKit.Gallery.Views.Pages;

[Page]
internal sealed class FrameworkView : CatalogView<FrameworkViewModel>
{
	public FrameworkView(
		FrameworkViewModel viewModel) : base(viewModel, "Framework", Colors.Indigo)
	{ }
}
