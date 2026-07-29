using System.Windows.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.Services;

namespace SkeleKit.Gallery.ViewModels;

internal sealed class SearchViewModel : GalleryViewModel
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

		OpenTopicCommand = Command.From<GalleryTopic>(OpenTopic);
	}


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


	void OpenTopic(
		GalleryTopic? topic)
	{
		if (topic is not null)
			_ = navigator.PushAsync(topic);
	}

	void Refresh()
	{
		Results = catalog.Search(query, area);
		OnPropertyChanged(nameof(EmptyTitle));
		OnPropertyChanged(nameof(EmptySummary));
	}
}
