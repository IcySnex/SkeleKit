using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.ViewModels;

internal abstract partial class CatalogViewModel : GalleryViewModel
{
	readonly INavigator navigator;


	protected CatalogViewModel(
		INavigator navigator,
		List<GallerySection> sections) : base(navigator)
	{
		this.navigator = navigator;

		Sections = sections;
	}


	public List<GallerySection> Sections { get; }


	[RelayCommand]
	Task OpenTopicAsync(
		GalleryTopic? topic) =>
		topic is null ? Task.CompletedTask : navigator.PushAsync(topic);
}
