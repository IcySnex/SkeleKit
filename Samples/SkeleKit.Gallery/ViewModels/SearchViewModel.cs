using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services.Abstract;
using SkeleKit.Gallery.Views;

namespace SkeleKit.Gallery.ViewModels;

internal sealed partial class SearchViewModel(
	IGalleryCatalog catalog,
	INavigator navigator) : ObservableObject
{
	GalleryArea? area;


	[ObservableProperty]
	public partial List<GalleryTopic> Results { get; set; } = [];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EmptyTitle))]
	[NotifyPropertyChangedFor(nameof(EmptySummary))]
	public partial string Query { get; set; } = "";

	public string EmptyTitle => string.IsNullOrWhiteSpace(Query)
		? "Search in SkeleKit"
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
