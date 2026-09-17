using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Platform;

internal sealed partial class SplitViewsViewModel(
	INavigator navigator) : ShowcaseViewModel
{
	public IReadOnlyList<Span> ShellCode { get; } =
	[
		new(
			"""
			SkeleApplication.CreateBuilder()
				.SplitView(split => split
					.Style(SplitViewStyle.TripleColumn)
					.Primary<FolderView>()
					.Supplementary<NoteListView>()
					.Secondary<NoteView>()
					.NavigationColumn(SplitViewColumn.Secondary))
				.Build()
				.Run(args);
			""")
	];

	public IReadOnlyList<Span> TabCode { get; } =
	[
		new(
			"""
			.Tabs(tabs => tabs
				.Tab<HomeView>("Home", "house")
				.Split(
					"Notes",
					"rectangle.split.2x1",
					split => split
						.Primary<NotebooksView>(weight: 1)
						.Secondary<NotesListView>(weight: 2)
						.Behavior(SplitViewBehavior.SideBySide)
						.Display(SplitViewDisplay.TwoColumns),
					TabPlacement.Hidden));

			// reach the hidden destination programmatically
			await navigator.SelectTabAsync("Notes");
			""")
	];

	public IReadOnlyList<Span> DemoCode { get; } =
	[
		new(
			"""
			// the Notes destination is declared as a hidden split view
			await navigator.SelectTabAsync("Notes");
			""")
	];

	public IReadOnlyList<Span> ColumnNavigationCode { get; } =
	[
		new(
			"""
			// pushes onto the secondary column's own stack
			await navigator.PushAsync(
				SplitViewColumn.Secondary,
				new NoteDetailViewModel(note));

			await navigator.PopAsync(SplitViewColumn.Secondary);
			""")
	];

	public IReadOnlyList<Span> VisibilityCode { get; } =
	[
		new(
			"""
			ISplitView split = ...;

			split.Hide(SplitViewColumn.Primary);
			split.Show(SplitViewColumn.Primary);

			// visibility queries need iOS 26 or later
			if (OperatingSystem.IsIOSVersionAtLeast(26))
				split.Toggle(SplitViewColumn.Primary);
			""")
	];


	[RelayCommand]
	Task OpenDemoAsync() =>
		navigator.SelectTabAsync("Notes");
}
