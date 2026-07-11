using BareUI.Gallery.Models;
using BareUI.Gallery.ViewModels.Demos;
using BareUI.Gallery.Views;

namespace BareUI.Gallery.Views.Demos;

/// <summary>
/// A native inset-grouped list, the shape Velura's Settings screen needs.
/// </summary>
public class ListDemo : ContentView<ListDemoViewModel>
{
	readonly CollectionView<SettingsEntry> entries = new()
	{
		Layout = CollectionLayout.List(grouped: true),
		ItemTemplate = () => new SettingsCell()
	};

	public ListDemo()
	{
		Title = "List";

		Content = entries;
	}

	protected override void OnViewModelAttached()
	{
		entries.ItemsSource = Bindable.From<IReadOnlyList<SettingsEntry>?>(ViewModel!.Entries);
		entries.SelectionCommand = ViewModel.OpenCommand;
	}
}
