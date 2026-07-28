using System.Windows.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal abstract class GalleryListViewModel : GalleryViewModel
{
	readonly INavigator navigator;


	protected GalleryListViewModel(
		INavigator navigator,
		List<GallerySection> sections) : base(navigator)
	{
		this.navigator = navigator;

		Sections = sections;
		OpenTopicCommand = Command.From<GalleryTopic>(OpenTopic);
	}


	public List<GallerySection> Sections { get; }
	public ICommand OpenTopicCommand { get; }


	void OpenTopic(
		GalleryTopic? topic)
	{
		if (topic is not null)
			_ = navigator.PushViewAsync(new TopicView(topic));
	}
}
