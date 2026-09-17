using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Notes;

namespace SkeleKit.Gallery.Views.Notes;

[Page]
internal sealed class NotebooksView : ContentView<NotesViewModel>
{
	public NotebooksView(NotesViewModel viewModel) : base(viewModel)
	{
		Title = "Notebooks";
		Background = Colors.GroupedBackground;

		Content = new CollectionView<Notebook>
		{
			ItemsSource = Bind(vm => vm.Notebooks),
			ItemTemplate = static () => new NotebookCell(),
			ItemCommand = viewModel.SelectNotebookCommand,
			Layout = CollectionLayout.List(),
			ShowsSeparators = true
		};
	}
}

internal sealed class NotebookCell : ItemView<Notebook>
{
	public NotebookCell()
	{
		Background = Colors.SecondaryGroupedBackground;
		HighlightBackground = Colors.Green.WithAlpha(0.16);

		Content = new Grid
		{
			Padding = new(16, 10),
			ColumnSpacing = 12,
			Columns =
			{
				38,
				GridLength.Star,
				GridLength.Auto
			},

			Children =
			{
				new Border
				{
					VerticalAlignment = VerticalAlignment.Center,
					Height = 38,
					Width = 38,
					CornerRadius = 10,
					Background = Colors.Green.WithAlpha(0.14),

					Child = new Image
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center,
						Height = 18,
						Width = 18,
						Source = Bind(notebook => notebook.Symbol)
							.ConvertTo(symbol => ImageSource.Symbol(symbol, colors: [Colors.Green]))
					}
				},

				new StackPanel
				{
					VerticalAlignment = VerticalAlignment.Center,
					Spacing = 2,

					Children =
					{
						new Label
						{
							Text = Bind(notebook => notebook.Title),
							TextStyle = TextStyle.Body,
							MaxLines = 1
						},

						new Label
						{
							Text = Bind(notebook => notebook.Summary),
							TextStyle = TextStyle.Footnote,
							TextColor = Colors.SecondaryLabel,
							MaxLines = 1
						}
					}
				}.Column(1),

				new Image
				{
					VerticalAlignment = VerticalAlignment.Center,
					Source = ImageSource.Symbol("chevron.right", weight: FontWeight.Semibold, colors: [Colors.TertiaryLabel])
				}.Column(2)
			}
		};
	}
}
