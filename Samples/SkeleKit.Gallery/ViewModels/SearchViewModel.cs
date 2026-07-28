using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class SearchViewModel : INotifyPropertyChanged
{
	readonly IGalleryCatalog catalog;
	readonly INavigator navigator;


	public SearchViewModel(
		IGalleryCatalog catalog,
		INavigator navigator)
	{
		this.catalog = catalog;
		this.navigator = navigator;

		OpenTopicCommand = Command.From<GalleryTopic>(OpenTopic);
	}


	public event PropertyChangedEventHandler? PropertyChanged;


	public List<GalleryTopic> Results
	{
		get;
		private set
		{
			if (ReferenceEquals(field, value))
				return;

			field = value;
			OnPropertyChanged();
		}
	} = [];

	public ICommand OpenTopicCommand { get; }


	public void Search(
		string query) =>
		Results = catalog.Search(query);


	void OpenTopic(
		GalleryTopic? topic)
	{
		if (topic is not null)
			_ = navigator.PushViewAsync(new TopicView(topic));
	}

	void OnPropertyChanged(
		[CallerMemberName] string? name = null) =>
		PropertyChanged?.Invoke(this, new(name));
}
