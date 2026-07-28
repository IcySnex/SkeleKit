using System.Windows.Input;

namespace SkeleKit.Gallery;

internal sealed class MainViewModel
{
	readonly INavigator navigator;


	public MainViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;

		OpenCategoryCommand = Command.From<GalleryCategory>(OpenCategory);
	}


	public List<GalleryCategory> Categories =>
		GalleryCatalog.Categories;

	public ICommand OpenCategoryCommand { get; }


	void OpenCategory(
		GalleryCategory? category)
	{
		if (category is not null)
			_ = navigator.PushViewAsync(new CategoryView(category));
	}
}
