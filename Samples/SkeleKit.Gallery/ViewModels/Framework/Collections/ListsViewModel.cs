using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Framework.Collections;

internal sealed record ListEntry(
	string Title);

internal sealed partial class ListsViewModel : ShowcaseViewModel
{
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ItemCountLabel))]
	[NotifyPropertyChangedFor(nameof(ListCode))]
	double itemCount = 4;

	[ObservableProperty]
	string selectedTitle = "None";


	public ObservableCollection<ListEntry> Items { get; } = [];

	public string ItemCountLabel
	{
		get
		{
			int count = (int)Math.Round(ItemCount);
			return $"{count} {(count == 1 ? "item" : "items")}";
		}
	}

	public IReadOnlyList<Span> ListCode =>
		Code(
			$$"""
			ObservableCollection<ListEntry> items = [];
			for (int index = 1; index <= {{(int)Math.Round(ItemCount)}}; index++)
				items.Add(new($"Item {index}"));

			Label selected = new()
			{
				Width = 100,
				Height = 20,
				Text = "None",
				TextStyle = TextStyle.Subheadline,
				TextAlignment = TextAlignment.Trailing,
				TextColor = Colors.SecondaryLabel
			};

			CollectionView<ListEntry> list = new()
			{
				Width = 300,
				Height = 248,
				ItemsSource = items,
				ItemTemplate = static () => new ListCell(),
				Layout = CollectionLayout.List(),
				ItemCommand = Command.From<ListEntry>(
					item => selected.Text = item.Title),
				ShowsSeparators = true,
				Background = Colors.SecondaryBackground,
				CornerRadius = 16
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
			""");


	public ListsViewModel()
	{
		SetItemCount((int)ItemCount);
	}


	partial void OnItemCountChanged(
		double value) =>
		SetItemCount((int)Math.Round(value));

	[RelayCommand]
	void Select(
		ListEntry item) =>
		SelectedTitle = item.Title;

	void SetItemCount(
		int count)
	{
		count = Math.Clamp(count, 1, 6);

		while (Items.Count < count)
			Items.Add(new($"Item {Items.Count + 1}"));

		while (Items.Count > count)
		{
			ListEntry removed = Items[^1];
			Items.RemoveAt(Items.Count - 1);

			if (SelectedTitle == removed.Title)
				SelectedTitle = "None";
		}
	}

	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];
}
