using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.ViewModels.Notes;

internal sealed partial class NotesViewModel(
	INavigator navigator,
	ISplitView splitView) : ObservableObject
{
	static readonly List<Notebook> NotebookData =
	[
		new(
			"Product",
			"lightbulb",
			[
				new(
					"Roadmap",
					"Shipped in 0.2.1",
					"Native split view support lands as the app shell, a tab destination and a sidebar destination. Per-column navigation keeps each stack independent."),
				new(
					"Research",
					"Interviews next week",
					"Talk to five teams that ship iPad apps. Focus on how they decide between a two- and a three-column layout.")
			]),

		new(
			"Design",
			"paintbrush",
			[
				new(
					"Sidebar widths",
					"Fractions and fixed widths",
					"Set preferred, minimum and maximum widths per column through ConfigureNative. SkeleKit keeps its own defaults so every app starts adaptive."),
				new(
					"Inspector",
					"iOS 26 and later",
					"Use the trailing inspector column for auxiliary controls. On earlier versions it is omitted without failing validation.")
			]),

		new(
			"Engineering",
			"wrench.and.screwdriver",
			[
				new(
					"Column navigation",
					"Push into any column",
					"The column overloads of INavigator push and pop the stack of a single column. When the split view collapses, they route to the single visible stack."),
				new(
					"Collapse behavior",
					"One stack on iPhone",
					"UIKit collapses the columns into a single navigation surface on compact size classes. An optional compact column can provide a dedicated collapsed experience.")
			])
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Notes))]
	[NotifyPropertyChangedFor(nameof(NotebookTitle))]
	[NotifyPropertyChangedFor(nameof(NotebookSummary))]
	Notebook selectedNotebook = NotebookData[0];


	public IReadOnlyList<Notebook> Notebooks =>
		NotebookData;

	public IReadOnlyList<Note> Notes =>
		SelectedNotebook.Notes;

	public string NotebookTitle =>
		SelectedNotebook.Title;

	public string NotebookSummary =>
		SelectedNotebook.Summary;


	[RelayCommand]
	void SelectNotebook(
		Notebook notebook)
	{
		SelectedNotebook = notebook;

		if (splitView.IsCollapsed)
			splitView.Show(SplitViewColumn.Secondary);
	}

	[RelayCommand]
	Task OpenNoteAsync(
		Note note) =>
		navigator.PushAsync(SplitViewColumn.Secondary, new NoteDetailViewModel(navigator, note));
}
