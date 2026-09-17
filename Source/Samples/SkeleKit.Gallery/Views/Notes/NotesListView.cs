using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Notes;

namespace SkeleKit.Gallery.Views.Notes;

[Page]
internal sealed class NotesListView : ContentView<NotesViewModel>
{
	public NotesListView(NotesViewModel viewModel) : base(viewModel)
	{
		Title = Bind(vm => vm.NotebookTitle);
		Background = Colors.GroupedBackground;

		Content = new CollectionView<Note>
		{
			ItemsSource = Bind(vm => vm.Notes),
			ItemTemplate = static () => new NoteCell(),
			ItemCommand = viewModel.OpenNoteCommand,
			Layout = CollectionLayout.List(),
			ShowsSeparators = true,

			Header = new Label
			{
				Text = Bind(vm => vm.NotebookSummary),
				TextStyle = TextStyle.Footnote,
				TextColor = Colors.SecondaryLabel
			}
		};
	}
}

internal sealed class NoteCell : ItemView<Note>
{
	public NoteCell()
	{
		Background = Colors.SecondaryGroupedBackground;
		HighlightBackground = Colors.Green.WithAlpha(0.16);

		Content = new StackPanel
		{
			Padding = new(16, 10),
			Spacing = 2,

			Children =
			{
				new Label
				{
					Text = Bind(note => note.Title),
					TextStyle = TextStyle.Body,
					MaxLines = 1
				},

				new Label
				{
					Text = Bind(note => note.Summary),
					TextStyle = TextStyle.Footnote,
					TextColor = Colors.SecondaryLabel,
					MaxLines = 1
				}
			}
		};
	}
}
