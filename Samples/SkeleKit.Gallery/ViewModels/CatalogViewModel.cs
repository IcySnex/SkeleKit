using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal abstract partial class CatalogViewModel : ObservableObject
{
	readonly INavigator navigator;


	protected CatalogViewModel(
		INavigator navigator,
		List<GallerySection> sections)
	{
		this.navigator = navigator;

		Sections = sections;
	}


	public List<GallerySection> Sections { get; }


	[RelayCommand]
	Task OpenTopicAsync(
		GalleryTopic? topic) =>
		topic is null ? Task.CompletedTask : navigator.PushAsync(topic);

	[RelayCommand]
	Task ShowInfoAsync() =>
		navigator.PresentViewAsync<AboutView>(ModalStyle.Sheet(Detent.Content, Detent.Large));
}
