using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record ListEntry(
	string Title);

internal sealed partial class ListsViewModel : ShowcaseViewModel
{
	int nextItem = 13;

	public ObservableCollection<ListEntry> Items { get; } = [];

	public IReadOnlyList<Span> ListCode { get; } =
		Code(
			"""
			CollectionView<ListEntry> list = new()
			{
				ItemsSource = viewModel.Items,
				ItemTemplate = static () => new ListCell(),
				Layout = CollectionLayout.List(),
				ItemCommand = viewModel.SelectCommand,
				ShowsSeparators = true,
				EmptyView = new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Text = "No items",
					TextColor = Colors.SecondaryLabel
				}
			};

			sealed record ListEntry(string Title);

			sealed class ListCell : ItemView<ListEntry>
			{
				public ListCell() =>
					Content = new Border
					{
						Height = 52,
						Child = new Label
						{
							Margin = new Thickness(16, 0),
							VerticalAlignment = VerticalAlignment.Center,
							Text = Bind(item => item.Title),
							TextStyle = TextStyle.Body
						}
					};
			}

			int nextItem = 13;

			ObservableCollection<ListEntry> Items { get; } =
			[
				.. Enumerable.Range(1, 12).Select(
					index => new ListEntry($"Item {index}"))
			];

			void Add() =>
				Items.Insert(0, new($"Item {nextItem++}"));

			void Remove()
			{
				if (Items.Count > 0)
					Items.RemoveAt(0);
			}

			void Select(ListEntry item) =>
				Haptics.Selection();
			""");


	public ListsViewModel()
	{
		for (int index = 1; index <= 12; index++)
			Items.Add(new($"Item {index}"));
	}


	bool CanRemove() =>
		Items.Count > 0;

	[RelayCommand]
	void Add()
	{
		Items.Insert(0, new($"Item {nextItem++}"));
		RemoveCommand.NotifyCanExecuteChanged();
	}

	[RelayCommand(CanExecute = nameof(CanRemove))]
	void Remove()
	{
		if (Items.Count > 0)
			Items.RemoveAt(0);

		RemoveCommand.NotifyCanExecuteChanged();
	}

	[RelayCommand]
	static void Select(
		ListEntry item) =>
		Haptics.Selection();

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
