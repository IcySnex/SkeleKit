using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.ViewModels.Abstract;

internal abstract partial class CatalogViewModel(
	INavigator navigator,
	List<GallerySection> sections) : ObservableObject
{
	public List<GallerySection> Sections { get; } = sections;


	[RelayCommand]
	Task OpenTopicAsync(
		GalleryTopic topic) =>
		navigator.PushAsync(topic.Destination);

	[RelayCommand]
	Task ShowInfoAsync() =>
		navigator.PresentAsync<AboutViewModel>(ModalStyle.Sheet(Detent.Content));
}
