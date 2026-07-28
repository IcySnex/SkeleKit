using System.Windows.Input;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal abstract class GalleryViewModel
{
	readonly INavigator navigator;


	protected GalleryViewModel(
		INavigator navigator)
	{
		this.navigator = navigator;
		ShowInfoCommand = Command.From(ShowInfo);
	}


	public ICommand ShowInfoCommand { get; }


	void ShowInfo() =>
		_ = navigator.PresentViewAsync<GalleryInfoView>(ModalStyle.Sheet(Detent.Medium, Detent.Large));
}
