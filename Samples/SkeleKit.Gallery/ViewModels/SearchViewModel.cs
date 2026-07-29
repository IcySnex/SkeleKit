using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;
using SkeleKit.Gallery.Views.Pages;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class SearchViewModel : ObservableObject
{
	readonly IGalleryCatalog catalog;
	readonly INavigator navigator;

	GalleryArea? area;


	public SearchViewModel(
		IGalleryCatalog catalog,
		INavigator navigator)
	{
		this.catalog = catalog;
		this.navigator = navigator;
	}


	[ObservableProperty]
	List<GalleryTopic> results = [];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EmptyTitle))]
	[NotifyPropertyChangedFor(nameof(EmptySummary))]
	string query = "";

	public string EmptyTitle => string.IsNullOrWhiteSpace(Query)
		? "Find anything in SkeleKit"
		: "No matching APIs";

	public string EmptySummary => string.IsNullOrWhiteSpace(Query)
		? "Search controls, framework features and native platform integrations."
		: "Try another term or broaden the selected category.";


	public void Search(
		string value)
	{
		Query = value;
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
		topic switch
		{
			{ Destination: Type destination } => navigator.PushAsync(destination),
			not null => navigator.PushAsync(topic),
			_ => Task.CompletedTask
		};

	[RelayCommand]
	Task ShowInfoAsync() =>
		navigator.PresentViewAsync<AboutView>(ModalStyle.Sheet(Detent.Content, Detent.Large));

	void Refresh() =>
		Results = catalog.Search(Query, area);
}
