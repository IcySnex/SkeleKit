using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.ViewModels.Notes;

internal sealed partial class NoteDetailViewModel(
	INavigator navigator,
	Note note) : ObservableObject
{
	public string Title { get; } = note.Title;

	public string Summary { get; } = note.Summary;

	public string Body { get; } = note.Body;


	[RelayCommand]
	Task PopAsync() =>
		navigator.PopAsync(SplitViewColumn.Secondary);
}
