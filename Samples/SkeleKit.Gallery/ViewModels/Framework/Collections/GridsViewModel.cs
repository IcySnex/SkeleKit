using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record GridEntry(
	string Number,
	string Title);

internal sealed partial class GridsViewModel : ShowcaseViewModel
{
	int nextItem = 25;

	public ObservableCollection<GridEntry> Items { get; } =
	[
		.. Enumerable.Range(1, 24).Select(
			index => new GridEntry(index.ToString("00"), $"Item {index}"))
	];

	public IReadOnlyList<Span> GridCode { get; } =
		Code(
			"""
			CollectionView<GridEntry> grid = new()
			{
				ItemsSource = viewModel.Items,
				ItemTemplate = static () => new GridCell(),
				Layout = CollectionLayout.Grid(columns: 3, spacing: 12),
				HighlightsSelection = false,
				EmptyView = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "No items",
					TextStyle = TextStyle.Headline,
					TextColor = Colors.SecondaryLabel
				}
			};

			sealed record GridEntry(string Number, string Title);

			sealed class GridCell : ItemView<GridEntry>
			{
				public GridCell() =>
					Content = new Border
					{
						Height = 112,
						Background = Colors.Teal.WithAlpha(0.14),
						CornerRadius = 16,
						Child = new StackPanel
						{
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							Spacing = 3,
							Children =
							{
								new Label
								{
									Text = Bind(item => item.Number),
									TextStyle = TextStyle.Title2,
									FontWeight = FontWeight.Bold,
									TextColor = Colors.Teal
								},
								new Label
								{
									Text = Bind(item => item.Title),
									TextStyle = TextStyle.Footnote,
									TextColor = Colors.SecondaryLabel
								}
							}
						}
					};
			}

			int nextItem = 25;

			ObservableCollection<GridEntry> Items { get; } =
			[
				.. Enumerable.Range(1, 24).Select(
					index => new GridEntry(index.ToString("00"), $"Item {index}"))
			];

			void Add()
			{
				int number = nextItem++;
				Items.Insert(0, new(number.ToString("00"), $"Item {number}"));
			}

			void Remove()
			{
				if (Items.Count > 0)
					Items.RemoveAt(0);
			}
			""");

	bool CanRemove() =>
		Items.Count > 0;

	[RelayCommand]
	void Add()
	{
		int number = nextItem++;
		Items.Insert(0, new(number.ToString("00"), $"Item {number}"));
		RemoveCommand.NotifyCanExecuteChanged();
	}

	[RelayCommand(CanExecute = nameof(CanRemove))]
	void Remove()
	{
		if (Items.Count > 0)
			Items.RemoveAt(0);

		RemoveCommand.NotifyCanExecuteChanged();
	}

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
