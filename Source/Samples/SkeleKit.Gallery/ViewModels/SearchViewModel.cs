using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services.Abstract;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class SearchViewModel(
	IGalleryCatalog catalog,
	INavigator navigator) : ObservableObject
{
	[ObservableProperty]
	public partial List<GalleryTopic> Results { get; set; } = [];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EmptyTitle))]
	[NotifyPropertyChangedFor(nameof(EmptySummary))]
	public partial string Query { get; set; } = "";

	[ObservableProperty]
	public partial int SelectedScopeIndex { get; set; }

	public string EmptyTitle => string.IsNullOrWhiteSpace(Query)
		? "Search in SkeleKit"
		: "No matching APIs";

	public string EmptySummary => string.IsNullOrWhiteSpace(Query)
		? "Search controls, framework features and native platform integrations."
		: "Try another term or broaden the selected category.";


	partial void OnQueryChanged(
		string value) =>
		Refresh();

	partial void OnSelectedScopeIndexChanged(
		int value) =>
		Refresh();


	[RelayCommand]
	Task OpenTopicAsync(
		GalleryTopic? topic) =>
		topic is not null
			? navigator.PushAsync(topic.Destination)
			: Task.CompletedTask;

	[RelayCommand]
	Task ShowInfoAsync() =>
		navigator.PresentAsync<AboutViewModel>(ModalStyle.Sheet(Detent.Content, Detent.Large));

	internal void CancelSearch() =>
		Query = "";

	void Refresh() =>
		Results = catalog.Search(
			Query,
			SelectedScopeIndex switch
			{
				1 => GalleryArea.Controls,
				2 => GalleryArea.Framework,
				3 => GalleryArea.Platform,
				_ => null
			});
}
