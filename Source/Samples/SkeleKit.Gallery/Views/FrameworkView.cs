using SkeleKit.Gallery.ViewModels;
using SkeleKit.Gallery.Views.Abstract;

namespace SkeleKit.Gallery.Views;

[Page]
internal sealed class FrameworkView : CatalogView<FrameworkViewModel>
{
	public FrameworkView(
		FrameworkViewModel viewModel) : base(viewModel, "Framework", Colors.Indigo)
	{ }
}
