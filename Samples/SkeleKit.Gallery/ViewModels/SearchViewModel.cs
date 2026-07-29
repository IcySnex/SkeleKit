using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class SearchViewModel : GalleryViewModel
{
	readonly IGalleryCatalog catalog;
	readonly INavigator navigator;

	string query = "";
	GalleryArea? area;


	public SearchViewModel(
		IGalleryCatalog catalog,
		INavigator navigator) : base(navigator)
	{
		this.catalog = catalog;
		this.navigator = navigator;
	}


	[ObservableProperty]
	List<GalleryTopic> results = [];

	public string EmptyTitle => string.IsNullOrWhiteSpace(query)
		? "Find anything in SkeleKit"
		: "No matching APIs";

	public string EmptySummary => string.IsNullOrWhiteSpace(query)
		? "Search controls, framework features and native platform integrations."
		: "Try another term or broaden the selected category.";


	public void Search(
		string value)
	{
		query = value;
		Refresh();
	}

	public void SelectScope(
		int index)
	{
		area = index switch
		{
			1 => GalleryArea.Controls,
			2 => GalleryArea.Framework,
			3 => GalleryArea.Platform,
			_ => null
		};

		Refresh();
	}


	[RelayCommand]
	Task OpenTopicAsync(
		GalleryTopic? topic) =>
		topic is null ? Task.CompletedTask : navigator.PushAsync(topic);

	void Refresh()
	{
		Results = catalog.Search(query, area);
		OnPropertyChanged(nameof(EmptyTitle));
		OnPropertyChanged(nameof(EmptySummary));
	}
}
