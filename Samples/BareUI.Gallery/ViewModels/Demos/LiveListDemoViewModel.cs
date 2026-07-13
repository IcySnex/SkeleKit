using System.Collections.ObjectModel;
using BareUI.Gallery.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BareUI.Gallery.ViewModels.Demos;

/// <summary>
/// An ObservableCollection drives the CollectionView: each change animates, and the empty view
/// appears on its own when the list runs dry.
/// </summary>
public partial class LiveListDemoViewModel : ObservableObject
{
	int next = 1;

	public ObservableCollection<TodoItem> Items { get; } = [];

	[RelayCommand]
	void Add()
	{
		Items.Insert(0, new($"Item {next++}", $"Added at {DateTime.Now:HH:mm:ss}"));
	}

	[RelayCommand]
	void Remove()
	{
		if (Items.Count > 0)
			Items.RemoveAt(0);
	}

	[RelayCommand]
	void Shuffle()
	{
		if (Items.Count > 1)
			Items.Move(Items.Count - 1, 0);
	}

	[RelayCommand]
	void Clear() =>
		Items.Clear();

	[RelayCommand]
	void Delete(
		TodoItem item)
	{
		Items.Remove(item);
		Haptics.Notify(HapticsNotification.Success);
	}

	[RelayCommand]
	void Duplicate(
		TodoItem item)
	{
		Items.Insert(Items.IndexOf(item), item with { Title = $"{item.Title} (copy)" });
		Haptics.Impact(HapticStyle.Light);
	}

	[ObservableProperty]
	public partial bool IsRefreshing { get; set; }

	[RelayCommand]
	async Task Refresh()
	{
		try
		{
			await Task.Delay(2000);

			Add();
			Haptics.Selection();
		}
		finally
		{
			IsRefreshing = false;
		}
	}

	public LiveListDemoViewModel()
	{
		for (int index = 0; index < 3; index++)
			Add();
	}
}
